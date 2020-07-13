import { HexCoordinates } from "./hex_coordinates";

// export interface ICell {
//     x: number;
//     y: number;
//     z: number;
//     terrainID: number;
//     own: number[];
//     cellType: number;
//     size: number;
//     position: [number, number, number];
//     scaling: [number, number, number];
//     attrNames: string[];
//     attrValues: string[];
// }

/**
 * 基础单元格
 * * 在此基础上 扩展 单元格边的宽度,延申出 EdgeCell 和 PointCell
 */
export type BaseCell = HexCell;
/**
 * 边单元格
 * * 基础单元 扩展 单元格边的宽度 后 具有宽度的边
 */
export type EdgeCell = HexCell;
/**
 * 点单元格
 * * 基础单元 扩展 单元格边的宽度 后 具有宽度的顶点
 */
export type PointCell = HexCell;

export type ICell = [number, number, number, number, number[], number, number, [number, number, number], [number, number, number], string[], string[]];

export class HexCell {
    /**
     * 单元格唯一标识
     */
    public readonly id: string = '';
    /**
     * 单元格大小
     */
    public readonly size: number;
    /**
     * 自定义数据键列表
     */
    public readonly attrKeys: string[];
    /**
     * 自定义数据值列表
     */
    public readonly attrValues: string[];
    /**
     * 是否为六边形单元格
     */
    public readonly isHex: boolean;
    /**
     * 是否旋转 - true 表示 某一角向上,false表示某一边向上
     */
    public readonly isRotate: boolean;
    /**
     * 单元格位置
     * * 导出时选择导出绝对位置还是相对位置 
     * * （绝对位置较常用）
     */
    public readonly position: [number, number, number];
    /**
     * 单元格缩放系数
     */
    public readonly scaling: [number, number, number];
    /**
     * 单元格地形ID
     */
    public readonly terrainID: number;
    /**
     * 单元格类型
     */
    public readonly cellType: number;
    /**
     * 单元格被共享信息
     */
    public readonly shareOwnInfo: number[];
    /**
     * 单元格
     * @param data 导入数据
     * @param size 网格层记录的默认尺寸
     * @param isHex 是否为六边形
     * @param isRotate 是否进行旋转
     */
    constructor(data: Uint8Array | ICell, size: number, isHex: boolean, isRotate: boolean) {

        this.size = size;
        this.isHex = isHex;
        this.isRotate = isRotate;

        this.cellType = 0;

        if (data instanceof Uint8Array) {

            this.attrKeys = [];
            this.attrValues = [];
            this.position = [0, 0, 0];
            this.scaling = [0, 0, 0];
            this.shareOwnInfo = [];
            this.terrainID = 0;

        } else {
            // this.x = data.x;
            // this.y = data.y;
            // this.z = data.z;

            // this.position = [data.position[0], data.position[1], data.position[2]];
            // this.scaling = data.scaling;
            // this.attrKeys = data.attrNames;
            // this.attrValues = data.attrValues;
            // this.size = data.size || this.size;
            // this.terrainID = data.terrainID || 0;

            // this.id = data.own.toString();

            this.position = data[7];
            this.scaling = data[8];
            this.attrKeys = data[9];
            this.attrValues = data[10];
            this.size = data[6] || this.size;
            this.terrainID = data[3] || 0;

            this.shareOwnInfo = data[4];
            this.id = HexCoordinates.FormatCellName(this.shareOwnInfo);

            this.cellType = data[5];
        }
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
    public computeBaseCellDistance(cell2: HexCell) : number {
        if (this.cellType < 10 && cell2.cellType < 10) {
            return HexCoordinates.ComputeBaseCellDistance(this.shareOwnInfo, cell2.shareOwnInfo, this.isHex, this.isRotate);
        } else {
            return -1;
        }
    }
}