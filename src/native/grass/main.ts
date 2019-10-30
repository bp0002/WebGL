import { RenderLauncher } from "./render_launcher";

const canvas = <HTMLCanvasElement>document.getElementById('your_canvas');

canvas.width = 800;
canvas.height = 800;

RenderLauncher.active(canvas, null);
// setInterval(() => {
//     // bar.onProcess('', '', 0, Math.abs(Math.sin(Date.now() / 5000) * 100), undefined);
// }, 50);