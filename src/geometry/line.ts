import { Nullable } from "../base/types";
import { Point3, Vector3 } from "../math/vector3";
import { POINT_POSITION_FLOAT_COUNT_3D, TRIANGLE_INDICES_COUNT } from "./base";

export class Line3DPreSegment2Point {
    public static tempDirection: Vector3 = new Vector3();
    public static tempPoint: Point3 = new Vector3();
    public static modify(preKeyPoint: Vector3, keyPoint: Point3, keyNormal: Vector3, keyPointIndex: number, halfWidth: number, positions: Float32Array, indices: Uint16Array) {
        let triangleCount = 2;
        let pointCount = 2;
        let pointOffset = keyPointIndex * pointCount;
        let positionOffset = keyPointIndex * pointCount * POINT_POSITION_FLOAT_COUNT_3D;
        let indicesOffset = (keyPointIndex - 1) * triangleCount * TRIANGLE_INDICES_COUNT;

        let direction = Line3DPreSegment2Point.tempDirection;
        Vector3.SubstractToRef(keyPoint, preKeyPoint, direction);
        Vector3.NormalizeToRef(direction, direction);

        // 延申方向
        Vector3.CrossToRef(direction, keyNormal, direction);

        Vector3.ScaleToRef(direction, halfWidth, direction);

        // 沿 D X N 的正向拓展第一个
        Vector3.AddToRef(keyPoint, direction, Line3DPreSegment2Point.tempPoint);
        positions[positionOffset + 0] = Line3DPreSegment2Point.tempPoint.x;
        positions[positionOffset + 1] = Line3DPreSegment2Point.tempPoint.y;
        positions[positionOffset + 2] = Line3DPreSegment2Point.tempPoint.z;

        // 沿 D X N 的反向拓展第一个
        Vector3.ScaleToRef(direction, -1, direction);
        Vector3.AddToRef(keyPoint, direction, Line3DPreSegment2Point.tempPoint);
        positions[positionOffset + 3] = Line3DPreSegment2Point.tempPoint.x;
        positions[positionOffset + 4] = Line3DPreSegment2Point.tempPoint.y;
        positions[positionOffset + 5] = Line3DPreSegment2Point.tempPoint.z;

        if (indicesOffset >= 0) {
            indices[indicesOffset + 0] = pointOffset - 1;
            indices[indicesOffset + 1] = pointOffset - 2;
            indices[indicesOffset + 2] = pointOffset;

            indices[indicesOffset + 3] = pointOffset - 1;
            indices[indicesOffset + 4] = pointOffset;
            indices[indicesOffset + 5] = pointOffset + 1;
        }
    }

    public keyPoints: Point3[] = [];
    public keyNormals: Vector3[] = [];
    public positions: Float32Array;
    public indices: Uint16Array;
    public width: number = 1;
    public constructor(keyPointCount: number, baseKeyPoint: Point3, baseKeyNormal: Vector3, width: number) {
        let triangleCountPerSegment = 2;
        let pointCountPerSegment = 2;
        let pointOffset = keyPointCount * pointCountPerSegment;
        let positionOffset = keyPointCount * pointCountPerSegment * POINT_POSITION_FLOAT_COUNT_3D;
        let indicesOffset = (keyPointCount - 1) * triangleCountPerSegment * TRIANGLE_INDICES_COUNT;

        this.positions = new Float32Array(positionOffset);
        this.indices = new Uint16Array(indicesOffset);

        this.width = width;
        let halfWidth = width / 2;
        for (let i = 0; i < keyPointCount; i++) {
            let point = new Vector3(baseKeyPoint.x, baseKeyPoint.y, baseKeyPoint.z);
            let normal = new Vector3(baseKeyNormal.x, baseKeyNormal.y, baseKeyNormal.z);

            this.keyPoints[i] = point;
            this.keyNormals[i] = normal;
            Line3DPreSegment2Point.modify(point, point, normal, i, halfWidth, this.positions, this.indices);
        }
    }

    public updateLastKeyPoint(point: Point3, normal: Vector3) {
        let triangleCountPerSegment = 2;
        let pointCountPerSegment = 2;
        let positionCountPerSegment = pointCountPerSegment * POINT_POSITION_FLOAT_COUNT_3D;
        let indicesCountPerSegment = triangleCountPerSegment * TRIANGLE_INDICES_COUNT;

        let positionLen = this.positions.length - positionCountPerSegment;
        for (let i = 0; i < positionLen; i++) {
            this.positions[i] = this.positions[i + positionCountPerSegment];
        }

        let keyPointResortCount = this.keyPoints.length - 1;
        let lastPoint = this.keyPoints[0];
        let prePoint = this.keyPoints[keyPointResortCount];
        for (let i = 0; i < keyPointResortCount; i++) {
            this.keyPoints[i] = this.keyPoints[i + 1];
        }
        this.keyPoints[keyPointResortCount] = lastPoint;

        Vector3.CopyFrom(point, lastPoint);
        Line3DPreSegment2Point.modify(prePoint, point, normal, keyPointResortCount, this.width / 2, this.positions, this.indices);
    }
}