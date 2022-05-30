import { Nullable } from "../../../base/types";
import { Camera } from "../../../camera/camera";
import { Node } from "../../../transform/3d/node";
import { ENodeModifierClassId, INodeModifier } from "./node_modifier";

export enum EBillboard {
    
}

export class BillboardModifier implements INodeModifier {
    classId: ENodeModifierClassId = ENodeModifierClassId.Billboard;
    forCache: boolean = false;

    public camera: Nullable<Camera> = null;

    modify(nodeRef: Node): void {
        throw new Error("Method not implemented.");
    }
    dispose(): void {
        throw new Error("Method not implemented.");
    }

}