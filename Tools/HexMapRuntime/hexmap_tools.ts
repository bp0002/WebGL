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
}