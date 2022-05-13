import { Nullable } from "../base/types";
import { Restorable } from "./base";

export class ManagedWebGLRenderingContext {
    public static readonly contentModes = ["webgl", "experimental-webgl", "webgl2", "webkit-3d", "moz-webgl"];

    private static pool: Map<HTMLCanvasElement, ManagedWebGLRenderingContext> = new Map();
    public static get(canvas: HTMLCanvasElement, contextConfig: any = { alpha: "true" }) {
        let result = ManagedWebGLRenderingContext.pool.get(canvas);
        if (!result) {
            result = new ManagedWebGLRenderingContext(canvas, contextConfig);
            ManagedWebGLRenderingContext.pool.set(canvas, result);
        }

        return result;
    }

    public canvas: HTMLCanvasElement;
    public gl: WebGLRenderingContext;
    private restorables = new Array<Restorable>();

    constructor(canvasOrContext: HTMLCanvasElement | WebGLRenderingContext, contextConfig: any = { alpha: "true" }) {
        // 为了兼容 Native端，Native端的canvasOrContext 不是HtmlCanvasElement
        // 注：微信小游戏平台，下面判断返回false：canvasOrContext["getContext"] instanceof Function
        if ((<HTMLCanvasElement>canvasOrContext)["getContext"]) {
            let canvas = canvasOrContext as HTMLCanvasElement;
            let gl: Nullable<WebGLRenderingContext> = null;
            for (var ii = 0; ii < ManagedWebGLRenderingContext.contentModes.length; ++ii) {
                try {
                    gl = <WebGLRenderingContext>canvas.getContext(ManagedWebGLRenderingContext.contentModes[ii], contextConfig);
                } catch (e) {
                    //
                }

                if (gl) {
                    break;
                }
            }

            this.gl = <WebGLRenderingContext>(gl);
            this.canvas = canvas;

            // Native端 没有 WebGL设备丢失事件
            if (!canvas.addEventListener) {
                return;
            }

            canvas.addEventListener("webglcontextlost", (e: any) => {
                e && e.preventDefault && e.preventDefault();
            });

            canvas.addEventListener("webglcontextrestored", (e: any) => {
                for (let i = 0, n = this.restorables.length; i < n; i++) {
                    this.restorables[i].restore();
                }
            });
        } else {
            this.gl = canvasOrContext as WebGLRenderingContext;
            this.canvas = <HTMLCanvasElement>this.gl.canvas;
        }
    }

    addRestorable(restorable: Restorable) {
        this.restorables.push(restorable);
    }

    removeRestorable(restorable: Restorable) {
        let index = this.restorables.indexOf(restorable);
        if (index > -1) {
            this.restorables.splice(index, 1);
        }
    }

    public dispose() {
        ManagedWebGLRenderingContext.pool.delete(this.canvas);
    }
}