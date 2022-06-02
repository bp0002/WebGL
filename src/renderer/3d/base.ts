import { ICamera } from "../../camera/base";
import { INode } from "../../transform/3d/base";
import { DisplayRect } from "./display_rect";

export interface IScene {
    //
    displayRect: DisplayRect;
    cameraList: ICamera[];
    rootNodeList: INode[];
    meshList: INode[];
    renderShadowMap(camera: ICamera): void;
    renderOpaque(camera: ICamera): void;
    renderTransparent(camera: ICamera): void;
}