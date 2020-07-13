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
    *       |   2    |o |   3    |
    *       +------------------------+
    *       |        |  |        |
    *       |   a    |1 |   b    |
    *       |        |  |        |
    *       |        |  |        |
    *       +--------+--+--------+---+
    *      - BaseCell: a,b,c,d
    *      - EdgeCell: 1,2,3,4
    *      - PointCell: o
    *      - 1 的 ShareOwnerBaseCell [ a, b ]
    *      - 2 的 ShareOwnerBaseCell [ a, c ]
    *      - o 的 ShareOwnerBaseCell [ a, b, c, d ]
    *      - ShareOwnerBaseCell 排列顺序规则: x 由小到大, 再 z 由小到大
    ```
* HexCell 归属唯一 BaseCell
    ```
    * (HexCell)单元格 所属 (BaseCell)单元格列表
    * * 列表数据 [ BaseCell.iX, BaseCell.iY, BaseCell.iZ]
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
    *      + 0,1,2,3 都归属于 0
    ```

## 具体数据实现

### 网格地图

* 网格层列表

### 网格层

* 名称

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

### 单元格

* ID

* 网格坐标

* 场景坐标

* 自定义数据键表

* 自定义数据值表

* 地形ID

* 单元格类型

* 单元格 被共享的单元格信息 列表 - 可唯一确定HexCell

## 功能

### 单层网格内功能

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

### 单个单元格功能

* 读取指定 自定义数据 键 对应的 值

* 修改指定 自定义数据 键 对应的 值

* (BaseCell) 计算与另一单元格的距离场距离

### 调试渲染