import { MathTools } from "../../math/math";

type NumberMathFunction = (x: number) => number;

export interface IPolygonData {
    vertexs?: [number, number][];
    vertexs3D?: [number, number, number][];
    faces: [number, number, number][];
}

export class GeometryTools {
    public static polygon(edgeCount: number, asCenterMode: boolean): IPolygonData {
        const vertexs3D: [number, number, number][] = [];
        const faces: [number, number, number][] = [];
        const radius = 1;
        let vertexCount: number = edgeCount;

        if (edgeCount >= 3) {
            const perRadian = 3.14 * 2 / edgeCount;

            if (asCenterMode) {
                vertexCount++;
                vertexs3D.push([0, 0, 0]);
            }

            for (let i = 0; i < edgeCount; i++) {
                const pos: [number, number, number] = [Math.cos(perRadian * i) * radius, Math.sin(perRadian * i) * radius, 0];
                vertexs3D.push(pos);
            }
            if (asCenterMode) {
                for (let i = 3; i <= vertexCount; i++) {
                    faces.push([0, i - 2, i - 1]);
                }
                faces.push([0, vertexCount - 1, 1]);
            } else {
                for (let i = 3; i <= vertexCount; i++) {
                    faces.push([0, i - 2, i - 1]);
                }
            }
        }

        return { vertexs3D, faces };
    }
    public static sphere(detailH: number, detailV: number) {
        const half          = this.sphereHalf(
                                    detailH,
                                    detailV,
                                    (x: number) => {
                                        const cos = Math.cos(Math.PI * x * 2);
                                        return cos; // Math.abs(4 * x - 2) - 1; //(1 * (cos / Math.abs(cos)) - cos); // * (cos / Math.abs(cos));
                                    },
                                    (x: number) => {
                                        // const sin = Math.abs(Math.cos(x) * Math.sin(x * 2)) * 0.9 + 0.2; // Math.sin(Math.PI * x * 8);
                                        // return sin; // (1 * (sin / Math.abs(sin)) - sin); // * (sin / Math.abs(sin));
                                        return Math.sin(Math.PI * x * 2);
                                    },
                                    (x: number) => { return Math.cos(Math.PI * x / 2); },
                                    (x: number) => {
                                        return Math.sin(Math.PI * x / 2);
                                        // return (Math.pow((x * 2 - 1), 3) + 1) / 2;
                                    },
                                    true
                                );
        const halfRevert    = this.sphereHalfRevert(
                                    detailH,
                                    detailV,
                                    (x: number) => { return Math.cos(Math.PI * x * 2); },
                                    (x: number) => { return Math.sin(Math.PI * x * 2); },
                                    (x: number) => { return Math.cos(Math.PI * x / 2); },
                                    (x: number) => { return Math.sin(Math.PI * x / 2); },
                                    // (x: number) => {
                                    //     return (Math.pow((x * 2 - 1), 3) + 1) / 2;
                                    // },
                                    true
                                );

        const result: IPolygonData = {
            vertexs3D: [],
            faces: []
        };

        if (result.vertexs3D) {
            half.circleList.forEach((cicle) => {
                cicle.forEach((point) => {
                    if (result.vertexs3D) {
                        result.vertexs3D.push(point);
                    }
                });
            });

            half.facesList.forEach((faces) => {
                faces.forEach((face) => {
                    result.faces.push(face);
                });
            });

            const halfPointCount = result.vertexs3D.length;

            halfRevert.circleList.forEach((cicle) => {
                cicle.forEach((point) => {
                    if (result.vertexs3D) {
                        result.vertexs3D.push(point);
                    }
                });
            });

            halfRevert.facesList.forEach((faces) => {
                faces.forEach((face) => {
                    if (result.vertexs3D) {
                        result.faces.push([face[0] + halfPointCount, face[1] + halfPointCount, face[2] + halfPointCount]);
                    }
                });
            });
        }

        return result;
    }
    /**
     * 棱锥体
     * @param detailH 底面多边形边数目
     */
    public static pyramid(detailH: number) {
        const detailV: number = 1;

        const half          = this.sphereHalf(
                                    detailH,
                                    detailV,
                                    (x: number) => { return Math.cos(Math.PI * x * 2); },
                                    (x: number) => { return Math.sin(Math.PI * x * 2); },
                                    (x: number) => { return Math.cos(Math.PI * x / 2); },
                                    (x: number) => { return Math.sin(Math.PI * x / 2); },
                                    true
                                );
        const halfRevert    = this.sphereHalfRevert(
                                    detailH,
                                    detailV,
                                    (x: number) => { return Math.cos(Math.PI * x * 2); },
                                    (x: number) => { return Math.sin(Math.PI * x * 2); },
                                    (x: number) => { return 1; },
                                    (x: number) => { return 0; },
                                    false
                                );

        const result: IPolygonData = {
            vertexs3D: [],
            faces: []
        };

        if (result.vertexs3D) {
            half.circleList.forEach((cicle) => {
                cicle.forEach((point) => {
                    if (result.vertexs3D) {
                        result.vertexs3D.push(point);
                    }
                });
            });

            half.facesList.forEach((faces) => {
                faces.forEach((face) => {
                    result.faces.push(face);
                });
            });

            const halfPointCount = result.vertexs3D.length;

            halfRevert.circleList.forEach((cicle) => {
                cicle.forEach((point) => {
                    if (result.vertexs3D) {
                        result.vertexs3D.push(point);
                    }
                });
            });

            halfRevert.facesList.forEach((faces) => {
                faces.forEach((face) => {
                    if (result.vertexs3D) {
                        result.faces.push([face[0] + halfPointCount, face[1] + halfPointCount, face[2] + halfPointCount]);
                    }
                });
            });
        }

        return result;
    }
    /**
     * 柱体
     * @param detailH 底面多边形边数目
     */
    public static column(detailH: number) {
        const detailV: number = 1;

        const half          = this.sphereHalf(
                                detailH,
                                detailV,
                                (x: number) => { return Math.cos(Math.PI * x * 2); },
                                (x: number) => { return Math.sin(Math.PI * x * 2); },
                                (x: number) => { return 1; },
                                (x: number) => { return x; },
                                false
                            );
        const halfRevert    = this.sphereHalfRevert(
                                detailH,
                                detailV,
                                (x: number) => { return Math.cos(Math.PI * x * 2); },
                                (x: number) => { return Math.sin(Math.PI * x * 2); },
                                (x: number) => { return 1; },
                                (x: number) => { return 0; },
                                false
                            );

        const result: IPolygonData = {
            vertexs3D: [],
            faces: []
        };

        if (result.vertexs3D) {
            half.circleList.forEach((cicle) => {
                cicle.forEach((point) => {
                    if (result.vertexs3D) {
                        result.vertexs3D.push(point);
                    }
                });
            });

            half.facesList.forEach((faces) => {
                faces.forEach((face) => {
                    result.faces.push(face);
                });
            });

            const halfPointCount = result.vertexs3D.length;

            halfRevert.circleList.forEach((cicle) => {
                cicle.forEach((point) => {
                    if (result.vertexs3D) {
                        result.vertexs3D.push(point);
                    }
                });
            });

            halfRevert.facesList.forEach((faces) => {
                faces.forEach((face) => {
                    if (result.vertexs3D) {
                        result.faces.push([face[0] + halfPointCount, face[1] + halfPointCount, face[2] + halfPointCount]);
                    }
                });
            });
        }

        return result;
    }
    /**
     * 反棱柱
     * @param detailH 底面多边形边数目
     */
    public static antiPrism(detailH: number) {
        const detailV: number = 1;

        const half          = this.sphereHalf(
                                detailH,
                                detailV,
                                (x: number) => { return Math.cos(Math.PI * x * 2); },
                                (x: number) => { return Math.sin(Math.PI * x * 2); },
                                (x: number) => { return 1; },
                                (x: number) => { return x; },
                                false
                            );
        const halfRevert    = this.sphereHalfRevert(
                                detailH,
                                detailV,
                                (x: number) => { return Math.cos(Math.PI * x * 2); },
                                (x: number) => { return Math.sin(Math.PI * x * 2); },
                                (x: number) => { return 1; },
                                (x: number) => { return 0; },
                                false
                            );

        const result: IPolygonData = {
            vertexs3D: [],
            faces: []
        };

        const deltaRadian = Math.PI / detailH;

        if (result.vertexs3D) {
            half.circleList.forEach((cicle) => {
                cicle.forEach((point) => {
                    if (result.vertexs3D) {
                        result.vertexs3D.push(point);
                    }
                });
            });

            half.facesList.forEach((faces) => {
                faces.forEach((face) => {
                    result.faces.push(face);
                });
            });

            const halfPointCount = result.vertexs3D.length;

            halfRevert.circleList.forEach((cicle) => {
                cicle.forEach((point) => {
                    if (result.vertexs3D) {
                        result.vertexs3D.push([MathTools.cos_a_add_b(point[1], point[0], deltaRadian), MathTools.sin_a_add_b(point[1], point[0], deltaRadian), point[2]]);
                    }
                });
            });

            halfRevert.facesList.forEach((faces) => {
                faces.forEach((face) => {
                    if (result.vertexs3D) {
                        result.faces.push([face[0] + halfPointCount, face[1] + halfPointCount, face[2] + halfPointCount]);
                    }
                });
            });
        }

        return result;
    }
    /**
     * 丝带
     * @param detailCount 细节点数目
     */
    public static ribbon(detailCount: number) {
        const one_dimensional_points = [];
        const one_dimensional_weights = [];

        const result: IPolygonData = {
            vertexs3D: [],
            faces: []
        };

        const xFunction = (x: number) => {
            return  x * Math.cos(Math.PI * x * 2) * 0.8;
        };
        const wFunction = (x: number) => {
            return  x < 0.3 ? (Math.sin(Math.PI * (x / 0.3) / 2)) : x > 0.8 ? (Math.cos(Math.PI * ((x - 0.8) / 0.2) / 2)) : 1.0;
        };
        for (let i = 0; i < detailCount; i++) {
            const x = i / detailCount;
            const y = xFunction(x);
            const w = wFunction(x) * 0.2;

            one_dimensional_points.push(y);
            one_dimensional_weights.push(w);

            if (result.vertexs3D) {
                result.vertexs3D[i] = [(x - 0.5) * 2, y - w, 0];
                result.vertexs3D[i + detailCount] = [(x - 0.5) * 2, y + w, 0];
            }

            result.faces = this.sphereRibbon(0, detailCount);
        }

        return result;
    }
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

