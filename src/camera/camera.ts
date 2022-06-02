import { Node } from "../transform/3d/node";
import { Matrix4x4 } from "../math/matrix4x4";
import { Vector3 } from "../math/vector3";
import { ICamera } from "./base";
import { Viewport } from "../renderer/3d/viewport";

export class Camera extends Node implements ICamera {

    public readonly viewMatrix: Matrix4x4 = Matrix4x4.Identity(4);

    public readonly projectionMatrix: Matrix4x4 = Matrix4x4.Identity(4);

    public readonly rotationMatrix = Matrix4x4.Identity(4);

    public getViewMatrix() {
        this.coordinateSys.quaternionToRotationMatrixRef(this._rotationQuaternion, this.rotationMatrix);
    }
}