import { INode } from "../../../transform/3d/base";

export enum ENodeModifierClassId {
    Billboard = 1,
    InfinitedDistance = 2,
    PivotMatrix = 3,
    NonUniformScaling = 4,
    LookAt = 5,
}

export interface INodeModifier {
    readonly classId: ENodeModifierClassId;
    forCache: boolean;
    modify(nodeRef: INode): void;
    dispose(): void;
}