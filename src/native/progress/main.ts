import { Bar } from "./progress";

const canvas = (<any>self).renderCanvas;

canvas.width = 1024;
canvas.height = 1024;

const bar = new Bar({
    canvas,
    bg: '../../../resources/texture.png'
});

bar.show('vvv', 100, 10);

setInterval(() => {
    bar.onProcess('', '', 0, Math.abs(Math.sin(Date.now() / 5000) * 100), undefined);
}, 50);