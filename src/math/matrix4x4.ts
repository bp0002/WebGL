import { Dim, Matrix, N4 } from "./matrix";
import { SquareMatrix } from "./square_matrix";

export class Matrix4x4 extends SquareMatrix<typeof N4> {
    public static tempMatrix = new Matrix4x4();

    constructor() {
        super(4, 4);
    }
}