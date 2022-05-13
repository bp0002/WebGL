import { Vector2 } from "../../math/vector2";
import { Matrix3x3, Node } from "./node";

export interface ITransformNode {
    scaling: Vector2;
}

export class TransformNode extends Node implements ITransformNode {
    protected _scaling: Vector2 = new Vector2(1, 1);

    public get scaling() {
        return this._scaling;
    }

    public computeWorldMatrix(force?: boolean): Matrix3x3 {
        if (force || this._rotation._isDirty || this._scaling._isDirty) {
            // TODO
        }

        if (force || this._position._isDirty) {
            // TODO
        }

        return this._worldMatrix;
    }
}