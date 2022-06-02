import { ICamera } from "../../../camera/base";

export interface ICameraOrthoGraphic extends ICamera {
    /**
     * 正交相机尺寸
     */
    size: number;
}

export function projectMatrix(camera: ICameraOrthoGraphic) {

}