import { HexCell, ICell, BaseCell } from "./hexcell";
import { HexCoordinates } from "./hex_coordinates";

export interface IGrid {
    name: string;
    cellSize: number;
    position: [number, number, number];
    rotation: [number, number, number];
    isHex: boolean;
    isRotate: boolean;
    enableEdge: boolean;
    cellEdge: number;
    cells: ICell[];

    minX: number;
    minZ: number;
    maxX: number;
    maxZ: number;
    terrainData?: number[];
}

export class HexGrid {
    /**
     * 网格唯一标识
     */
    public readonly id: string;
    /**
     * 单元格缓存堆
     * @key 单元格唯一标识
     * @value 单元格
     */
    public readonly map: Map<string, HexCell> = new Map();
    public readonly list: HexCell[];
    /**
     * 网格绝对位置-x
     */
    public readonly x_pos: number;
    /**
     * 网格绝对位置-y
     */
    public readonly y_pos: number;
    /**
     * 网格绝对位置-z
     */
    public readonly z_pos: number;
    /***
     * 网格内的单元格默认尺寸
     */
    public readonly cellSize: number;
    /**
     * 是否为六边形
     */
    public readonly isHex: boolean;
    /**
     * 是否进行旋转
     */
    public readonly isRotate: boolean;
    /**
     * 是否激活了单元格边的宽度
     */
    public readonly enableEdge: boolean = false;
    /**
     * 单元格边的宽度
     */
    public readonly cellEdge: number = 0;
    /**
     * 网格内单元格网格坐标AABB
     */
    public readonly minIX: number = Number.MAX_SAFE_INTEGER;
    /**
     * 网格内单元格网格坐标AABB
     */
    public readonly minIZ: number = Number.MAX_SAFE_INTEGER;
    /**
     * 网格内单元格网格坐标AABB
     */
    public readonly maxIX: number = Number.MIN_SAFE_INTEGER;
    /**
     * 网格内单元格网格坐标AABB
     */
    public readonly maxIZ: number = Number.MIN_SAFE_INTEGER;

    /**
     * 网格内单元格网格坐标AABB宽度
     */
    public readonly cellWidth: number = 0;
    /**
     * 网格内单元格网格坐标AABB高度
     */
    public readonly cellHeight: number = 0;

    /**
     * 网格内地形数据
     */
    public readonly terrain: Uint8Array | undefined;
    /**
     * 网格内地形ID对应的网格数据堆
     */
    public readonly terrainMap: Map<number, HexCell> = new Map();
    /**
     * 网格内地形ID对应的地形坐标 - 绝对坐标
     */
    public readonly terrainPosMap: Map<number, [number, number, number]> = new Map();
    /**
     * 网格
     * @param data 导入网格数据
     * @param terrainData 导入地形数据
     */
    constructor(data: IGrid, terrainData?: Uint8Array) {
        this.list = [];
        this.id = '';
        this.x_pos = 0;
        this.y_pos = 0;
        this.z_pos = 0;

        this.cellSize = 1;
        this.isHex = false;
        this.isRotate = false;

        this.id = data.name;
        this.x_pos = data.position[0];
        this.y_pos = data.position[1];
        this.z_pos = data.position[2];

        this.cellSize = data.cellSize;
        this.isHex = data.isHex;
        this.isRotate = data.isRotate;

        this.enableEdge = !!data.enableEdge;
        this.cellEdge = data.cellEdge;

        this.minIX = data.minX;
        this.minIZ = data.minZ;
        this.maxIX = data.maxX;
        this.maxIZ = data.maxZ;

        this.cellWidth = data.maxX - data.minX + 1;
        this.cellHeight = data.maxZ - data.minZ + 1;

        data.cells.forEach((info) => {
            const cell = new HexCell(info, this.cellSize, this.isHex, this.isRotate);
            this.list.push(cell);
            this.map.set(cell.id, cell);
            this.terrainMap.set(cell.terrainID, cell);
        });

        if (data.terrainData) {
            this.terrain = new Uint8Array(data.terrainData);
        }

        if (terrainData) {
            this.terrain = terrainData;
        }
    }

    /**
     * 查找指定ID的单元格
     * @param id 单元格ID
     * @default
     */
    public searchWithID(id: string) : HexCell | undefined {
        return this.map.get(id);
    }

    /**
     * 依据 共享信息 查询 单元格
     * @param id 单元格ID
     * @default
     */
    public searchWithShareInfo(shareInfo: number[]) : HexCell | undefined {
        const id = HexCoordinates.FormatCellName(shareInfo);
        return this.map.get(id);
    }

