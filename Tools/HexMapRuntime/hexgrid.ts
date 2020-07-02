import { HexCell, ICell } from "./hexcell";
import { HexCoordinates } from "./hex_coordinates";

interface IGrid {
    name: string;
    width: number;
    height: number;
    cellSize: number;
    position: [number, number, number];
    rotation: [number, number, number];
    isHex: boolean;
    isRotate: boolean;
    cells: ICell[];
}

export class HexGrid {
    public readonly id: string;
    public readonly map: Map<string, HexCell>;
    public readonly list: HexCell[];
    public readonly x_pos: number;
    public readonly y_pos: number;
    public readonly z_pos: number;
    public readonly width: number;
    public readonly height: number;
    public readonly cellSize: number;
    public readonly isHex: boolean;
    public readonly isRotate: boolean;

    constructor(data: Uint8Array | IGrid) {
        this.map = new Map();
        this.list = [];
        this.id = '';
        this.x_pos = 0;
        this.y_pos = 0;
        this.z_pos = 0;
        this.width = 0;
        this.height = 0;

        this.cellSize = 1;
        this.isHex = false;
        this.isRotate = false;

        if (data instanceof Uint8Array) {

        } else {
            this.id = data.name;
            this.x_pos = data.position[0];
            this.y_pos = data.position[1];
            this.z_pos = data.position[2];
            this.width = data.width;
            this.height = data.height;

            this.cellSize = data.cellSize;
            this.isHex = data.isHex;
            this.isRotate = data.isRotate;

            data.cells.forEach((info) => {
                const cell = new HexCell(info, this.cellSize, this.isHex, this.isRotate);
                this.list.push(cell);
                this.map.set(cell.id, cell);
            });
        }
    }

    /**
     * 查找指定ID的单元格
     * @param id 单元格ID
     */
    public searchWithID(id: string) {
        return this.map.get(id);
    }

    /**
     * 查找具有指定数据键的单元格
     * @param key 指定数据键
     */
    public searchWithKey(key: string) {
        const list: HexCell[] = [];

        this.map.forEach((cell) => {
            if (cell.attrKeys.includes(key)) {
                list.push(cell);
            }
        });

        return list;
    }

    /**
     * 查找具有指定 数据键值对 的单元格
     * @param key 指定数据键
     * @param value 指定数据值
     */
    public searchWithKeyValue(key: string, value: any) {
        const list: HexCell[] = [];

        this.map.forEach((cell) => {
            const index = cell.attrKeys.indexOf(key);
            if (index >= 0 && cell.attrValues[index] === value) {
                list.push(cell);
            }
        });

        return list;
    }

    /**
     * 查找与 指定目标单元格 具有 指定距离 的单元格
     * @param target 指定目标单元格
     * @param distance 指定距离
     */
    public searchWithDistance(target: HexCell, distance: number) {
        let coordinates = HexCoordinates.FromIntPosition(target.x, target.y, target.z, this.cellSize, target.isHex, target.isRotate);
        const list = coordinates.SelectByDistance(distance, this.cellSize, this.isHex, this.isRotate);

        const resList: HexCell[][] = [];

        for (let i = 0; i <= distance; i++) {
            let temp = list[i];
            const tempResList: HexCell[] = [];

            if (temp) {
                let tempCount = temp.length;

                for (let j = 0; j < tempCount; j++) {
                    const cell = this.searchWithID(temp[j].getID());
                    if (cell) {
                        tempResList.push(cell);
                    }
                }
            }

            resList.push(tempResList);
        }

        return resList;
    }

    /**
     * 获取两个单元格间的距离
     * @param cell0 目标单元格
     * @param cell1 目标单元格
     */
    public getDistance(cell0: HexCell, cell1: HexCell) {
        return cell0.computeDistance(cell1);
    }
}