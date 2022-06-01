import { Dim, Matrix } from "./matrix";

export class SquareMatrix<N extends Dim> extends Matrix<N, N> {
    private _isIdentityDirty: boolean;
    private _isIdentity: boolean;

    /** @hidden */
    public _isDirty: boolean = true;

    constructor(n: N, value?: number) {
        super(n, n, value);

        this._isIdentityDirty = false;
        this._isIdentity = false;
    }

    /**
     * 矩阵是否为单位矩阵
     * @returns bool
     */
    public isIdentity() {
        if (this._isDirty || this._isIdentityDirty) {
            this._isIdentity = true;

            for (let i = 0; i < this.col; i++) {
                let indexDiagonal = i * this.col + i;
                for (let j = 0; j < this.row; j++) {
                    let index = i * this.col + j;
                    if (index == indexDiagonal) {
                        this._isIdentity = this._m[index] == 1.;
                    } else {
                        this._isIdentity = this._m[index] == 0.;
                    }

                    if (!this._isIdentity) {
                        break;
                    }
                }

                if (!this._isIdentity) {
                    break;
                }
            }
        }

        return this._isIdentity;
    }

    /**
     * 创建单位方阵
     * @param n 行列数目
     * @return Identity Matrix
     */
    public static Identity<N extends Dim>(n: N): SquareMatrix<N> {
        let result = new SquareMatrix(n, 0.);
        SquareMatrix.IdentityToRef(result);

        return result;
    }

    /**
     * 将矩阵设置为单位矩阵
     * @param result result
     */
    public static IdentityToRef<N extends Dim>(result: SquareMatrix<N>) {
        Matrix.ModifyToRef(result, 0.);

        for (let i = 0; i < result.col; i++) {
            if (i >= result.row) {
                break;
            }
            result._m[ i * result.col + i] = 1;
        }

        result._isIdentity = true;
        result._isIdentityDirty = false;
        result._isDirty = true;
        return result;
    }

    /**
     * 求方阵的逆
     * @param source source
     * @param result multiplicative inverse
     */
    public static InvertToRef<N extends Dim, T extends SquareMatrix<N>>(source: T, result: T) {
        //
        if (source.isIdentity()) {
            SquareMatrix.IdentityToRef(result);
            result._isDirty = true;
            return;
        }

        let row = source.row;

        if (row === 4) {
            return SquareMatrix.InvertToRefN4(<SquareMatrix<4>>source, <SquareMatrix<4>>result);
        } else {
            throw new Error(`InvertToRef Not Implment!`);
        }
    }

