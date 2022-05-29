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

    public static Cross(v: Vector2) {
        let result = new Vector2();
        Vector2.CrossToRef(v, result);
        return result;
    }

    public static CrossToRef(v: Vector2, result: Vector2) {
        let x = v.x;
        let y = v.y;
        result.x = -y;
        result.y = x;
    }

    public static FF(v: Vector2, b: Vector2) {
        return v.x * b.y - b.x * v.y;
    }
}