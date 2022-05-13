import { Matrix } from "./matrix";
import { Row } from "./row";

export class Quaternion extends Row<4> {

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

    /** @hidden */
    public get z() {
        return this._m[2];
    }
    public set z(value: number) {
        if (!this._isEqual(this._m[2], value)) {
            this._m[2] = value;

            this._isDirty = true;
        }
    }

    /** @hidden */
    public get w() {
        return this._m[3];
    }
    public set w(value: number) {
        if (!this._isEqual(this._m[3], value)) {
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
    }

    public static InverseToRef(source: Quaternion, result: Quaternion) {
        result._m[0] = -source._m[0];
        result._m[1] = -source._m[1];
        result._m[2] = -source._m[2];
        result._m[3] = +source._m[3];
    }

    /**
     * 欧拉角(弧度)转换到四元数
     * @param x Pitch 弧度
     * @param y Yaw 弧度
     * @param z Roll 弧度
     * @param result Quaternion
     */
    public static EulerAnglesToRef(x: number, y: number, z: number, result: Quaternion) {
        Quaternion.RotationYawPitchRollToRef(y, x, z, result);
    }

    /**
     * 欧拉角(弧度)转换到四元数 (in the z-y-x orientation (Tait-Bryan angles))
     * @param yaw rotation around Y axis
     * @param pitch rotation around X axis
     * @param roll rotation around Z aixs
     * @param result quaternion
     */
    public static RotationYawPitchRollToRef(yaw: number, pitch: number, roll: number, result: Quaternion) {
        let halfRoll    = roll  * 0.5;
        let halfPitch   = pitch * 0.5;
        let halfYaw     = yaw   * 0.5;

        let sinRoll     = Math.sin(halfRoll);
        let cosRoll     = Math.cos(halfRoll);
        let sinPitch    = Math.sin(halfPitch);
        let cosPitch    = Math.cos(halfPitch);
        let sinYaw      = Math.sin(halfYaw);
        let cosYaw      = Math.cos(halfYaw);

        result._m[0]    = (cosYaw * sinPitch * cosRoll) + (sinYaw * cosPitch * sinRoll);
        result._m[1]    = (sinYaw * cosPitch * cosRoll) - (cosYaw * sinPitch * sinRoll);
        result._m[2]    = (cosYaw * cosPitch * sinRoll) - (sinYaw * sinPitch * cosRoll);
        result._m[3]    = (cosYaw * cosPitch * cosRoll) + (sinYaw * sinPitch * sinRoll);
    }

    public static RotationAlphaBetaGammaToRef(alpha: number, beta: number, gamma: number, result: Quaternion) {
        let halfGammaPlusAlpha  = (gamma + alpha) * 0.5;
        let halfGammaMinusAlpha = (gamma - alpha) * 0.5;
        let halfBeta            = beta * 0.5;

        result._m[0]    = Math.cos(halfGammaMinusAlpha) * Math.sin(halfBeta);
        result._m[1]    = Math.sin(halfGammaMinusAlpha) * Math.sin(halfBeta);
        result._m[2]    = Math.sin(halfGammaPlusAlpha)  * Math.cos(halfBeta);
        result._m[3]    = Math.cos(halfGammaPlusAlpha)  * Math.cos(halfBeta);
    }

    public static TransformToEularAnglesRef(source: Quaternion, result: Row<3>) {
        let qz = source.z, qx = source.x, qy = source.y, qw = source.w;
        let sqw = qw * qw, sqx = qx * qx, sqy = qy * qy, sqz = qz * qz;

        let zAxisY = qy * qz - qx * qw;
        let limit = 0.4999999;

        if (zAxisY < -limit) {
            result.m[0] = 2 * Math.atan2(qy, qw);
            result.m[1] = Math.PI / 2;
            result.m[2] = 0;
        } else if (zAxisY > limit) {
            result.m[0] = 2 * Math.atan2(qy, qw);
            result.m[1] = -Math.PI / 2;
            result.m[2] = 0;
        } else {
            result.m[0] = Math.atan2(2.0 * (qx * qy + qz + qw), (-sqz - sqx + sqy + sqw));
            result.m[1] = Math.asin(-2.0 * (qz * qy - qz * qw));
            result.m[2] = Math.atan2(2.0 * (qz * qx + qy * qw), (sqz - sqx - sqy + sqw));
        }
    }

    public static TransformToMatrixRef(quat: Quaternion, result: Matrix<4, 4>) {
        var xx = quat.x * quat.x;
        var yy = quat.y * quat.y;
        var zz = quat.z * quat.z;
        var xy = quat.x * quat.y;
        var zw = quat.z * quat.w;
        var zx = quat.z * quat.x;
        var yw = quat.y * quat.w;
        var yz = quat.y * quat.z;
        var xw = quat.x * quat.w;

        result.m[0] = 1.0 - (2.0 * (yy + zz));
        result.m[1] = 2.0 * (xy + zw);
        result.m[2] = 2.0 * (zx - yw);
        result.m[3] = 0.0;

        result.m[4] = 2.0 * (xy - zw);
        result.m[5] = 1.0 - (2.0 * (zz + xx));
        result.m[6] = 2.0 * (yz + xw);
        result.m[7] = 0.0;

        result.m[8] = 2.0 * (zx + yw);
        result.m[9] = 2.0 * (yz - xw);
        result.m[10] = 1.0 - (2.0 * (yy + xx));
        result.m[11] = 0.0;

        result.m[12] = 0.0;
        result.m[13] = 0.0;
        result.m[14] = 0.0;
        result.m[15] = 1.0;

        result._isDirty = true;
    }
}