export class FloatScalar {
    public static Pi = Math.PI;
    public static TwoPi = Math.PI * 2;
    public static isNaN(value: number): boolean {
        return isNaN(value);
    }
    /**
     * Boolean: 检查两个数值是否相等
     * @param a left value
     * @param b right value
     * @returns a == b
     */
    public static Equal(a: number, b: number): boolean {
        return a == b;
    }
    /**
     * Boolean: 检查两个数值是否在指定误差范围内相等
     * @param a left value
     * @param b right value
     * @param epsilon (default = 1.401298E-45)
     * @returns if the absolute diffrence between a and b is lower than epsilon
     */
    public static EqualWithEpsilon(a: number, b: number, epsilon: number = 1.401298E-45): boolean {
        let diff = a - b;
        return -epsilon < diff && diff < epsilon;
    }
    /**
     * Returens -1 if value is negative, returns +1 if value is positive, or reuturn 0
     * @param value the value
     */
    public static Sign(value: number): 0 | 1 | -1 {
        value = +value;

        if (value === 0 || this.isNaN(value)) {
            return 0;
        }

        return value > 0 ? 1 : -1;
    }
    /**
     * Returns the value it self if it's between minimum and maximum
     * Returns minimum if the value is lower than minimum
     * Returns maximum if the value is greater than maximum
     * @param value the value to clamp
     * @param minimum the minimum (default 0)
     * @param maximum the maximum (default 1)
     * @returns the clamped value
     */
    public static Clamp(value: number, minimum: number = 0, maximum: number = 1): number {
        return Math.min(maximum, Math.max(minimum, value));
    }
    /**
     * Normalized the value between 0.0 and 1.0 using min and max value
     * @param value the value
     * @param min min to normalize between
     * @param max max to normalize between
     * @returns the normalize value
     */
    public static Normalize(value: number, min: number, max: number): number {
        if (max != min) {
            return this.Clamp((value - min) / (max - min));
        } else {
            return 0.;
        }
    }
    /**
     * Denormalize the value between 0.0 and 1.0 using min and max value
     * @param normalised the normalised value
     * @param min min to normalize between
     * @param max max to normalize between
     * @returns the normalize value
     */
    public static Denormalize(normalised: number, min: number, max: number): number {
        return normalised * (max - min) + min;
    }
    /**
     * Detransform the value to new coordinate using min and max value
     * @param transformed the transformed value
     * @param min min to transform between
     * @param max max to transform between
     * @returns the transform value
     */
    public static Detransform(transformed: number, min: number, max: number) {
        return min * (1.0 - transformed) +  max * transformed;
    }
    /**
     * Transform the value to new coordinate using min and max value
     * @param value the value
     * @param min min to transform between
     * @param max max to transform between
     * @returns the transform value
     */
    public static Transform(value: number, min: number, max: number) {
        if (max != min) {
            return (value - min) / (max - min);
        } else {
            return 0.;
        }
    }
    /**
     * Diffrent with modulo operator
     * Repeat(-5, 3) = 1
     * -Infinity to Infinity
     * -5 mod 3 = -2
     * 0 to -Infinity or Infinity
     * @param value the value
     * @param length step length
     */
    public static Repeat(value: number, length: number): number {
        return value - Math.floor(value / length) * length;
    }
    /**
     * PingPongs the value t
     * @param value the value
     * @param length the step length
     */
    public static PingPong(value: number, length: number): number {
        let t = FloatScalar.Repeat(value, length * 2.0);
        return length - Math.abs(t - length);
    }
    /**
     * Calculate the shortest diffrence between two angles in degrees.
     * @param current current degrees
     * @param target target degrees
     * @returns delta degrees
     */
    public static DeltaAngle(current: number, target: number): number {
        let num = FloatScalar.Repeat(target - current, 360.0);
        if (num > 180.0) {
            num -= 360.0;
        }

        return num;
    }
    public static Step(x1: number, x2: number): number {
        return x1 <= x2 ? 1. : 0.;
    }
    public static SmoothStep(from: number, to: number, tx: number): number {
        let t = FloatScalar.Clamp(tx, 0., 1.);
        let tt = t * t;
        t = -2.0 * tt * t + 3.0 * tt;

        return FloatScalar.Detransform(t, from, to);
    }
    public static Lerp(from: number, to: number, tx: number): number {
        return FloatScalar.Detransform(tx, from, to);
    }
    /**
     * Hermite interplate
     * @param value1 spline value
     * @param tangent1 spline tangent
     * @param value2 spline value
     * @param tangent2 spline tangent
     * @param amount amount
     */
    public static Hermite(value1: number, tangent1: number, value2: number, tangent2: number, amount: number) : number {
        let squared = amount * amount;
        let cubed = squared * amount;
        let part1 = ((2.0 * cubed) - (3.0 * squared)) + 1.0;
        let part2 = (-2.0 * cubed) + (3.0 * squared);
        let part3 = (cubed - (2.0 * squared)) + amount;
        let part4 = cubed - squared;

        return value1 * part1 + value2 * part2 + tangent1 * part3 + tangent2 * part4;
    }

    public static RandomRange(min: number, max: number) {
        if (min == max) {
            return min;
        } else {
            return FloatScalar.Lerp(min, max, Math.random());
        }
    }

    public static FormatString(value: number, length: number, decimalLenght: number, char: string = " ") {
        let result = value.toFixed(decimalLenght);
        let tempLength = result.length;
        for (let i = length - tempLength; i >= 0; i--) {
            result = char + result;
        }

        return result;
    }
}