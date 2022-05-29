import { Point2, Vector2 } from "../math/vector2";

/**
 * 直线的标准隐式形式 F(X) = (X - P)n
 */
export class StraightLine2DStandardImplicitForm {
    /**
     * 直线的标准隐式形式 F(X) = (X - P)n
     * @param P 
     * @param n 
     */
    constructor(
        /**
         * 确定的一个点
         */
        public P: Point2,
        /**
         * 法向
         */
        public n: Vector2
    ) {

    }

    public static CreateByTwoPoint(P: Point2, Q: Point2) {
        let v = new Vector2();
        Vector2.SubstractToRef(Q, P, v);

        Vector2.CrossToRef(v, v);

        return new StraightLine2DStandardImplicitForm(P, v);
    }

    public intersectionWithParametricEquationLine(otherLine: StraightLine2DParametricEquation, result: Point2): boolean {
        let u = new Vector2();
        Vector2.SubstractToRef(otherLine.Q, otherLine.P, u);

        let v = new Vector2();
        Vector2.SubstractToRef(otherLine.P, this.P, u);

        let flag = true;
        let udotn = Vector2.Dot(u, this.n);

        if (udotn === 0) {
            flag = false;
        } else {
            let t0 = -(Vector2.Dot(v, this.n)) / Vector2.Dot(u, this.n);
            otherLine.compute(t0, result);
        }

        u.dispose();
        v.dispose();

        (<any>u) = undefined;
        (<any>u) = undefined;

        return flag;
    }
}

export class StraightLine2DParametricEquation {
    constructor(
        public P: Point2,
        public Q: Point2
    ) {

    }

    public compute(t: number, result: Point2) {
        Vector2.SubstractToRef(this.Q, this.P, result);
        Vector2.ScaleToRef(result, t, result);
        Vector2.AddToRef(result, this.P, result);
    }

    /**
     * 求与另一直线的交点
     * @param otherLine line
     */
    public intersectionWithLine(otherLine: StraightLine2DParametricEquation, result: Point2): boolean {
        let A = this.P, B = this.Q;
        let C = otherLine.P, D = otherLine.Q;

        let BD = result;
        Vector2.SubstractToRef(B, D, BD);

        let v = new Vector2();
        Vector2.SubstractToRef(A, B, BD);
        Vector2.CrossToRef(v, v);
        let u = new Vector2();
        Vector2.SubstractToRef(C, D, u);

        let flag = true;
        let udotv = Vector2.Dot(u, v);
        if (udotv === 0) {
            flag = false;
        } else {
            let s0 = Vector2.Dot(BD, v) / Vector2.Dot(u, v);
            otherLine.compute(s0, result);
        }

        u.dispose();
        v.dispose();

        (<any>u) = undefined;
        (<any>u) = undefined;

        return flag;
    }
}