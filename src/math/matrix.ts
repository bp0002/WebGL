import { PerformanceConfigurator } from "./performanceConfigurator";
import { createFloat32Array, recycleFloat32Array } from "./pool";
import { FloatScalar } from "./scalar";

export const N1 = 1;
export const N2 = 2;
export const N3 = 3;
export const N4 = 4;
export type Dim = typeof N1 | typeof N2 | typeof N3 | typeof N4;

/**
 * 行主序 存储
 * * 0  1  2  3
 * * 4  5  6  7
 * * 8  9 10 11
 * * ......
 */
export class Matrix<R extends Dim, C extends Dim> {
    /** @hidden */
    protected _m: Float32Array;
    public get m(): Float32Array { return this._m; }

    /** @hidden */
    public readonly _size: number;
    public readonly row: R;
    public readonly col: C;

    /** @hidden */
    public _isDirty: boolean = true;

    /**
     * 创建一个矩阵
     * @param row 行数目
     * @param col 列数目
     * @param value 元素值
     */
    constructor(row: R, col: C, value: number = 0) {
        this._size = col * row;
        this.row = row;
        this.col = col;

        this._m = createFloat32Array(this._size);

        for (let i = 0; i < this._size; i++) {
            this._m[i] = value;
        }
    }

    public isEqual(b: Matrix<R, C>) {
        let size = this._m.length;
        let result = true;
        for (let i = 0; i < size; i++) {
            if (!FloatScalar.Equal(this._m[i], b._m[i])) {
                result = false;
                break;
            }
        }

        return result;
    }

    public dispose() {
        recycleFloat32Array(this._m);
        (<any>this)._m = undefined;
    }

    /**
     * 获取矩阵数据副本
     * @returns Float32Array
     */
    public toArray() {
        return new Float32Array(this._m.buffer.slice(0));
    }

    /**
     * 获取矩阵格式化显示字符串
     * @param length 每个元素显示时的字符总长度
     * @param decimalLenght 小数部分显示字符数目
     * @returns 格式化字符串
     */
    public toFormatString(length: number = 12, decimalLenght: number = 4, breakString: string = '\r\n') {
        let result = '';
        for (let j = 0; j < this.row; j++) {
            for (let i = 0; i < this.col; i++) {
                let sourceIndex = j * this.col + i;

                result += FloatScalar.FormatString(this._m[sourceIndex], length, decimalLenght) + ',';
            }

            result += breakString;
        }

        return result;
    }

    /**
     * 行主序存储的索引计算
     * @param rowIndex 行序号
     * @param colIndex 列序号
     * @param row 行数目
     * @param col 列数目
     * @returns 数组索引
     */
    public static RowMajorOrder<R extends Dim, C extends Dim>(rowIndex: Dim, colIndex: Dim, row: R, col: C) {
        return (rowIndex - 1) * col + (colIndex - 1);
    }

    /**
     * 列主序存储的索引计算
     * @param rowIndex 行序号
     * @param colIndex 列序号
     * @param row 行数目
     * @param col 列数目
     * @returns 数组索引
     */
    public static ColMajorOrder<R extends Dim, C extends Dim>(rowIndex: Dim, colIndex: Dim, row: R, col: C): number {
        return (rowIndex - 1) + (colIndex - 1) * row;
    }

    /**
     * 获取矩阵指定 列 的子矩阵
     * @param source source matrix
     * @param colIndex index of column
     * @param result result matrix
     */
    public static GetColumn<R extends Dim, C extends Dim, T extends Matrix<R, 1>>(source: Matrix<R, C>, colIndex: number, result: T) {
        let rowCount = source.row;
        for (let i = 0; i < rowCount; i++) {
            result._m[i] = source._m[i * source.col + colIndex];
        }
    }

    /**
     * 获取矩阵指定 行 的子矩阵
     * @param source source matrix
     * @param rowIndex index of row
     * @param result result matrix
     */
    public static GetRow<R extends Dim, C extends Dim, T extends Matrix<C, 1>>(source: Matrix<R, C>, rowIndex: number, result: T) {
        let colCount = source.col;
        for (let i = 0; i < colCount; i++) {
            result._m[i] = source._m[rowIndex * source.col + i];
        }
    }

