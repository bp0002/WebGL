import { HexGrid, IGrid } from "./hexgrid";
import { GridSquareR } from "./grid_square_r";
import { GridSquare } from "./grid_square";
import { GridHex } from "./grid_hex";
import { GridHexR } from "./grid_hex_r";

export namespace HexMap {
    export const GridMap: Map<string, HexGrid> = new Map();
    export function Format(gridData: Uint8Array | any, terrainData?: Uint8Array) {
        const grid = Create(gridData, terrainData);

        if (GridMap.get(grid.id)) {
            console.warn(`重复的 Grid 名称 ${grid.id}`);
        } else {
            GridMap.set(grid.id, grid);
        }
    }
    /**
     * 分类创建 Grid
     * @param data grid 数据
     * @param terrainData 地形数据
     */
    function Create(data: IGrid, terrainData?: Uint8Array) : HexGrid {
        if (data.isHex) {
            if (data.isRotate) {
                return new GridHexR(data, terrainData);
            } else {
                return new GridHex(data, terrainData);
            }
        } else {
            if (data.isRotate) {
                return new GridSquareR(data, terrainData);
            } else {
                return new GridSquare(data, terrainData);
            }
        }
    }
}