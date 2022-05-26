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
            }
        }

        return this._isIdentity;
    }

    /**
     * 创建单位矩阵
     * @param row 行数目
     * @param col 列数目
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
     * 只能对方阵使用
     * @param source source
     * @param result multiplicative inverse
     */
     public static InvertToRef<N extends Dim, T extends SquareMatrix<N>>(source: T, result: T) {
        //
    }

    /**
     * 判断方阵是否是奇异的 - 不存在乘法逆元(逆矩阵)
     * @param source source matrix
     */
    public static IsSingular<N extends Dim, T extends SquareMatrix<N>>(source: T): boolean {
        return true;
    }

    /**
     * 求行列式 - 当N>5，消元法优于递归
     * @param target SquareMatrix
     * @returns number
     */
    public static Determinant<N extends Dim, T extends SquareMatrix<N>>(target: T): number {
        let result = 0;

        let iteration = target.row;
        // for ()

        return result;
    }
}