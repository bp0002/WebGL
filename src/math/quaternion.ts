import { Row } from "./row";
import { FloatScalar } from "./scalar";

export class Quaternion extends Row<4> {

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

    constructor(x: number = 0, y: number = 0, z: number = 0, w: number = 1) {
        super(4);

        this._m[0] = x;
        this._m[1] = y;
        this._m[2] = z;
        this._m[3] = w;
    }

    public isIdentity(): boolean {
        return this._m[0] === 0 && this._m[1] === 0 && this._m[2] === 0 && this._m[3] === 1;
    }

    public static IsIdentity(quaternion: Quaternion): boolean {
        return quaternion && quaternion._m[0] === 0 && quaternion._m[1] === 0 && quaternion._m[2] === 0 && quaternion._m[3] === 1;
    }

    public static Identity() {
        return new Quaternion(0, 0, 0, 1);
    }

    public static IdentityToRef(result: Quaternion) {
        result._m[0] = 0;
        result._m[1] = 0;
        result._m[2] = 0;
        result._m[3] = 1;

        result._isDirty = true;
    }

    public static InverseToRef(source: Quaternion, result: Quaternion) {
        result._m[0] = -source._m[0];
        result._m[1] = -source._m[1];
        result._m[2] = -source._m[2];
        result._m[3] = +source._m[3];

        result._isDirty = true;
    }
}