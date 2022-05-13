import { Nullable } from "../../base/types";
import { Matrix4x4 } from "../../math/matrix4x4";
import { Vector3 } from "../../math/vector3";
import { Node } from "./node";

export interface ITransformNode {
    scaling: Vector3;
}

export class TransformNode extends Node implements ITransformNode {
    protected _scaling: Vector3 = new Vector3(1, 1, 1);

    public get scaling() {
        return this._scaling;
    }

    public computeWorldMatrix(force: boolean = false): Matrix4x4 {
        if (force || this._rotationQuaternion._isDirty || this._scaling._isDirty) {
            // TODO
        }

        if (force || this._position._isDirty) {
            // TODO
        }

        return this._worldMatrix;
    }
}