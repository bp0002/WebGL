import { Point3, Vector3 } from "../math/vector3";
import { StraightLine3DParametricEquation } from "./line_3d";

export class Sphere {
    constructor(
        /**
         * 球心坐标
         */
        public P: Point3,
        /**
         * 半径
         */
        public r: number
    ) {

    }

    /**
     * 球与直线交点
     * @param line 直线
     * @param result1 交点1
     * @param result2 交点2
     * @param doCompute 是否求解交点
     * @return 交点数目
     */
    public intersectionWithLine(line: StraightLine3DParametricEquation, result1: Point3, result2: Point3, doCompute: boolean): number {
        let d = new Vector3();
        Vector3.SubstractToRef(line.Q, line.P, d);

        let v = new Vector3();
        Vector3.SubstractToRef(line.P, this.P, v);

        let ddotv = Vector3.Dot(d, v);
        let ddotd = Vector3.Dot(d, d);
        let vdotv = Vector3.Dot(v, v);

        let rr = this.r * this.r;

        let resultCount = 0;
        let temp = 4 * (ddotv * ddotv - (vdotv - rr) * ddotd);
        // 2个解
        if (temp > 0) {
            resultCount = 2;
            if (doCompute) {
                let sqrt = Math.sqrt(temp);
                let t1 = (-2 * ddotv + sqrt) / 2 * (vdotv - rr);
                let t2 = (-2 * ddotv - sqrt) / 2 * (vdotv - rr);
                line.compute(t1, result1);
                line.compute(t2, result2);
            }
        // 1个解
        } else if (temp = 0) {
            resultCount = 1;
            if (doCompute) {
                let t1 = (-2 * ddotv + 0) / 2 * (vdotv - rr);
                line.compute(t1, result1);
            }
        // 0个解
        }

        d.dispose();
        v.dispose();

        (<any>d) = undefined;
        (<any>v) = undefined;

        return resultCount;
    }
}