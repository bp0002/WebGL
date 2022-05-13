import { Quaternion } from "../math/quaternion";
import { Vector3 } from "../math/vector3";
import { Camera } from "./camera";

export interface ICameraPerspective {
    fov: number;
    position: Vector3;
    rotationQuaternion: Quaternion;
    minZ: number;
    maxZ: number;
}

export function projectMatrix(camera: Camera) {

}