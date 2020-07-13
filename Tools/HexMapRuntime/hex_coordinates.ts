import { Parame } from "./parame";
import { HexMapTools } from "./hexmap_tools";

export namespace HexCoordinates {
    export const  sin45 = Math.sin(Math.PI * 0.25);
    export const  cos45 = Math.cos(Math.PI * 0.25);

    export const  squrt2 = 1.414213562373;
    export const  squrt3 = Math.pow(3, 0.5);

    export let  TempFX: number;
    export let  TempFY: number;
    export let  TempFZ: number;

    /**
     * (HexCell)单元格 所属共享 (BaseCell)单元格列表
     * * 列表数据 [ BaseCell0.iX, BaseCell0.iY, BaseCell0.iZ, ...]
     * * 没有宽度边的单元格，所属共享 即自己
     * * 有宽度边的单元格 - 有 块/边/点 三种单元格
     *  + BaseCell 所属共享 即自己
     *  + EdgeCell 所属有 2 个 BaseCell
     *  + PointCell 所属有 4 个 BaseCell
     *       +        +  +        +
     *       |        |  |        |
     *       +-----------------------+
     *       |        |  |        |
     *       |   c    |4 |   d    |
     *       |        |  |        |
     *       |        |  |        |
     *       +-----------------------+
     *       |   2    |o |    3   |
     *       +------------------------+
     *       |        |  |        |
     *       |   a    |  |   b    |
     *       |        |1 |        |
     *       |        |  |        |
     *       +--------+--+--------+---+
     *      - BaseCell: a,b,c,d
     *      - EdgeCell: 1,2,3,4
     *      - PointCell: o
     *      - 1 的 ShareOwnerBaseCell [ a, b ]
     *      - 2 的 ShareOwnerBaseCell [ a, c ]
     *      - o 的 ShareOwnerBaseCell [ a, b, c, d ]
     *      - ShareOwnerBaseCell 排列顺序规则: x 由小到大, 再 z 由小到大
     */
    export const  TempShareOwnerList: number[] = [];
    /**
     * (HexCell)单元格 所属 (BaseCell)单元格列表
     * * 列表数据 [ BaseCell0.iX, BaseCell0.iY, BaseCell0.iZ, ...]
     * * 没有宽度边的单元格，所属即自己
     * * 有宽度边的单元格 - 有 块/边/点 三种单元格
     *  +-------+--+
     *  |   2   |3 |
     *  +----------+
     *  |       |  |
     *  |   0   |1 |
     *  |       |  |
     *  |       |  |
     *  +-------+--+
     *      + 0 - BaseCell
     *      + 1,2 - EdgeCell
     *      + 3 - PointCell
     */
    export const TempBaseCell: number[] = [];

    /**
     * 计算两 Hex 坐标间距离
     * @param a Hex 坐标信息
     * @param b Hex 坐标信息
     */
    export function  ComputeBaseCellDistance(aShareList: number[], bShareList: number[], isHex: boolean, isRotate: boolean) : number
    {
        const ahx = aShareList[0];
        const ahy = aShareList[1];
        const ahz = aShareList[2];

        const bhx = bShareList[0];
        const bhy = bShareList[1];
        const bhz = bShareList[2];

        if (!isHex && !isRotate)
        {
            if ((ahx - bhx) * (ahz - bhz) > 0)
            {
                return Math.abs(ahx - bhx) + (Math.abs(ahy - bhy) + Math.abs(ahz - bhz));
            } else
            {
                return Math.max(Math.abs(ahx - bhx), Math.max(Math.abs(ahy - bhy), Math.abs(ahz - bhz)));
            }
        }
        else
        {
            return Math.max(Math.abs(ahx - bhx), Math.max(Math.abs(ahy - bhy), Math.abs(ahz - bhz)));
        }
    }

    /**
     * 从 像素 坐标位置 获取 BaseCell 信息
     * 像素坐标为单元格相对 网格层 原点坐标
     * @param x x
     * @param y y
     * @param z z
     * @param size 单元格尺寸
     * @param isHex 是否六边形
     * @param isRotate 是否旋转- 一条边在上
     */
    export function  FormatBaseCellFromPixelPosition(x: number, y: number, z: number, size: number, isHex: boolean, isRotate: boolean) {
        if (isHex) {
            HexCoordinates.FormatBaseCellFromPixelPositionHex(x, y, z, size, isHex, isRotate);
        } else {
            HexCoordinates.FormatBaseCellFromPixelPositionSquare(x, y, z, size, isHex, isRotate);
        }
    }

