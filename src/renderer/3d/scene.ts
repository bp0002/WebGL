import { ICamera } from "../../camera/base";
import { INode } from "../../transform/3d/base";
import { IScene } from "./base";
import { DisplayRect } from "./display_rect";

export class Scene implements IScene {
    cameraList: ICamera[] = [];
    rootNodeList: INode[] = [];
    meshList: INode[] = [];
    renderShadowMap(camera: ICamera): void {
        throw new Error("Method not implemented.");
    }
    renderOpaque(camera: ICamera): void {
        throw new Error("Method not implemented.");
    }
    renderTransparent(camera: ICamera): void {
        throw new Error("Method not implemented.");
    }
    public readonly displayRect: DisplayRect = new DisplayRect(0, 0, 1, 1);
}