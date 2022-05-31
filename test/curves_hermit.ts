import { canvas2DDisplay } from "../src/display/canvas2d_display";
import { FloatScalar } from "../src/math/scalar";

// Hermit
let v0 = 0, t0 = 8.728395, f0 = 0.0;
let v1 = 1, t1 = 0, f1 = 0.2517014;
function hermiteTest(x: number) {
    return FloatScalar.Hermite(v0, t0 * (f1 - f0), v1, t1 * (f1 - f0), x);
}
let canvas = document.createElement('canvas');
document.body.appendChild(canvas);
canvas.width = 500;
canvas.height = 1000;
let ctx = <CanvasRenderingContext2D>canvas.getContext('2d');
ctx.fillStyle = '#0f0';
ctx.fillRect(0, canvas.height - 500, canvas.width, 1);
for (let i = 0; i < 500; i++) {
    let x = i / 500;
    let v = hermiteTest(x);
    canvas2DDisplay(ctx, x * canvas.width, canvas.height - v * 500, '#f00');
}