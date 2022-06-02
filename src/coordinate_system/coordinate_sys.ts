import { Matrix4x4 } from "../math/matrix4x4";
import { Quaternion } from "../math/quaternion";
import { Vector3 } from "../math/vector3";

export interface ICoordinateSystem {
    tempMatrix4x4A: Matrix4x4;
    tempMatrix4x4B: Matrix4x4;
    tempMatrix4x4C: Matrix4x4;
    tempMatrix4x4D: Matrix4x4;
    tempQuaternionA: Vector3;
    tempQuaternionB: Vector3;
    tempQuaternionC: Vector3;
    tempQuaternionD: Vector3;
    tempVector3A: Vector3;
    tempVector3B: Vector3;
    tempVector3C: Vector3;
    tempVector3D: Vector3;
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
     * 四元数转换为欧拉角
     * @param quat 四元数
     * @param result 结果
     * @link https://en.wikipedia.org/wiki/Conversion_between_quaternions_and_Euler_angles
     * @link http://www.euclideanspace.com/maths/geometry/rotations/conversions/quaternionToEuler/
     */
    quaternionToEulerAngles(quat: Quaternion, result: Vector3): void;
    /**
     * 欧拉角(弧度)转换到四元数
     * @param x Pitch 弧度
     * @param y Yaw 弧度
     * @param z Roll 弧度
     * @param result Quaternion
     */
    eulerAnglesToQuaternion(x: number, y: number, z: number, result: Quaternion): void;
    /**
     * 欧拉角(弧度)转换到四元数 (in the z-y-x orientation (Tait-Bryan angles))
     * @param yaw rotation around Y axis
     * @param pitch rotation around X axis
     * @param roll rotation around Z aixs
     * @param result quaternion
     */
    rotationYawPitchRollToQuaternion(yaw: number, pitch: number, roll: number, result: Quaternion): void;
    /**
     * Creates a new quaternion from the given Euler float angles expressed in z-x-z orientation
     * @param alpha defines the rotation around first axis
     * @param beta defines the rotation around second axis
     * @param gamma defines the rotation around third axis
     * @param result quaternion
     */
    rotationAlphaBetaGammaToQuaternion(alpha: number, beta: number, gamma: number, result: Quaternion): void;
    /**
     * 将旋转矩阵转换为四元数
     * @param source 源旋转矩阵
     * @param result 结果四元数
     */
    rotationMatrixToQuaternion(source: Matrix4x4, result: Quaternion): void;

    /**
     * 对目标向量实施指定矩阵变换
     * @param x 坐标X
     * @param y 坐标y
     * @param z 坐标z
     * @param transformation 目标变换矩阵
     * @param result 结果坐标
     */
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

    /**
     * 从目标矩阵获取对应旋转矩阵
     * @param source 源矩阵
     * @param result 结果矩阵
     */
    getRotationMatrixFromMatrix(source: Matrix4x4, result: Matrix4x4): void;
    lookAtToViewMatrix(eye: Vector3, target: Vector3, up: Vector3, result: Matrix4x4): void;
}