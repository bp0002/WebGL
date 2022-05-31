import { ESpace } from "../../../math/axis";
import { Matrix4x4 } from "../../../math/matrix4x4";
import { Vector3 } from "../../../math/vector3";
import { Node } from "../../../transform/3d/node";
import { ENodeModifierClassId, INodeModifier } from "./node_modifier";

export class ModifierLookAt implements INodeModifier {
    public static tempDirection = new Vector3(1, 0, 0);
    public static tempMatrix1 = new Matrix4x4();
    public static tempMatrix2 = new Matrix4x4();
    classId: ENodeModifierClassId = ENodeModifierClassId.LookAt;
    forCache: boolean = false;
    /**
     * the position (must be in same space as current mesh) to look at
     */
    target: Vector3 = new Vector3(0, 0, 0);
    /**
     * the choosen space of the target
     */
    spcae: ESpace = ESpace.LOCAL;
    /**
     * optional yaw (y-axis) correction in radians
     */
    yawCor: number = 0;
    /**
     * optional pitch (x-axis) correction in radians
     */
    pitchCor: number = 0;
    /**
     * optional roll (z-axis) correction in radians
     */
    rollCor: number = 0;
    modify(nodeRef: Node): void {
        let pos = this.spcae === ESpace.LOCAL ? nodeRef.position : nodeRef.absolutePosition;

        Vector3.SubstractToRef(this.target, pos, ModifierLookAt.tempDirection);
        nodeRef.coordinateSys.directionToQuaternion(ModifierLookAt.tempDirection, nodeRef.rotationQuaternion, 0, 0, 0);

        if (this.spcae === ESpace.WORLD && nodeRef.parentNode) {
            nodeRef.coordinateSys.quaternionToRotationMatrixRef(nodeRef.rotationQuaternion, ModifierLookAt.tempMatrix1);

            nodeRef.coordinateSys.getRotationMatrixFromMatrix(nodeRef.parentNode.computeWorldMatrix(), ModifierLookAt.tempMatrix2);
            Matrix4x4.InvertToRef(ModifierLookAt.tempMatrix2, ModifierLookAt.tempMatrix2);
            Matrix4x4.MultiplyToRef(ModifierLookAt.tempMatrix1, ModifierLookAt.tempMatrix2, ModifierLookAt.tempMatrix1);

            nodeRef.coordinateSys.rotationMatrixToQuaternion(ModifierLookAt.tempMatrix1, nodeRef.rotationQuaternion);
        }
    }
    dispose(): void {
        this.target.dispose();

        (<any>this).target = undefined;
    }
}