import { Quaternion } from "../math/quaternion";
import { Vector3 } from "../math/vector3";
import { Camera } from "./camera";

export interface ICameraOrthoGraphic {
    /**
     * 正交相机尺寸
     */
    size: number;
    position: Vector3;
    rotationQuaternion: Quaternion;
    minZ: number;
    maxZ: number;
}

export function projectMatrix(camera: Camera) {

}