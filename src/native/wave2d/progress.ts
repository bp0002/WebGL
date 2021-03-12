import { fs_wave2d, vs_wave2d } from "../lib/shader_wave2d";
import { DataBufferCfg, Mesh, Scene, ShaderCfg, TextureInstance, WebGLInstance, WebGLInstanceOpt } from "../lib/webgl";
import { RenderLauncher } from "./render_launcher";

/**
 * 本地进度条
 */

export class Wave2D {
    // 浪高
    public height: number = 0;
    public iter: number = 0;
    // 加载的当前进度
    public time: number = 0;
    public color0: number = 0;
    public color1: number = 0;
    private old: number = 0;
    private last: number = 0;
    private opacity: number = 0;
    private timeRef: number = 0;

    constructor(canvas: HTMLCanvasElement) {
        
        const opt: WebGLInstanceOpt = <any>{};

        opt.canvas = canvas;

        RenderLauncher.webgldemo = new WebGLInstance(opt);
        TextureInstance.loadCall = RenderLauncher.createTextureLoad;

        const webgldemo: WebGLInstance = RenderLauncher.webgldemo;

        if (!webgldemo.gl) {
            return;
        }

        const shader06 = new ShaderCfg('S_Wave2D', vs_wave2d, fs_wave2d);

        const scene = new Scene('06', webgldemo);
        const dataBuffer02 = new DataBufferCfg('Wave2D');
        dataBuffer02.addVertex(-1, -1, 0);
        dataBuffer02.addUV(0, 0);
        dataBuffer02.addColor(1,1,1,1);
        dataBuffer02.addVertex(1, -1, 0);
        dataBuffer02.addUV(1, 0);
        dataBuffer02.addColor(1,1,1,1);
        dataBuffer02.addVertex(1, 1, 0);
        dataBuffer02.addUV(1, 1);
        dataBuffer02.addColor(1,1,1,1);
        dataBuffer02.addVertex(-1, 1, 0);
        dataBuffer02.addUV(0, 1);
        dataBuffer02.addColor(1,1,1,1);
        dataBuffer02.addFace(0, 1, 2);
        dataBuffer02.addFace(0, 2, 3);
        dataBuffer02.update(webgldemo.gl);

        const meshBG = new Mesh('shader06', dataBuffer02, shader06);
        meshBG.translate[0] = 0.0;
        meshBG.translate[1] = 0.0;
        meshBG.scale[0] = 1;
        meshBG.scale[1] = 1;
        scene.addMesh(meshBG);

        webgldemo.renderLoop = (timestamp) => {
            webgldemo.clearColor();
            scene.viewport[0] = 0;
            scene.viewport[1] = 0;
            scene.viewport[2] = webgldemo.width;
            scene.viewport[3] = webgldemo.height;
            scene.render(true);
        };

        webgldemo.loop(0);
    }
}