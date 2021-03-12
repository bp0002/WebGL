import { Wave2D } from "./progress";

const canvas = (<any>self).renderCanvas;

canvas.width = 800;
canvas.height = 800;

const bar = new Wave2D(canvas);

// setInterval(() => {
//     bar.onProcess('', '', 0, Math.abs(Math.sin(Date.now() / 5000) * 100), undefined);
// }, 50);