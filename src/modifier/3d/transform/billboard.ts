import { Nullable } from "../../../base/types";
import { Camera } from "../../../camera/camera";
import { ESpace } from "../../../math/axis";
import { Quaternion } from "../../../math/quaternion";
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
    private static translation = new Vector3();
    private static scaling = new Vector3();
    private static quaternion = new Quaternion();
    private static eulerAngle = new Vector3();
    public static BILLBOARDMODE_NONE = 0;
    public static BILLBOARDMODE_X = 1;
    public static BILLBOARDMODE_Y = 2;
    public static BILLBOARDMODE_Z = 4;
    public static BILLBOARDMODE_ALL = 7;

    readonly classId: ENodeModifierClassId = ENodeModifierClassId.Billboard;
    forCache: boolean = false;
    space: ESpace = ESpace.LOCAL;
    mode: number = BillboardModifier.BILLBOARDMODE_NONE;

    public camera: Nullable<Camera> = null;

    modify(nodeRef: Node): void {
        if (this.mode <= 0) {
            return;
        }

        if (this.space === ESpace.LOCAL) {
            this.local(nodeRef);
        } else if (this.space === ESpace.WORLD) {
            this.world(nodeRef);
        }
    }

    dispose(): void {
        //
    }

    private local(nodeRef: Node) {
        nodeRef.coordinateSys.quaternionToEulerAngles(nodeRef.rotationQuaternion, BillboardModifier.eulerAngle);
        if ((this.mode & BillboardModifier.BILLBOARDMODE_X) !== BillboardModifier.BILLBOARDMODE_X) {
            BillboardModifier.eulerAngle.x = 0;
        }
        if ((this.mode & BillboardModifier.BILLBOARDMODE_Y) !== BillboardModifier.BILLBOARDMODE_Y) {
            BillboardModifier.eulerAngle.y = 0;
        }
        if ((this.mode & BillboardModifier.BILLBOARDMODE_Z) !== BillboardModifier.BILLBOARDMODE_Z) {
            BillboardModifier.eulerAngle.z = 0;
        }

        nodeRef.coordinateSys.eulerAnglesToQuaternion(BillboardModifier.eulerAngle.x, BillboardModifier.eulerAngle.y, BillboardModifier.eulerAngle.z, nodeRef.rotationQuaternion);
    }
    private world(nodeRef: Node) {
        let WorldMatrix = nodeRef.worldMatrix;
        nodeRef.coordinateSys.decompose(WorldMatrix, BillboardModifier.scaling, BillboardModifier.quaternion, BillboardModifier.translation);
        nodeRef.coordinateSys.quaternionToEulerAngles(BillboardModifier.quaternion, BillboardModifier.eulerAngle);
        if ((this.mode & BillboardModifier.BILLBOARDMODE_X) !== BillboardModifier.BILLBOARDMODE_X) {
            BillboardModifier.eulerAngle.x = 0;
        }
        if ((this.mode & BillboardModifier.BILLBOARDMODE_Y) !== BillboardModifier.BILLBOARDMODE_Y) {
            BillboardModifier.eulerAngle.y = 0;
        }
        if ((this.mode & BillboardModifier.BILLBOARDMODE_Z) !== BillboardModifier.BILLBOARDMODE_Z) {
            BillboardModifier.eulerAngle.z = 0;
        }
        nodeRef.coordinateSys.eulerAnglesToQuaternion(BillboardModifier.eulerAngle.x, BillboardModifier.eulerAngle.y, BillboardModifier.eulerAngle.z, nodeRef.rotationQuaternion);
        nodeRef.coordinateSys.composeToRef(BillboardModifier.scaling, BillboardModifier.quaternion, BillboardModifier.translation, WorldMatrix);
    }
}