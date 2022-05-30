import { WebGLInstance, WebGLInstanceOpt, ShaderCfg, Scene, DataBufferCfg, Mesh, TextureInstance } from "../lib/webgl";
import { vs_ribbon, fs_ribbon } from "../lib/shader_ribbon";
import { MathTools } from "../../math/math";

export type RenderFlag = 'grass' | 'progress';

export class RenderLauncher {
    public static webgldemo: WebGLInstance;
    public static opt: any = {};
    public static mesh: Mesh;
    public static scene: Scene;
    public static active(canvas: HTMLCanvasElement, args: any) {
        return RenderLauncher.simple(canvas, args);
    }
    public static puase() {

    }
    public static destroy() {
        if (RenderLauncher.webgldemo && !RenderLauncher.webgldemo.isDestroy) {
            RenderLauncher.webgldemo.destroy();
            (<any>RenderLauncher.webgldemo) = null;
        }
    }
    public static loadImageSucc = (img: HTMLImageElement, fname: string) => {
        TextureInstance.loaded(img, fname, RenderLauncher.webgldemo);
    }
    public static createTextureLoad = (fname: string, engine: WebGLInstance, cb: (img: HTMLImageElement, fname: string, engine: WebGLInstance) => void) => {

        const img = new Image();
        img.onload = () => {
            const width = MathTools.nextPowerOfTwo(img.width);
            const height = MathTools.nextPowerOfTwo(img.height);
            if (img.width !== width || img.height !== height) {
                const canvas = document.createElement('canvas');
                canvas.width = width;
                canvas.height = height;
                // document.body.appendChild(canvas);

                const ctx = <CanvasRenderingContext2D>canvas.getContext('2d');
                ctx.drawImage(img, 0, 0, width, height);
                ctx.save();

                RenderLauncher.loadImageSucc(<any>canvas, fname);
            } else {
                RenderLauncher.loadImageSucc(img, fname);
            }
            // setTimeout(() => { loadImageSucc(img, data.fname); }, 2000);
        };
        img.src = fname;
    }
    public static simple(canvas: HTMLCanvasElement, args: any) {
        const opt: WebGLInstanceOpt = <any>{};

        opt.canvas = canvas;

        RenderLauncher.webgldemo = new WebGLInstance(opt);
        TextureInstance.loadCall = RenderLauncher.createTextureLoad;

        const webgldemo = RenderLauncher.webgldemo;

        if (!RenderLauncher.webgldemo.gl) {
            return;
        }

        const shader01 = new ShaderCfg('01', vs_ribbon,  fs_ribbon);

        this.scene = new Scene('01', webgldemo);

        const dataBuffer01 = new DataBufferCfg('01');

        const mesh01 = new Mesh('mesh01', dataBuffer01, shader01);
        mesh01.rotate[0] = 1.57;
        mesh01.triangleFrame = true;

        // mesh01.texture = RenderLauncher.webgldemo.createTexture('/resources/alpha.png');
        RenderLauncher.mesh = mesh01;
        this.scene.addMesh(mesh01);

        return mesh01;
    }
}
