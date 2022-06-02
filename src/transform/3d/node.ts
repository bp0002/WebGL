import { Nullable } from "../../base/types";
import { ICoordinateSystem } from "../../coordinate_system/coordinate_sys";
import { LeftHandCoordinateSys3D } from "../../coordinate_system/left_coordinate_sys_3d";
import { Matrix4x4 } from "../../math/matrix4x4";
import { Quaternion } from "../../math/quaternion";
import { SquareMatrix } from "../../math/square_matrix";
import { Vector3 } from "../../math/vector3";
import { INodeModifier } from "../../modifier/3d/transform/node_modifier";
import { INode } from "./base";

export class Node implements INode {

    public readonly id: string;

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

    public get worldMatrix() {
        return this._worldMatrix;
    }

    protected _parentNode: Nullable<INode> = null;
    public get parentNode() {
        return this._parentNode;
    }

    protected _isAbsoluteSynced: boolean = false;
    protected _absolutePosition: Vector3 = new Vector3();
    protected _absoluteScaling: Vector3 = new Vector3();
    protected _absoluteRotationQuaternion: Quaternion = new Quaternion();
    public get absolutePosition() {
        return this._absolutePosition;
    }
    public get absoluteScaling() {
        this._syncAbsoluteScalingAndRotation();
        return this._absoluteScaling;
    }
    public get absoluteRotationQuaternion() {
        this._syncAbsoluteScalingAndRotation();
        return this._absoluteRotationQuaternion;
    }

    protected _children: Nullable<Node[]> = null;

    public billboardModifier: Nullable<INodeModifier> = null;

    public infinitedDistanceModifier: Nullable<INodeModifier> = null;

    /**
     * 针对局部坐标系的修改器
     * @tip 会在正常的局部坐标系计算之前使用
     */
    public modifierLocalList: INodeModifier[] = [];
    /**
     * 针对世界坐标系的修改器
     * @tip 会在正常的世界坐标系计算之后使用
     */
    public modifierWorldList: INodeModifier[] = [];

    constructor(
        id: string,
        public coordinateSys: ICoordinateSystem
    ) {
        this.id = id;
    }

    public modifyRotationByEulerAngle(x: number, y: number, z: number) {
        this.coordinateSys.rotationYawPitchRollToQuaternion(y, x, z, this._rotationQuaternion);
    }

    public setParent(value: Nullable<INode>, keepAbsolute?: boolean) {
        this._parentNode = value;
    }

    /**
     * 计算世界矩阵
     * @param force 是否强制重新计算
     * @returns 返回节点世界矩阵的引用，外部应当只读
     */
    public computeWorldMatrix(force: boolean = false): Matrix4x4 {
        if (force || this._position._isDirty || this._rotationQuaternion._isDirty) {
            //
        }

        // Modify for local
        let modifierCount = this.modifierLocalList.length;
        for (let i = 0; i < modifierCount; i++) {
            let modifier = this.modifierLocalList[i];
            modifier.modify(this);
        }

        // Compose LocalMatrix
        this.coordinateSys.composeToRef(this._scaling, this._rotationQuaternion, this.position, this._localMatrix);

        // Compute WorldMatrix
        if (this._parentNode) {
            Matrix4x4.MultiplyToRef(this._localMatrix, this._parentNode.worldMatrix, this._worldMatrix);
        } else {
            Matrix4x4.CopyTo(this._localMatrix, this._worldMatrix);
        }

        // Modify for world
        modifierCount = this.modifierWorldList.length;
        for (let i = 0; i < modifierCount; i++) {
            let modifier = this.modifierWorldList[i];
            modifier.modify(this);
        }

        // 解出全局坐标系下 的三元信息
        this._isAbsoluteSynced = false;
        this.coordinateSys.decompose(this._worldMatrix, undefined, undefined, this._absolutePosition);

        return this._worldMatrix;
    }

    public getLocalMatrix(result: Matrix4x4) {
        this.coordinateSys.composeToRef(this._scaling, this._rotationQuaternion, this.position, result);
    }

    protected _syncAbsoluteScalingAndRotation() {
        if (!this._isAbsoluteSynced) {
            this.coordinateSys.decompose(this._worldMatrix, this._absoluteScaling, this._absoluteRotationQuaternion, undefined);
            this._isAbsoluteSynced = true;
        }
    }

    public dispose() {
        this._scaling.dispose();
        this._position.dispose();
        this._rotationQuaternion.dispose();
        this._localMatrix.dispose();
        this._worldMatrix.dispose();

        this.modifierLocalList.forEach((modifier) => {
            if (!modifier.forCache) {
                modifier.dispose();
            }
        });
        this.modifierLocalList.length = 0;

        this.modifierWorldList.forEach((modifier) => {
            if (!modifier.forCache) {
                modifier.dispose();
            }
        });
        this.modifierWorldList.length = 0;

        (<any>this)._scaling = null;
        (<any>this)._position = null;
        (<any>this)._rotationQuaternion = null;
        (<any>this)._localMatrix = null;
        (<any>this)._worldMatrix = null;
        (<any>this).modifierLocalList = null;
        (<any>this).modifierWorldList = null;
    }
}