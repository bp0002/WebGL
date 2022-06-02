import { Matrix } from "../math/matrix";
import { Matrix4x4 } from "../math/matrix4x4";
import { Quaternion } from "../math/quaternion";
import { SquareMatrix } from "../math/square_matrix";
import { Vector3 } from "../math/vector3";
import { ICoordinateSystem } from "./coordinate_sys";

export class LeftHandCoordinateSys3D implements ICoordinateSystem {
    public tempMatrix4x4A = new Matrix4x4();
    public tempMatrix4x4B = new Matrix4x4();
    public tempMatrix4x4C = new Matrix4x4();
    public tempMatrix4x4D = new Matrix4x4();
    public tempQuaternionA = new Vector3();
    public tempQuaternionB = new Vector3();
    public tempQuaternionC = new Vector3();
    public tempQuaternionD = new Vector3();
    public tempVector3A = new Vector3();
    public tempVector3B = new Vector3();
    public tempVector3C = new Vector3();
    public tempVector3D = new Vector3();
    getRotationMatrixWithAixsAndAngle(axisDirection: Vector3, angleRadians: number, result: Matrix4x4): void {
        let cosA = Math.cos(angleRadians);
        let one_cosA = 1.0 - cosA;
        let sinA = Math.sin(angleRadians);

        let x = axisDirection.x;
        let y = axisDirection.y;
        let z = axisDirection.z;

        let xx = x * x, xy = x * y, xz = x * z, yy = y * y, yz = y * z, zz = z * z;

        result.m[Matrix.RowMajorOrder(1, 1, 4, 4)] = cosA * 1 + one_cosA * xx + sinA * (0);
        result.m[Matrix.RowMajorOrder(1, 2, 4, 4)] = cosA * 0 + one_cosA * xy + sinA * (-z);
        result.m[Matrix.RowMajorOrder(1, 3, 4, 4)] = cosA * 0 + one_cosA * xz + sinA * (y);
        result.m[Matrix.RowMajorOrder(2, 1, 4, 4)] = cosA * 0 + one_cosA * xy + sinA * (z);
        result.m[Matrix.RowMajorOrder(2, 2, 4, 4)] = cosA * 1 + one_cosA * yy + sinA * (0);
        result.m[Matrix.RowMajorOrder(2, 3, 4, 4)] = cosA * 0 + one_cosA * yz + sinA * (-x);
        result.m[Matrix.RowMajorOrder(3, 1, 4, 4)] = cosA * 0 + one_cosA * xz + sinA * (-y);
        result.m[Matrix.RowMajorOrder(3, 2, 4, 4)] = cosA * 0 + one_cosA * yz + sinA * (x);
        result.m[Matrix.RowMajorOrder(3, 3, 4, 4)] = cosA * 1 + one_cosA * zz + sinA * (0);

        result._isDirty = true;
    }
    vector3RotateWithAxisAndAngle(axisDirection: Vector3, angleRadians: number, source: Vector3, result: Vector3): void {
        let cosA = Math.cos(angleRadians);
        let one_cosA = 1.0 - cosA;
        let sinA = Math.sin(angleRadians);

        let tempV = new Vector3(axisDirection.x, axisDirection.y, axisDirection.z);
        Vector3.CrossToRef(tempV, source, tempV);

        Vector3.ScaleToRef(tempV, sinA, result);

        let dot = Vector3.Dot(axisDirection, source);
        Vector3.ScaleToRef(axisDirection, dot * one_cosA, tempV);
        Vector3.AddToRef(result, tempV, result);

        Vector3.ScaleToRef(source, cosA, tempV);
        Vector3.AddToRef(result, tempV, result);

        tempV.dispose();
    }
    decompose(target: Matrix4x4, scaling?: Vector3, rotation?: Quaternion, translation?: Vector3): boolean {
        let result = true;

        if (target.isIdentity()) {
            if (translation) {
                translation.x = 0;
                translation.y = 0;
                translation.z = 0;
            }
            if (scaling) {
                scaling.x = 1;
                scaling.y = 1;
                scaling.z = 1;
            }
            if (rotation) {
                rotation.x = 0;
                rotation.y = 0;
                rotation.z = 0;
                rotation.w = 1;
            }

            return true;
        }

        const m = target.m;

        if (translation) {
            translation.x = m[12];
            translation.y = m[13];
            translation.z = m[14];
        }

        let sx = 1, sy = 1, sz = 1;
        let   m11 = m[0], m12 = m[1], m13 = m[2]
            , m21 = m[4], m22 = m[5], m23 = m[6]
            , m31 = m[8], m32 = m[9], m33 = m[10];

        sx = Math.sqrt(m11 * m11 + m12 * m12 + m13 * m13);
        sy = Math.sqrt(m21 * m21 + m22 * m22 + m23 * m23);
        sz = Math.sqrt(m31 * m31 + m32 * m32 + m33 * m33);

        if (Matrix4x4.Determinant(target) <= 0) {
            sy *= -1;
        }

        if (scaling) {
            scaling.x = sx;
            scaling.y = sy;
            scaling.z = sz;
        }

        if (sx === 0 || sy === 0 || sz === 0) {
            if (rotation) {
                rotation.x = 0;
                rotation.y = 0;
                rotation.z = 0;
                rotation.w = 1;
            }

            return false;
        }

        if (rotation) {
            sx = 1 / sx, sy = 1 / sy; sz = 1 / sz;
            let tempM = this.tempMatrix4x4A.m;
            // tempM[ 0] = m[ 0] * sx, tempM[ 1] = m[ 1] * sx, tempM[ 2] = m[ 2] * sx, tempM[ 3] = 0,
            // tempM[ 4] = m[ 4] * sy, tempM[ 5] = m[ 5] * sy, tempM[ 6] = m[ 6] * sy, tempM[ 7] = 0,
            // tempM[ 8] = m[ 8] * sz, tempM[ 9] = m[ 9] * sz, tempM[10] = m[10] * sz, tempM[11] = 0,
            // tempM[12] = 0,          tempM[13] = 0,          tempM[14] = 0,          tempM[15] = 1;

            tempM[ 0] = m[ 0] * sx, tempM[ 1] = m[ 1] * sx, tempM[ 2] = m[ 2] * sx, tempM[ 3] = 0,
            tempM[ 4] = m[ 4] * sy, tempM[ 5] = m[ 5] * sy, tempM[ 6] = m[ 6] * sy, tempM[ 7] = 0,
            tempM[ 8] = m[ 8] * sz, tempM[ 9] = m[ 9] * sz, tempM[10] = m[10] * sz, tempM[11] = 0,
            tempM[12] = 0,          tempM[13] = 0,          tempM[14] = 0,          tempM[15] = 1;

            this.tempMatrix4x4A._isDirty = true;

            this.rotationMatrixToQuaternion(this.tempMatrix4x4A, rotation);
        }

        return result;
    }
    composeToRef(scaling: Vector3, rotation: Quaternion, translation: Vector3, result: Matrix4x4): void {
        let m = result.m;
        let x = rotation.x, y = rotation.y, z = rotation.z, w = rotation.w;
        let x2 = x + x, y2 = y + y, z2 = z + z;
        let xx = x * x2, xy = x * y2, xz = x * z2;
        let yy = y * y2, yz = y * z2, zz = z * z2;
        let wx = w * x2, wy = w * y2, wz = w * z2;

        let sx = scaling.x, sy = scaling.y, sz = scaling.z;

        m[0] = (1 - (yy + zz)) * sx;
        m[1] = (xy + wz) * sx;
        m[2] = (xz - wy) * sx;
        m[3] = 0;

        m[4] = (xy - wz) * sy;
        m[5] = (1 - (xx + zz)) * sy;
        m[6] = (yz + wx) * sy;
        m[7] = 0;

        m[8] = (xz + wy) * sz;
        m[9] = (yz - wx) * sz;
        m[10] = (1 - (xx + yy)) * sz;
        m[11] = 0;

        m[12] = translation.x;
        m[13] = translation.y;
        m[14] = translation.z;
        m[15] = 1;

        result._isDirty = true;
    }
    quaternionToRotationMatrixRef(quat: Quaternion, result: Matrix4x4): void {
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
    quaternionToEulerAngles(quat: Quaternion, result: Vector3) {
        let qz = quat.z, qx = quat.x, qy = quat.y, qw = quat.w;
        let sqw = qw * qw, sqx = qx * qx, sqy = qy * qy, sqz = qz * qz;

        let zAxisY = qy * qz - qx * qw;
        let limit = 0.4999999;

        if (zAxisY < -limit) {
            result.m[1] = 2 * Math.atan2(qy, qw);
            result.m[0] = Math.PI / 2;
            result.m[2] = 0;
        } else if (zAxisY > limit) {
            result.m[1] = 2 * Math.atan2(qy, qw);
            result.m[0] = -Math.PI / 2;
            result.m[2] = 0;
        } else {
            result.m[2] = Math.atan2(2.0 * (qx * qy + qz * qw), (-sqz - sqx + sqy + sqw));
            result.m[0] = Math.asin(-2.0 * (qz * qy - qx * qw));
            result.m[1] = Math.atan2(2.0 * (qz * qx + qy * qw), (sqz - sqx - sqy + sqw));
        }

        result._isDirty = true;

        // let qx = quat.x, qy = quat.y, qz = quat.z, qw = quat.w;

        // let sqx = qx * qx, sqy = qy * qy, sqz = qz * qz, sqw = qw * qw;

        // let zAxisY = qx * qy + qz * qw;

        // let limit = 0.49999999;

        // if (zAxisY < -limit) {
        //     result.z = - 2 * Math.atan2(qx, qw);
        //     result.y = Math.asin(2 * qx * qy + 2 * qz * qw);
        //     result.x = 0;
        // } else if (zAxisY > limit) {
        //     result.z = 2 * Math.atan2(qx, qw);
        //     result.y = Math.asin(2 * qx * qy + 2 * qz * qw);
        //     result.x = 0;
        // } else {
        //     result.z = Math.atan2(2 * (qy * qw - qx * qz), 1 - 2 * (sqy - sqz));
        //     result.y = Math.asin(2 * qx * qy + 2 * qz * qw);
        //     result.x = Math.atan2(2 * (qx * qw - qy * qz), 1 - 2 * (sqx - sqz));
        // }
    }
    eulerAnglesToQuaternion(x: number, y: number, z: number, result: Quaternion) {
        this.rotationYawPitchRollToQuaternion(y, x, z, result);
    }
    rotationYawPitchRollToQuaternion(yaw: number, pitch: number, roll: number, result: Quaternion) {
        let halfRoll    = roll  * 0.5;
        let halfPitch   = pitch * 0.5;
        let halfYaw     = yaw   * 0.5;

        let sinRoll     = Math.sin(halfRoll);
        let cosRoll     = Math.cos(halfRoll);
        let sinPitch    = Math.sin(halfPitch);
        let cosPitch    = Math.cos(halfPitch);
        let sinYaw      = Math.sin(halfYaw);
        let cosYaw      = Math.cos(halfYaw);

        result.x    = (cosYaw * sinPitch * cosRoll) + (sinYaw * cosPitch * sinRoll);
        result.y    = (sinYaw * cosPitch * cosRoll) - (cosYaw * sinPitch * sinRoll);
        result.z    = (cosYaw * cosPitch * sinRoll) - (sinYaw * sinPitch * cosRoll);
        result.w    = (cosYaw * cosPitch * cosRoll) + (sinYaw * sinPitch * sinRoll);
    }
    rotationAlphaBetaGammaToQuaternion(alpha: number, beta: number, gamma: number, result: Quaternion) {
        let halfGammaPlusAlpha  = (gamma + alpha) * 0.5;
        let halfGammaMinusAlpha = (gamma - alpha) * 0.5;
        let halfBeta            = beta * 0.5;

        result.m[0]    = Math.cos(halfGammaMinusAlpha) * Math.sin(halfBeta);
        result.m[1]    = Math.sin(halfGammaMinusAlpha) * Math.sin(halfBeta);
        result.m[2]    = Math.sin(halfGammaPlusAlpha)  * Math.cos(halfBeta);
        result.m[3]    = Math.cos(halfGammaPlusAlpha)  * Math.cos(halfBeta);

        result._isDirty = true;
    }
    rotationMatrixToQuaternion(source: Matrix4x4, result: Quaternion): void {
        const m = source.m;
        let   m11 = m[0], m12 = m[4], m13 = m[8]
            , m21 = m[1], m22 = m[5], m23 = m[9]
            , m31 = m[2], m32 = m[6], m33 = m[10];

        let trace = m11 + m22 + m33;
        let s = 0;

        if (trace > 0) {
            s = 0.5 / Math.sqrt(trace + 1.0);

            result.w = 0.25 / s;
            result.x = (m32 - m23) * s;
            result.y = (m13 - m31) * s;
            result.z = (m21 - m12) * s;
        } else if (m11 > m22 && m11 > m33) {
            s = 2.0 * Math.sqrt(1.0 + m11 - m22 - m33);

            result.w = (m32 - m23) / s;
            result.x = 0.25 * s;
            result.y = (m12 + m21) / s;
            result.z = (m13 + m31) / s;
        } else if (m22 > m33) {
            s = 2.0 * Math.sqrt(1.0 + m22 - m11 - m33);

            result.w = (m13 - m31) / s;
            result.x = (m12 + m21) / s;
            result.y = 0.25 * s;
            result.z = (m23 + m32) / s;
        } else {
            s = 2.0 * Math.sqrt(1.0 + m33 - m11 - m22);

            result.w = (m21 - m12) / s;
            result.x = (m13 + m31) / s;
            result.y = (m23 + m32) / s;
            result.z = 0.25 * s;
        }
    }
    transformCoordinatesFromFloatsToRef(x: number, y: number, z: number, transformation: Matrix4x4, result: Vector3): void {
        let m = transformation.m;

        let rx = x * m[ 0] + y * m[ 4] + z * m[ 8] + m[12];
        let ry = x * m[ 1] + y * m[ 5] + z * m[ 9] + m[13];
        let rz = x * m[ 2] + y * m[ 6] + z * m[10] + m[14];

        let rw = 1 / (x * m[ 3] + y * m[ 7] + z * m[ 11] + m[15]);

        result.x = rx * rw;
        result.y = ry * rw;
        result.z = rz * rw;
    }

    public directionToQuaternion(direction: Vector3, quaternion: Quaternion, yawCor: number = 0, pitchCor: number = 0, rollCor: number = 0) {
        let yaw = -Math.atan2(direction.z, direction.x) + Math.PI / 2;
        let len = Math.sqrt(direction.x * direction.x + direction.z * direction.z);
        let pitch = -Math.atan2(direction.y, len);

        this.rotationYawPitchRollToQuaternion(yaw + yawCor, pitch + pitchCor, rollCor, quaternion);
    }

    public getRotationMatrixFromMatrix(source: Matrix4x4, result: Matrix4x4): void {
        if (this.decompose(source, this.tempVector3A, undefined, undefined)) {
            SquareMatrix.IdentityToRef(result);
            return;
        } else {
            let m = source.m;
            let tempM = result.m;
            let sx = 1 / this.tempVector3A.x, sy = 1 / this.tempVector3A.y, sz = 1 / this.tempVector3A.z;

            tempM[ 0] = m[ 0] * sx, tempM[ 1] = m[ 1] * sx, tempM[ 2] = m[ 2] * sx, tempM[ 3] = 0,
            tempM[ 4] = m[ 4] * sy, tempM[ 5] = m[ 5] * sy, tempM[ 6] = m[ 6] * sy, tempM[ 7] = 0,
            tempM[ 8] = m[ 8] * sz, tempM[ 9] = m[ 9] * sz, tempM[10] = m[10] * sz, tempM[11] = 0,
            tempM[12] = 0,          tempM[13] = 0,          tempM[14] = 0,          tempM[15] = 1;

            result._isDirty = true;
        }
    }
    lookAtToViewMatrix(eye: Vector3, target: Vector3, up: Vector3, result: Matrix4x4): void {
        let xAxis = this.tempVector3A;
        let yAxis = this.tempVector3B;
        let zAxis = this.tempVector3C;

        Vector3.SubstractToRef(target, eye, zAxis);
        Vector3.NormalizeToRef(zAxis, zAxis);

        Vector3.CrossToRef(up, zAxis, xAxis);
        Vector3.NormalizeToRef(xAxis, xAxis);

        Vector3.CrossToRef(zAxis, xAxis, yAxis);
        Vector3.NormalizeToRef(yAxis, yAxis);

        let ex = -Vector3.Dot(xAxis, eye);
        let ey = -Vector3.Dot(yAxis, eye);
        let ez = -Vector3.Dot(zAxis, eye);

        let m = result.m;
        m[ 0] = xAxis.x, m[ 1] = yAxis.x, m[ 2] = zAxis.x, m[ 3] = 0,
        m[ 4] = xAxis.y, m[ 5] = yAxis.y, m[ 6] = zAxis.y, m[ 7] = 0,
        m[ 8] = xAxis.z, m[ 9] = yAxis.z, m[10] = zAxis.z, m[11] = 0,
        m[12] = ex,      m[13] = ey,      m[14] = ez,      m[15] = 0;

        result._isDirty = true;
    }
}