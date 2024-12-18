export type TVerticeID = number;
export type TPoint = [number, number];
export type TVector = [number, number];
export interface IArea {
    vertices: TVerticeID[];
    min: TPoint;
    max: TPoint;
}
export interface IPolygon {
    triangles: [TVerticeID, TVerticeID, TVerticeID][];
    min: TPoint;
    max: TPoint;
    [index: string]: any;
}
export interface IVertices {
    get(id: TVerticeID): TPoint;
}
export interface IPointLink {
    getNext(curr: TVerticeID): TVerticeID;
}
export function isInMinMax(min: TPoint, max: TPoint, point: TPoint) {
    return (min[0] <= point[0] && min[1] <= point[1]) && (point[0] <= max[0] && point[1] <= max[1]);
}
export function crossPoint(a: TPoint, b0: TPoint, b1: TPoint): number {
    return (b0[0] - a[0]) * (b1[1] - a[1]) - (b0[1] - a[1]) * (b1[0] - a[0]);
}
export function vector(a: TPoint, b: TPoint): TVector {
    return [a[0] - b[0], a[1] - b[1]];
}
export function isInTriangles(a: TPoint, b: TPoint, c: TPoint, p: TPoint): boolean {
    let aa = a;
    let bb = b;
    let cc = c;
    if (crossPoint(a, b, c) < 0) {
        cc = b;
        bb = c;
    }
    return crossPoint(aa, bb, p) > 0 && crossPoint(bb, cc, p) > 0 && crossPoint(cc, aa, p) > 0;
}
export function isInPolygon(p: TPoint, polygon: IPolygon, data: IVertices, isInMinMax: (min: TPoint, max: TPoint, point: TPoint) => boolean): boolean {
    let result = false;
    if (isInMinMax(polygon.min, polygon.max, p)) {
        let len = polygon.triangles.length;
        for (let i = 0; i < len; i++) {
            let t = polygon.triangles[i];
            if (isInTriangles(data.get(t[0]), data.get(t[1]), data.get(t[2]), p)) {
                result = true;
                break;
            }
        }
    }
    return result;
}
export function edgesToPolygon(edges: [TVerticeID, TVerticeID][], data: IVertices) {
    
}

export function pointLinkToPolygon(links: Links, data: IVertices) {
}
export type TLink = [TVerticeID, number, number, number];

