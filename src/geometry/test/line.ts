import { display } from "../../display/html_display";
import { FloatScalar } from "../../math/scalar";
import { Vector3 } from "../../math/vector3";
import { Line3DPreSegment2Point } from "../line";

let line = new Line3DPreSegment2Point(4, new Vector3(0, 0, 0), new Vector3(0, 1, 0), 2);
let len = line.positions.length / 3;
for (let i = 0; i < len; i++) {
    display(
        `${FloatScalar.FormatString(line.positions[i * 3 + 0], 12, 4)},${FloatScalar.FormatString(line.positions[i * 3 + 1], 12, 4)},${FloatScalar.FormatString(line.positions[i * 3 + 2], 12, 4)}`
    );
}
display(`----------------------------------------------`);
line.updateLastKeyPoint(new Vector3(1, 0, 0), new Vector3(0, 1, 0));
for (let i = 0; i < len; i++) {
    display(
        `${FloatScalar.FormatString(line.positions[i * 3 + 0], 12, 4)},${FloatScalar.FormatString(line.positions[i * 3 + 1], 12, 4)},${FloatScalar.FormatString(line.positions[i * 3 + 2], 12, 4)}`
    );
}
display(`----------------------------------------------`);
line.updateLastKeyPoint(new Vector3(2, 0, 0), new Vector3(0, 0, 1));
for (let i = 0; i < len; i++) {
    display(
        `${FloatScalar.FormatString(line.positions[i * 3 + 0], 12, 4)},${FloatScalar.FormatString(line.positions[i * 3 + 1], 12, 4)},${FloatScalar.FormatString(line.positions[i * 3 + 2], 12, 4)}`
    );
}
display(`----------------------------------------------`);
line.updateLastKeyPoint(new Vector3(3, 0, 0), new Vector3(0, -1, 0));
for (let i = 0; i < len; i++) {
    display(
        `${FloatScalar.FormatString(line.positions[i * 3 + 0], 12, 4)},${FloatScalar.FormatString(line.positions[i * 3 + 1], 12, 4)},${FloatScalar.FormatString(line.positions[i * 3 + 2], 12, 4)}`
    );
}