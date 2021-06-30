import { WebGLInstance, WebGLInstanceOpt, ShaderCfg, Scene, DataBufferCfg, Mesh, TextureInstance } from "../lib/webgl";
import { vs_multi_line_diff_speed, fs_multi_line_diff_speed } from "../lib/shader_multi_line_diff_speed";
import { vs_sin_cos, fs_sin_cos } from "../lib/shader_sin_cos";
import { vs_multi_line_cross, fs_multi_line_cross } from "../lib/shader_multi_line_cross";
import { vs_polygon, fs_polygon } from "../lib/shader_polygon";
import { vs_texture, fs_texture } from "../lib/shader_texture";
import { vs_progress, fs_progress } from "../lib/shader_progress";
import { vs_texture_grass, fs_texture_grass } from "../lib/shader_texture_grass";

export type RenderFlag = 'grass' | 'progress';

export class RenderLauncher {
    public static webgldemo: WebGLInstance;
    public static opt: any = {};
    public static loadImageSucc = (img: HTMLImageElement, fname: string) => {
        TextureInstance.loaded(img, fname, RenderLauncher.webgldemo);
    }
    public static createTextureLoad = (fname: string, engine: WebGLInstance, cb: (img: HTMLImageElement, fname: string, engine: WebGLInstance) => void) => {

        if ((<any>self).promiseLoadImage) {
            (<any>self).promiseLoadImage(fname).then((data: any) => {
                RenderLauncher.loadImageSucc(data, fname);
            });
        }
        else {
            const img = new Image();
            img.onload = () => {
                RenderLauncher.loadImageSucc(img, fname);
                // setTimeout(() => { loadImageSucc(img, data.fname); }, 2000);
            };
            img.src = fname;
        }

        // (<any>self).getATSCData().then((data: any) => {
        //     RenderLauncher.loadImageSucc(data, fname);
        // });
    }
}