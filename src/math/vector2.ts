import { Row } from "./row";

export type Point2 = Vector2;

export class Vector2 extends Row<2> {
    /** @hidden */
    public _isEqual(oldValue: number, newValue: number) {
        return oldValue != newValue;
    }

    /** @hidden */
    public get x() {
        return this._m[0];
    }
    public set x(value: number) {
        if (!this._isEqual(this._m[0], value)) {
            this._m[0] = value;
            this._isDirty = true;
        }
    }

    /** @hidden */
    public get y() {
        return this._m[1];
    }
    public set y(value: number) {
        if (!this._isEqual(this._m[1], value)) {
            this._m[1] = value;
            this._isDirty = true;
        }
    }

    constructor(x: number = 0, y: number = 0) {
        super(2);

        this._m[0] = x;
        this._m[1] = y;
    }

    public static Cross(a: Vector2, b: Vector2): number {
        return a.x * b.y - b.x * a.y;
    }
}