    /**
     * 创建元素全为 0 的矩阵
     * @param row 行数目
     * @param col 列数目
     * @return Zero Matrix
     */
    public static Zero<R extends Dim, C extends Dim>(row: R, col: C): Matrix<R, C> {
        let result = new Matrix(row, col, 0.);
        return result;
    }

    /**
     * 修改元素为 指定数值
     * @param target target matrix
     * @param value value number
     */
    public static ModifyToRef<R extends Dim, C extends Dim>(target: Matrix<R, C>, value: number) {
        for (let i = 0; i < target._size; i++) {
            target._m[i] = value;
        }
        target._isDirty = true;
    }

    /**
     * 修改矩阵指定元素为 指定数值 - 行主序
     * @param target target matrix
     * @param rowIndex index of row
     * @param colIndex index of col
     * @param value value number
     */
    public static ModifyCellToRef<R extends Dim, C extends Dim>(target: Matrix<R, C>, rowIndex: Dim, colIndex: Dim, value: number) {
        if (rowIndex <= target.row && colIndex <= target.col) {
            target._m[Matrix.RowMajorOrder(rowIndex, colIndex, target.row, target.col)] = value;
            target._isDirty = true;
        } else {
            throw new Error(`Matrix ModifyToRef Error: rowIndex or colIndex is out of bound.`);
        }
    }

    /**
     * 创建元素全为 1 的矩阵
     * @param row 行数目
     * @param col 列数目
     * @return One Matrix
     */
    public static One<R extends Dim, C extends Dim>(row: R, col: C): Matrix<R, C> {
        let result = new Matrix(row, col, 1.);
        return result;
    }

    /**
     * 矩阵复制
     * @returns result matrix
     */
    public clone() {
        let result = new Matrix(this.row, this.col, 0.);
        Matrix.CopyTo(this, result);

        return result;
    }

    /**
     * 矩阵复制
     * @param source source matrix
     * @param result result matrix
     */
    public static CopyTo<R extends Dim, C extends Dim, T extends Matrix<R, C>>(source: T, result: T) {
        for (let i = 0; i < result._size; i++) {
            result._m[i] = source._m[i];
        }

        result._isDirty = true;
    }

    /**
     * 检查矩阵行列是否相同
     * @param left left matrix
     * @param right right matrix
     * @returns bool
     */
    public static SameColRow<R extends Dim, C extends Dim, R2 extends Dim, C2 extends Dim>(left: Matrix<R, C>, right: Matrix<R2, C2>) {
        return <Dim>left.col === right.col && <Dim>left.row === right.row;
    }

    /**
     * 矩阵加法
     * @param right right matrix
     * @returns result matrix
     */
    public add(right: Matrix<R, C>) {
        let result = new Matrix(this.row, this.col, 0.);
        Matrix.AddToRef(this, right, result);

        return result;
    }

    /**
     * 矩阵加法
     * @param left left matrix
     * @param right right matrix
     * @param result result matrix
     */
    public static AddToRef<R extends Dim, C extends Dim, T extends Matrix<R, C>>(left: T, right: T, result: T) {
        let size = left._size;
        for (let i = 0; i < size; i++) {
            result._m[i] = left._m[i] + right._m[i];
        }

        result._isDirty = true;
    }

    /**
     * 矩阵减法
     * @param right right matrix
     * @returns result matrix
     */
    public substract(right: Matrix<R, C>) {
        let result = new Matrix(this.row, this.col);
        Matrix.SubstractToRef(this, right, result);

        return result;
    }

    /**
     * 矩阵减法
     * @param left left matrix
     * @param right right matrix
     * @param result result matrix
     */
    public static SubstractToRef<R extends Dim, C extends Dim, T extends Matrix<R, C>>(left: T, right: T, result: T) {
        let size = left._size;
        for (let i = 0; i < size; i++) {
            result._m[i] = left._m[i] - right._m[i];
        }

        result._isDirty = true;
    }

