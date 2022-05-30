/**
 * 数学函数库
 */

export class MathTools {
    public static sin(x: number) {
        return Math.sin(x);
    }
    public static cos(x: number) {
        return Math.cos(x);
    }
    public static polarCoordToCartesian = (num: number) => {
        return {
            x: Math.cos(num) * num,
            y: Math.sin(num) * num
        };
    }
    public static isPrimeNumber(n: number) {
        if (n < 2) {
            return false;
        }
        for (let i = 2; i <= n - 1; i++) {
            if (n % i == 0) {
                return false;
            }
        }
        return true;
    }
    /**
     * @description 下一个2的冥的数
     */
    public static nextPowerOfTwo(value: number) {
        --value;
        value |= value >> 1;
        value |= value >> 2;
        value |= value >> 4;
        value |= value >> 8;
        value |= value >> 16;

        return ++value;
    }
    /**
     * sin(a+b) = sin(a) * cos(b) + cos(a) * sin(b)
     * @param sin_a
     * @param cos_a
     * @param b
     */
    public static sin_a_add_b(sin_a: number, cos_a: number, b: number) {
        return sin_a * Math.cos(b) + cos_a * Math.sin(b);
    }
    /**
     * cos(a+b) = cos(a) * cos(b) - sin(a) * sin(b)
     * @param sin_a
     * @param cos_a
     * @param b
     */
    public static cos_a_add_b(sin_a: number, cos_a: number, b: number) {
        return cos_a * Math.cos(b) - sin_a * Math.sin(b);
    }
}