import { HexGrid } from "./hexgrid";
import { HexCell, BaseCell } from "./hexcell";

export class GridHexR extends HexGrid {

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
                const tempList: BaseCell[] = [];

                for (let i = 0; i < t; i++)
                {
                    ////
                    temp = this.searchBaseCellWithShareInfo(
                        hx + t,
                        hy + (-i),
                        hz + (-(t - i))
                    );
                    if (temp && tempList.includes(temp)) {
                        tempList.push(temp);
                    }

                    temp = this.searchBaseCellWithShareInfo(
                        hx + t * -1,
                        hy + (-(t - i)) * -1,
                        hz + (-i) * -1
                    );
                    if (temp && tempList.includes(temp)) {
                        tempList.push(temp);
                    }

                    ////
                    temp = this.searchBaseCellWithShareInfo(
                        hx + (-i),
                        hy + t,
                        hz + (-(t - i)),
                    );
                    if (temp && tempList.includes(temp)) {
                        tempList.push(temp);
                    }

                    temp = this.searchBaseCellWithShareInfo(
                        hx + (-(t - i)) * -1,
                        hy + t * -1,
                        hz + (-i) * -1,
                    );
                    if (temp && tempList.includes(temp)) {
                        tempList.push(temp);
                    }

                    ////
                    temp = this.searchBaseCellWithShareInfo(
                        hx + (-(t - i)),
                        hy + (-i),
                        hz + t,
                    );
                    if (temp && tempList.includes(temp)) {
                        tempList.push(temp);
                    }

                    temp = this.searchBaseCellWithShareInfo(
                        hx + (-i) * -1,
                        hy + (-(t - i)) * -1,
                        hz + t * -1,
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
}