import { HexGrid, IGrid } from "./hexgrid";
import { HexCoordinates } from "./hex_coordinates";
import { HexCell, BaseCell } from "./hexcell";
import { Parame } from "./parame";
import { HexMapTools } from "./hexmap_tools";

export class GridSquareR extends HexGrid {

    /**
     * @override
     */
    public searchBaseCellsByDistance(centerCell: BaseCell, distance: number) : BaseCell[][] {
        const res: BaseCell[][] = [];

        const hx = centerCell.shareOwnInfo[0];
        const hy = centerCell.shareOwnInfo[1];
        const hz = centerCell.shareOwnInfo[2];

        let temp: HexCell | undefined;

        if (distance < 1)
        {

        }
        else
        {
            let start = 1;
            let tempID = '';

            res.push([centerCell]);

            for (let t = start; t <= distance; t++)
            {
                let tempCount = t * 2 + 1;

                const tempList: BaseCell[] = [];

                for (let i = 0; i < tempCount; i++)
                {
                    temp = this.searchBaseCellWithShareInfo(
                        hx + t,
                        hy,
                        hz + t - i
                    );
                    if (temp && tempList.includes(temp)) {
                        tempList.push(temp);
                    }

                    temp = this.searchBaseCellWithShareInfo(
                        hx + t * -1,
                        hy,
                        hz + t - i
                    );
                    if (temp && tempList.includes(temp)) {
                        tempList.push(temp);
                    }

                    temp = this.searchBaseCellWithShareInfo(
                        hx + t - i,
                        hy,
                        hz + t,
                    );
                    if (temp && tempList.includes(temp)) {
                        tempList.push(temp);
                    }

                    temp = this.searchBaseCellWithShareInfo(
                        hx + t - i,
                        hy,
                        hz - t,
                    );
                    if (temp && tempList.includes(temp)) {
                        tempList.push(temp);
                    }
                }

                res.push(tempList);
            }

        }

        return res;
    }

    /**
     * @override
     */
    public rayCasterWithLocalPos(posX: number, posY: number, posZ: number) : HexCell | undefined {
        let id = "";
        HexCoordinates.FormatBaseCellFromPixelPosition(posX, posY, posZ, this.cellSize, this.isHex, this.isRotate);
        id = HexCoordinates.FormatCellName(HexCoordinates.TempBaseCell);

        if (this.enableEdge) {
            HexCoordinates.ComputeBaseCellLocalPos(HexCoordinates.TempBaseCell[0], HexCoordinates.TempBaseCell[1], HexCoordinates.TempBaseCell[2], this.cellSize, this.isHex, this.isRotate);
            const localX = posX - HexCoordinates.TempFX;
            const localY = posY - HexCoordinates.TempFY;
            const localZ = posZ - HexCoordinates.TempFZ;

            const cellType = HexCoordinates.CheckHitCellType(localX, localY, localZ, this.cellEdge, this.cellSize, this.isHex, this.isRotate);

            if (cellType > 10) {
                HexCoordinates.FormatShareOwnerListWithTypeS(HexCoordinates.TempBaseCell[0], HexCoordinates.TempBaseCell[1], HexCoordinates.TempBaseCell[2], cellType);
                id = HexCoordinates.FormatCellName(HexCoordinates.TempShareOwnerList);
            }
        }

        return this.searchWithID(id);
    }

    /**
     * @override
     */
    public rayCasterTerrainIDWithLocalPos(posX: number, posY: number, posZ: number) : number {
        let terrainID = -1;

        HexCoordinates.FormatBaseCellFromPixelPosition(posX, posY, posZ, this.cellSize, this.isHex, this.isRotate);

        if (this.enableEdge) {
            HexCoordinates.ComputeBaseCellLocalPos(HexCoordinates.TempBaseCell[0], HexCoordinates.TempBaseCell[1], HexCoordinates.TempBaseCell[2], this.cellSize, this.isHex, this.isRotate);
            const localX = posX - HexCoordinates.TempFX;
            const localY = posY - HexCoordinates.TempFY;
            const localZ = posZ - HexCoordinates.TempFZ;

            const cellType = HexCoordinates.CheckHitCellType(localX, localY, localZ, this.cellEdge, this.cellSize, this.isHex, this.isRotate);

            terrainID = this.getTerrrainIDByCellAndType(HexCoordinates.TempBaseCell[0], HexCoordinates.TempBaseCell[1], HexCoordinates.TempBaseCell[2], cellType);

        } else {
            terrainID = this.getTerrrainIDByCellAndType(HexCoordinates.TempBaseCell[0], HexCoordinates.TempBaseCell[1], HexCoordinates.TempBaseCell[2], 0);
        }

        return terrainID;
    }