export class Links {
    protected pointLinks: Map<TVerticeID, TLink[]> = new Map();
    constructor(edges: [TVerticeID, TVerticeID][], data: IVertices) {
        let edgeCount = edges.length;
        for (let i = 0; i < edgeCount; i++) {
            let edge    = edges[i];
            let from    = edge[0];
            let to      = edge[1];
            let a       = data.get(from);
            let b       = data.get(to);
            let x = b[0] - a[0];
            let y = b[1] - a[1];
            let l = Math.sqrt(x * x + y * y);
            {
                let links = this.pointLinks.get(from);
                if (links == undefined) {
                    this.pointLinks.set(from, []);
                    links = this.pointLinks.get(from);
                }
                if (links) {
                    if (links.findIndex(item => item[0] == to) < 0) {
                        links.push([to, x / l, y / l, 0]);
                    }
                }
            }
            {
                let links = this.pointLinks.get(to);
                if (links == undefined) {
                    this.pointLinks.set(to, []);
                    links = this.pointLinks.get(to);
                }
                if (links) {
                    if (links.findIndex(item => item[0] == from) < 0) {
                        links.push([from, -x / l, -y / l, 0]);
                    }
                }
            }
        }
        this.pointLinks.forEach((links) => {
            // 按 与 x 轴逆时针夹角递增排序
            links.sort((a, b) => {
                if (a[2] >= 0 && b[2] >= 0) {
                    return a[1] - b[1] > 0 ? -1 : 1;
                } else if (a[2] < 0 && b[2] < 0){
                    return a[1] - b[1] > 0 ? 1 : -1;
                } else {
                    return a[2] - b[2] > 0 ? -1 : 1;
                }
            });
            let len = links.length;
            // 计算每边与排序后下一条边的逆时针夹角
            for (let i = 0; i < len; i++) {
                let cur  = links[i];
                let next = links[ (i + len + 1) % len ];
                let x1 =  cur[1]; let y1 =  cur[2];
                let x2 = next[1]; let y2 = next[2];
                let angle = Math.acos(x1 * x2 + y1 * y2);
                if ((x1 * y2 - x2 * y1) < 0) {
                    angle = Math.PI * 2 - angle;
                }
                cur[3] = angle;
            }
        });
    }
    public polygons(data: IVertices) {
        let pointCount = this.pointLinks.size;
        let points: TVerticeID[] = [];
        this.pointLinks.forEach((v, k) => {
            points.push(k);
        });

        // 已被确定为多边形的边的信息 - 每个有向边信息只会被一个多边形使用一次
        let findedLink: [TVerticeID, TVerticeID][] = [];
        // 一次查找经过的边信息
        let findingLink: [TVerticeID, TVerticeID][] = [];
        let polygons: TVerticeID[][] = [];
        // 以每个点为起点寻找逆时针环绕的边构成的多边形
        for (let i = 0; i < pointCount; i++) {
            let point = points[i];
            let links = this.pointLinks.get(point);
            if (links) {
                let len = links.length;
                // 分别以该点关联的边为起始边寻找
                for (let i = 0; i < len; i++) {
                    let link = links[i];
                    let next: TLink = link;
                    let cur = point;
                    let polygon: TVerticeID[] = [cur];
                    let max = 512;
                    let angle = 0;
                    // 从零记录查找经过的边信息
                    findingLink.length = 0;
                    while (max >= 0 && next != undefined && next[0] != point) {
                        // 检查目标边是否已被多边形使用, 是则说明此次查找的多边形已记录,跳过此次查找
                        if (findedLink.findIndex((val) => { return val[0] == cur && val[1] == next[0] }) >= 0) {
                            cur = undefined;
                            break;
                        }
                        polygon.push(next[0]);
                        // console.error(cur, next[0]);
                        findingLink.push([cur, next[0]]);

                        let temp = this.findLinkNext(next, cur);
                        if (temp) {
                            cur = next[0];
                            next = temp[0];
                            angle += temp[1];
                        } else {
                            cur = undefined;
                            break;
                        }
                        max --;
                    }
                    
                    // console.error(cur, next[0]);
                    findingLink.push([cur, next[0]]);

                    // 当前起始边与结束边闭合, 且累计的预期内角和满足多边形内角和定理
                    if (cur != undefined && links[(i + len + 1) % len][0] == cur && angle <= (polygon.length - 2) * Math.PI) {
                        let len = polygon.length;
                        let min = polygon[0];
                        let idx = 0;
                        for (let i = 0; i < len; i++) {
                            if (min > polygon[i]) {
                                idx = i;
                                min = polygon[i];
                            }
                        }
                        let temp = [];
                        for (let i = idx; i < idx + len; i++) {
                            temp.push(polygon[i % len]);
                        }
                        findedLink.push(...findingLink);
                        // let has = polygons.findIndex((v) => { let flag = true; v.forEach((v, i) => { flag = flag && v == temp[i] }); return flag; }) >= 0;
                        // if (!has) {
                            polygons.push(temp);
                        // }
                    }
                }
            }
        }
        // console.error(findedLink);
    
        let polygonsnew: IPolygon[] = [];
        polygons.forEach((polygon) => {
            let len = polygon.length;
            let polygonnew: IPolygon = {
                triangles: [],
                min: [Number.MAX_SAFE_INTEGER, Number.MAX_SAFE_INTEGER],
                max: [Number.MIN_SAFE_INTEGER, Number.MIN_SAFE_INTEGER],
            }
            for (let i = 0; i < len - 2; i++) {
                let a = polygon[i + 0];
                let b = polygon[i + 1];
                let c = polygon[i + 2];
                polygonnew.triangles.push([a, b, c]);
                let ap = data.get(a);
                let bp = data.get(b);
                let cp = data.get(c);
                polygonnew.min[0] = Math.min(ap[0], bp[0], cp[0], polygonnew.min[0]);
                polygonnew.min[1] = Math.min(ap[1], bp[1], cp[1], polygonnew.min[1]);
                polygonnew.max[0] = Math.max(ap[0], bp[0], cp[0], polygonnew.max[0]);
                polygonnew.max[1] = Math.max(ap[1], bp[1], cp[1], polygonnew.max[1]);
            }
            polygonnew.points = polygon;
            polygonsnew.push(polygonnew);
        });
        return polygonsnew;
    }
    protected findLinkNext(cur: TLink, linking: TVerticeID): [TLink, number] | undefined {
        let links = this.pointLinks.get(cur[0]);
        if (links) {
            let idx = links.findIndex((a) => a[0] == linking);
            let len = links.length;
            if (idx >= 0) {
                let link = links[(idx + len - 1) % len];
                // let x2 = -cur[1]; let y2 = -cur[2];
                // let x1 = link[1]; let y1 = link[2];
                // let angle = Math.acos(x1 * x2 + y1 * y2);
                // if ((x1 * y2 - x2 * y1) < 0) {
                //     angle = Math.PI * 2 - angle;
                // }
                // if (link[3] < 2) {
                //     link[3] += 1;
                //     return link[0];
                // } else {
                //     return undefined;
                // }
                return [link, link[3]];
            } else {
                return undefined;
            }
        }
        return undefined;
    }
}

export function test() {
    let points = {
        data: [
            [0, 0],
            [-10, 10],
            [10, 10],
            [20, 0],
            [0, 20],
            [20, 50],
            [-10, 40],
            [25, 30],
        ],
        get: function(id: TVerticeID) {
            return this.data[id];
        }
    };
    let edges: [TVerticeID, TVerticeID][] = [
        [0, 1],
        [0, 2],
        [0, 3],
        [1, 2],
        [1, 6],
        [2, 3],
        [2, 5],
        [3, 7],
        [5, 7],
        [2, 4],
        [4, 6],
        [5, 6]
    ];

    let links = new Links(edges, points);
    let polygons = links.polygons(points);
    console.error(polygons);
    console.error(links);

    let pick: TPoint = [10, 20];
    let polygoncount = polygons.length;
    for (let i = 0; i < polygoncount; i++) {
        let polygon = polygons[i];
        if (isInPolygon(pick, polygon, points, isInMinMax)) {
            console.error("Point in Polygon", pick, polygon);
            break;
        }
    }
}