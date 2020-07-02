export class HexCoordinates {
    public hx: number = 0;
    public hy: number = 0;
    public hz: number = 0;

    public fx: number = 0;
    public fy: number = 0;
    public fz: number = 0;

    public col: number = 0;
    public row: number = 0;

    public readonly isRotate: boolean;
    public readonly isHex: boolean;

    public static squrt2 = 1.414213562373;
    public static squrt3 = Math.pow(3, 0.5);

    constructor(isHex: boolean, isRotate: boolean) {
        this.isHex = isHex;
        this.isRotate = isRotate;
    }

    /**
     * 获取ID
     */
    public getID() {
        return `${this.hx}_${this.hy}_${this.hz}`;
    }

    /**
     * 查询指定距离内的坐标信息
     * @param distance 距离
     * @param size 坐标尺寸
     * @param isHex 是否为六边形
     * @param isRotate 是否旋转- 一条边在上
     */
    public SelectByDistance(distance: number, size: number, isHex: boolean, isRotate: boolean): HexCoordinates[][]
    {
        if (isHex) {
            return this.SelectByDistanceHex(distance, size, isHex, isRotate);
        } else {
            return this.SelectByDistanceSquare(distance, size, isHex, isRotate);
        }

    }

    /**
     * 查询指定距离内的坐标信息
     * @param distance 距离
     * @param size 坐标尺寸
     * @param isHex 是否为六边形
     * @param isRotate 是否旋转- 一条边在上
     */
    private SelectByDistanceHex(distance: number, size: number, isHex: boolean, isRotate: boolean): HexCoordinates[][]
    {
        const list = [];
        const checkList: string[] = [];

        if (distance < 1)
        {
        }
        else
        {
            let start = 1;
            let tempID = '';

            list.push([this]);
            checkList.push(this.getID());

            for (let t = start; t <= distance; t++)
            {

                const tempList = [];

                for (let i = 0; i <= t; i++)
                {
                    let temp = HexCoordinates.FromIntPostionHex(
                        this.hx + t,
                        this.hy + (-i),
                        this.hz + (-(t - i)),
                        size, isRotate
                    );
                    tempID = temp.getID();

                    if (!checkList.includes(tempID)) {
                        checkList.push(tempID);
                        tempList.push(temp);
                    }

                    temp = HexCoordinates.FromIntPostionHex(
                        this.hx + t * -1,
                        this.hy + (-(t - i)) * -1,
                        this.hz + (-i) * -1,
                        size, isRotate
                    );
                    tempID = temp.getID();

                    if (!checkList.includes(tempID)) {
                        checkList.push(tempID);
                        tempList.push(temp);
                    }
                }
                for (let i = 0; i <= t; i++)
                {
                    let temp = HexCoordinates.FromIntPostionHex(
                        this.hx + (-i),
                        this.hy + t,
                        this.hz + (-(t - i)),
                        size, isRotate
                    );
                    tempID = temp.getID();

                    if (!checkList.includes(tempID)) {
                        checkList.push(tempID);
                        tempList.push(temp);
                    }

                    temp = HexCoordinates.FromIntPostionHex(
                        this.hx + (-(t - i)) * -1,
                        this.hy + t * -1,
                        this.hz + (-i) * -1,
                        size, isRotate
                    );
                    tempID = temp.getID();

                    if (!checkList.includes(tempID)) {
                        checkList.push(tempID);
                        tempList.push(temp);
                    }
                }

                for (let i = 0; i <= t; i++)
                {
                    let temp = HexCoordinates.FromIntPostionHex(
                        this.hx + (-(t - i)),
                        this.hy + (-i),
                        this.hz + t,
                        size, isRotate
                    );
                    tempID = temp.getID();

                    if (!checkList.includes(tempID)) {
                        checkList.push(tempID);
                        tempList.push(temp);
                    }

                    temp = HexCoordinates.FromIntPostionHex(
                        this.hx + (-i) * -1,
                        this.hy + (-(t - i)) * -1,
                        this.hz + t * -1,
                        size, isRotate
                    );
                    tempID = temp.getID();

                    if (!checkList.includes(tempID)) {
                        checkList.push(tempID);
                        tempList.push(temp);
                    }
                }

                list.push(tempList);
            }

        }

        return list;
    }

    /**
     * 查询指定距离内的坐标信息
     * @param distance 距离
     * @param size 坐标尺寸
     * @param isHex 是否为六边形
     * @param isRotate 是否旋转- 一条边在上
     */
    private SelectByDistanceSquare(distance: number, size: number, isHex: boolean, isRotate: boolean) : HexCoordinates[][]
    {
        const list = [];
        const checkList: string[] = [];

        if (distance < 1)
        {
        }
        else
        {
            let start = 1;
            let tempID = '';

            list.push([this]);
            checkList.push(this.getID());

            for (let t = start; t <= distance; t++)
            {
                let tempCount = t * 2 + 1;
                let temp: HexCoordinates;

                const tempList = [];

                for (let i = 0; i < tempCount; i++)
                {
                    if (isRotate)
                    {
                        temp = HexCoordinates.FromIntPostionSquare(
                                this.hx + t,
                                0,
                                this.hz + t - i,
                                size,
                                isRotate
                        );
                        tempID = temp.getID();

                        if (!checkList.includes(tempID)) {
                            checkList.push(tempID);
                            tempList.push(temp);
                        }

                        temp = HexCoordinates.FromIntPostionSquare(
                                this.hx + t * -1,
                                0,
                                this.hz + t - i,
                                size,
                                isRotate
                        );
                        tempID = temp.getID();

                        if (!checkList.includes(tempID)) {
                            checkList.push(tempID);
                            tempList.push(temp);
                        }

                        temp = HexCoordinates.FromIntPostionSquare(
                                this.hx + t - i,
                                0,
                                this.hz + t,
                                size,
                                isRotate
                        );
                        tempID = temp.getID();

                        if (!checkList.includes(tempID)) {
                            checkList.push(tempID);
                            tempList.push(temp);
                        }

                        temp = HexCoordinates.FromIntPostionSquare(
                                this.hx + t - i,
                                0,
                                this.hz - t,
                                size,
                                isRotate
                        );
                        tempID = temp.getID();

                        if (!checkList.includes(tempID)) {
                            checkList.push(tempID);
                            tempList.push(temp);
                        }

                    } else
                    {
                        temp = HexCoordinates.FromIntPostionSquare(
                                this.hx + t,
                                0,
                                this.hz - i,
                                size,
                                isRotate
                        );
                        tempID = temp.getID();

                        if (!checkList.includes(tempID)) {
                            checkList.push(tempID);
                            tempList.push(temp);
                        }

                        temp = HexCoordinates.FromIntPostionSquare(
                                this.hx + t * -1,
                                0,
                                this.hz + i,
                                size,
                                isRotate
                        );
                        tempID = temp.getID();

                        if (!checkList.includes(tempID)) {
                            checkList.push(tempID);
                            tempList.push(temp);
                        }

                        temp = HexCoordinates.FromIntPostionSquare(
                                this.hx + t - i,
                                0,
                                this.hz + i,
                                size,
                                isRotate
                        );
                        tempID = temp.getID();

                        if (!checkList.includes(tempID)) {
                            checkList.push(tempID);
                            tempList.push(temp);
                        }

                        temp = HexCoordinates.FromIntPostionSquare(
                                this.hx - t + i,
                                0,
                                this.hz - i,
                                size,
                                isRotate
                        );
                        tempID = temp.getID();

                        if (!checkList.includes(tempID)) {
                            checkList.push(tempID);
                            tempList.push(temp);
                        }

                    }
                }

                list.push(tempList);
            }
        }

        return list;
    }

    /**
     * 从 Hex 坐标位置创建 坐标信息
     * @param iX x
     * @param iY y
     * @param iZ z
     * @param size 单元格尺寸
     * @param isHex 是否六边形
     * @param isRotate 是否旋转- 一条边在上
     */
    public static FromIntPosition(iX: number, iY: number, iZ: number, size: number, isHex: boolean, isRotate: boolean) {
        if (isHex) {
            return HexCoordinates.FromIntPostionHex(iX, iY, iZ, size, isRotate);
        } else {
            return HexCoordinates.FromIntPostionSquare(iX, iY, iZ, size, isRotate);
        }
    }

    /**
     * 计算两 Hex 坐标间距离
     * @param a Hex 坐标信息
     * @param b Hex 坐标信息
     */
    public static ComputeDistance(a: HexCoordinates, b: HexCoordinates) : number
    {
        if (!a.isHex && !a.isRotate)
        {
            if ((a.hx - b.hx) * (a.hz - b.hz) > 0)
            {
                return Math.abs(a.hx - b.hx) + (Math.abs(a.hy - b.hy) + Math.abs(a.hz - b.hz));
            } else
            {
                return Math.max(Math.abs(a.hx - b.hx), Math.max(Math.abs(a.hy - b.hy), Math.abs(a.hz - b.hz)));
            }
        }
        else
        {
            return Math.max(Math.abs(a.hx - b.hx), Math.max(Math.abs(a.hy - b.hy), Math.abs(a.hz - b.hz)));
        }
    }

    /**
     * 从 像素 坐标位置创建 坐x标信息
     * 像素坐标为单元格相对 网格层 原点坐标
     * @param x x
     * @param y y
     * @param z z
     * @param size 单元格尺寸
     * @param isHex 是否六边形
     * @param isRotate 是否旋转- 一条边在上
     */
    public static FromPixelPosition(x: number, y: number, z: number, size: number, isHex: boolean, isRotate: boolean) : HexCoordinates {
        if (isHex) {
            return HexCoordinates.FromPixelPositionHex(x, y, z, size, isHex, isRotate);
        } else {
            return HexCoordinates.FromPixelPositionSquare(x, y, z, size, isHex, isRotate);
        }
    }

    /**
     * 从 Hex 坐标位置创建 坐标信息
     * @param iX x
     * @param iY y
     * @param iZ z
     * @param size 单元格尺寸
     * @param isRotate 是否旋转- 一条边在上
     */
    private static FromIntPostionHex(iX: number, iY: number, iZ: number, size: number, isRotate: boolean) : HexCoordinates
    {
        size = size / 2.0;

        let res = new HexCoordinates(true, isRotate);

        if (!isRotate)
        {

            res.hx = iX;
            res.hy = iY;
            res.hz = iZ;
            res.fx = size * (HexCoordinates.squrt3 * iX + HexCoordinates.squrt3 / 2 * iZ);
            res.fz = size * (3.0 / 2 * iZ);

            res.col = res.hx + (res.hz - (res.hz % 1)) / 2;
            res.row = res.hz;
        }
        else
        {
            res.hx = iX;
            res.hy = iY;
            res.hz = iZ;
            res.fx = size * (3.0 / 2 * iX);
            res.fz = size * (HexCoordinates.squrt3 / 2 * iX + HexCoordinates.squrt3 * iZ);

            res.col = res.hx;
            res.row = res.hz + (res.hx - (res.hx % 1)) / 2;
        }

        return res;
    }

    /**
     * 从 Hex 坐标位置创建 坐标信息
     * @param iX x
     * @param iY y
     * @param iZ z
     * @param size 单元格尺寸
     * @param isRotate 是否旋转- 一条边在上
     */
    private static FromIntPostionSquare(iX: number, iY: number, iZ: number, size: number, isRotate: boolean) : HexCoordinates
    {
        const squrt2 = 1.414213562373;

        let res = new HexCoordinates(false, isRotate);

        if (!isRotate)
        {

            res.hx = iX;
            res.hy = iY;
            res.hz = iZ;
            res.fx = size * (squrt2 * iX + squrt2 / 2 * iZ);
            res.fz = size * (squrt2 / 2 * iZ);

            res.col = res.hx + (res.hz - (res.hz % 1)) / 2;
            res.row = res.hz;
        }
        else
        {
            res.hx = iX;
            res.hy = iY;
            res.hz = iZ;
            res.fx = size * (iX);
            res.fz = size * (iZ);

            res.col = res.hx;
            res.row = res.hz;
        }

        return res;
    }

    /**
     * 从 像素 坐标位置创建 坐x标信息
     * 像素坐标为单元格相对 网格层 原点坐标
     * @param x x
     * @param y y
     * @param z z
     * @param size 单元格尺寸
     * @param isHex 是否六边形
     * @param isRotate 是否旋转- 一条边在上
     */
    private static FromPixelPositionSquare(x: number, y: number, z: number, size: number, isHex: boolean, isRotate: boolean) : HexCoordinates {

        //size = size / 2.0f;

        let res = new HexCoordinates(true, isRotate);

        let fx, fy, fz;
        //int hx, hy, hz;

        fx = x;
        fy = y;
        fz = z;

        // 尖角向上
        if (!isRotate)
        {
            let q = (HexCoordinates.squrt2 / 2 * x - HexCoordinates.squrt2 / 2 * z) / size;
            let r = (HexCoordinates.squrt2 * z) / size;

            let fhx = q;
            let fhy = -q - r;
            let fhz = r;

            let iX = Math.round(fhx);
            let iY = 0;
            let iZ = Math.round(fhz);

            res.hx = iX;
            res.hy = iY;
            res.hz = iZ;
            res.fx = size * (HexCoordinates.squrt2 * iX + HexCoordinates.squrt2 / 2 * iZ);
            res.fz = size * (HexCoordinates.squrt2 / 2 * iZ);

            res.col = res.hx + (res.hz - (res.hz % 1)) / 2;
            res.row = res.hz;
        }
        else
        {
            let q = (x) / size;
            let r = (z) / size;

            let fhx = q;
            let fhy = -q - r;
            let fhz = r;

            let iX = Math.round(fhx);
            let iY = 0;
            let iZ = Math.round(fhz);

            //if (iX + iY + iZ != 0)
            //{
            //    float dx = Math.Abs(fhx - iX);
            //    float dy = Math.Abs(fhy - iY);
            //    float dz = Math.Abs(-fhx - fhy - iZ);

            //    if (dx > dy && dx > dz)
            //    {
            //        iX = -iY - iZ;
            //    }
            //    else
            //    {
            //        iZ = -iX - iY;
            //    }
            //}

            res.hx = iX;
            res.hy = iY;
            res.hz = iZ;
            res.fx = size * (iX);
            res.fz = size * (iZ);

            res.col = res.hx;
            res.row = res.hz;
        }

        res.fy = fy;

        return res;
    }

    /**
     * 从 像素 坐标位置创建 坐x标信息
     * 像素坐标为单元格相对 网格层 原点坐标
     * @param x x
     * @param y y
     * @param z z
     * @param size 单元格尺寸
     * @param isHex 是否六边形
     * @param isRotate 是否旋转- 一条边在上
     */
    private static FromPixelPositionHex(x: number, y: number, z: number, size: number, isHex: boolean, isRotate: boolean) : HexCoordinates {

        size = size / 2.0;

        let res = new HexCoordinates(true, isRotate);

        let fx, fy, fz;
        //int hx, hy, hz;

        fx = x;
        fy = y;
        fz = z;

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
                else
                {
                    iZ = -iX - iY;
                }
            }

            res.hx = iX;
            res.hy = iY;
            res.hz = iZ;
            res.fx = size * (HexCoordinates.squrt3 * iX + HexCoordinates.squrt3 / 2 * iZ);
            res.fz = size * (3.0 / 2 * iZ);

            res.col = res.hx + (res.hz - (res.hz % 1)) / 2;
            res.row = res.hz;
        } else
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
                else
                {
                    iZ = -iX - iY;
                }
            }

            res.hx = iX;
            res.hy = iY;
            res.hz = iZ;
            res.fx = size * (3.0 / 2 * iX);
            res.fz = size * (HexCoordinates.squrt3 / 2 * iX + HexCoordinates.squrt3 * iZ);

            res.col = res.hx;
            res.row = res.hz + (res.hx - (res.hx % 1)) / 2;
        }

        res.fy = fy;

        return res;
    }
}