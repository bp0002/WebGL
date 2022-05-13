import { IClone } from "../base/types";
import { Dim, Matrix } from "./matrix";

export class Column<R extends Dim> extends Matrix<R, 1> implements IClone<Column<R>> {
    constructor(row: R) {
        super(row, 1);
    }

    public clone(): Column<R> {
        return new Column<R>(this.row);
    }
}