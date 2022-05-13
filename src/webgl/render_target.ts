import { Nullable } from "../base/types";
import { ManagedWebGLRenderingContext } from "./webgl";

export class RenderTexture {
    constructor(
        public readonly _webGLTexture: WebGLTexture,
        public readonly width: number,
        public readonly height: number,
    ) {

    }
    dispose() {
    }
}

export class RenderTarget extends RenderTexture {
    constructor(
        _webGLTexture: WebGLTexture,
        public readonly _framebuffer: WebGLFramebuffer,
        public readonly _renderbuffer: Nullable<WebGLRenderbuffer>,
        public readonly context: ManagedWebGLRenderingContext,
        width: number,
        height: number
    ) {
        super(_webGLTexture, width, height);
    }
    dispose() {
        super.dispose();

        const gl = this.context.gl;
        if (this._webGLTexture) {
            gl.deleteTexture(this._webGLTexture);
        }
        if (this._framebuffer) {
            gl.deleteFramebuffer(this._framebuffer);
        }
        if (this._renderbuffer) {
            gl.deleteRenderbuffer(this._renderbuffer);
        }
    }
}

export enum EWrap {
    REPEAT = 0,
    CLAMP_TO_EDGE = 1,
    MIRRORED_REPEAT = 2
}

export enum EFilter {
    LINEAR = 0,
    NEAREST = 1,
    NEAREST_MIPMAP_NEAREST = 2,
    LINEAR_MIPMAP_NEAREST = 3,
    NEAREST_MIPMAP_LINEAR = 4,
    LINEAR_MIPMAP_LINEAR = 5
}

export class RenderTargetFactory {
    private static Context: ManagedWebGLRenderingContext;

    private static Canvas: HTMLCanvasElement;

    private static TempScreenTexture: WebGLTexture;
    private static TempScreenFBO: WebGLFramebuffer;
    private static TempScreenRenderTarget: RenderTarget;

    private static UnuseRTArray: RenderTarget[] = [];
    private static UsedRTArray: RenderTarget[] = [];

    private static getRTCached(width: number, height: number, needDepth: boolean): Nullable<RenderTarget> {
        const count = this.UnuseRTArray.length;
        let result: Nullable<RenderTarget> = null;

        for (let i = count - 1; i >= 0; i--) {
            const rt = this.UnuseRTArray[i];
            if (
                rt.width == width
                && rt.height == height
                && !!rt._renderbuffer == needDepth
            ) {
                result = rt;
                this.UnuseRTArray.splice(i, 1);
                break;
            }
        }

        return result;
    }

    public static GLWrap(wrap: EWrap, gl: WebGLRenderingContext) {
        switch (wrap) {
            case (EWrap.REPEAT) : {
                return gl.REPEAT;
            }
            case (EWrap.CLAMP_TO_EDGE) : {
                return gl.CLAMP_TO_EDGE;
            }
            case (EWrap.MIRRORED_REPEAT) : {
                return gl.MIRRORED_REPEAT;
            }
        }
    }

    public static GLMagFilter(filter: EFilter, gl: WebGLRenderingContext) {
        switch (filter) {
            case (EFilter.LINEAR) : {
                return gl.LINEAR;
            }
            case (EFilter.NEAREST) : {
                return gl.NEAREST;
            }
            default: {
                return gl.LINEAR;
            }
        }
    }

    public static GLMinFilter(filter: EFilter, gl: WebGLRenderingContext) {
        switch (filter) {
            case (EFilter.LINEAR) : {
                return gl.LINEAR;
            }
            case (EFilter.LINEAR_MIPMAP_LINEAR) : {
                return gl.LINEAR_MIPMAP_LINEAR;
            }
            case (EFilter.LINEAR_MIPMAP_NEAREST) : {
                return gl.LINEAR_MIPMAP_NEAREST;
            }
            case (EFilter.NEAREST) : {
                return gl.NEAREST;
            }
            case (EFilter.NEAREST_MIPMAP_LINEAR) : {
                return gl.NEAREST_MIPMAP_LINEAR;
            }
            case (EFilter.NEAREST_MIPMAP_NEAREST) : {
                return gl.NEAREST_MIPMAP_NEAREST;
            }
            default: {
                return gl.NEAREST_MIPMAP_LINEAR;
            }
        }
    }

    public static getContext() {
        return this.Context;
    }

