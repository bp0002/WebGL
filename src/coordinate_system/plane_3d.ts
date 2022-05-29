import { Point3, Vector3 } from "../math/vector3";
import { StraightLine3DParametricEquation } from "./line_3d";

/**
 * 3D 平面
 */
export class Plane3DImplicitForm {
    constructor(
        /**
         * 平面经过指定点
         */
        public P: Point3,
        /**
         * 平面法向
         */
        public n: Vector3
    ) {

    }

    /**
     * 平面与直线交点
     * @param line 直线
     * @param result 交点
     */
    public intersectionWithLine(line: StraightLine3DParametricEquation, result: Point3): boolean {
        let P = line.P;
        let d = new Vector3();
        Vector3.SubstractToRef(line.Q, line.P, d);

        let n = this.n;
        let Q = this.P;
        let QP = result;
        Vector3.SubstractToRef(Q, P, QP);

        let flag = true;
        let ddotn = Vector3.Dot(d, n);
        if (ddotn === 0) {
            flag = false;
        } else {
            let t = Vector3.Dot(QP, n) / Vector3.Dot(d, n);
            line.compute(t, result);
        }

        d.dispose();

        (<any>d) = undefined;

        return flag;
    }
}