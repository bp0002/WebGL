import { Dim, Matrix } from "./matrix";
import { SquareMatrix } from "./square_matrix";

export class Matrix4x4 extends SquareMatrix<4> {
    constructor() {
        super(4, 4);
    }

    public static Determinant<N extends Dim, T extends SquareMatrix<N>>(target: T): number {
        let result = 0;

        if (target.isIdentity()) {
            result = 1;
        } else {
            let m = target.m;
            let   m11 = m[ 0], m12 = m[ 1], m13 = m[ 2], m14 = m[ 3]
                , m21 = m[ 4], m22 = m[ 5], m23 = m[ 6], m24 = m[ 7]
                , m31 = m[ 8], m32 = m[ 9], m33 = m[10], m34 = m[11]
                , m41 = m[12], m42 = m[13], m43 = m[14], m44 = m[15];

            let det_m31_m42 = m31 * m42 - m41 * m32;
            let det_m32_m43 = m32 * m43 - m42 * m33;
            let det_m33_m44 = m33 * m44 - m43 * m34;
            let det_m31_m44 = m31 * m44 - m41 * m34;
            let det_m31_m43 = m31 * m43 - m41 * m33;
            let det_m32_m44 = m32 * m44 - m42 * m34;

            let c0fact_11 = m22 * (+det_m33_m44) + m23 * (-det_m32_m44) + m24 * (+det_m32_m43);
            let c0fact_12 = m21 * (+det_m33_m44) + m23 * (-det_m31_m44) + m24 * (+det_m31_m43);
            let c0fact_13 = m21 * (+det_m32_m44) + m22 * (-det_m31_m44) + m24 * (+det_m31_m42);
            let c0fact_14 = m21 * (+det_m32_m43) + m22 * (-det_m31_m43) + m23 * (+det_m31_m42);

            result = m11 * (+c0fact_11) + m12 * (-c0fact_12) + m13 * (+c0fact_13) + m14 * (-c0fact_14);
        }

        return result;
    }

}