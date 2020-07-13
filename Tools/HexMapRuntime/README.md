# HexMapRuntime

## 基础数据描述

* 网格
    + 描述一定区域内的有限多个单元格，提供各种查询和修改单元格数据的接口

* 单元格 HexCell
    + 分类
        - BaseCell - 基础单元格
        - EdgeCell - 基础单元格拓展具有宽度的边
        - PointCell - 基础单元格拓展具有宽度的顶点
* HexCell 被多个 BaseCell 共享
    + HexCell 可以由 所属共享 (BaseCell)单元格列表 唯一确定
    ```
      (HexCell)单元格 所属共享 (BaseCell)单元格列表
      * 列表数据 [ BaseCell0.iX, BaseCell0.iY, BaseCell0.iZ, ...]
      * 没有宽度边的单元格，所属共享 即自己
      * 有宽度边的单元格 - 有 块/边/点 三种单元格
       + BaseCell 所属共享 即自己
       + EdgeCell 所属有 2 个 BaseCell
       + PointCell 所属有 4 个 BaseCell
            +        +  +        +
            |        |  |        |
            +-----------------------+
            |        |  |        |
            |   c    |4 |   d    |
            |        |  |        |
            |        |  |        |
            +-----------------------+
            |   2    |o |   3    |
            +------------------------+
            |        |  |        |
            |   a    |1 |   b    |
            |        |  |        |
            |        |  |        |
            +--------+--+--------+---+
           - BaseCell: a,b,c,d
           - EdgeCell: 1,2,3,4
           - PointCell: o
           - 1 的 ShareOwnerBaseCell [ a, b ]
           - 2 的 ShareOwnerBaseCell [ a, c ]
           - o 的 ShareOwnerBaseCell [ a, b, c, d ]
           - ShareOwnerBaseCell 排列顺序规则: x 由小到大, 再 z 由小到大
    ```
* HexCell 归属唯一 BaseCell
    ```
      (HexCell)单元格 所属 (BaseCell)单元格列表
      * 列表数据 [ BaseCell.iX, BaseCell.iY, BaseCell.iZ]
      * 没有宽度边的单元格，所属即自己
      * 有宽度边的单元格 - 有 块/边/点 三种单元格
       +-------+--+
       |   2   |3 |
       +----------+
       |       |  |
       |   0   |1 |
       |       |  |
       |       |  |
       +-------+--+
           + 0 - BaseCell
           + 1,2 - EdgeCell
           + 3 - PointCell
           + 0,1,2,3 都归属于 0
    ```

## 具体数据实现

### 网格地图

* 网格层列表

### 网格层

#### 数据
* 名称 - 唯一标识

* 单元格列表

* 网格宽高

* 网格原点场景坐标

* 单元格尺寸

* 单元格类型

* 单元格旋转类型

* 是否拓展单元格边的宽度

* 激活边宽度后,边的宽度值

* 有效 BaseCell 单元格 AABB

* 地形数据

* 以 HexCell 名称 保存的 HexCell 列表

* 以 HexCell 地形ID 保存的 HexCell 列表

* 以 HexCell 地形ID 保存的 地形ID 对应坐标 列表

* 依据 单元格的共享信息 查询 单元格

#### 接口
* 通过 ID 查询 HexCell

* 通过 属性键 查询 HexCell

* 通过 属性键值对 查询 HexCell

* 由世界坐标/相对坐标 查询命中的 BaseCell

* 由世界坐标/相对坐标 查询命中的 地形ID

* 由地形ID 查询对应 地形数据

* 由地形ID 查询 相邻 地形ID列表

* 由地形ID 查询对应 HexCell

* 由 AABB 和 旋转弧度，计算包含的 地形ID列表

* 计算两个单元格（BaseCell）间的距离场距离

* 查找与目标 BaseCell 指定距离内所有 BaseCell - 结果为按距离从 0 到 n 排列的数组