    private static InvertToRefN4<T extends SquareMatrix<4>>(source: T, result: T) {
        let m = source.m;
        let   m11 = m[ 0], m12 = m[ 1], m13 = m[ 2], m14 = m[ 3]
            , m21 = m[ 4], m22 = m[ 5], m23 = m[ 6], m24 = m[ 7]
            , m31 = m[ 8], m32 = m[ 9], m33 = m[10], m34 = m[11]
            , m41 = m[12], m42 = m[13], m43 = m[14], m44 = m[15];

        let det_33_44 = m33 * m44 - m43 * m34;
        let det_32_44 = m32 * m44 - m42 * m34;
        let det_32_43 = m32 * m43 - m42 * m33;
        let det_31_44 = m31 * m44 - m41 * m34;
        let det_31_43 = m31 * m43 - m41 * m33;
        let det_31_42 = m31 * m42 - m41 * m32;

        let cofact_11 = +(m22 * (det_33_44) - m23 * (det_32_44) + m24 * (det_32_43));
        let cofact_12 = -(m21 * (det_33_44) - m23 * (det_31_44) + m24 * (det_31_43));
        let cofact_13 = +(m21 * (det_32_44) - m22 * (det_31_44) + m24 * (det_31_42));
        let cofact_14 = -(m21 * (det_32_43) - m22 * (det_31_43) + m23 * (det_31_42));

        let det = m11 * cofact_11 + m12 * cofact_12 + m13 * cofact_13 + m14 * cofact_14;

        if (det === 0) {
            Matrix.CopyTo(source, result);
        } else {
            let detInv = 1.0 / det;
            let m = source.m;
            let   m11 = m[ 0], m12 = m[ 1], m13 = m[ 2], m14 = m[ 3]
                , m21 = m[ 4], m22 = m[ 5], m23 = m[ 6], m24 = m[ 7]
                , m31 = m[ 8], m32 = m[ 9], m33 = m[10], m34 = m[11]
                , m41 = m[12], m42 = m[13], m43 = m[14], m44 = m[15];

            let det_23_44 = m23 * m44 - m43 * m24;
            let det_22_44 = m22 * m44 - m42 * m24;
            let det_22_43 = m22 * m43 - m42 * m23;
            let det_21_44 = m21 * m44 - m41 * m24;
            let det_21_43 = m21 * m43 - m41 * m23;
            let det_21_42 = m21 * m42 - m41 * m22;
            let det_23_34 = m23 * m34 - m33 * m24;
            let det_22_34 = m22 * m34 - m32 * m24;
            let det_22_33 = m22 * m33 - m32 * m23;
            let det_21_34 = m21 * m34 - m31 * m24;
            let det_21_33 = m21 * m33 - m31 * m23;
            let det_21_32 = m21 * m32 - m31 * m22;

            let cofact_21 = -(m12 * det_33_44 - m13 * det_32_44 + m14 * det_32_43);
            let cofact_22 = +(m11 * det_33_44 - m13 * det_31_44 + m14 * det_31_43);
            let cofact_23 = -(m11 * det_32_44 - m12 * det_31_44 + m14 * det_31_42);
            let cofact_24 = +(m11 * det_32_43 - m12 * det_31_43 + m13 * det_31_42);

            let cofact_31 = +(m12 * det_23_44 - m13 * det_22_44 + m14 * det_22_43);
            let cofact_32 = -(m11 * det_23_44 - m13 * det_21_44 + m14 * det_21_43);
            let cofact_33 = +(m11 * det_22_44 - m12 * det_21_44 + m14 * det_21_42);
            let cofact_34 = -(m11 * det_22_43 - m12 * det_21_43 + m13 * det_21_42);

            let cofact_41 = -(m12 * det_23_34 - m13 * det_22_34 + m14 * det_22_33);
            let cofact_42 = +(m11 * det_23_34 - m13 * det_21_34 + m14 * det_21_33);
            let cofact_43 = -(m11 * det_22_34 - m12 * det_21_34 + m14 * det_21_32);
            let cofact_44 = +(m11 * det_22_33 - m12 * det_21_33 + m13 * det_21_32);

            result.m[ 0] = cofact_11 * detInv, result.m[ 4] = cofact_12 * detInv, result.m[ 8] = cofact_13 * detInv, result.m[12] = cofact_14 * detInv,
            result.m[ 1] = cofact_21 * detInv, result.m[ 5] = cofact_22 * detInv, result.m[ 9] = cofact_23 * detInv, result.m[13] = cofact_24 * detInv,
            result.m[ 2] = cofact_31 * detInv, result.m[ 6] = cofact_32 * detInv, result.m[10] = cofact_33 * detInv, result.m[14] = cofact_34 * detInv,
            result.m[ 3] = cofact_41 * detInv, result.m[ 7] = cofact_42 * detInv, result.m[11] = cofact_43 * detInv, result.m[15] = cofact_44 * detInv;

            result._isDirty = true;
        }
    }

    /**
     * 求行列式 - 当N>5，消元法优于递归
     * @param target SquareMatrix
     * @returns number
     */
    public static Determinant<N extends Dim, T extends SquareMatrix<N>>(target: T): number {
        let row = target.row;
        let result = 0;

        if (row == 4) {
            return SquareMatrix.DeterminantN4(<SquareMatrix<4>>target);
        }

        throw new Error(`Determinant Not Implment!`);
    }

    private static DeterminantN4<T extends SquareMatrix<4>>(target: T): number {
        let result = 0;

        if (target.isIdentity()) {
            result = 1;
        } else {
            let m = target.m;
            let   m11 = m[ 0], m12 = m[ 1], m13 = m[ 2], m14 = m[ 3]
                , m21 = m[ 4], m22 = m[ 5], m23 = m[ 6], m24 = m[ 7]
                , m31 = m[ 8], m32 = m[ 9], m33 = m[10], m34 = m[11]
                , m41 = m[12], m42 = m[13], m43 = m[14], m44 = m[15];

            let det_33_44 = m33 * m44 - m43 * m34;
            let det_32_44 = m32 * m44 - m42 * m34;
            let det_32_43 = m32 * m43 - m42 * m33;
            let det_31_44 = m31 * m44 - m41 * m34;
            let det_31_43 = m31 * m43 - m41 * m33;
            let det_31_42 = m31 * m42 - m41 * m32;

            let cofact_11 = +(m22 * (det_33_44) - m23 * (det_32_44) + m24 * (det_32_43));
            let cofact_12 = -(m21 * (det_33_44) - m23 * (det_31_44) + m24 * (det_31_43));
            let cofact_13 = +(m21 * (det_32_44) - m22 * (det_31_44) + m24 * (det_31_42));
            let cofact_14 = -(m21 * (det_32_43) - m22 * (det_31_43) + m23 * (det_31_42));

            result = m11 * cofact_11 + m12 * cofact_12 + m13 * cofact_13 + m14 * cofact_14;
        }

        return result;
    }

    /**
     * 判断方阵是否是奇异的 - 不存在乘法逆元(逆矩阵)
     * @param source source matrix
     */
    public static IsSingular<N extends Dim, T extends SquareMatrix<N>>(source: T): boolean {
        throw new Error(`IsSingular Not Implment!`);
    }
}