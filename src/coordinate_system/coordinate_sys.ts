import { Matrix4x4 } from "../math/matrix4x4";
import { Quaternion } from "../math/quaternion";
import { Vector3 } from "../math/vector3";

export interface ICoordinateSystem {
    /**
     * 获取绕任意轴旋转指定角度的旋转矩阵表达
     * @param axisDirection 旋转轴
     * @param angleRadians 旋转弧度
     * @param result 结果矩阵
     */
    getRotationMatrixWithAixsAndAngle(axisDirection: Vector3, angleRadians: number, result: Matrix4x4): void;

    /**
     * 计算目标向量绕轴旋转指定角度后的结果
     * @link https://en.wikipedia.org/wiki/Rodrigues'_rotation_formula
     * @param axisDirection 旋转轴
     * @param angleRadians 旋转弧度
     * @param source 源向量
     * @param result 结果向量
     */
    vector3RotateWithAxisAndAngle(axisDirection: Vector3, angleRadians: number, source: Vector3, result: Vector3): void;

    /**
     * 拆解矩阵数据
     * @param target 目标矩阵
     * @param scaling 缩放
     * @param rotation 旋转四元数
     * @param translation 位移
     * @returns 是否成功
     */
    decompose(target: Matrix4x4, scaling?: Vector3, rotation?: Quaternion, translation?: Vector3): boolean;

    /**
     * Sets a matrix to a value composed by merging scale (vector3), rotation (quaternion) and translation (vector3)
     * @param scaling defines the scale vector3
     * @param rotation defines the rotation quaternion
     * @param translation defines the translation vector3
     * @param result defines the target matrix
     */
    composeToRef(scaling: Vector3, rotation: Quaternion, translation: Vector3, result: Matrix4x4): void;

    /**
     * 四元数转换为旋转矩阵
     * @param quat 四元数
     * @param result 结果矩阵
     */
    quaternionToRotationMatrixRef(quat: Quaternion, result: Matrix4x4): void;

    /**
     * 将旋转矩阵转换为四元数
     * @param source 源旋转矩阵
     * @param result 结果四元数
     */
    rotationMatrixToQuaternion(source: Matrix4x4, result: Quaternion): void;

    transformCoordinatesFromFloatsToRef(x: number, y: number, z: number, transformation: Matrix4x4, result: Vector3): void;

    /**
     * Sets this transform node rotation to the given local axis.
     * @param direction the axis in local space
     * @param quaternion result quaternion
     * @param yawCor optional yaw (y-axis) correction in radians
     * @param pitchCor optional pitch (x-axis) correction in radians
     * @param rollCor optional roll (z-axis) correction in radians
     * @returns this TransformNode
     */
    directionToQuaternion(direction: Vector3, quaternion: Quaternion, yawCor: number, pitchCor: number, rollCor: number): void;

    getRotationMatrixFromMatrix(source: Matrix4x4, result: Matrix4x4): void;
}