    /**
     * 标量乘法
     * @param scalar scale value
     * @returns new matrix
     */
    public scale(scalar: number) {
        let result = new Matrix(this.row, this.col);
        Matrix.ScaleToRef(this, scalar, result);

        return result;
    }
    /**
     * 标量乘法
     * @param left target matrix
     * @param scalar scale value
     * @param result result matrix
     */
    public static ScaleToRef<R extends Dim, C extends Dim, T extends Matrix<R, C>>(left: T, scalar: number, result: T) {
        let size = left._size;
        for (let i = 0; i < size; i++) {
            result._m[i] = left._m[i] * scalar;
        }

        result._isDirty = true;
    }

    /**
     * 转置
     */
    public transpose(): Matrix<C, R> {
        let result = new Matrix(this.col, this.row);
        Matrix.TransposeToRef(this, result);

        return result;
    }

    /**
     * 转置: 只能对行列匹配的矩阵使用
     * @param source source matrix
     * @param result Transpose result
     * @tip T(T(A))     = A
     * @tip T(xA)       = xT(A)
     * @tip T(A + B)    = T(A) + T(B)
     * @tip T(AB)       = T(B)T(A)
     */
    public static TransposeToRef<R extends Dim, C extends Dim>(source: Matrix<R, C>, result: Matrix<C, R>) {
        let temp = source.toArray();
        for (let j = 0; j < source.row; j++) {
            for (let i = 0; i < source.col; i++) {
                let sourceIndex = j * source.col + i;
                let resultIndex = i * result.col + j;

                result._m[resultIndex] = temp[sourceIndex];
            }
        }

        result._isDirty = true;
    }

    /**
     * 矩阵乘法 Left * Right = Result
     * @param right right matrix
     * @returns result
     */
    public multiply<N extends Dim>(right: Matrix<C, N>): Matrix<R, N> {
        let resultRow = this.row;
        let resultCol = right.col;
        let result = new Matrix(resultRow, resultCol);
        Matrix.MultiplyToRef(this, right, result);

        return result;
    }

    /**
     * 矩阵乘法 Left * Right = Result
     * @param left left matrix
     * @param right right matrix
     * @param result result matrix
     * @tip AB !== BA; eg: A: Matrix<3, 2>, B: Matrix<2, 3>
     * @tip result.row = left.row
     * @tip result.col = right.col
     * @tip compute number of calculate iteration == left.col == right.row
     */
    public static MultiplyToRef<R extends Dim, N extends Dim, C extends Dim, >(left: Matrix<R, N>, right: Matrix<N, C>, result: Matrix<R, C>) {
        // if (left.row === left.col && left.isIdentity()) {
        //     Matrix.CopyFrom(right, result);
        // } else if (right.row === right.col && right.isIdentity()) {
        //     Matrix.CopyFrom(left, result);
        // } else {
            let size = left.m.length;
            let temp = new Float32Array(size);
            let tempLength = left.col;
            let resultRow = left.row;
            let resultCol = right.col;
            let tempLeftRow = new Matrix(tempLength, 1);
            let tempRightCol = new Matrix(tempLength, 1);
            let tempCount = tempLength;

            for (let i = 0; i < resultRow; i++) {
                for (let j = 0; j < resultCol; j++) {
                    let resultIndex = i * resultCol + j;

                    Matrix.GetRow(left, i, tempLeftRow);
                    Matrix.GetColumn(right, j, tempRightCol);

                    temp[resultIndex] = 0;
                    for (let k = 0; k < tempCount; k++) {
                        temp[resultIndex] += tempLeftRow._m[k] * tempRightCol._m[k];
                    }
                }
            }

            for (let i = 0; i < size; i++) {
                result._m[i] = temp[i];
            }

            result._isDirty = true;
        // }
    }

    // public static Dot(right: Matrix) {

    // }
    // public static Cross(right: Matrix, result: Matrix) {

    // }

    /**
     * Sum Of Quared Residuals 残差平方和
     * @param left left matrix
     * @param right right matrix
     */
    public static SumOfQuaredResiduals<R extends Dim, C extends Dim>(left: Matrix<R, C>, right: Matrix<R, C>): number {
        let len = left._size;
        let result = 0;
        let tempDiff = 0;
        for (let i = 0; i < len; i++) {
            tempDiff = left._m[i] - right._m[i];
            result += tempDiff * tempDiff;
        }

        return result;
    }