    public getTerrrainIDByCellAndType(iX: number, iY: number, iZ: number, cellType: number) : number {
        let terrainID = -1;

        if (this.enableEdge) {
            let wIndex = (iX - this.minIX);
            let hIndex = (iZ - this.minIZ);

            const t = this.cellWidth * 4;

            terrainID = hIndex * this.cellWidth + wIndex;

            switch (cellType) {
                case (Parame.RSquareType): {
                    terrainID = terrainID * 4;
                    break;
                }
                case (Parame.RSquareEdgeType1): {
                    terrainID = terrainID * 4 + 1;
                    break;
                }
                case (Parame.RSquareEdgeType2): {
                    terrainID = terrainID * 4 - t + 2;
                    break;
                }
                case (Parame.RSquareEdgeType3): {
                    terrainID = terrainID * 4 - 3;
                    break;
                }
                case (Parame.RSquareEdgeType4): {
                    terrainID = terrainID * 4 + 2;
                    break;
                }
                case (Parame.RSquarePointType1): {
                    terrainID = terrainID * 4 + 3;
                    break;
                }
                case (Parame.RSquarePointType2): {
                    terrainID = terrainID * 4 - t + 3;
                    break;
                }
                case (Parame.RSquarePointType3): {
                    terrainID = terrainID * 4 - t - 1;
                    break;
                }
                case (Parame.RSquarePointType4): {
                    terrainID = terrainID * 4 - 1;
                    break;
                }
            }
        }
        else
        {
            let wIndex = (iX - this.minIX);
            let hIndex = (iZ - this.minIZ);

            terrainID = hIndex * this.cellWidth + wIndex;
        }

        return terrainID;
    }

    /**
     * @override
     */
    public getNearTerrainIDByTerrainID(terrainID: number, res: number[]) : void {

        if (this.enableEdge) {
            const t = this.cellWidth * 4;
            switch (terrainID % 4) {
                case (0): {
                    res.push(
                        terrainID - t - 1,  terrainID - t + 2,  terrainID - t + 3,
                        terrainID - 3,                          terrainID + 1,
                        terrainID - 1,      terrainID + 2,      terrainID + 3,
                    );
                    break;
                }
                case (1): {
                    res.push(
                        terrainID - t + 1,  terrainID - t + 2,  terrainID - t + 5,
                        terrainID - 1,                          terrainID + 3,
                        terrainID + 1,      terrainID + 2,      terrainID + 5,
                    );
                    break;
                }
                case (2): {
                    res.push(
                        terrainID - 5,      terrainID - 2,      terrainID - 1,
                        terrainID - 3,                          terrainID + 1,
                        terrainID + t - 5,  terrainID + t - 2,  terrainID - t - 1,
                    );
                    break;
                }
                case (3): {
                    res.push(
                        terrainID - 3,      terrainID - 2,      terrainID + 1,
                        terrainID - 1,                          terrainID + 3,
                        terrainID + t - 3,  terrainID + t - 2,  terrainID + t + 1,
                    );
                    break;
                }
            }
        } else {
            res.push(
                terrainID - this.cellWidth - 1, terrainID - this.cellWidth, terrainID - this.cellWidth + 1,
                terrainID - 1,                                              terrainID + 1,
                terrainID + this.cellWidth - 1, terrainID + this.cellWidth, terrainID + this.cellWidth + 1,
            );
        }
    }

    /**
     * @override
     */
    public getPositionByTerrainID(terrainID: number) : [number, number, number]  | undefined {
        if (!this.terrainPosMap.has(terrainID)) {
            if (this.enableEdge) {

                const tempID = Math.floor(terrainID / 4);
                const temp = terrainID % 4;

                let wIndex = tempID % this.cellWidth + this.minIX;
                let hIndex = Math.floor(tempID / this.cellWidth) + this.minIZ;

                HexCoordinates.ComputeBaseCellLocalPos(wIndex, 0, hIndex, this.cellSize, this.isHex, this.isRotate);

                switch (temp) {
                    case (1): {
                        HexCoordinates.TempFX += this.cellSize / 2;
                        break;
                    }
                    case (2): {
                        HexCoordinates.TempFZ += this.cellSize / 2;
                        break;
                    }
                    case (3): {
                        HexCoordinates.TempFX += this.cellSize / 2;
                        HexCoordinates.TempFZ += this.cellSize / 2;
                        break;
                    }
                }

                this.terrainPosMap.set(terrainID, [HexCoordinates.TempFX + this.x_pos, HexCoordinates.TempFY + this.y_pos, HexCoordinates.TempFZ + this.z_pos]);

            } else {

                let wIndex = terrainID % this.cellWidth + this.minIX;
                let hIndex = Math.floor(terrainID / this.cellWidth) + this.minIZ;

                HexCoordinates.ComputeBaseCellLocalPos(wIndex, 0, hIndex, this.cellSize, this.isHex, this.isRotate);

                this.terrainPosMap.set(terrainID, [HexCoordinates.TempFX + this.x_pos, HexCoordinates.TempFY + this.y_pos, HexCoordinates.TempFZ + this.z_pos]);
            }
        }

        return this.terrainPosMap.get(terrainID);
    }

