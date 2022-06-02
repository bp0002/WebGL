import { IGeometry } from "../../geometry/base";
import { IMaterial } from "../../material/base";

export interface IMesh {
    geometry: IGeometry;
    material: IMaterial;
    render(passId: number): void;
}