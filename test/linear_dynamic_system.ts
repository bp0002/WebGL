import { MarkovOne } from "../src/linear_dynamic_system/markov";
import { canvas2DDisplay } from "../src/display/canvas2d_display";
import { FloatScalar } from "../src/math/scalar";
import { display } from "../src/display/html_display";

function breakRow() {
    display('--------------------------------------------------------------<br>');
}

let markov = new MarkovOne();
markov.F = 0.001;
markov.r = -0.001;
markov.deltaH = 0.01;
let markovResult: [number, number][] = [];
let canvas = document.createElement('canvas');
document.body.appendChild(canvas);
canvas.width = 1000;
canvas.height = 500;
let ctx = <CanvasRenderingContext2D>canvas.getContext('2d');
let tempDisplay = (p: number, v: number) => {
    markovResult.push([p, v]);
    canvas2DDisplay(ctx, markovResult.length * markov.deltaH * 50,         500 - (p / 10000) * 500, '#f00');
    canvas2DDisplay(ctx, 500 + markovResult.length * markov.deltaH * 50,   500 - (v / 100) * 500, '#0f0');
};
for (let i = 0; i < 1000; i++) {
    markov.compute(tempDisplay);
}

display(`<br>`);
for (let i = 0; i < 1000; i++) {
    display(FloatScalar.FormatString(i * markov.deltaH, 8, 2, "-") + ", " + FloatScalar.FormatString(markovResult[i][0], 28, 6, "-") + ", " + FloatScalar.FormatString(markovResult[i][1], 28, 6, "-") + '<br>');
}
breakRow();
