import { Point3, Vector3 } from "../math/vector3";

/**
 * 直线的标准隐式形式 F(X) = (X - P)n
 */
export class StraightLine3DStandardImplicitForm {
    /**
     * 直线的标准隐式形式 F(X) = (X - P)n
     * @param P 
     * @param n 
     */
    constructor(
        /**
         * 确定的一个点
         */
        public P: Point3,
        /**
         * 法向
         */
        public n: Vector3
    ) {

    }

    public static CreateByTwoPoint(P: Point3, Q: Point3) {
        let v = new Vector3();
        Vector3.SubstractToRef(Q, P, v);
    }

    public intersectionWithParametricEquationLine(otherLine: StraightLine3DParametricEquation, result: Point3): boolean {
        let u = new Vector3();
        Vector3.SubstractToRef(otherLine.Q, otherLine.P, u);

        let v = new Vector3();
        Vector3.SubstractToRef(otherLine.P, this.P, u);

        let flag = true;
        let udotn = Vector3.Dot(u, this.n);

        if (udotn === 0) {
            flag = false;
        } else {
            let t0 = -(Vector3.Dot(v, this.n)) / Vector3.Dot(u, this.n);
            otherLine.compute(t0, result);
        }

        u.dispose();
        v.dispose();

        (<any>u) = undefined;
        (<any>u) = undefined;

        return flag;
    }
}

export class StraightLine3DParametricEquation {
    constructor(
        public P: Point3,
        public Q: Point3
    ) {

    }

    public compute(t: number, result: Point3) {
        Vector3.SubstractToRef(this.Q, this.P, result);
        Vector3.ScaleToRef(result, t, result);
        Vector3.AddToRef(result, this.P, result);
    }

    public intersectionWithParametricEquationLine(otherLine: StraightLine3DParametricEquation, result: Point3) {

    }
}