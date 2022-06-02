import { Row } from "../../math/row";
import { FloatScalar } from "../../math/scalar";

export class Viewport extends Row<4> {

    /** @hidden */
    public get x() {
        return this._m[0];
    }
    public set x(value: number) {
        if (!FloatScalar.Equal(this._m[0], value)) {
            this._m[0] = value;
            this._isDirty = true;
        }
    }

    /** @hidden */
    public get y() {
        return this._m[1];
    }
    public set y(value: number) {
        if (!FloatScalar.Equal(this._m[1], value)) {
            this._m[1] = value;
            this._isDirty = true;
        }
    }

    /** @hidden */
    public get width() {
        return this._m[2];
    }
    public set width(value: number) {
        if (!FloatScalar.Equal(this._m[2], value)) {
            this._m[2] = value;
            this._isDirty = true;
        }
    }

    /** @hidden */
    public get height() {
        return this._m[3];
    }
    public set height(value: number) {
        if (!FloatScalar.Equal(this._m[3], value)) {
            this._m[3] = value;
            this._isDirty = true;
        }
    }
    constructor(x: number = 0, y: number = 0, width: number = 0, height: number = 0) {
        super(4);

        this._m[0] = x;
        this._m[1] = y;
        this._m[2] = width;
        this._m[3] = height;
    }
}