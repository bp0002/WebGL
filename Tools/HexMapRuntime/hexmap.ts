import { HexGrid } from "./hexgrid";

export namespace HexMap {
    export const GridMap: Map<string, HexGrid> = new Map();
    export function Format(data: ArrayBuffer | any[]) {
        if (data instanceof ArrayBuffer) {

        } else {
            data.forEach((ele) => {
                if (ele instanceof Array) {

                } else {
                    if (ele.grids) {
                        (<any[]>ele.grids).forEach((v) => {
                            const grid = new HexGrid(v);
                            if (GridMap.get(grid.id)) {
                                console.warn(`重复的 Grid 名称 ${grid.id}`);
                            } else {
                                GridMap.set(grid.id, grid);
                            }
                        });
                    }
                }
            });
        }
    }
}