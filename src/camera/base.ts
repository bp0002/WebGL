import { Viewport } from "../renderer/3d/viewport";
import { Matrix4x4 } from "../math/matrix4x4";
import { Vector3 } from "../math/vector3";
import { INode } from "../transform/3d/base";

export interface ICamera extends INode {
    /**
     * @tip 受 worldmatrix 变化, minz
     */
    readonly viewMatrix: Matrix4x4;
    readonly projectionMatrix: Matrix4x4;
    readonly rotationMatrix: Matrix4x4;
}