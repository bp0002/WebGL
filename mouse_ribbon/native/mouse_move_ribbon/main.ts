import { RenderLauncher } from "./render_launcher";
import { Mesh } from "../lib/webgl";

// ##################################################
export interface IPolygonData {
    vertexs?: [number, number][];
    vertexs3D?: [number, number, number][];
    faces: [number, number, number][];
}

export class MouseTrial {
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
     * down事件标识
     */
    private downFlag: boolean = false;
    /**
     * 事件点记录列表
     */
    private historyX: number[] = [];
    /**
     * 事件点记录列表
     */
    private historyY: number[] = [];
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
    private canvasWidth: number;
    private canvasHeight: number;
    constructor(mesh: Mesh, gl: WebGLRenderingContext, canvasWidth: number, canvasHeight: number) {
        this.mesh = mesh;
        this.gl = gl;
        this.canvasWidth = canvasWidth;
        this.canvasHeight = canvasHeight;

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
        return Math.sin(Math.PI * x) * 0.01;
    }
    /**
     * 点数据更新
     */
    public update = () => {

        this.historyX.pop();
        this.historyX.unshift(this.currX);
        this.historyY.pop();
        this.historyY.unshift(this.currY);

        // Update the points to correspond with history.
        for (let i = 0; i < this.ropeSize; i++) {
            const p = this.points[i];

            // Smooth the curve with cubic interpolation to prevent sharp edges.
            const ix = MouseTrial.cubicInterpolation(this.historyX, i / this.ropeSize * this.historySize);
            const iy = MouseTrial.cubicInterpolation(this.historyY, i / this.ropeSize * this.historySize);

            p[0] = ix;
            p[1] = iy;
        }

        if (this.mesh) {
            const dataBuffer01 = this.mesh.dataBufferCfg;
            dataBuffer01.clearVertex();
            dataBuffer01.clearColor();
            dataBuffer01.clearFace();
            dataBuffer01.clearUV();

            // const sphere = GeometryTools.ribbon_from_line(points);
            const sphere = MouseTrial.ribbon_from_line2(this.points, 90, undefined, this.weightFunction, this.canvasWidth / this.canvasHeight);

            if (sphere) {
                if (sphere.vertexs3D) {
                    sphere.vertexs3D.forEach((vertex, index, arr) => {
                        dataBuffer01.addVertex(vertex[0], vertex[1], vertex[2]);
                        dataBuffer01.addColor(0.8, 0, 0, 1 - (index / (arr.length / 2) - Math.floor(index / (arr.length / 2))));
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

            dataBuffer01.update(<WebGLRenderingContext>this.gl);
        }
    }
    public moveCall = (e: PointerEvent) => {
        if (this.downFlag) {
            this.currX = (e.clientX - this.canvasWidth / 2) / this.canvasWidth * 2;
            this.currY = - (e.clientY - this.canvasHeight / 2) / this.canvasHeight * 2;
        }
    }
    public upCall = (e: PointerEvent) => {
        this.downFlag = false;
    }
    public downCall = (e: PointerEvent) => {
        this.downFlag = true;

        this.currX = (e.clientX - this.canvasWidth / 2) / this.canvasWidth * 2;
        this.currY = - (e.clientY - this.canvasHeight / 2) / this.canvasHeight * 2;

        this.historyX.length = 0;
        this.historyY.length = 0;

        // Create history array.
        for (let i = 0; i < this.historySize; i++) {
            this.historyX.push(this.currX);
            this.historyY.push(this.currY);
        }

        this.update();
    }
}

// ########################################################################
// Demo
const canvas = <HTMLCanvasElement>document.getElementById('your_canvas');

canvas.width = window.innerWidth;
canvas.height = window.innerHeight;

// 创建渲染 - 可以是其他渲染引擎
const mesh = RenderLauncher.active(canvas, null);

// 渲染循环

RenderLauncher.webgldemo.renderLoop = (timestamp: number) => {
    RenderLauncher.webgldemo.clearColor();

    RenderLauncher.scene.viewport[0] = 0;
    RenderLauncher.scene.viewport[1] = 0;
    RenderLauncher.scene.viewport[2] = RenderLauncher.webgldemo.width;
    RenderLauncher.scene.viewport[3] = RenderLauncher.webgldemo.height;
    RenderLauncher.scene.render(true);
};

RenderLauncher.webgldemo.loop(0);

// 启动拖尾模块
const MouseTrial0 = new MouseTrial(<Mesh>mesh, <WebGLRenderingContext>RenderLauncher.webgldemo.gl, canvas.width, canvas.height);

// 拖尾数据更新 - 如果没有操作，点数据将向最后的位置收缩
setInterval(MouseTrial0.update, 16);

canvas.addEventListener('pointerdown', MouseTrial0.downCall);
canvas.addEventListener('pointerup', MouseTrial0.upCall);
canvas.addEventListener('pointermove', MouseTrial0.moveCall);

// setInterval(() => {
//     // bar.onProcess('', '', 0, Math.abs(Math.sin(Date.now() / 5000) * 100), undefined);
// }, 50);