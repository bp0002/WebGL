import { RenderLauncher } from "./render_launcher";
import { GeometryTools } from "../../math/geometry";

const canvas = <HTMLCanvasElement>document.getElementById('your_canvas');

canvas.width = window.innerWidth;
canvas.height = window.innerHeight;

const points: [number, number][] = [];

const mesh = RenderLauncher.active(canvas, null);

let downFlag: boolean = false;
let upTime: number = 0;

const historyX: number[] = [];
const historyY: number[] = [];
const ropeSize = 100;
const historySize = 20;

// Create rope points.
for (let i = 0; i < ropeSize; i++) {
    points.push([0, 0]);
}

// Create history array.
for (let i = 0; i < historySize; i++) {
    historyX.push(0);
    historyY.push(0);
}

const downCall = (e: MouseEvent) => {
    upTime = 0;
    downFlag = true;

    currX = (e.clientX - canvas.width / 2) / canvas.width * 2;
    currY = - (e.clientY - canvas.height / 2) / canvas.height * 2;

    historyX.length = 0;
    historyY.length = 0;

    // Create history array.
    for (let i = 0; i < historySize; i++) {
        historyX.push(currX);
        historyY.push(currY);
    }

    update();
};

const upCall = (e: MouseEvent) => {
    downFlag = false;
};

let currX: number = 0;
let currY: number = 0;

const moveCall = (e: MouseEvent) => {
    if (downFlag) {
        currX = (e.clientX - canvas.width / 2) / canvas.width * 2;
        currY = - (e.clientY - canvas.height / 2) / canvas.height * 2;
    }
};

const update = () => {

    historyX.pop();
    historyX.unshift(currX);
    historyY.pop();
    historyY.unshift(currY);

    // Update the points to correspond with history.
    for (let i = 0; i < ropeSize; i++) {
        const p = points[i];

        // Smooth the curve with cubic interpolation to prevent sharp edges.
        const ix = cubicInterpolation(historyX, i / ropeSize * historySize);
        const iy = cubicInterpolation(historyY, i / ropeSize * historySize);

        p[0] = ix;
        p[1] = iy;
    }

    if (mesh) {
        const dataBuffer01 = mesh.dataBufferCfg;
        dataBuffer01.clearVertex();
        dataBuffer01.clearColor();
        dataBuffer01.clearFace();
        dataBuffer01.clearUV();

        // const sphere = GeometryTools.ribbon_from_line(points);
        const sphere = GeometryTools.ribbon_from_line2(points, 90, undefined, (x: number) => 0.01);

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

        dataBuffer01.update(<WebGLRenderingContext>RenderLauncher.webgldemo.gl);
    }
};

/**
 * Cubic interpolation based on https://github.com/osuushi/Smooth.js
 */
const clipInput = (k: number, arr: number[]) => {
    if (k < 0) { k = 0; }
    if (k > arr.length - 1) { k = arr.length - 1; }
    return arr[k];
};

const getTangent = (k: number, factor: number, array: number[]) => {
    return factor * (clipInput(k + 1, array) - clipInput(k - 1, array)) / 2;
};

const cubicInterpolation = (array: number[], t: number, tangentFactor?: number) => {
    if (tangentFactor == null) {
        tangentFactor = 1;
    }

    const k = Math.floor(t);
    const m = [getTangent(k, tangentFactor, array), getTangent(k + 1, tangentFactor, array)];
    const p = [clipInput(k, array), clipInput(k + 1, array)];
    t -= k;
    const t2 = t * t;
    const t3 = t * t2;
    return (2 * t3 - 3 * t2 + 1) * p[0] + (t3 - 2 * t2 + t) * m[0] + (-2 * t3 + 3 * t2) * p[1] + (t3 - t2) * m[1];
};

setInterval(update, 16);

canvas.addEventListener('pointerdown', downCall);
canvas.addEventListener('pointerup', upCall);
canvas.addEventListener('pointermove', moveCall);

// setInterval(() => {
//     // bar.onProcess('', '', 0, Math.abs(Math.sin(Date.now() / 5000) * 100), undefined);
// }, 50);