    /**
     * 各项平方和
     * @param left matrix
     * @returns Sum Of Quared
     */
    public static SumOfQuared<R extends Dim, C extends Dim, T extends Matrix<R, C>>(left: T): number {
        let len = left._size;
        let result = 0;
        let tempDiff = 0;
        for (let i = 0; i < len; i++) {
            tempDiff = left._m[i];
            result += tempDiff * tempDiff;
        }

        return result;
    }

    /**
     * 归一化各项 - 如果为 0 矩阵, 则返回第一项为 1 的结果
     * @param left matrix
     * @param result matrix
     */
     public static NormalizeToRef<R extends Dim, C extends Dim, T extends Matrix<R, C>>(left: T, result: T) {
        let len = left._size;

        let length = Math.sqrt(Matrix.SumOfQuared(left));
        if (length == 0) {
            result._m[0] = 1;
            for (let i = 1; i < len; i++) {
                result._m[i] = 0;
            }
        } else {
            for (let i = 0; i < len; i++) {
                result._m[i] = left._m[i] / length;
            }
        }

        result._isDirty = true;
    }

    public static FractToRef<R extends Dim, C extends Dim, T extends Matrix<R, C>>(target: T, result: T) {
        let len = target._size;
        for (let i = 0; i < len; i++) {
            result._m[i] = target._m[i] % 1;
        }

        result._isDirty = true;
    }

    public static FloorToRef<R extends Dim, C extends Dim, T extends Matrix<R, C>>(target: T, result: T) {
        let len = target._size;
        for (let i = 0; i < len; i++) {
            result._m[i] = Math.floor(target._m[i]);
        }

        result._isDirty = true;
    }

    public static CeilToRef<R extends Dim, C extends Dim, T extends Matrix<R, C>>(target: T, result: T) {
        let len = target._size;
        for (let i = 0; i < len; i++) {
            result._m[i] = Math.ceil(target._m[i]);
        }

        result._isDirty = true;
    }

    public static RoundToRef<R extends Dim, C extends Dim, T extends Matrix<R, C>>(target: T, result: T) {
        let len = target._size;
        for (let i = 0; i < len; i++) {
            result._m[i] = Math.round(target._m[i]);
        }

        result._isDirty = true;
    }

    /**
     * 在两个矩阵间 线性 插值
     * @param from 起始矩阵
     * @param to 结束矩阵
     * @param amount 插值因子
     * @param result 结果矩阵
     */
    public static LerpToRef<R extends Dim, C extends Dim, T extends Matrix<R, C>>(from: T, to: T, amount: number, result: T) {
        let len = from._size;
        let tempDiff = 0;
        for (let i = 0; i < len; i++) {
            tempDiff = to._m[i] - from._m[i];
            result._m[i] = from._m[i] + tempDiff * amount;
        }

        result._isDirty = true;
    }

    /**
     * divide every item
     * @param left Matrix
     * @param right Matrix
     * @param result Matrix
     */
    public static DivideItemsToRef<R extends Dim, C extends Dim, T extends Matrix<R, C>>(left: T, right: T, result: T) {
        let len = left._size;
        for (let i = 0; i < len; i++) {
            result._m[i] = left._m[i] / right._m[i];
        }

        result._isDirty = true;
    }

    /**
     * 在两个矩阵间 Hermite 插值
     * @param value1 起始数值矩阵
     * @param tangent1 起始 tangent 数值
     * @param value2 结束数值矩阵
     * @param tangent2 结束 tangent 数值
     * @param amount 插值因子
     * @param result 结果矩阵
     */
    public static Hermite<R extends Dim, C extends Dim, T extends Matrix<R, C>>(value1: T, tangent1: T, value2: T, tangent2: T, amount: number, result: T) {
        let squared = amount * amount;
        let cubed = amount * squared;
        let part1 = ((2.0 * cubed) - (3.0 * squared)) + 1.0;
        let part2 = (-2.0 * cubed) + (3.0 * squared);
        let part3 = (cubed - (2.0 * squared)) + amount;
        let part4 = cubed - squared;

        let len = value1._size;
        for (let i = 0; i < len; i++) {
            result._m[i] = (((value1._m[i] * part1) + (value2._m[i] * part2)) + (tangent1._m[i] * part3)) + (tangent2._m[i] * part4);
        }

        result._isDirty = true;
    }
}