    /**
     * 从 像素 坐标位置 获取 BaseCell 信息
     * 像素坐标为单元格相对 网格层 原点坐标
     * @param x x
     * @param y y
     * @param z z
     * @param size 单元格尺寸
     * @param isHex 是否六边形
     * @param isRotate 是否旋转- 一条边在上
     */
    export function FormatBaseCellFromPixelPositionSquare(x: number, y: number, z: number, size: number, isHex: boolean, isRotate: boolean) {

        let HX, HY, HZ;

        // 尖角向上
        if (!isRotate)
        {
            HX = Math.round((HexCoordinates.cos45 * x - HexCoordinates.sin45 * z) / size);
            HY = 0;
            HZ = Math.round((HexCoordinates.sin45 * x + HexCoordinates.cos45 * z) / size);
        }
        else
        {
            HX = Math.round((x) / size);
            HY = 0;
            HZ = Math.round((z) / size);
        }

        HexCoordinates.TempBaseCell.length = 0;
        HexCoordinates.TempBaseCell.push(HX, HY, HZ);
    }

    /**
     * 从 像素 坐标位置 获取 BaseCell 信息
     * 像素坐标为单元格相对 网格层 原点坐标
     * @param x x
     * @param y y
     * @param z z
     * @param size 单元格尺寸
     * @param isHex 是否六边形
     * @param isRotate 是否旋转- 一条边在上
     */
    export function FormatBaseCellFromPixelPositionHex(x: number, y: number, z: number, size: number, isHex: boolean, isRotate: boolean) {

        size = size / 2.0;

        let HX, HY, HZ;

        // 尖角向上
        if (!isRotate)
        {
            let q = (HexCoordinates.squrt3 / 3 * x - 1.0 / 3 * z) / size;
            let r = (2.0 / 3 * z) / size;

            let fhx = q;
            let fhy = -q - r;
            let fhz = r;

            let iX = Math.round(fhx);
            let iY = Math.round(fhy);
            let iZ = Math.round(fhz);

            if (iX + iY + iZ != 0)
            {
                let dx = Math.abs(fhx - iX);
                let dy = Math.abs(fhy - iY);
                let dz = Math.abs(-fhx - fhy - iZ);

                if (dx > dy && dx > dz)
                {
                    iX = -iY - iZ;
                }
                else if (dy > dz) {
                    iY = -iX - iZ;
                }
                else
                {
                    iZ = -iX - iY;
                }
            }

            HX = iX;
            HY = iY;
            HZ = iZ;
        }
        else
        {
            let q = (2.0 / 3 * x) / size;
            let r = (-1.0 / 3 * x + HexCoordinates.squrt3 / 3 * z) / size;

            let fhx = q;
            let fhy = -q - r;
            let fhz = r;

            let iX = Math.round(fhx);
            let iY = Math.round(fhy);
            let iZ = Math.round(fhz);

            if (iX + iY + iZ != 0)
            {
                let dx = Math.abs(fhx - iX);
                let dy = Math.abs(fhy - iY);
                let dz = Math.abs(-fhx - fhy - iZ);

                if (dx > dy && dx > dz)
                {
                    iX = -iY - iZ;
                }
                else if (dy > dz) {
                    iY = -iX - iZ;
                }
                else
                {
                    iZ = -iX - iY;
                }
            }

            HX = iX;
            HY = iY;
            HZ = iZ;
        }

        HexCoordinates.TempBaseCell.length = 0;
        HexCoordinates.TempBaseCell.push(HX, HY, HZ);
    }

    export function  FormatCellName(shareInfo: number[]) {
        return `[${shareInfo.toString()}]`;
    }

    /**
     * 计算 指定 BaseCell 相对坐标
     * @param iX  网格坐标系 X 坐标
     * @param iY  网格坐标系 Y 坐标
     * @param iZ  网格坐标系 Z 坐标
     * @param cellSize 单元格大小
     * @param isHex 是否六边形
     * @param isRotate 是否旋转
     */
    export function  ComputeBaseCellLocalPos(iX: number, iY: number, iZ: number, cellSize: number, isHex: boolean, isRotate: boolean) {
        if (isHex) {
            if (isRotate) {
                HexCoordinates.TempFX = cellSize * (3.0 / 2 * iX);
                HexCoordinates.TempFY = 0;
                HexCoordinates.TempFZ = cellSize * (HexCoordinates.squrt3 / 2 * iX + HexCoordinates.squrt3 * iZ);
            }
            else
            {
                HexCoordinates.TempFX = cellSize * (HexCoordinates.squrt3 * iX + HexCoordinates.squrt3 / 2 * iZ);
                HexCoordinates.TempFY = 0;
                HexCoordinates.TempFZ = cellSize * (3.0 / 2 * iZ);
            }
        }
        else
        {
            if (isRotate) {
                HexCoordinates.TempFX = cellSize * iX;
                HexCoordinates.TempFY = 0;
                HexCoordinates.TempFZ = cellSize * iZ;
            }
            else
            {
                let q = cellSize * iX;
                let r = cellSize * iY;

                HexCoordinates.TempFX = HexCoordinates.cos45 * q + HexCoordinates.sin45 * r;
                HexCoordinates.TempFY = 0;
                HexCoordinates.TempFZ = -HexCoordinates.sin45 * q + HexCoordinates.cos45 * r;
            }
        }
    }