    public static init(canvas: HTMLCanvasElement, context: ManagedWebGLRenderingContext) {
        this.Context     = context;
        this.Canvas      = canvas;

        // this.TempScreenRenderTarget     = this.createRenderTarget(canvas.width, canvas.height, true);
        // this.TempScreenTexture          = this.TempScreenRenderTarget._webGLTexture;
        // this.TempScreenFBO              = this.TempScreenRenderTarget._framebuffer;
    }

    // public static getTempScreenFBO() {
    //     return this.TempScreenFBO;
    // }

    // public static getTempScreenTexture() {
    //     return this.TempScreenTexture;
    // }

    // public static getTempScreenRenderTarget() {
    //     return this.TempScreenRenderTarget;
    // }

    public static createRenderTarget(width: number, height: number, needDepth: boolean = false, magFilter: EFilter = EFilter.NEAREST, minFilter: EFilter = EFilter.NEAREST, wrapS: EWrap = EWrap.CLAMP_TO_EDGE, wrapT: EWrap = EWrap.CLAMP_TO_EDGE, mipmap: boolean = false): RenderTarget {

        let result: Nullable<RenderTarget> = this.getRTCached(width, height, needDepth);

        const gl = this.Context.gl;

        if (!result) {
            let w = width;
            let h = height;

            // 创建纹理
            const tex: Nullable<WebGLTexture> = gl.createTexture();
            if (!tex) {
                throw new Error(`gl.createTexture Error.`);
            }

            gl.activeTexture(gl.TEXTURE0);
            gl.bindTexture(gl.TEXTURE_2D, tex);
            gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_MAG_FILTER, this.GLMagFilter(magFilter, gl));
            gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_MIN_FILTER, this.GLMinFilter(minFilter, gl));
            gl.texParameteri(
                gl.TEXTURE_2D,
                gl.TEXTURE_WRAP_S,
                this.GLWrap(wrapS, gl),
            );
            gl.texParameteri(
                gl.TEXTURE_2D,
                gl.TEXTURE_WRAP_T,
                this.GLWrap(wrapT, gl)
            );
            gl.texImage2D(gl.TEXTURE_2D, 0, gl.RGBA, w, h, 0, gl.RGBA, gl.UNSIGNED_BYTE, null);

            // 创建fbo
            const fbo: Nullable<WebGLFramebuffer> = gl.createFramebuffer();
            if (!fbo) {
                throw new Error(`gl.createFramebuffer Error.`);
            }