```
export declare class HexGrid {
    /**
     * 网格唯一标识
     */
    readonly id: string;
    /**
     * 单元格缓存堆
     * @key 单元格唯一标识
     * @value 单元格
     */
    readonly map: Map<string, HexCell>;
    readonly list: HexCell[];
    /**
     * 网格绝对位置-x
     */
    readonly x_pos: number;
    /**
     * 网格绝对位置-y
     */
    readonly y_pos: number;
    /**
     * 网格绝对位置-z
     */
    readonly z_pos: number;
    /***
     * 网格内的单元格默认尺寸
     */
    readonly cellSize: number;
    /**
     * 是否为六边形
     */
    readonly isHex: boolean;
    /**
     * 是否进行旋转
     */
    readonly isRotate: boolean;
    /**
     * 是否激活了单元格边的宽度
     */
    readonly enableEdge: boolean;
    /**
     * 单元格边的宽度
     */
    readonly cellEdge: number;
    /**
     * 网格内单元格网格坐标AABB
     */
    readonly minIX: number;
    /**
     * 网格内单元格网格坐标AABB
     */
    readonly minIZ: number;
    /**
     * 网格内单元格网格坐标AABB
     */
    readonly maxIX: number;
    /**
     * 网格内单元格网格坐标AABB
     */
    readonly maxIZ: number;
    /**
     * 网格内单元格网格坐标AABB宽度
     */
    readonly cellWidth: number;
    /**
     * 网格内单元格网格坐标AABB高度
     */
    readonly cellHeight: number;
    /**
     * 网格内地形数据
     */
    readonly terrain: Uint8Array | undefined;
    /**
     * 网格内地形ID对应的网格数据堆
     */
    readonly terrainMap: Map<number, HexCell>;
    /**
     * 网格内地形ID对应的地形坐标 - 绝对坐标
     */
    readonly terrainPosMap: Map<number, [number, number, number]>;
    /**
     * 网格
     * @param data 导入网格数据
     * @param terrainData 导入地形数据
     */
    constructor(data: IGrid, terrainData?: Uint8Array);
    /**
     * 查找指定ID的单元格
     * @param id 单元格ID
     * @default
     */
    searchWithID(id: string): HexCell | undefined;
    /**
     * 依据 共享信息 查询 单元格
     * @param id 单元格ID
     * @default
     */
    searchWithShareInfo(shareInfo: number[]): HexCell | undefined;
    /**
     * 依据 共享信息 查询 BaseCell 单元格
     * @param id 单元格ID
     * @default
     */
    searchBaseCellWithShareInfo(iX: number, iY: number, iZ: number): HexCell | undefined;
    /**
     * 查找具有指定数据键的单元格
     * @param key 指定数据键
     * @default
     */
    searchWithKey(key: string): HexCell[];
    /**
     * 查找具有指定 数据键值对 的单元格
     * @param key 指定数据键
     * @param value 指定数据值
     * @default
     */
    searchWithKeyValue(key: string, value: any): HexCell[];
    /**
     * 通过世界坐标查询目标单元格
     * @param posX 浮点
     * @param posY 浮点
     * @param posZ 浮点
     * @default
     */
    rayCasterWithWorldPos(posX: number, posY: number, posZ: number): HexCell | undefined;
    /**
     * 通过相对Grid坐标查询目标单元格
     * @param posX 浮点
     * @param posY 浮点
     * @param posZ 浮点
     * @default
     */
    rayCasterWithLocalPos(posX: number, posY: number, posZ: number): HexCell | undefined;
    /**
     * 通过世界坐标查询目标单元格 TerrainID
     * @param posX 浮点
     * @param posY 浮点
     * @param posZ 浮点
     * @default
     */
    rayCasterTerrainIDWithWorldPos(posX: number, posY: number, posZ: number): number;
    /**
     * 通过相对Grid坐标查询目标单元格 TerrainID
     * @param posX 浮点
     * @param posY 浮点
     * @param posZ 浮点
     * @default
     */
    rayCasterTerrainIDWithLocalPos(posX: number, posY: number, posZ: number): number;
    /**
     * 获取两个（BaseCell）单元格间的距离
     * @param cell0 目标单元格
     * @param cell1 目标单元格
     * @default
     */
    computeBaseCellsDistance(cell0: HexCell, cell1: HexCell): number;
    /**
     * 获取指定地块地形信息
     * @param terrainID 地块地形ID
     * @default
     */
    getTerrainInfo(terrainID: number): number;
    /**
     * 获取指定地形ID的地块的邻居地形ID
     * @param terrainID 地块地形ID
     * @default
     */
    getNearTerrainIDByTerrainID(terrainID: number, res: number[]): void;
    /**
     * 获取 地形ID 对应坐标
     * @param terrainID 地形ID
     * @returns [number, number, number]
     * * 当前仅 适用于 isHex:false, isRotate:false
     * @default
     */
    getPositionByTerrainID(terrainID: number): [number, number, number] | undefined;
    /**
     * 获取 地形 对应 Cell
     * @param terrainID 地形ID
     * @default
     */
    getCellByTerrainID(terrainID: number): HexCell | undefined;
    /**
     * 通过 Cell 获取地形ID
     * @param iX Cell坐标
     * @param iY Cell坐标
     * @param iZ Cell坐标
     * @param cellType Cell类型
     */
    getTerrrainIDByCellAndType(iX: number, iY: number, iZ: number, cellType: number): number;
    /**
     * 计算 OBB 包含的地形ID列表
     * @param aabb aabb
     * @param rotate 旋转 - (顺时针的弧度值)
     * @param res 结果数组
     * @default
     */
    computeContainTerrains(aabb: [number, number, number, number], rotate: number, res: number[]): void;
    /**
     * 查找与目标 BaseCell 指定距离内所有 BaseCell
     * @param centerCell 目标 BaseCell
     * @param distance 距离场距离
     */
    searchBaseCellsByDistance(centerCell: BaseCell, distance: number): BaseCell[][];
}
```