    /**
     * @override
     */
    public computeContainTerrains(aabb: [number, number, number, number], rotate: number, res: number[]) : void {
        const aabbLoc = [aabb[0] - this.x_pos, aabb[1] - this.x_pos, aabb[2] - this.z_pos, aabb[3] - this.z_pos];
        const centerX = (aabbLoc[0] + aabbLoc[1]) / 2;
        const centerZ = (aabbLoc[2] + aabbLoc[3]) / 2;

        const minX = Math.min(aabbLoc[0], aabbLoc[1]);
        const maxX = Math.max(aabbLoc[0], aabbLoc[1]);
        const minZ = Math.min(aabbLoc[2], aabbLoc[3]);
        const maxZ = Math.max(aabbLoc[2], aabbLoc[3]);

        const sin = Math.sin(rotate);
        const cos = Math.cos(rotate);

        const w = Math.abs((aabbLoc[0] - aabbLoc[1]) / 2);
        const h = Math.abs((aabbLoc[2] - aabbLoc[3]) / 2);

        if (this.enableEdge) {
            const minBaseCellX = Math.max(Math.floor(minX / this.cellSize), this.minIX);
            const maxBaseCellX = Math.min(Math.floor(maxX / this.cellSize), this.maxIX);
            const minBaseCellZ = Math.max(Math.floor(minZ / this.cellSize), this.minIZ);
            const maxBaseCellZ = Math.min(Math.floor(maxZ / this.cellSize), this.maxIZ);

            const tempPos: [number, number] = [0, 0];
            for (let j = minBaseCellZ; j <= maxBaseCellZ; j++) {
                for (let i = minBaseCellX; i <= maxBaseCellX; i++) {
                    // RSquareType
                    const baseTerrainID = this.getTerrrainIDByCellAndType(i, 0, j, Parame.RSquareType);
                    HexCoordinates.ComputeBaseCellLocalPos(i, 0, j, this.cellSize, this.isHex, this.isRotate);
                    HexCoordinates.TempFX -= centerX;
                    HexCoordinates.TempFZ -= centerZ;

                    HexMapTools.rotatePos(cos, sin, HexCoordinates.TempFX, HexCoordinates.TempFZ, false, tempPos);
                    if (HexMapTools.checkRectContainPosition(w, h, tempPos[0], tempPos[1])) {
                        res.push(baseTerrainID);
                    }

                    // RSquareEdgeType1
                    HexMapTools.rotatePos(cos, sin, HexCoordinates.TempFX + this.cellSize / 2, HexCoordinates.TempFZ, false, tempPos);
                    if (HexMapTools.checkRectContainPosition(w, h, tempPos[0], tempPos[1])) {
                        res.push(baseTerrainID + 1);
                    }

                    // RSquareEdgeType4
                    HexMapTools.rotatePos(cos, sin, HexCoordinates.TempFX, HexCoordinates.TempFZ + this.cellSize / 2, false, tempPos);
                    if (HexMapTools.checkRectContainPosition(w, h, tempPos[0], tempPos[1])) {
                        res.push(baseTerrainID + 2);
                    }

                    // RSquarePointType1
                    HexMapTools.rotatePos(cos, sin, HexCoordinates.TempFX + this.cellSize / 2, HexCoordinates.TempFZ + this.cellSize / 2, false, tempPos);
                    if (HexMapTools.checkRectContainPosition(w, h, tempPos[0], tempPos[1])) {
                        res.push(baseTerrainID + 3);
                    }
                }
            }
        }
        else
        {
            const minBaseCellX = Math.max(Math.ceil(minX / this.cellSize), this.minIX);
            const maxBaseCellX = Math.min(Math.floor(maxX / this.cellSize), this.maxIX);
            const minBaseCellZ = Math.max(Math.ceil(minZ / this.cellSize), this.minIZ);
            const maxBaseCellZ = Math.min(Math.floor(maxZ / this.cellSize), this.maxIZ);

            if (Math.abs(rotate) < 0.01) {
                for (let j = minBaseCellZ; j <= maxBaseCellZ; j++) {
                    for (let i = minBaseCellX; i <= maxBaseCellX; i++) {
                        res.push(this.getTerrrainIDByCellAndType(i, 0, j, 0));
                    }
                }
            }
            else
            {
                const tempPos: [number, number] = [0, 0];
                for (let j = minBaseCellZ; j <= maxBaseCellZ; j++) {
                    for (let i = minBaseCellX; i <= maxBaseCellX; i++) {
                        HexCoordinates.ComputeBaseCellLocalPos(i, 0, j, this.cellSize, this.isHex, this.isRotate);
                        HexCoordinates.TempFX -= centerX;
                        HexCoordinates.TempFZ -= centerZ;
                        HexMapTools.rotatePos(cos, sin, HexCoordinates.TempFX, HexCoordinates.TempFZ, false, tempPos);
                        if (HexMapTools.checkRectContainPosition(w, h, tempPos[0], tempPos[1])) {
                            res.push(this.getTerrrainIDByCellAndType(i, 0, j, 0));
                        }
                    }
                }
            }
        }
    }
}