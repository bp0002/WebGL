import { Bar } from "./progress";

const canvas = (<any>self).renderCanvas;

canvas.width = 800;
canvas.height = 800;

const bar = new Bar({
    canvas,
    bg: '../../../resources/choice_light.png'
});

bar.show('vvv', 100, 10);

setInterval(() => {
    bar.onProcess('', '', 0, Math.abs(Math.sin(Date.now() / 5000) * 100), undefined);
}, 50);