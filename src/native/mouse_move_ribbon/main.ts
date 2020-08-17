import { RenderLauncher } from "./render_launcher";
import { Mesh } from "../lib/webgl";

// ##################################################
let panel: HTMLDivElement;

const createButton = (tag: string, clikCall: (arg: string) => void) => {
    const div = document.createElement('div');
    div.innerHTML = `
        <span style="color: black;">${tag}</span><input id="${tag}" type="string" value="(function(x){ return Math.sin(x*x*Math.PI) * 0.01; })" style="width:90%;height:25px;font-size:20px;" />
    `;
    div.style.width = '400px';
    div.style.height = 'auto';
    div.style.backgroundColor = 'green';

    const span = document.createElement('span');
    div.appendChild(span);
    span.textContent = '  应用  ';
    span.addEventListener(
        'pointerdown',
        (e) => {
            const arg = <HTMLInputElement>document.getElementById(tag);
            clikCall && clikCall(arg.value);
        }
    );

    createPanel().appendChild(div);
};

const createPanel = () => {
    if (!panel) {
        panel = document.createElement('div');
        panel.style.width = '400px';
        panel.style.height = '50px';
        panel.style.overflowY = 'auto';
        panel.style.position = 'absolute';
        document.body.appendChild(panel);
    }

    return panel;
};

createButton(
    'weight_function',
    (arg: string) => {
        // const reg = /edgeCount:(.+)-()/;
        // const resResult = arg.match(reg);
        if (arg && RenderLauncher.mesh) {
            try {
                // tslint:disable-next-line:no-eval
                MouseTrial0.weightFunction = eval(arg);
            } catch (e) {
                MouseTrial0.weightFunction = (x: number) => { return Math.sin(Math.PI * x) * 0.01; };
                alert(e);
            }

        }
    }
);

// ##################################################
export interface IPolygonData {
    vertexs?: [number, number][];
    vertexs3D?: [number, number, number][];
    faces: [number, number, number][];
}

export class MouseTrial {
    /**
     * 消失速度
     */
    public static UpdateSpeed: number = 1;
    /**
     * 正常消失过程最少关键点数 - 关键点数超过该值，则消失速度应用 UpdateSpeed
     */
    public static MinCount: number = 8;

