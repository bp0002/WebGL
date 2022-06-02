import { Nullable } from "../base/types";
import { IRenderBuffer, IRenderContext } from "../renderer/base";

export const POINT_POSITION_FLOAT_COUNT_3D = 3;
export const POINT_POSITION_FLOAT_COUNT_2D = 2;
export const POINT_NORMAL_FLOAT_COUNT_3D = 2;
export const POINT_UV_FLOAT_COUNT = 2;
export const TRIANGLE_POINT_COUNT = 3;
export const TRIANGLE_INDICES_COUNT = 3;

export type TGeometryDataKind = string;
export type TGeometryDataType = "float32" | "uint16" | "uint8";

export interface IGeometryData {
    renderBuffer: IRenderBuffer;
    /**
     * 在一组数据中的起始点的偏移量 - 单位 byte
     * @example 一个 chunk 存储 一个 Point 的 坐标 和 uv: [x,y,z,u,v], 数值类型为 float32, 4 个 byte 表示一个数据值
     * 坐标为 3 个数据值: x,y,z  存储在第 0,1,2 的位置
     * 坐标数据的 inChunkOffset 为 0 * 4 (0 表示坐标数据值的起点为 x 在chunk的偏移量)
     * UV为 2 个数据值: u, v  存储在第 3,4 的位置
     * UV数据的 inChunkOffset 为 3 * 4 (3 表示UV数据值的起点为 u 在chunk的偏移量)
     */
    inChunkOffset: number;
    /**
     * 在一组数据中的起始点的偏移量 - 单位 byte
     * @example 一个 chunk 存储 一个 Point 的 坐标 和 uv: [x,y,z,u,v], 数值类型为 float32, 4 个 byte 表示一个数据值
     * 坐标为 3 个数据值: x,y,z  存储在第 0,1,2 的位置
     * 坐标数据的 inChunkSize 为 3 (3 表示坐标数据值的数量)
     * UV为 2 个数据值: u, v  存储在第 3,4 的位置
     * UV数据的 inChunkSize 为 2 (2 表示UV数据值的数量)
     */
    inChunkSize: number;
}

export interface IGeometry {
    getData(kind: TGeometryDataKind): IGeometryData;
}