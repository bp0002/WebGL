import { Vector3 } from "./vector3";

export enum ESpace {
    LOCAL = 0,
    WORLD = 1,
    BONE = 2
}

export class Axis {
    /** X axis */
    public static X: Vector3 = new Vector3(1.0, 0.0, 0.0);
    /** Y axis */
    public static Y: Vector3 = new Vector3(0.0, 1.0, 0.0);
    /** Z axis */
    public static Z: Vector3 = new Vector3(0.0, 0.0, 1.0);
}