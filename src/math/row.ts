import { Dim, Matrix } from "./matrix";

export type Point<C extends Dim> = Row<C>;

export class Row<C extends Dim> extends Matrix<1, C> {
    constructor(col: C) {
        super(1, col);
    }

    public static Dot<C extends Dim>(left: Row<C>, right: Row<C>): number {
        let result = 0;
        for (let i = 0; i < left.col; i++) {
            result += left._m[i] * right._m[i];
        }

        return result;
    }

    public static DistanceSquared<C extends Dim>(start: Row<C>, end: Row<C>): number {
        return Row.SumOfQuaredResiduals(end, start);
    }

    public static Distance<C extends Dim>(start: Row<C>, end: Row<C>): number {
        return Math.sqrt(Row.SumOfQuaredResiduals(end, start));
    }

    public static Length<C extends Dim>(target: Row<C>): number {
        return Math.sqrt(Row.SumOfQuared(target));
    }
}