### 单元格

#### 数据
* ID - 唯一标识

* 网格坐标

* 场景坐标

* 自定义数据键表

* 自定义数据值表

* 地形ID

* 单元格类型

* 单元格 被共享的单元格信息 列表 - 可唯一确定HexCell

#### 接口
* 读取指定 自定义数据 键 对应的 值

* 修改指定 自定义数据 键 对应的 值

* (BaseCell) 计算与另一单元格的距离场距离

```
export declare class HexCell {
    /**
     * 单元格唯一标识
     */
    readonly id: string;
    /**
     * 单元格大小
     */
    readonly size: number;
    /**
     * 自定义数据键列表
     */
    readonly attrKeys: string[];
    /**
     * 自定义数据值列表
     */
    readonly attrValues: string[];
    /**
     * 是否为六边形单元格
     */
    readonly isHex: boolean;
    /**
     * 是否旋转 - true 表示 某一角向上,false表示某一边向上
     */
    readonly isRotate: boolean;
    /**
     * 单元格位置
     * * 导出时选择导出绝对位置还是相对位置
     * * （绝对位置较常用）
     */
    readonly position: [number, number, number];
    /**
     * 单元格缩放系数
     */
    readonly scaling: [number, number, number];
    /**
     * 单元格地形ID
     */
    readonly terrainID: number;
    /**
     * 单元格类型
     */
    readonly cellType: number;
    /**
     * 单元格被共享信息
     */
    readonly shareOwnInfo: number[];
    /**
     * 单元格
     * @param data 导入数据
     * @param size 网格层记录的默认尺寸
     * @param isHex 是否为六边形
     * @param isRotate 是否进行旋转
     */
    constructor(data: Uint8Array | ICell, size: number, isHex: boolean, isRotate: boolean);
    /**
     * 更改数据
     * @param key 键
     * @param value 值
     */
    modifyData(key: string, value: any): boolean;
    /**
     * 读取数据值
     * @param key 目标键
     */
    readData(key: string): any;
    /**
     * 计算与目标单元的距离
     * @param cell2 目标单元
     */
    computeBaseCellDistance(cell2: HexCell): number;
}
```

### 调试渲染