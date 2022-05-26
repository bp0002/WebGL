import { Nullable } from "../../base/types";
import { Matrix } from "../../math/matrix";
import { SquareMatrix } from "../../math/square_matrix";
import { Vector2 } from "../../math/vector2";

export type Matrix3x3 = Matrix<3, 3>;

export interface INode {
    id: string;
    position: Vector2;
    rotation: Vector2;
    parentNode: Nullable<INode>;
    localMatrix: Matrix3x3;

    setParent(value: Nullable<Node>, keepAbsolute?: boolean): void;
    computeWorldMatrix(force?: boolean): Matrix3x3;
    getLocalMatrix(result: Matrix3x3): void;
}

export class Node implements INode {
    protected _position: Vector2 = new Vector2(0, 0);
    protected _rotation: Vector2 = new Vector2(0, 0);

    public get position() {
        return this._position;
    }
    public get rotation() {
        return this._rotation;
    }

    protected _worldMatrix: Matrix3x3 = SquareMatrix.Identity(3);
    protected _localMatrix: Matrix3x3 = SquareMatrix.Identity(3);

    public get localMatrix() {
        return this._localMatrix;
    }

    public readonly id: string;

    protected _parentNode: Nullable<Node> = null;
    public get parentNode() {
        return this._parentNode;
    }

    protected _children: Nullable<Node[]> = null;

    constructor(id: string) {
        this.id = id;
    }

    public setParent(value: Nullable<Node>, keepAbsolute?: boolean) {
        this._parentNode = value;
    }

    public computeWorldMatrix(force: boolean = false): Matrix3x3 {
        if (force || this._position._isDirty || this._rotation._isDirty) {
            // TODO
        }

        return this._worldMatrix;
    }

    public getLocalMatrix(result: Matrix3x3): void {

    }
}