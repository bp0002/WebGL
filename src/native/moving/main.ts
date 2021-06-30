import { Moving } from "./moving";

const canvas = (<any>self).renderCanvas;

canvas.width = 800;
canvas.height = 800;

const bar = new Moving(canvas);