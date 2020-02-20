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
    private currX: number = 0;
    private currY: number = 0;
    private points: [number, number][] = [];
    private downFlag: boolean = false;
    private historyX: number[] = [];
    private historyY: number[] = [];
    public ropeSize: number = 100;
    public historySize: number = 30;
    public mesh: Mesh | null = null;
    public gl: WebGLRenderingContext;
    public mistake: number = 5;
    constructor(mesh: Mesh, gl: WebGLRenderingContext) {
        this.mesh = mesh;
        this.gl = gl;

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
    public weightFunction = (x: number) => {
        return Math.sin(x * Math.PI) * 0.01;
    }
    public update = () => {
        let flag = true;

        const lastX = this.historyX.pop();
        if (lastX !== this.currX || this.downFlag) {
            this.historyX.unshift(this.currX);
        }
        const lastY = this.historyY.pop();
        if (lastY !== this.currY || this.downFlag) {
            this.historyY.unshift(this.currY);
        }

        flag = this.historyY.length >= 4;

        if (flag) {
            // Update the points to correspond with history.
            for (let i = 0; i < this.ropeSize; i++) {
                const p = this.points[i];

                // Smooth the curve with cubic interpolation to prevent sharp edges.
                const ix = MouseTrial.cubicInterpolation(this.historyX, i / this.ropeSize * this.historySize);
                const iy = MouseTrial.cubicInterpolation(this.historyY, i / this.ropeSize * this.historySize);

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
                const sphere = MouseTrial.ribbon_from_line2(this.points, 90, undefined, this.weightFunction, canvas.width / canvas.height);

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
            }

            dataBuffer01.update(<WebGLRenderingContext>this.gl);
        }
    }
    public moveCall = (e: MouseEvent) => {
        if (this.downFlag) {
            this.currX = (Math.round(e.clientX / this.mistake) * this.mistake - canvas.width / 2) / canvas.width * 2;
            this.currY = - (Math.round(e.clientY / this.mistake) * this.mistake - canvas.height / 2) / canvas.height * 2;
        }
    }
    public upCall = (e: MouseEvent) => {
        this.downFlag = false;
    }
    public downCall = (e: MouseEvent) => {
        this.downFlag = true;

        this.currX = (Math.round(e.clientX / this.mistake) * this.mistake - canvas.width / 2) / canvas.width * 2;
        this.currY = - (Math.round(e.clientY / this.mistake) * this.mistake - canvas.height / 2) / canvas.height * 2;

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

const canvas = <HTMLCanvasElement>document.getElementById('your_canvas');

canvas.width = window.innerWidth;
canvas.height = window.innerHeight;

const mesh = RenderLauncher.active(canvas, null);
const MouseTrial0 = new MouseTrial(<Mesh>mesh, <WebGLRenderingContext>RenderLauncher.webgldemo.gl);

setInterval(MouseTrial0.update, 16);

canvas.addEventListener('pointerdown', MouseTrial0.downCall);
canvas.addEventListener('pointerup', MouseTrial0.upCall);
canvas.addEventListener('pointermove', MouseTrial0.moveCall);

// setInterval(() => {
//     // bar.onProcess('', '', 0, Math.abs(Math.sin(Date.now() / 5000) * 100), undefined);
// }, 50);