    export function  ComputeLocalPos(iX: number, iY: number, iZ: number, cellType: number, isHex: boolean, isRotate: boolean) {

    }

    /**
     * 由 单元格 内部相对坐标,检查命中的单元格类型
     * 类型为 Parame 中定义
     * @param hitLocalPosX 单元格 内部相对坐标
     * @param hitLocalPosY 单元格 内部相对坐标
     * @param hitLocalPosZ 单元格 内部相对坐标
     * @param edgeWidth 单元格边宽度
     * @param cellSize 单元格尺寸
     * @param isHex 是否六边形
     * @param isRotate 是否旋转
     */
    export function  CheckHitCellType(hitLocalPosX: number, hitLocalPosY: number, hitLocalPosZ: number, edgeWidth: number, cellSize: number, isHex: boolean, isRotate: boolean) {

        let res = 0;

        if (isHex)
        {
            if (isRotate)
            {
                res = Parame.RHexType;
            }
            else
            {
                res = Parame.HexType;
            }
        }
        else
        {

            if (isRotate)
            {
                res = Parame.RSquareType;

                let tempWidth = (1 - edgeWidth) * cellSize * 0.5;

                if (Math.abs(hitLocalPosZ) >= (tempWidth) && Math.abs(hitLocalPosX) >= (tempWidth))
                {
                    if (hitLocalPosZ > 0)
                    {
                        if (hitLocalPosX > 0)
                        {
                            res = Parame.RSquarePointType1;
                        } else
                        {
                            res = Parame.RSquarePointType4;
                        }
                    }
                    else
                    {
                        if (hitLocalPosX > 0)
                        {
                            res = Parame.RSquarePointType2;
                        }
                        else
                        {
                            res = Parame.RSquarePointType3;
                        }
                    }
                }
                else if (Math.abs(hitLocalPosZ) >= (tempWidth) || Math.abs(hitLocalPosX) >= (tempWidth))
                {
                    if (hitLocalPosX > tempWidth)
                    {
                        res = Parame.RSquareEdgeType1;
                    }
                    else if (hitLocalPosX < -tempWidth)
                    {
                        res = Parame.RSquareEdgeType3;
                    }
                    else if (hitLocalPosZ > tempWidth)
                    {
                        res = Parame.RSquareEdgeType4;
                    }
                    else if (hitLocalPosZ < -tempWidth)
                    {
                        res = Parame.RSquareEdgeType2;
                    }
                }
            }
            else
            {
                res = Parame.SquareType;

                let tempPos: [number, number] = [0, 0];
                HexMapTools.rotatePos(HexCoordinates.sin45, HexCoordinates.sin45, hitLocalPosX, hitLocalPosZ, false, tempPos);

                hitLocalPosX = tempPos[0];
                hitLocalPosZ = tempPos[1];

                let tempWidth = (1 - edgeWidth) * cellSize * 0.5;

                if (Math.abs(hitLocalPosZ) >= (tempWidth) && Math.abs(hitLocalPosX) >= (tempWidth))
                {
                    if (hitLocalPosZ > 0)
                    {
                        if (hitLocalPosX > 0)
                        {
                            res = Parame.SquarePointType1;
                        } else
                        {
                            res = Parame.SquarePointType4;
                        }
                    }
                    else
                    {
                        if (hitLocalPosX > 0)
                        {
                            res = Parame.SquarePointType2;
                        }
                        else
                        {
                            res = Parame.SquarePointType3;
                        }
                    }
                }
                else if (Math.abs(hitLocalPosZ) >= (tempWidth) || Math.abs(hitLocalPosX) >= (tempWidth))
                {
                    if (hitLocalPosX > tempWidth)
                    {
                        res = Parame.SquareEdgeType1;
                    }
                    else if (hitLocalPosX < -tempWidth)
                    {
                        res = Parame.SquareEdgeType3;
                    }
                    else if (hitLocalPosZ > tempWidth)
                    {
                        res = Parame.SquareEdgeType4;
                    }
                    else if (hitLocalPosZ < -tempWidth)
                    {
                        res = Parame.SquareEdgeType2;
                    }
                }
            }
        }

        return res;
    }

