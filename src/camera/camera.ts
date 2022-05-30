import { Node } from "../transform/3d/node";
import { Matrix4x4 } from "../math/matrix4x4";
import { Quaternion } from "../math/quaternion";
import { Vector3 } from "../math/vector3";
import { ECameraMode } from "./base";
import { ICameraOrthoGraphic } from "./orthographic";
import { ICameraPerspective } from "./perspective";
import { LeftHandCoordinateSys3D } from "../coordinate_system/left_coordinate_sys_3d";

export class Camera extends Node implements ICameraPerspective, ICameraOrthoGraphic {
    protected static defaultUp = new Vector3(0, 1, 0);
    protected static rotationMatrix = Matrix4x4.Identity(4);

    public _minZ: number = 1;
    public get minZ(): number {
        return this._minZ;
    }
    public set minZ(value: number) {
        this._minZ = value;
    }

    public _maxZ: number = 1000;
    public get maxZ(): number {
        return this._maxZ;
    }
    public set maxZ(value: number) {
        this._maxZ = value;
    }

    public _size: number = 10;
    public get size(): number {
        return this._size;
    }
    public set size(value: number) {
        this._size = value;
    }

    public _fov: number = 0.9;
    public get fov(): number {
        return this._fov;
    }
    public set fov(value: number) {
        this._fov = value;
    }

    public _mode: ECameraMode = ECameraMode.Perspective;
    public get mode(): ECameraMode {
        return this._mode;
    }
    public set mode(value: ECameraMode) {
        this._mode = value;
    }

    private _projectMatrix: Matrix4x4 = new Matrix4x4();
    private _viewMatrix: Matrix4x4 = new Matrix4x4();

    public readonly upVector: Vector3 = new Vector3(0, 1, 0);

    /**
     * 相机投影在渲染矩形上的区域信息
     */
    public readonly viewport: [number, number, number, number] = [0, 0, 1, 1];

    public getViewMatrix() {
        this.coordinateSys.quaternionToRotationMatrixRef(this._rotationQuaternion, Camera.rotationMatrix);
    }
}