export namespace HexMapTools {
    export function checkRectContainPosition(width: number, height: number, posX: number, posY: number) {
        return -width <= posX && posX <= width && -height <= posY && posY <= height;
    }
    /**
     * 计算2D坐标旋转结果
     * @param cos 顺时针角度cos
     * @param sin 顺时针角度sin
     * @param posX 2D坐标
     * @param posY 2D坐标
     * @param isClockwise 坐标是否做顺时针旋转
     */
    export function rotatePos(cos: number, sin: number, posX: number, posY: number, isClockwise: boolean, res: [number, number]) {
        if (isClockwise) {
            res[0] = cos * posX + sin * posY;
            res[1] = -sin * posX + cos * posY;
        } else {
            res[0] = cos * posX - sin * posY;
            res[1] = sin * posX + cos * posY;
        }
    }

    export function rotateAABB(aabb: [number, number, number, number], radian: number) : [number, number, number, number] {
        const cos = Math.cos(radian);
        const sin = Math.sin(radian);

        const x0    = cos * aabb[0] + sin * aabb[2];
        const x00   = cos * aabb[0] + sin * aabb[3];
        const x1    = cos * aabb[1] + sin * aabb[2];
        const x11   = cos * aabb[1] + sin * aabb[3];

        const z0    = -sin * aabb[0] + cos * aabb[2];
        const z00   = -sin * aabb[0] + cos * aabb[3];
        const z1    = -sin * aabb[1] + cos * aabb[2];
        const z11   = -sin * aabb[1] + cos * aabb[3];

        return [
            Math.min(x0, x00, x1, x11),
            Math.max(x0, x00, x1, x11),
            Math.min(z0, z00, z1, z11),
            Math.max(z0, z00, z1, z11)
        ];
    }
}