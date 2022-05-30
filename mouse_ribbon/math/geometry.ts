import { MathTools } from "./math";

type NumberMathFunction = (x: number) => number;

export interface IPolygonData {
    vertexs?: [number, number][];
    vertexs3D?: [number, number, number][];
    faces: [number, number, number][];
}

export class GeometryTools {
    /**
     * 丝带
     * @param detailCount 细节点数目
     */
    public static ribbon_from_line(points: [number, number][]) {

        const detailCount = points.length;

        if (detailCount < 2) {
            return;
        }

        const result: IPolygonData = {
            vertexs3D: [],
            faces: []
        };

        for (let i = 1; i < detailCount; i++) {
            if (result.vertexs3D) {
                result.vertexs3D[i] = [points[i][0], points[i][1] - 0.02, 0];
                result.vertexs3D[i + detailCount] = [points[i][0], points[i][1] + 0.02, 0];
            }
        }

        result.faces = this.sphereRibbon(0, detailCount);

        return result;
    }
    /**
     * 丝带
     * @param detailCount 细节点数目
     */
    public static ribbon2(detailCount: number, delta: number = 0) {
        const one_dimensional_points = [];
        const one_dimensional_weights = [];

        const result: IPolygonData = {
            vertexs3D: [],
            faces: []
        };

        const xFunction = (x: number) => {
            x = x - Math.floor(x);
            return  Math.cos(Math.PI * x * 2) * 0.8;
        };
        const wFunction = (x: number) => {
            x = x - Math.floor(x);
            return  x < 0.3 ? (Math.sin(Math.PI * (x / 0.3) / 2)) : x > 0.8 ? (Math.cos(Math.PI * ((x - 0.8) / 0.2) / 2)) : 1.0;
        };

        const linePoints: [number, number][] = [];
        for (let i = 0; i < detailCount; i++) {
            const x = i / detailCount;
            const y = xFunction(x + delta);
            const w = wFunction(x + delta) * 0.2;

            // one_dimensional_points.push(y);
            // one_dimensional_weights.push(w);

            linePoints.push([(x - 0.5) * 2, y]);

            // if (result.vertexs3D) {
            //     result.vertexs3D[i] = [(x - 0.5) * 2, y - w, 0];
            //     result.vertexs3D[i + detailCount] = [(x - 0.5) * 2, y + w, 0];
            // }

            // result.faces = this.sphereRibbon(0, detailCount);
        }

        return this.ribbon_from_line2(linePoints, 10, wFunction);
    }
    public static ribbon_from_line2(points: [number, number][], deltaAngle: number, deltaAngleFunction?: (x: number) => number, weightFunction?: (x: number) => number, widthScaleHeight: number = 1) {

        const result: IPolygonData = {
            vertexs3D: [],
            faces: []
        };

        const detailCount = points.length;

        if (detailCount < 2) {
            return;
        }

        let cos = 0, sin = 0, deltaCos = 0, deltaSin = 0, deltaXY = 0, deltaX = 0, deltaY = 0, prePoint: [number, number] = [points[0][0], points[0][1]], nxtPoint: [number, number] = [points[0][0], points[0][1]];
        let currDeltaAngle: number;
        let deltaDistance = 0;
        let lastTempPoint: [number, number] = [points[0][0], points[0][1]];

        for (let i = 1; i < detailCount; i++) {
            nxtPoint = points[i];
            deltaX  = nxtPoint[0] - prePoint[0];
            deltaY  = nxtPoint[1] - prePoint[1];

            deltaXY = Math.sqrt(deltaX * deltaX + deltaY * deltaY);
            cos     = deltaXY === 0 ? 0 : deltaX / deltaXY;
            sin     = deltaXY === 0 ? 0 : deltaY / deltaXY;

            currDeltaAngle  = deltaAngleFunction ? deltaAngle * deltaAngleFunction((i - 1) / detailCount) : deltaAngle;
            deltaCos        = Math.cos(Math.PI * currDeltaAngle / 180);
            deltaSin        = Math.sin(Math.PI * currDeltaAngle / 180);

            deltaDistance   = weightFunction ? weightFunction(i / detailCount) : 0.01;

            // if (deltaXY > deltaDistance) {

                if (result.vertexs3D) {
                    result.vertexs3D[i - 1] = [
                        (cos * deltaCos - sin * deltaSin) * deltaDistance / widthScaleHeight  + prePoint[0],
                        (sin * deltaCos + cos * deltaSin) * deltaDistance + prePoint[1],
                        0
                    ];
                    result.vertexs3D[i - 1 + detailCount] = [
                        (cos * deltaCos + sin * deltaSin) * deltaDistance / widthScaleHeight + prePoint[0],
                        (sin * deltaCos - cos * deltaSin) * deltaDistance + prePoint[1],
                        0
                    ];
                }

                lastTempPoint = prePoint;
            // } else {
            //     if (result.vertexs3D) {
            //         result.vertexs3D[i - 1] = [
            //             (cos * deltaCos - sin * deltaSin) * deltaDistance + lastTempPoint[0],
            //             (sin * deltaCos + cos * deltaSin) * deltaDistance + lastTempPoint[1],
            //             0
            //         ];
            //         result.vertexs3D[i - 1 + detailCount] = [
            //             (cos * deltaCos + sin * deltaSin) * deltaDistance + lastTempPoint[0],
            //             (sin * deltaCos - cos * deltaSin) * deltaDistance + lastTempPoint[1],
            //             0
            //         ];
            //     }
            // }

            prePoint = points[i];
        }

        deltaCos        = Math.cos(Math.PI * 45 / 180);
        deltaSin        = Math.sin(Math.PI * 45 / 180);
        if (result.vertexs3D) {
            result.vertexs3D[detailCount - 1] = [
                (cos * deltaCos - sin * deltaSin) * deltaXY / widthScaleHeight + prePoint[0],
                (sin * deltaSin + cos * deltaCos) * deltaXY + prePoint[1],
                0
            ];
            result.vertexs3D[detailCount - 1 + detailCount] = [
                (cos * deltaCos + sin * deltaSin) * deltaXY / widthScaleHeight + prePoint[0],
                (sin * deltaSin - cos * deltaCos) * deltaXY + prePoint[1],
                0
            ];
        }

        result.faces = this.sphereRibbon(0, detailCount);

        return result;
    }
    public static sphereRibbon(pointStartIndex: number, pointCount: number): [number, number, number][] {
        const faces: [number, number, number][] = [];

        for (let i = 0; i < pointCount - 1; i++) {
            faces.push(
                [
                    pointStartIndex + i,
                    pointStartIndex + i + 1,
                    pointStartIndex + i + 1 + pointCount
                ],
                [
                    pointStartIndex + i,
                    pointStartIndex + i + 1 + pointCount,
                    pointStartIndex + i + pointCount
                ]
            );
        }

        return faces;
    }
}