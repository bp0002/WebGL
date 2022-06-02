import { ICamera } from "../../../camera/base";
import { Viewport } from "../../../renderer/3d/viewport";
import { Vector3 } from "../../../math/vector3";

export enum ECameraMode {
    Perspective = 0,
    Orthographic = 1,
}

export enum EVersionAdaptMode {
    Horizontal = 0,
    Vertical = 1,
}

export interface ICameraModifier {
    forCache: boolean;
    /**
     * 相机投影在渲染矩形上的区域信息
     */
    readonly viewport: Viewport;
    /**
     * 在相机局部坐标系下的相机UP方向
     */
    readonly up: Vector3;
    minZ: number;
    maxZ: number;
    modify(camera: ICamera): void;
    dispose(): void;
}