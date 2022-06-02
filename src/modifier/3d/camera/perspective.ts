import { ICamera } from "../../../camera/base";

export interface ICameraPerspective extends ICamera {
    fov: number;
}

export function projectMatrix(camera: ICameraPerspective) {

}