import { Row } from "./row";
import { FloatScalar } from "./scalar";

export class Vector4 extends Row<4> {

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
    public get z() {
        return this._m[2];
    }
    public set z(value: number) {
        if (!FloatScalar.Equal(this._m[2], value)) {
            this._m[2] = value;
            this._isDirty = true;
        }
    }

    /** @hidden */
    public get w() {
        return this._m[3];
    }
    public set w(value: number) {
        if (!FloatScalar.Equal(this._m[3], value)) {
            this._m[3] = value;
            this._isDirty = true;
        }
    }
    constructor(x: number = 0, y: number = 0, z: number = 0, w: number = 0) {
        super(4);

        this._m[0] = x;
        this._m[1] = y;
        this._m[2] = z;
        this._m[3] = w;
    }
}