        return this.ribbon_from_line2(linePoints, 90, wFunction);
    }
    public static ribbon_from_line2(points: [number, number][], deltaAngle: number, deltaAngleFunction?: (x: number) => number, weightFunction?: (x: number) => number) {

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
            cos     = deltaX / deltaXY;
            sin     = deltaY / deltaXY;

            currDeltaAngle  = deltaAngleFunction ? deltaAngle * deltaAngleFunction((i - 1) / detailCount) : deltaAngle;
            deltaCos        = Math.cos(Math.PI * currDeltaAngle / 180);
            deltaSin        = Math.sin(Math.PI * currDeltaAngle / 180);

            deltaDistance   = weightFunction ? weightFunction(i / detailCount) : 0.01;

            // if (deltaXY > deltaDistance) {

                if (result.vertexs3D) {
                    result.vertexs3D[i - 1] = [
                        (cos * deltaCos - sin * deltaSin) * deltaDistance + prePoint[0],
                        (sin * deltaCos + cos * deltaSin) * deltaDistance + prePoint[1],
                        0
                    ];
                    result.vertexs3D[i - 1 + detailCount] = [
                        (cos * deltaCos + sin * deltaSin) * deltaDistance + prePoint[0],
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

        if (result.vertexs3D) {
            result.vertexs3D[detailCount - 1] = [
                (cos * deltaCos - sin * deltaSin) * deltaXY + prePoint[0],
                (sin * deltaSin + cos * deltaCos) * deltaXY + prePoint[1],
                0
            ];
            result.vertexs3D[detailCount - 1 + detailCount] = [
                (cos * deltaCos + sin * deltaSin) * deltaXY + prePoint[0],
                (sin * deltaSin - cos * deltaCos) * deltaXY + prePoint[1],
                0
            ];
        }

        result.faces = this.sphereRibbon(0, detailCount);

        return result;
    }
    public static sphereHalfRevert(detailH: number, detailV: number, xFunction: NumberMathFunction, yFunction: NumberMathFunction, detailHFucntion: NumberMathFunction, detailVFucntion: NumberMathFunction, topIsProtruding: boolean = true) {
        const { circleList, facesList } = this.sphereHalf(detailH, detailV, xFunction, yFunction, detailHFucntion, detailVFucntion, topIsProtruding);

        circleList.forEach((circle) => {
            circle.reverse();
            circle.forEach((point) => {
                point[2] *= -1;
            });
            // const total = circle.length;
            // const count = circle.length / 2;
            // for (let i = 0; i < count; i++) {
            //     const temp = circle[i];

            //     circle[i]           = circle[total - i];
            //     circle[total - i]   = temp;
            // }
        });

        return { circleList, facesList };
    }
    public static sphereHalf(detailH: number, detailV: number, xFunction: NumberMathFunction, yFunction: NumberMathFunction, detailHFucntion: NumberMathFunction, detailVFucntion: NumberMathFunction,  topIsProtruding: boolean = true) {
        const baseCirclePoints = this.circlePoints(detailH, xFunction, yFunction);
        const circleList: [number, number, number][][] = [];
        const facesList: [number, number, number][][] = [];

        let circlePointCount: number = 0;

        const vcount = topIsProtruding ? detailV : detailV + 1 ;

        for (let i = 0; i < vcount; i++) {
            const vDistance = detailVFucntion(i / detailV);
            const radius    = detailHFucntion(i / detailV); // Math.sqrt(1 - (i * i) / (detailV * detailV));

            // const vDistance = Math.sin(Math.PI * i / detailV / 2);
            // const radius    = Math.cos(Math.PI * i / detailV / 2);

            const tempList  = this.circlePointsScale(this.copyCirclePoints(baseCirclePoints), radius);

            circlePointCount = tempList.length;

            if (i > 0) {
                facesList.push(this.sphereRing(circlePointCount * (i - 1), circlePointCount));
            }
            circleList.push(this.circleFillZ(tempList, vDistance));
        }

        facesList.push(this.sphereTop(circlePointCount * (circleList.length - 1), circlePointCount));

        circleList.push([[0, 0, detailVFucntion(1)]]);

        return {
            circleList,
            facesList
        };
    }
    public static sphereRing(pointStartIndex: number, pointCount: number): [number, number, number][] {
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

        const i = pointCount - 1;
        faces.push(
            [
                pointStartIndex + i,
                pointStartIndex + 0,
                pointStartIndex + 0 + pointCount
            ],
            [
                pointStartIndex + i,
                pointStartIndex + 0 + pointCount,
                pointStartIndex + i + pointCount
            ]
        );

        return faces;
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
    public static sphereTop(pointStartIndex: number, pointCount: number) {
        const faces: [number, number, number][] = [];

        for (let i = 0; i < pointCount - 1; i++) {
            faces.push(
                [
                    pointStartIndex + i,
                    pointStartIndex + i + 1,
                    pointStartIndex + pointCount
                ]
            );
        }

        faces.push(
            [
                pointStartIndex + pointCount - 1,
                pointStartIndex + 0,
                pointStartIndex + pointCount
            ]
        );

        return faces;
    }
    private static circleFillZ(circle: [number, number][], z: number) {
        const result: [number, number, number][] = [];
        circle.forEach((point) => {
            result.push([point[0], point[1], z]);
        });

        return result;
    }
    private static circlePoints(detail: number, xFunction: (x: number) => number, yFunction: (x: number) => number) {
        const points: [number, number][] = [];

        for (let i = 0; i < detail; i++) {
            points.push([xFunction(i / detail), yFunction(i / detail)]);
        }

        return points;
    }
    private static circlePointsDym(detail: number, radius: number, xFunction: (x: number) => number, yFunction: (x: number) => number) {
        const points = this.circlePoints(detail, xFunction, yFunction);

        return this.circlePointsScale(points, radius);
    }
    private static circlePointsScale(points: [number, number][], radius: number) {

        points.forEach((point) => {
            point[0] *= radius;
            point[1] *= radius;
        });

        return points;
    }
    private static copyCirclePoints(points: [number, number][]) {
        const result: [number, number][] = [];
        points.forEach((point) => {
            result.push([point[0], point[1]]);
        });

        return result;
    }
}