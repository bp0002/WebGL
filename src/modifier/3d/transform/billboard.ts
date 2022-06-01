import { Nullable } from "../../../base/types";
import { Camera } from "../../../camera/camera";
import { ESpace } from "../../../math/axis";
import { Vector3 } from "../../../math/vector3";
import { Node } from "../../../transform/3d/node";
import { ENodeModifierClassId, INodeModifier } from "./node_modifier";

export enum EBillboard {
    BillboardNone   = 0,
    BillboardX      = 1,
    BillboardY      = 2,
    BillboardZ      = 3,
    BillboardAll    = 4,
}

export class BillboardModifier implements INodeModifier {
    readonly classId: ENodeModifierClassId = ENodeModifierClassId.Billboard;
    forCache: boolean = false;
    space: ESpace = ESpace.LOCAL;
    mode: EBillboard = EBillboard.BillboardNone;

    public camera: Nullable<Camera> = null;

    private rotation = new Vector3();

    modify(nodeRef: Node): void {
    }

    dispose(): void {
        //
    }

    private local(nodeRef: Node) {
        switch (this.mode) {
            case(EBillboard.BillboardX): {
                // nodeRef.coordinateSys.;
                break;
            }
            case(EBillboard.BillboardY): {
                break;
            }
            case(EBillboard.BillboardZ): {
                break;
            }
            case(EBillboard.BillboardAll): {
                break;
            }
            default: {
                break;
            }
        }
    }
    private world(nodeRef: Node) {
        switch (this.mode) {
            case(EBillboard.BillboardX): {
                // nodeRef.coordinateSys.;
                break;
            }
            case(EBillboard.BillboardY): {
                break;
            }
            case(EBillboard.BillboardZ): {
                break;
            }
            case(EBillboard.BillboardAll): {
                break;
            }
            default: {
                break;
            }
        }
    }
}