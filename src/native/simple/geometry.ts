import { MathTools } from "../../math/math";

export interface IPolygonData {
    vertexs?: [number, number][];
    vertexs3D?: [number, number, number][];
    faces: [number, number, number][];
}

export class GeometryTools {
    public static polygon(edgeCount: number, asCenterMode: boolean): IPolygonData {
        const vertexs: [number, number][] = [];
        const faces: [number, number, number][] = [];
        const radius = 1;
        let vertexCount: number = edgeCount;

        if (edgeCount >= 3) {
            const perRadian = 3.14 * 2 / edgeCount;

            if (asCenterMode) {
                vertexCount++;
                vertexs.push([0, 0]);
            }

            for (let i = 0; i < edgeCount; i++) {
                const pos: [number, number] = [Math.cos(perRadian * i) * radius, Math.sin(perRadian * i) * radius];
                vertexs.push(pos);
            }
            if (asCenterMode) {
                for (let i = 3; i <= vertexCount; i++) {
                    faces.push([0, i - 2, i - 1]);
                }
                faces.push([0, vertexCount - 1, 1]);
            } else {
                for (let i = 3; i <= vertexCount; i++) {
                    faces.push([0, i - 2, i - 1]);
                }
            }
        }

        return { vertexs, faces };
    }
    public static sphere(detailH: number, detailV: number) {
        const half          = this.sphereHalf(detailH, detailV, (x: number) => { return Math.cos(Math.PI * x / 2); }, (x: number) => { return Math.sin(Math.PI * x / 2); }, true);
        const halfRevert    = this.sphereHalfRevert(detailH, detailV, (x: number) => { return Math.cos(Math.PI * x / 2); }, (x: number) => { return Math.sin(Math.PI * x / 2); }, true);

        const result: IPolygonData = {
            vertexs3D: [],
            faces: []
        };

        if (result.vertexs3D) {
            half.circleList.forEach((cicle) => {
                cicle.forEach((point) => {
                    if (result.vertexs3D) {
                        result.vertexs3D.push(point);
                    }
                });
            });

            half.facesList.forEach((faces) => {
                faces.forEach((face) => {
                    result.faces.push(face);
                });
            });

            const halfPointCount = result.vertexs3D.length;

            halfRevert.circleList.forEach((cicle) => {
                cicle.forEach((point) => {
                    if (result.vertexs3D) {
                        result.vertexs3D.push(point);
                    }
                });
            });

            halfRevert.facesList.forEach((faces) => {
                faces.forEach((face) => {
                    if (result.vertexs3D) {
                        result.faces.push([face[0] + halfPointCount, face[1] + halfPointCount, face[2] + halfPointCount]);
                    }
                });
            });
        }

        return result;
    }
    /**
     * 棱锥体
     * @param detailH 底面多边形边数目
     */
    public static pyramid(detailH: number) {
        const detailV: number = 1;

        const half          = this.sphereHalf(detailH, detailV, (x: number) => { return Math.cos(Math.PI * x / 2); }, (x: number) => { return Math.sin(Math.PI * x / 2); }, true);
        const halfRevert    = this.sphereHalfRevert(detailH, detailV, (x: number) => { return 1; }, (x: number) => { return 0; }, false);

        const result: IPolygonData = {
            vertexs3D: [],
            faces: []
        };

        if (result.vertexs3D) {
            half.circleList.forEach((cicle) => {
                cicle.forEach((point) => {
                    if (result.vertexs3D) {
                        result.vertexs3D.push(point);
                    }
                });
            });

            half.facesList.forEach((faces) => {
                faces.forEach((face) => {
                    result.faces.push(face);
                });
            });

            const halfPointCount = result.vertexs3D.length;

            halfRevert.circleList.forEach((cicle) => {
                cicle.forEach((point) => {
                    if (result.vertexs3D) {
                        result.vertexs3D.push(point);
                    }
                });
            });

            halfRevert.facesList.forEach((faces) => {
                faces.forEach((face) => {
                    if (result.vertexs3D) {
                        result.faces.push([face[0] + halfPointCount, face[1] + halfPointCount, face[2] + halfPointCount]);
                    }
                });
            });
        }

        return result;
    }
    /**
     * 柱体
     * @param detailH 底面多边形边数目
     */
    public static column(detailH: number) {
        const detailV: number = 1;

        const half          = this.sphereHalf(detailH, detailV, (x: number) => { return 1; }, (x: number) => { return x; }, false);
        const halfRevert    = this.sphereHalfRevert(detailH, detailV, (x: number) => { return 1; }, (x: number) => { return 0; }, false);

        const result: IPolygonData = {
            vertexs3D: [],
            faces: []
        };

        if (result.vertexs3D) {
            half.circleList.forEach((cicle) => {
                cicle.forEach((point) => {
                    if (result.vertexs3D) {
                        result.vertexs3D.push(point);
                    }
                });
            });

            half.facesList.forEach((faces) => {
                faces.forEach((face) => {
                    result.faces.push(face);
                });
            });

            const halfPointCount = result.vertexs3D.length;

            halfRevert.circleList.forEach((cicle) => {
                cicle.forEach((point) => {
                    if (result.vertexs3D) {
                        result.vertexs3D.push(point);
                    }
                });
            });

            halfRevert.facesList.forEach((faces) => {
                faces.forEach((face) => {
                    if (result.vertexs3D) {
                        result.faces.push([face[0] + halfPointCount, face[1] + halfPointCount, face[2] + halfPointCount]);
                    }
                });
            });
        }

        return result;
    }
    /**
     * 反棱柱
     * @param detailH 底面多边形边数目
     */
    public static antiPrism(detailH: number) {
        const detailV: number = 1;

        const half          = this.sphereHalf(detailH, detailV, (x: number) => { return 1; }, (x: number) => { return x; }, false);
        const halfRevert    = this.sphereHalfRevert(detailH, detailV, (x: number) => { return 1; }, (x: number) => { return 0; }, false);

        const result: IPolygonData = {
            vertexs3D: [],
            faces: []
        };

        const deltaRadian = Math.PI / detailH;

        if (result.vertexs3D) {
            half.circleList.forEach((cicle) => {
                cicle.forEach((point) => {
                    if (result.vertexs3D) {
                        result.vertexs3D.push(point);
                    }
                });
            });

            half.facesList.forEach((faces) => {
                faces.forEach((face) => {
                    result.faces.push(face);
                });
            });

            const halfPointCount = result.vertexs3D.length;

            halfRevert.circleList.forEach((cicle) => {
                cicle.forEach((point) => {
                    if (result.vertexs3D) {
                        result.vertexs3D.push([MathTools.cos_a_add_b(point[1], point[0], deltaRadian), MathTools.sin_a_add_b(point[1], point[0], deltaRadian), point[2]]);
                    }
                });
            });

            halfRevert.facesList.forEach((faces) => {
                faces.forEach((face) => {
                    if (result.vertexs3D) {
                        result.faces.push([face[0] + halfPointCount, face[1] + halfPointCount, face[2] + halfPointCount]);
                    }
                });
            });
        }

        return result;
    }
    public static sphereHalfRevert(detailH: number, detailV: number, detailHFucntion: (x: number) => number, detailVFucntion: (x: number) => number, topIsProtruding: boolean = true) {
        const { circleList, facesList } = this.sphereHalf(detailH, detailV, detailHFucntion, detailVFucntion, topIsProtruding);

        circleList.forEach((circle) => {
            circle.reverse();
            circle.forEach((point) => {
                point[2] *= -1;
            });
            // const total = circle.length;
            // const count = circle.length / 2;
            // for (let i = 0; i < count; i++) {
            //     const temp = circle[i];

            //     circle[i]           = circle[total - i];
            //     circle[total - i]   = temp;
            // }
        });

        return { circleList, facesList };
    }
    public static sphereHalf(detailH: number, detailV: number, detailHFucntion: (x: number) => number, detailVFucntion: (x: number) => number, topIsProtruding: boolean = true) {
        const baseCirclePoints = this.circlePoints(detailH);
        const circleList: [number, number, number][][] = [];
        const facesList: [number, number, number][][] = [];

        let circlePointCount: number = 0;

        const vcount = topIsProtruding ? detailV : detailV + 1 ;

        for (let i = 0; i < vcount; i++) {
            const vDistance = detailVFucntion(i / detailV);
            const radius    = detailHFucntion(i / detailV); // Math.sqrt(1 - (i * i) / (detailV * detailV));

            // const vDistance = Math.sin(Math.PI * i / detailV / 2);
            // const radius    = Math.cos(Math.PI * i / detailV / 2);

            const tempList  = this.circlePointsScale(this.copyCirclePoints(baseCirclePoints), radius);

            circlePointCount = tempList.length;

            if (i > 0) {
                facesList.push(this.sphereRing(circlePointCount * (i - 1), circlePointCount));
            }
            circleList.push(this.circleFillZ(tempList, vDistance));
        }

        facesList.push(this.sphereTop(circlePointCount * (circleList.length - 1), circlePointCount));

        circleList.push([[0, 0, detailVFucntion(1)]]);

        return {
            circleList,
            facesList
        };
    }
    public static sphereRing(pointStartIndex: number, pointCount: number): [number, number, number][] {
        const faces: [number, number, number][] = [];

        for (let i = 0; i < pointCount - 1; i++) {
            faces.push(
                [
                    pointStartIndex + i,
                    pointStartIndex + i + 1,
                    pointStartIndex + i + 1 + pointCount
                ],
                [
                    pointStartIndex + i,
                    pointStartIndex + i + 1 + pointCount,
                    pointStartIndex + i + pointCount
                ]
            );
        }

        const i = pointCount - 1;
        faces.push(
            [
                pointStartIndex + i,
                pointStartIndex + 0,
                pointStartIndex + 0 + pointCount
            ],
            [
                pointStartIndex + i,
                pointStartIndex + 0 + pointCount,
                pointStartIndex + i + pointCount
            ]
        );

        return faces;
    }
    public static sphereTop(pointStartIndex: number, pointCount: number) {
        const faces: [number, number, number][] = [];

        for (let i = 0; i < pointCount - 1; i++) {
            faces.push(
                [
                    pointStartIndex + i,
                    pointStartIndex + i + 1,
                    pointStartIndex + pointCount
                ]
            );
        }

        faces.push(
            [
                pointStartIndex + pointCount - 1,
                pointStartIndex + 0,
                pointStartIndex + pointCount
            ]
        );

        return faces;
    }
    private static circleFillZ(circle: [number, number][], z: number) {
        const result: [number, number, number][] = [];
        circle.forEach((point) => {
            result.push([point[0], point[1], z]);
        });

        return result;
    }
    private static circlePoints(detail: number) {
        const points: [number, number][] = [];
        const perRadian = Math.PI * 2 / detail;

        for (let i = 0; i < detail; i++) {
            points.push([Math.cos(perRadian * i), Math.sin(perRadian * i)]);
        }

        return points;
    }
    private static circlePointsDym(detail: number, radius: number) {
        const points = this.circlePoints(detail);

        return this.circlePointsScale(points, radius);
    }
    private static circlePointsScale(points: [number, number][], radius: number) {

        points.forEach((point) => {
            point[0] *= radius;
            point[1] *= radius;
        });

        return points;
    }
    private static copyCirclePoints(points: [number, number][]) {
        const result: [number, number][] = [];
        points.forEach((point) => {
            result.push([point[0], point[1]]);
        });

        return result;
    }
}