            let rbo: Nullable<WebGLRenderbuffer> = null;
            if (needDepth) {
                rbo = gl.createRenderbuffer();
                gl.bindRenderbuffer(gl.RENDERBUFFER, rbo);
                gl.renderbufferStorage(gl.RENDERBUFFER, gl.DEPTH_COMPONENT16, w, h);
                gl.bindRenderbuffer(gl.RENDERBUFFER, null);
            }
            gl.bindFramebuffer(gl.FRAMEBUFFER, fbo);
            gl.framebufferTexture2D(gl.FRAMEBUFFER, gl.COLOR_ATTACHMENT0, gl.TEXTURE_2D, tex, 0);
            if (mipmap) {
                gl.generateMipmap(gl.TEXTURE_2D);
            }
            if (needDepth) {
                gl.framebufferRenderbuffer(gl.FRAMEBUFFER, gl.DEPTH_ATTACHMENT, gl.RENDERBUFFER, rbo);
            }
            gl.bindFramebuffer(gl.FRAMEBUFFER, null);
            gl.bindTexture(gl.TEXTURE_2D, null);
            result = new RenderTarget(tex, fbo, rbo, this.Context, w, h);
        }
        else {
            const tex = result._webGLTexture;
            gl.activeTexture(gl.TEXTURE0);
            gl.bindTexture(gl.TEXTURE_2D, tex);
            gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_MAG_FILTER, this.GLMagFilter(magFilter, gl));
            gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_MIN_FILTER, this.GLMinFilter(minFilter, gl));
            gl.texParameteri(
                gl.TEXTURE_2D,
                gl.TEXTURE_WRAP_S,
                this.GLWrap(wrapS, gl),
            );
            gl.texParameteri(
                gl.TEXTURE_2D,
                gl.TEXTURE_WRAP_T,
                this.GLWrap(wrapT, gl)
            );
            gl.bindTexture(gl.TEXTURE_2D, null);
        }

        this.UsedRTArray.push(result);

        return result;
    }

    public static unuseRenderTarget(rt: RenderTarget) {
        if (!rt) {
            return;
        }

        const index = this.UsedRTArray.indexOf(rt);
        if (index >= 0) {
            this.UsedRTArray.splice(index, 1);
            this.UnuseRTArray.push(rt);
        }
        else {
            console.warn(`RT Is Not Using.`);
        }
    }

    public static createRenderTargetFloat(width: number, height: number, needDepth: boolean = false): RenderTarget {

        const gl = this.Context.gl;

        let ext = gl.getExtension('OES_texture_float');
        let ext2 = gl.getExtension('WEBGL_color_buffer_float');

        if (!ext2) {
            throw new Error(`WEBGL_color_buffer_float not get`);
        }

        let w = width;
        let h = height;

        // 创建纹理
        const tex = gl.createTexture();
        if (!tex) {
            throw new Error(`gl.createTexture Error.`);
        }

        gl.activeTexture(gl.TEXTURE0);
        gl.bindTexture(gl.TEXTURE_2D, tex);
        gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_MIN_FILTER, gl.LINEAR);
        gl.texParameteri(
            gl.TEXTURE_2D,
            gl.TEXTURE_WRAP_S,
            gl.CLAMP_TO_EDGE,
        );
        gl.texParameteri(
            gl.TEXTURE_2D,
            gl.TEXTURE_WRAP_T,
            gl.CLAMP_TO_EDGE,
        );

        let data = new Float32Array(w * h * 4);
        for (let i = 0; i < data.length; ++i) {
            data[i] = 0;
        }
        gl.texImage2D(gl.TEXTURE_2D, 0, ext2.RGBA32F_EXT, w, h, 0, ext2.RGBA32F_EXT, gl.FLOAT, data);
        gl.bindTexture(gl.TEXTURE_2D, null);

        // 创建fbo
        const fbo = gl.createFramebuffer();
        if (!fbo) {
            throw new Error(`gl.createFramebuffer Error.`);
        }

        let rbo: Nullable<WebGLRenderbuffer> = null;
        if (needDepth) {
            rbo = gl.createRenderbuffer();
            gl.bindRenderbuffer(gl.RENDERBUFFER, rbo);
            gl.renderbufferStorage(gl.RENDERBUFFER, gl.DEPTH_COMPONENT16, w, h);
            gl.bindRenderbuffer(gl.RENDERBUFFER, null);
        }
        gl.bindFramebuffer(gl.FRAMEBUFFER, fbo);
        gl.framebufferTexture2D(gl.FRAMEBUFFER, gl.COLOR_ATTACHMENT0, gl.TEXTURE_2D, tex, 0);
        if (needDepth) {
            gl.framebufferRenderbuffer(gl.FRAMEBUFFER, gl.DEPTH_ATTACHMENT, gl.RENDERBUFFER, rbo);
        }
        gl.bindFramebuffer(gl.FRAMEBUFFER, null);

        return new RenderTarget(tex, fbo, rbo, this.Context, w, h);
    }

    public static  nearPower2(v: number) {
        let result = 4;
        while (result < v) {
            result *= 2;
        }

        return result;
    }

    public static bindAsRenderTarget(context: ManagedWebGLRenderingContext, tex: WebGLTexture, width: number, height: number): RenderTarget {
        let gl = context.gl;
        // 创建纹理
        gl.bindTexture(gl.TEXTURE_2D, tex);

        // 创建fbo
        const fbo = gl.createFramebuffer();
        if (!fbo) {
            throw new Error(`gl.createFramebuffer Error.`);
        }

        let rbo: Nullable<WebGLRenderbuffer> = null;
        gl.bindFramebuffer(gl.FRAMEBUFFER, fbo);
        gl.framebufferTexture2D(gl.FRAMEBUFFER, gl.COLOR_ATTACHMENT0, gl.TEXTURE_2D, tex, 0);
        gl.bindFramebuffer(gl.FRAMEBUFFER, null);
        gl.bindTexture(gl.TEXTURE_2D, null);

        return new RenderTarget(tex, fbo, rbo, this.Context, width, height);
    }
    public static unBindAsRenderTarget(context: ManagedWebGLRenderingContext, rt: RenderTarget) {
        const gl = context.gl;

        if (rt._framebuffer) {
            gl.deleteFramebuffer(rt._framebuffer);
        }
        if (rt._renderbuffer) {
            gl.deleteRenderbuffer(rt._renderbuffer);
        }
    }
}