    /**
     * 依据 共享信息 查询 BaseCell 单元格
     * @param id 单元格ID
     * @default
     */
    public searchBaseCellWithShareInfo(iX: number, iY: number, iZ: number) : HexCell | undefined {
        return this.map.get(`[${iX},${iY},${iZ}]`);
    }

    /**
     * 查找具有指定数据键的单元格
     * @param key 指定数据键
     * @default
     */
    public searchWithKey(key: string) : HexCell[] {
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
     * @default
     */
    public searchWithKeyValue(key: string, value: any) : HexCell[] {
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
     * 通过世界坐标查询目标单元格
     * @param posX 浮点
     * @param posY 浮点
     * @param posZ 浮点
     * @default
     */
    public rayCasterWithWorldPos(posX: number, posY: number, posZ: number) : HexCell | undefined {
        posX = posX - this.x_pos;
        posY = posY - this.y_pos;
        posZ = posZ - this.z_pos;

        return this.rayCasterWithLocalPos(posX, posY, posZ);
    }

    /**
     * 通过相对Grid坐标查询目标单元格
     * @param posX 浮点
     * @param posY 浮点
     * @param posZ 浮点
     * @default
     */
    public rayCasterWithLocalPos(posX: number, posY: number, posZ: number) : HexCell | undefined {
        return undefined; 
    }

    /**
     * 通过世界坐标查询目标单元格 TerrainID
     * @param posX 浮点
     * @param posY 浮点
     * @param posZ 浮点
     * @default
     */
    public rayCasterTerrainIDWithWorldPos(posX: number, posY: number, posZ: number) : number {
        posX = posX - this.x_pos;
        posY = posY - this.y_pos;
        posZ = posZ - this.z_pos;

        return this.rayCasterTerrainIDWithLocalPos(posX, posY, posZ);
    }

    /**
     * 通过相对Grid坐标查询目标单元格 TerrainID
     * @param posX 浮点
     * @param posY 浮点
     * @param posZ 浮点
     * @default
     */
    public rayCasterTerrainIDWithLocalPos(posX: number, posY: number, posZ: number) : number {
        return -1;
    }

    /**
     * 获取两个（BaseCell）单元格间的距离
     * @param cell0 目标单元格
     * @param cell1 目标单元格
     * @default
     */
    public computeBaseCellsDistance(cell0: HexCell, cell1: HexCell) : number {
        return cell0.computeBaseCellDistance(cell1);
    }

    /**
     * 获取指定地块地形信息
     * @param terrainID 地块地形ID
     * @default
     */
    public getTerrainInfo(terrainID: number) : number {
        if (this.terrain)  {
            return this.terrain[terrainID];
        } else {
            return 0;
        }
    }

    /**
     * 修改指定地块地形信息
     * @param terrainID 地块地形ID
     * @param value 地形数据
     * @default
     */
    public modifyTerrainInfo(terrainID: number, value: number) : number {
        if (this.terrain && value >= 0 && value < 256)  {
            return this.terrain[terrainID] = value;
        } else {
            return 0;
        }
    }

    /**
     * 获取指定地形ID的地块的邻居地形ID
     * @param terrainID 地块地形ID
     * @default
     */
    public getNearTerrainIDByTerrainID(terrainID: number, res: number[]) : void {
        //
    }

    /**
     * 获取 地形ID 对应坐标
     * @param terrainID 地形ID
     * @returns [number, number, number]
     * * 当前仅 适用于 isHex:false, isRotate:false
     * @default
     */
    public getPositionByTerrainID(terrainID: number) : [number, number, number] | undefined {
        return undefined;
    }

    /**
     * 获取 地形 对应 Cell
     * @param terrainID 地形ID
     * @default
     */
    public getCellByTerrainID(terrainID: number) : HexCell | undefined {
        return this.terrainMap.get(terrainID);
    }

    /**
     * 通过 Cell 获取地形ID
     * @param iX Cell坐标
     * @param iY Cell坐标
     * @param iZ Cell坐标
     * @param cellType Cell类型
     */
    public getTerrrainIDByCellAndType(iX: number, iY: number, iZ: number, cellType: number) : number {
        return -1;
    }

    /**
     * 计算 OBB 包含的地形ID列表
     * @param aabb aabb
     * @param rotate 旋转 - (顺时针的弧度值)
     * @param res 结果数组
     * @default
     */
    public computeContainTerrains(aabb: [number, number, number, number], rotate: number, res: number[]) : void {
        //
    }

    /**
     * 查找与目标 BaseCell 指定距离内所有 BaseCell
     * @param centerCell 目标 BaseCell
     * @param distance 距离场距离
     */
    public searchBaseCellsByDistance(centerCell: BaseCell, distance: number) : BaseCell[][] {
        const res: BaseCell[][] = [];

        return res;
    }
}