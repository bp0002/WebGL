import { Matrix } from "../math/matrix";
import { Vector2 } from "../math/vector2";

export class MarkovOne {
    public x: number = 0;
    public m: number = 1;
    public F: number = 1;
    public r: number = 0.2;
    public h: number = 0.0;
    public deltaH: number = 0.01;
    public pv: Vector2 = new Vector2(0, 0);
    private A: Matrix<2, 2> = new Matrix(2, 2);
    private B: Matrix<2, 1> = new Matrix(2, 1);
    public compute(display: (p: number, v: number) => void) {
        this.h += this.deltaH;
        let h = this.h;
        let r = this.r;
        let m = this.m;
        Matrix.ModifyCellToRef(this.A, 1, 1, 1);
        Matrix.ModifyCellToRef(this.A, 1, 2, h);
        Matrix.ModifyCellToRef(this.A, 2, 1, 0);
        Matrix.ModifyCellToRef(this.A, 2, 2, 1 - h * r / m);

        Matrix.ModifyCellToRef(this.B, 1, 1, 0);
        Matrix.ModifyCellToRef(this.B, 2, 1, h / m);

        let result = this.A.multiply(this.pv.transpose()).add(this.B.scale(this.F));
        Matrix.ModifyCellToRef(this.pv, 1, 1, result.m[0]);
        Matrix.ModifyCellToRef(this.pv, 1, 2, result.m[1]);
        display(result.m[0], result.m[1]);
    }
}