    // 三次插值线条
    public static cubicInterpolation(array: number[], t: number, tangentFactor?: number) {
        if (tangentFactor == null) {
            tangentFactor = 1;
        }

        const k = Math.floor(t);
        const m = [MouseTrial.getTangent(k, tangentFactor, array), MouseTrial.getTangent(k + 1, tangentFactor, array)];
        const p = [MouseTrial.clipInput(k, array), MouseTrial.clipInput(k + 1, array)];
        t -= k;
        const t2 = t * t;
        const t3 = t * t2;
        return (2 * t3 - 3 * t2 + 1) * p[0] + (t3 - 2 * t2 + t) * m[0] + (-2 * t3 + 3 * t2) * p[1] + (t3 - t2) * m[1];
    }
    public static getTangent(k: number, factor: number, array: number[]) {
        return factor * (MouseTrial.clipInput(k + 1, array) - MouseTrial.clipInput(k - 1, array)) / 2;
    }
    /**
     * Cubic interpolation based on https://github.com/osuushi/Smooth.js
     */
    public static clipInput(k: number, arr: number[]) {
        if (k < 0) { k = 0; }
        if (k > arr.length - 1) { k = arr.length - 1; }
        return arr[k];
    }
    // 插值线条结果创建丝带顶点/三角形数据
    public static ribbon_from_line2(points: [number, number][], deltaAngle: number, deltaAngleFunction?: (x: number) => number, weightFunction?: (x: number) => number, widthScaleHeight: number = 1) {

        const result: IPolygonData = {
            vertexs3D: [],
            faces: []
        };

        const detailCount = points.length;

        if (detailCount < 2) {
            return;
        }

        let cos = 0, sin = 0, deltaCos = 0, deltaSin = 0, deltaXY = 0, deltaX = 0, deltaY = 0, prePoint: [number, number] = [points[0][0], points[0][1]], nxtPoint: [number, number] = [points[0][0], points[0][1]];
        let currDeltaAngle: number;
        let deltaDistance = 0;
        let lastTempPoint: [number, number] = [points[0][0], points[0][1]];

        for (let i = 1; i < detailCount; i++) {
            nxtPoint = points[i];
            deltaX  = nxtPoint[0] - prePoint[0];
            deltaY  = nxtPoint[1] - prePoint[1];

            deltaXY = Math.sqrt(deltaX * deltaX + deltaY * deltaY);
            cos     = deltaXY === 0 ? 0 : deltaX / deltaXY;
            sin     = deltaXY === 0 ? 0 : deltaY / deltaXY;

            currDeltaAngle  = deltaAngleFunction ? deltaAngle * deltaAngleFunction((i - 1) / detailCount) : deltaAngle;
            deltaCos        = Math.cos(Math.PI * currDeltaAngle / 180);
            deltaSin        = Math.sin(Math.PI * currDeltaAngle / 180);

            deltaDistance   = weightFunction ? weightFunction(i / detailCount) : 0.01;

            // if (deltaXY > deltaDistance) {

                if (result.vertexs3D) {
                    result.vertexs3D[i - 1] = [
                        (cos * deltaCos - sin * deltaSin) * deltaDistance / widthScaleHeight  + prePoint[0],
                        (sin * deltaCos + cos * deltaSin) * deltaDistance + prePoint[1],
                        0
                    ];
                    result.vertexs3D[i - 1 + detailCount] = [
                        (cos * deltaCos + sin * deltaSin) * deltaDistance / widthScaleHeight + prePoint[0],
                        (sin * deltaCos - cos * deltaSin) * deltaDistance + prePoint[1],
                        0
                    ];
                }

                lastTempPoint = prePoint;
            // } else {
            //     if (result.vertexs3D) {
            //         result.vertexs3D[i - 1] = [
            //             (cos * deltaCos - sin * deltaSin) * deltaDistance + lastTempPoint[0],
            //             (sin * deltaCos + cos * deltaSin) * deltaDistance + lastTempPoint[1],
            //             0
            //         ];
            //         result.vertexs3D[i - 1 + detailCount] = [
            //             (cos * deltaCos + sin * deltaSin) * deltaDistance + lastTempPoint[0],
            //             (sin * deltaCos - cos * deltaSin) * deltaDistance + lastTempPoint[1],
            //             0
            //         ];
            //     }
            // }

            prePoint = points[i];
        }

        deltaCos        = Math.cos(Math.PI * 45 / 180);
        deltaSin        = Math.sin(Math.PI * 45 / 180);
        if (result.vertexs3D) {
            result.vertexs3D[detailCount - 1] = [
                (cos * deltaCos - sin * deltaSin) * deltaXY / widthScaleHeight + prePoint[0],
                (sin * deltaSin + cos * deltaCos) * deltaXY + prePoint[1],
                0
            ];
            result.vertexs3D[detailCount - 1 + detailCount] = [
                (cos * deltaCos + sin * deltaSin) * deltaXY / widthScaleHeight + prePoint[0],
                (sin * deltaSin - cos * deltaCos) * deltaXY + prePoint[1],
                0
            ];
        }

        result.faces = this.sphereRibbon(0, detailCount);

        return result;
    }
    // 丝带三角形数据
    public static sphereRibbon(pointStartIndex: number, pointCount: number): [number, number, number][] {
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

        return faces;
    }
    /**
     * down事件标识
     */
    public downFlag: boolean = false;
    /**
     * 当前事件点
     */
    private currX: number = 0;
    /**
     * 当前事件点
     */
    private currY: number = 0;
    /**
     * 插值点列表
     */
    private points: [number, number][] = [];
    /**
     * 事件点记录列表
     */
    private historyX: number[] = [0, 0, 0, 0, 0];
    /**
     * 事件点记录列表
     */
    private historyY: number[] = [0, 0, 0, 0, 0];
    /**
     * 插值点数量
     */
    public ropeSize: number = 100;
    /**
     * 事件点记录数量
     */
    public historySize: number = 20;
    public mesh: Mesh | null = null;
    public gl: WebGLRenderingContext;
    public weight: number = 10;
    public mistake: number = 5;
    private canvasWidth: number;
    private canvasHeight: number;
    /**
     * 颜色rgb
     */
    public color: [number, number, number];
    constructor(mesh: Mesh, gl: WebGLRenderingContext, canvasWidth: number, canvasHeight: number) {
        this.mesh = mesh;
        this.gl = gl;
        this.canvasWidth = canvasWidth;
        this.canvasHeight = canvasHeight;
        this.color = [1, 0, 0];

        // Create rope points.
        for (let i = 0; i < this.ropeSize; i++) {
            this.points.push([0, 0]);
        }

        // Create history array.
        for (let i = 0; i < this.historySize; i++) {
            this.historyX.push(0);
            this.historyY.push(0);
        }
    }
    /**
     * 丝带从尾部到头部宽度变化函数
     */
    public weightFunction = (x: number) => {
        return Math.sin(Math.PI * x) * 3 * this.weight / this.canvasWidth;
    }

