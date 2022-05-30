import { Row } from "./row";
import { FloatScalar } from "./scalar";

export type Point3 = Vector3;

export class Vector3 extends Row<3> {

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

    constructor(x: number = 0, y: number = 0, z: number = 0) {
        super(3);

        this._m[0] = x;
        this._m[1] = y;
        this._m[2] = z;

        return this;
    }

    public copyFromFloats(x: number, y: number, z: number) {
        this._m[0] = x;
        this._m[1] = y;
        this._m[2] = z;

        this._isDirty = true;
    }

    public static Cross(a: Vector3, b: Vector3): Vector3 {
        let result = new Vector3(0, 0, 0);

        Vector3.CrossToRef(a, b, result);

        return result;
    }

    public static CrossToRef(a: Vector3, b: Vector3, result: Vector3) {
        let ax = a.x, ay = a.y, az = a.z;
        let bx = b.x, by = b.y, bz = b.z;
        result._m[0] = ay * bz - by * az;
        result._m[1] = az * bx - bz * ax;
        result._m[2] = ax * by - bx * ay;
    }
}