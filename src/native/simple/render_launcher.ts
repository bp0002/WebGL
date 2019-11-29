import { WebGLInstance, WebGLInstanceOpt, ShaderCfg, Scene, DataBufferCfg, Mesh, TextureInstance } from "../lib/webgl";
import { vs_multi_line_diff_speed, fs_multi_line_diff_speed } from "../lib/shader_multi_line_diff_speed";
import { vs_sin_cos, fs_sin_cos } from "../lib/shader_sin_cos";
import { vs_multi_line_cross, fs_multi_line_cross } from "../lib/shader_multi_line_cross";
import { vs_polygon, fs_polygon } from "../lib/shader_polygon";
import { vs_texture, fs_texture } from "../lib/shader_texture";
import { vs_progress, fs_progress } from "../lib/shader_progress";
import { vs_texture_grass, fs_texture_grass } from "../lib/shader_texture_grass";
import { MathTools } from "../../math/math";
import { GeometryTools } from "../../math/geometry";
import { vs_simple, fs_simple } from "../lib/shader_simple";

export type RenderFlag = 'grass' | 'progress';

export class RenderLauncher {
    public static webgldemo: WebGLInstance;
    public static opt: any = {};
    public static mesh: Mesh;
    public static active(canvas: HTMLCanvasElement, args: any) {
        RenderLauncher.simple(canvas, args);
        return RenderLauncher.webgldemo;
    }
    public static puase() {

    }
    public static destroy() {
        if (RenderLauncher.webgldemo && !RenderLauncher.webgldemo.isDestroy) {
            RenderLauncher.webgldemo.destroy();
            (<any>RenderLauncher.webgldemo) = null;

            if (RenderLauncher.opt.meshProgress) {
                RenderLauncher.opt.meshProgress = undefined;
            }
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

        const shader01 = new ShaderCfg('01', vs_simple,  fs_simple);

        const scene01 = new Scene('01', webgldemo);

        const dataBuffer01 = new DataBufferCfg('01');
        // const polygon8 = GeometryTools.polygon(8, true);

        // if (polygon8.vertexs) {
        //     polygon8.vertexs.forEach((vertex) => {
        //         dataBuffer01.addVertex(vertex[0], vertex[1], 0);
        //         dataBuffer01.addColor(1, 0, 0, 1);
        //     });
        // }

        // polygon8.faces.forEach((face) => {
        //     dataBuffer01.addFace(face[0], face[1], face[2]);
        // });
        // dataBuffer01.update(<WebGLRenderingContext>webgldemo.gl);

        const sphere = GeometryTools.antiPrism(5);

        if (sphere.vertexs3D) {
            sphere.vertexs3D.forEach((vertex) => {
                dataBuffer01.addVertex(vertex[0], vertex[1], vertex[2]);
                dataBuffer01.addColor(Math.abs(vertex[2]), 0, 0, 1);
            });
        }

        sphere.faces.forEach((face) => {
            dataBuffer01.addFace(face[0], face[1], face[2]);
        });
        dataBuffer01.update(<WebGLRenderingContext>webgldemo.gl);

        const mesh01 = new Mesh('mesh01', dataBuffer01, shader01);
        mesh01.translate[0] = 0.0;
        mesh01.translate[1] = 0.0;
        mesh01.scale[0] = 1.0;
        mesh01.scale[1] = 1.0;
        mesh01.rotate[0] = 1.57;
        mesh01.wireFrame = true;
        mesh01.pointFrame = true;
        // mesh01.triangleFrame = true;
        // mesh01.texture = RenderLauncher.webgldemo.createTexture('/resources/alpha.png');
        scene01.addMesh(mesh01);

        RenderLauncher.mesh = mesh01;

        webgldemo.renderLoop = (timestamp: number) => {
            webgldemo.clearColor();

            scene01.viewport[0] = 0;
            scene01.viewport[1] = 0;
            scene01.viewport[2] = webgldemo.width;
            scene01.viewport[3] = webgldemo.height;
            scene01.render(true);

        };

        webgldemo.loop(0);

        // setInterval(() => {
        //     dataBuffer01.clearVertex();
        //     dataBuffer01.clearColor();
        //     dataBuffer01.clearFace();

        //     const sphere = GeometryTools.pyramid(5);

        //     if (sphere.vertexs3D) {
        //         sphere.vertexs3D.forEach((vertex) => {
        //             dataBuffer01.addVertex(vertex[0], vertex[2], vertex[1]);
        //             dataBuffer01.addColor(Math.abs(vertex[2]), 0, 0, 1);
        //         });
        //     }

        //     sphere.faces.forEach((face) => {
        //         dataBuffer01.addFace(face[0], face[1], face[2]);
        //     });

        //     dataBuffer01.update(<WebGLRenderingContext>webgldemo.gl);
        // }, 2000);

        // let delta = 0;
        // setInterval(() => {
        //     dataBuffer01.clearVertex();
        //     dataBuffer01.clearColor();
        //     dataBuffer01.clearFace();

        //     // const sphere = GeometryTools.sphere(100, 100);
        //     const sphere = GeometryTools.ribbon2(100, delta += 0.01);

        //     if (sphere) {
        //         if (sphere.vertexs3D) {
        //             sphere.vertexs3D.forEach((vertex, index, arr) => {
        //                 dataBuffer01.addVertex(vertex[0], vertex[1], vertex[2]);
        //                 dataBuffer01.addColor(0.8, 0, 0, 1);
        //             });
        //         }

        //         sphere.faces.forEach((face) => {
        //             dataBuffer01.addFace(face[0], face[1], face[2]);
        //         });
        //     }

        //     dataBuffer01.update(<WebGLRenderingContext>webgldemo.gl);
        // }, 20);

        return mesh01;
    }
    public static updateProgress(num: number) {
        if (RenderLauncher.opt.meshProgress) {
            RenderLauncher.opt.meshProgress.ufloat = num;
        }
    }
}