    /**
     * 查找 (HexCell)单元格 的所属共享 (BaseCell)单元格列表
     * @param ownX BaseCell 网格系统坐标
     * @param ownY BaseCell 网格系统坐标
     * @param ownZ BaseCell 网格系统坐标
     * @param cellType HexCell 在 BaseCell 中的类型
     * @param isHex 是否六边形
     * @param isRotate 是否旋转
     */
    export function  FormatShareOwnerListWithType(ownX: number, ownY: number, ownZ: number, cellType: number, isHex: boolean, isRotate: boolean) {
        ownY = 0;

        HexCoordinates.TempShareOwnerList.length = 0;

        const nearList = HexCoordinates.TempShareOwnerList;

        if (isHex)
        {
            nearList.push(ownX, ownY, ownZ);
        }
        else
        {
            HexCoordinates.FormatShareOwnerListWithTypeS(ownX, ownY, ownZ, cellType);
        }
    }

    /**
     * 查找 (HexCell)单元格 的所属共享 (BaseCell)单元格列表 - (旋转正方形的网格系统)
     * @param ownX BaseCell 网格系统坐标
     * @param ownY BaseCell 网格系统坐标
     * @param ownZ BaseCell 网格系统坐标
     * @param cellType HexCell 在 BaseCell 中的类型
     */
    export function  FormatShareOwnerListWithTypeS(ownX: number, ownY: number, ownZ: number, cellType: number) {
        ownY = 0;

        HexCoordinates.TempShareOwnerList.length = 0;

        const nearList = HexCoordinates.TempShareOwnerList;

        switch (cellType)
        {
            case (Parame.RSquarePointType1):
            case (Parame.SquarePointType1):
                {
                    nearList.push(ownX, ownY, ownZ);
                    nearList.push(ownX + 1, ownY, ownZ);
                    nearList.push(ownX, ownY, ownZ + 1);
                    nearList.push(ownX + 1, ownY, ownZ + 1);
                    break;
                }
            case (Parame.RSquarePointType2):
            case (Parame.SquarePointType2):
                {
                    nearList.push(ownX, ownY, ownZ - 1);
                    nearList.push(ownX + 1, ownY, ownZ - 1);
                    nearList.push(ownX, ownY, ownZ);
                    nearList.push(ownX + 1, ownY, ownZ);
                    break;
                }
            case (Parame.RSquarePointType3):
            case (Parame.SquarePointType3):
                {
                    nearList.push(ownX - 1, ownY, ownZ - 1);
                    nearList.push(ownX, ownY, ownZ - 1);
                    nearList.push(ownX - 1, ownY, ownZ);
                    nearList.push(ownX, ownY, ownZ);
                    break;
                }
            case (Parame.RSquarePointType4):
            case (Parame.SquarePointType4):
                {
                    nearList.push(ownX - 1, ownY, ownZ);
                    nearList.push(ownX, ownY, ownZ);
                    nearList.push(ownX - 1, ownY, ownZ + 1);
                    nearList.push(ownX, ownY, ownZ + 1);
                    break;
                }
            case (Parame.RSquareEdgeType1):
            case (Parame.SquareEdgeType1):
                {
                    nearList.push(ownX, ownY, ownZ);
                    nearList.push(ownX + 1, ownY, ownZ);
                    break;
                }
            case (Parame.RSquareEdgeType2):
            case (Parame.SquareEdgeType2):
                {
                    nearList.push(ownX, ownY, ownZ - 1);
                    nearList.push(ownX, ownY, ownZ);
                    break;
                }
            case (Parame.RSquareEdgeType3):
            case (Parame.SquareEdgeType3):
                {
                    nearList.push(ownX - 1, ownY, ownZ);
                    nearList.push(ownX, ownY, ownZ);
                    break;
                }
            case (Parame.RSquareEdgeType4):
            case (Parame.SquareEdgeType4):
                {
                    nearList.push(ownX, ownY, ownZ);
                    nearList.push(ownX, ownY, ownZ + 1);
                    break;
                }
            default: {
                nearList.push(ownX, ownY, ownZ);
            }
        }
    }
}