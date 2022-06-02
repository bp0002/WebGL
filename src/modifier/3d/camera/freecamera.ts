import { Nullable } from "../../../base/types";
import { ICamera } from "../../../camera/base";
import { ICoordinateSystem } from "../../../coordinate_system/coordinate_sys";
import { Viewport } from "../../../renderer/3d/viewport";
import { Matrix4x4 } from "../../../math/matrix4x4";
import { Vector3 } from "../../../math/vector3";
import { ECameraMode, ICameraModifier } from "./camera_modifier";

export class FreeCamera implements ICameraModifier {
    protected static defaultUp = new Vector3(0, 1, 0);
    protected static defaultReference = new Vector3(0, 0, 1);

    readonly viewport: Viewport = new Viewport(0, 0, 1, 1);
    readonly up: Vector3 = new Vector3(0, 1, 0);

    public lockedGlobalTarget: Nullable<Vector3> = null;

    private _cameraDirty: boolean = true;

    public _minZ: number = 1;
    public get minZ(): number {
        return this._minZ;
    }
    public set minZ(value: number) {
        this._minZ = value;
        this._cameraDirty = true;
    }

    public _maxZ: number = 1000;
    public get maxZ(): number {
        return this._maxZ;
    }
    public set maxZ(value: number) {
        this._maxZ = value;
        this._cameraDirty = true;
    }

    public _size: number = 10;
    public get size(): number {
        return this._size;
    }
    public set size(value: number) {
        this._size = value;
        this._cameraDirty = true;
    }

    public _fov: number = 0.9;
    public get fov(): number {
        return this._fov;
    }
    public set fov(value: number) {
        this._fov = value;
        this._cameraDirty = true;
    }

    public _mode: ECameraMode = ECameraMode.Perspective;
    public get mode(): ECameraMode {
        return this._mode;
    }
    public set mode(value: ECameraMode) {
        this._mode = value;
        this._cameraDirty = true;
    }

    forCache: boolean = false;

    modify(camera: ICamera): void {
        this.modifyViewMatrix(camera);
        this.modifyProjectionMatrix(camera);
    }
    dispose(): void {
        this.lockedGlobalTarget = null;
    }

    private modifyProjectionMatrix(camera: ICamera) {
        let aspectRatio = 1.0;
    }

    private modifyViewMatrix(camera: ICamera) {
        let coordinateSys = camera.coordinateSys;

        let localUp = coordinateSys.tempVector3A;
        let localTarget = coordinateSys.tempVector3B;
        let cameraLocalRotateMatrix = coordinateSys.tempMatrix4x4A;

        // 绑定目标则需要计算出本地旋转
        if (this.lockedGlobalTarget) {
            localTarget.copyFromFloats(this.lockedGlobalTarget.x, this.lockedGlobalTarget.y, this.lockedGlobalTarget.z);

            if (camera.parentNode) {
                let parentMatrixInvert = coordinateSys.tempMatrix4x4A;
                // ViewMatrix 与 TransformMatrix 是互逆的
                let parentMatrix = camera.parentNode.worldMatrix;
                Matrix4x4.InvertToRef(parentMatrix, parentMatrixInvert);

                coordinateSys.transformCoordinatesFromFloatsToRef(this.lockedGlobalTarget.x, this.lockedGlobalTarget.y, this.lockedGlobalTarget.z, parentMatrixInvert, localTarget);
            }

            let direction = coordinateSys.tempVector3C;
            Vector3.SubstractToRef(localTarget, camera.position, direction);

            let cameraRotateMatrix = coordinateSys.tempMatrix4x4A;
            coordinateSys.lookAtToViewMatrix(camera.position, localTarget, FreeCamera.defaultUp, cameraRotateMatrix);
            Matrix4x4.InvertToRef(cameraRotateMatrix, cameraRotateMatrix);

            let m = cameraRotateMatrix.m;
            let rx = 0.0, ry = 0.0, rz = 0.0;
            if (m[10] != 0.0) {
                rx = Math.atan(m[ 6] / m[10]);
            }
            if (direction.x != 0.0) {
                if (direction.x >= 0.0) {
                    ry = -Math.atan(direction.z / direction.x) + Math.PI / 2.0;
                } else {
                    ry = -Math.atan(direction.z / direction.x) - Math.PI / 2.0;
                }
            }
            if (isNaN(rx)) {
                rx = 0.0;
            }
            if (isNaN(ry)) {
                ry = 0.0;
            }
            coordinateSys.rotationYawPitchRollToQuaternion(ry, rx, rz, camera.rotationQuaternion);

            // coordinateSys.directionToQuaternion(direction, camera.rotationQuaternion, 0, 0, 0);

            coordinateSys.quaternionToRotationMatrixRef(camera.rotationQuaternion, cameraLocalRotateMatrix);

            coordinateSys.transformCoordinatesFromFloatsToRef(FreeCamera.defaultUp.x, FreeCamera.defaultUp.y, FreeCamera.defaultUp.z, cameraLocalRotateMatrix, localUp);

        // 未绑定目标, 通过状态计算出目标点(以本地<0,0,1>为正向)
        } else {
            coordinateSys.quaternionToRotationMatrixRef(camera.rotationQuaternion, cameraLocalRotateMatrix);

            coordinateSys.transformCoordinatesFromFloatsToRef(FreeCamera.defaultUp.x, FreeCamera.defaultUp.y, FreeCamera.defaultUp.z, cameraLocalRotateMatrix, localUp);

            coordinateSys.transformCoordinatesFromFloatsToRef(FreeCamera.defaultReference.x, FreeCamera.defaultReference.y, FreeCamera.defaultReference.z, cameraLocalRotateMatrix, localTarget);
            Vector3.AddToRef(camera.position, localTarget, localTarget);
        }

        FreeCamera.computeViewMatrix(camera, localTarget, localUp);
    }

    private static computeViewMatrix(camera: ICamera, localTarget: Vector3, localUp: Vector3) {
        camera.coordinateSys.lookAtToViewMatrix(camera.position, localTarget, localUp, camera.viewMatrix);

        if (camera.parentNode) {
            // ViewMatrix 与 TransformMatrix 是互逆的
            let parentMatrix = camera.parentNode.worldMatrix;
            Matrix4x4.InvertToRef(camera.viewMatrix, camera.viewMatrix);
            Matrix4x4.MultiplyToRef(camera.viewMatrix, parentMatrix, camera.viewMatrix);
            Matrix4x4.InvertToRef(camera.viewMatrix, camera.viewMatrix);
        }
    }
}