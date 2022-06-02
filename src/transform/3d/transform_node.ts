import { Vector3 } from "../../math/vector3";
import { ITransformNode } from "./base";
import { Node } from "./node";

export class TransformNode extends Node implements ITransformNode {
    protected _scaling: Vector3 = new Vector3(1, 1, 1);

    public get scaling() {
        return this._scaling;
    }
}