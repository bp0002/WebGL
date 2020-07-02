import { HexCoordinates } from "./hex_coordinates";

export interface ICell {
    x: number;
    y: number;
    z: number;
    position: [number, number, number];
    scaling: [number, number, number];
    attrNames: string[];
    attrValues: string[];
    size: number;
}

export class HexCell {
    public readonly id: string;
    public row: number = 0;
    public col: number = 0;
    public readonly x: number;
    public readonly y: number;
    public readonly z: number;
    public readonly size: number;
    public readonly attrKeys: string[];
    public readonly attrValues: string[];
    public readonly isHex: boolean;
    public readonly isRotate: boolean;
    public readonly position: [number, number, number];
    public readonly scaling: [number, number, number];

    constructor(data: Uint8Array | ICell, size: number, isHex: boolean, isRotate: boolean) {
        this.x = 0;
        this.y = 0;
        this.z = 0;

        //

        this.size = size;
        this.isHex = isHex;
        this.isRotate = isRotate;

        if (data instanceof Uint8Array) {

            this.attrKeys = [];
            this.attrValues = [];
            this.position = [0, 0, 0];
            this.scaling = [0, 0, 0];

        } else {
            this.x = data.x;
            this.y = data.y;
            this.z = data.z;

            this.position = [data.position[0], data.position[1], data.position[2]];
            this.scaling = data.scaling;
            this.attrKeys = data.attrNames;
            this.attrValues = data.attrValues;
            this.size = data.size || this.size;
        }

        this.id = `${this.x}_${this.y}_${this.z}`;
    }

    /**
     * 更改数据
     * @param key 键
     * @param value 值
     */
    public modifyData(key: string, value: any) {
        let flag = false;

        const index = this.attrKeys.indexOf(key);
        if (index >= 0) {
            this.attrValues[index] = value;
            flag = true;
        }

        return flag;
    }

    /**
     * 读取数据值
     * @param key 目标键
     */
    public readData(key: string) {
        let value: any ;
        const index = this.attrKeys.indexOf(key);
        if (index >= 0) {
            value = this.attrValues[index];
        }

        return value;
    }

    /**
     * 计算与目标单元的距离
     * @param cell2 目标单元
     */
    public computeDistance(cell2: HexCell) {
        let a = HexCell.ToHexCoordinates(this);
        let b = HexCell.ToHexCoordinates(cell2);

        return HexCoordinates.ComputeDistance(a, b);
    }

    /**
     * 获取对应 Hex 坐标信息
     * @param cell 目标单元
     */
    public static ToHexCoordinates(cell: HexCell) {
        return HexCoordinates.FromIntPosition(cell.x, cell.y, cell.z, cell.size, cell.isHex, cell.isRotate);
    }
}