    public setTexture(img: HTMLImageElement){

    }
    /**
     * 点数据更新
     */
    public update = (speed: number = 1) => {
        let flag = true;

        // const lastX = this.historyX.pop();
        // if (lastX !== this.currX || this.downFlag) {
        //     this.historyX.unshift(this.currX);
        // }
        // const lastY = this.historyY.pop();
        // if (lastY !== this.currY || this.downFlag) {
        //     this.historyY.unshift(this.currY);
        // }
        this.updatePoints(speed);

        const [xArr, yArr] = this.formatHistory(this.historyX, this.historyY);
        const count = xArr.length;
        flag = count >= 4;
        // const count1 = this.historyX.length;
        // flag = this.historyX.length >= 4;

        if (flag) {
            // Update the points to correspond with history.
            for (let i = 0; i < this.ropeSize; i++) {
                const p = this.points[i];

                // Smooth the curve with cubic interpolation to prevent sharp edges.
                const ix = MouseTrial.cubicInterpolation(xArr, i / this.ropeSize * count);
                const iy = MouseTrial.cubicInterpolation(yArr, i / this.ropeSize * count);

                p[0] = ix;
                p[1] = iy;
            }
        }

        if (this.mesh) {
            const dataBuffer01 = this.mesh.dataBufferCfg;
            dataBuffer01.clearVertex();
            dataBuffer01.clearColor();
            dataBuffer01.clearFace();
            dataBuffer01.clearUV();

            if (flag) {

                // const sphere = GeometryTools.ribbon_from_line(points);
                const sphere = MouseTrial.ribbon_from_line2(this.points, 90, undefined, this.weightFunction, this.canvasWidth / this.canvasHeight);

                if (sphere) {
                    if (sphere.vertexs3D) {
                        sphere.vertexs3D.forEach((vertex, index, arr) => {
                            dataBuffer01.addVertex(vertex[0], vertex[1], vertex[2]);
                            dataBuffer01.addColor(this.color[0], this.color[1], this.color[2], 1 - (index / (arr.length / 2) - Math.floor(index / (arr.length / 2))));
                            if (index < arr.length / 2) {
                                dataBuffer01.addUV(0, index % 2);
                            } else {
                                dataBuffer01.addUV(1, index % 2);
                            }
                        });
                    }

                    sphere.faces.forEach((face) => {
                        dataBuffer01.addFace(face[0], face[1], face[2]);
                    });
                }
            }
            dataBuffer01.update(<WebGLRenderingContext>this.gl);
        }
    }
    public moveCall = (e: TouchPoint) => {
        if (this.downFlag) {
            this.currX = (Math.round(e.x / this.mistake) * this.mistake - this.canvasWidth / 2) / this.canvasWidth * 2;
            this.currY = - (Math.round(e.y / this.mistake) * this.mistake - this.canvasHeight / 2) / this.canvasHeight * 2;
        }
    }
    public upCall = (e: TouchPoint) => {
        this.downFlag = false;
    }
    public downCall = (e: TouchPoint) => {
        this.downFlag = true;

        this.currX = (Math.round(e.x / this.mistake) * this.mistake - this.canvasWidth / 2) / this.canvasWidth * 2;
        this.currY = - (Math.round(e.y / this.mistake) * this.mistake - this.canvasHeight / 2) / this.canvasHeight * 2;

        this.historyX.length = 0;
        this.historyY.length = 0;

        // Create history array.
        for (let i = 0; i < 5; i++) {
            this.historyX.push(this.currX + (i - 2) * 0.0005);
            this.historyY.push(this.currY + (i - 2) * 0.0005);
        }

        this.update();
    }
    private updatePoints(speed: number = 1) {
        const lastX = this.historyX.pop();
        const lastY = this.historyY.pop();

        speed--;
        if (speed > 0 && this.historyX.length > MouseTrial.MinCount) {
            this.updatePoints(speed);
        } else {
            if (lastX !== this.currX || this.downFlag) {
                this.historyX.unshift(this.currX);
            }

            if (lastY !== this.currY || this.downFlag) {
                this.historyY.unshift(this.currY);
            }
        }
    }
    private formatHistory(xArr: number[], yArr: number[]) {
        const newX: number[] = [];
        const newY: number[] = [];

        const count = xArr.length;

        let preX: number = -1, preY: number = -1, curX: number, curY: number;
        for (let i = 0; i < count; i++) {
            curX = xArr[i];
            curY = yArr[i];
            if (preX !== curX || preY !== curY) {
                newX.push(curX);
                newY.push(curY);
            }
            preX = curX;
            preY = curY;
        }

        return [newX, newY];
    }
}

export interface TouchPoint {
    x: number;
    y: number;
}

const canvas = <HTMLCanvasElement>document.getElementById('your_canvas');

canvas.width = window.innerWidth;
canvas.height = window.innerHeight;

const mesh = RenderLauncher.active(canvas, null);
const MouseTrial0 = new MouseTrial(<Mesh>mesh, <WebGLRenderingContext>RenderLauncher.webgldemo.gl, canvas.width, canvas.height);

setInterval(() => {
    MouseTrial0.update(MouseTrial.UpdateSpeed);
}, 16);

canvas.addEventListener('pointerdown', MouseTrial0.downCall);
canvas.addEventListener('pointerup', MouseTrial0.upCall);
canvas.addEventListener('pointermove', MouseTrial0.moveCall);