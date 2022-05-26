import { Nullable } from "../../base/types";
import { Matrix4x4 } from "../../math/matrix4x4";
import { Quaternion } from "../../math/quaternion";
import { SquareMatrix } from "../../math/square_matrix";
import { Vector3 } from "../../math/vector3";

export interface INode {
    id: string;
    position: Vector3;
    rotationQuaternion: Quaternion;
    parentNode: Nullable<INode>;

    modifyRotationByEulerAngle(x: number, y: number, z: number): void;
    setParent(value: Nullable<INode>, keepAbsolute?: boolean): void;
    computeWorldMatrix(force?: boolean): Matrix4x4;
    getLocalMatrix(result: Matrix4x4): void;
}

export class Node implements INode {
    protected _position: Vector3 = new Vector3();
    public get position(): Vector3 {
        return this._position;
    }

    protected _scaling: Vector3 = new Vector3(1, 1, 1);

    protected _rotationQuaternion: Quaternion = new Quaternion();
    public get rotationQuaternion(): Quaternion {
        return this._rotationQuaternion;
    }

    protected _localMatrix: Matrix4x4 = SquareMatrix.Identity(4);
    protected _worldMatrix: Matrix4x4 = SquareMatrix.Identity(4);

    public readonly id: string;

    protected _parentNode: Nullable<Node> = null;
    public get parentNode() {
        return this._parentNode;
    }

    protected _children: Nullable<Node[]> = null;

    constructor(id: string) {
        this.id = id;
    }

    public modifyRotationByEulerAngle(x: number, y: number, z: number) {

    }

    public setParent(value: Nullable<Node>, keepAbsolute?: boolean) {
        this._parentNode = value;
    }

    public computeWorldMatrix(force: boolean = false): Matrix4x4 {
        if (force || this._position._isDirty || this._rotationQuaternion._isDirty) {
            //
        }

        if (this._parentNode) {
            Matrix4x4.MultiplyToRef(this._localMatrix, this._parentNode._worldMatrix, this._worldMatrix);
        } else {
            Matrix4x4.CopyFrom(this._localMatrix, this._worldMatrix);
        }

        return this._worldMatrix;
    }

    public getLocalMatrix(result: Matrix4x4) {

    }
}