import { Nullable } from "../../base/types";
import { ICoordinateSystem } from "../../coordinate_system/coordinate_sys";
import { Matrix4x4 } from "../../math/matrix4x4";
import { Quaternion } from "../../math/quaternion";
import { Vector3 } from "../../math/vector3";

export interface INode {
    id: string;

    coordinateSys: ICoordinateSystem;

    readonly position: Vector3;

    readonly rotationQuaternion: Quaternion;
    readonly worldMatrix: Matrix4x4;

    readonly absolutePosition: Vector3;
    readonly absoluteScaling: Vector3;
    readonly absoluteRotationQuaternion: Quaternion;

    get parentNode(): Nullable<INode>;

    modifyRotationByEulerAngle(x: number, y: number, z: number): void;
    setParent(value: Nullable<INode>, keepAbsolute?: boolean): void;
    computeWorldMatrix(force?: boolean): Matrix4x4;
    getLocalMatrix(result: Matrix4x4): void;
}

export interface ITransformNode extends INode {
    readonly scaling: Vector3;
}