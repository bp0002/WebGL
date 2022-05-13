import { Nullable } from "../base/types";
import { ManagedWebGLRenderingContext } from "./webgl";

export interface IPostProcessOpt {
    mode: number;
    sourceWidth: number;
    sourceHeight: number;
    sourceTexture: WebGLTexture;
    renderToScreen?: boolean;
    alphaMode?: number;
    option?: any;
}

export class ShaderCompiler {
    public static DEGREE = 'degree';
    public static POSITION = 'position';
    public static DELTA = 'delta';
    public static TEXTURE_SAMPLER = 'textureSampler';
    public static TEXTURE_SAMPLER_2 = 'textureSampler2';
    private context: ManagedWebGLRenderingContext;
    private vs: Nullable<WebGLShader> = null;
    private vsSource: string;
    private fs: Nullable<WebGLShader> = null;
    private fsSource: string;
    private program: Nullable<WebGLProgram> = null;
    private tmp2x2: Float32Array = new Float32Array(2 * 2);
    private tmp3x3: Float32Array = new Float32Array(3 * 3);
    private tmp4x4: Float32Array = new Float32Array(4 * 4);

    constructor(context: ManagedWebGLRenderingContext | WebGLRenderingContext, private vertexShader: string, private fragmentShader: string) {

        this.vsSource           = vertexShader;
        this.fsSource           = fragmentShader;
        this.context            = context instanceof ManagedWebGLRenderingContext ? context : new ManagedWebGLRenderingContext(context);

        this.context.addRestorable(this);
        this.compile();
    }

    public getProgram() { return this.program; }
    public getVertexShader() { return this.vertexShader; }
    public getFragmentShader() { return this.fragmentShader; }
    public getVertexShaderSource() { return this.vsSource; }
    public getFragmentSource() { return this.fsSource; }

    private compile() {
        let gl = this.context.gl;
        try {
            this.vs = this.compileShader(gl.VERTEX_SHADER, this.vertexShader);
            this.fs = this.compileShader(gl.FRAGMENT_SHADER, this.fragmentShader);
            this.program = this.compileProgram(this.vs, this.fs);
        } catch (e) {
            this.dispose();
            throw e;
        }
    }

    private compileShader(type: number, source: string) {
        let gl = this.context.gl;
        let shader = gl.createShader(type);
        if (shader) {
            gl.shaderSource(shader, source);
            gl.compileShader(shader);
            if (!gl.getShaderParameter(shader, gl.COMPILE_STATUS)) {
                let error = "Couldn't compile shader: " + gl.getShaderInfoLog(shader);
                gl.deleteShader(shader);
                if (!gl.isContextLost()) {
                    throw new Error(error);
                }
            }
        } else {
            throw new Error(`gl.createShader Error.`);
        }
        return shader;
    }

    private compileProgram(vs: WebGLShader, fs: WebGLShader) {
        let gl = this.context.gl;
        let program = gl.createProgram();
        if (program) {
            gl.attachShader(program, vs);
            gl.attachShader(program, fs);
            gl.linkProgram(program);

            if (!gl.getProgramParameter(program, gl.LINK_STATUS)) {
                let error = "Couldn't compile shader program: " + gl.getProgramInfoLog(program);
                gl.deleteProgram(program);
                if (!gl.isContextLost()) {
                    throw new Error(error);
                }
            }
        } else {
            throw new Error(`gl.createProgram Error.`);
        }
        return program;
    }

    restore() {
        this.compile();
    }

    public bind() {
        this.context.gl.useProgram(this.program);
    }

    public unbind() {
        this.context.gl.useProgram(null);
    }

    public setUniformi(uniform: string, value: number) {
        this.context.gl.uniform1i(this.getUniformLocation(uniform), value);
    }

    public setUniformf(uniform: string, value: number) {
        this.context.gl.uniform1f(this.getUniformLocation(uniform), value);
    }

    public setUniform2f(uniform: string, value: number, value2: number) {
        this.context.gl.uniform2f(this.getUniformLocation(uniform), value, value2);
    }

    public setUniform3f(uniform: string, value: number, value2: number, value3: number) {
        this.context.gl.uniform3f(this.getUniformLocation(uniform), value, value2, value3);
    }

    public setUniform4f(uniform: string, value: number, value2: number, value3: number, value4: number) {
        this.context.gl.uniform4f(this.getUniformLocation(uniform), value, value2, value3, value4);
    }

    public setUniform2x2f(uniform: string, value: ArrayLike<number>) {
        let gl = this.context.gl;
        this.tmp2x2.set(value);
        gl.uniformMatrix2fv(this.getUniformLocation(uniform), false, this.tmp2x2);
    }

    public setUniform3x3f(uniform: string, value: ArrayLike<number>) {
        let gl = this.context.gl;
        this.tmp3x3.set(value);
        gl.uniformMatrix3fv(this.getUniformLocation(uniform), false, this.tmp3x3);
    }

    public setUniform4x4f(uniform: string, value: ArrayLike<number>) {
        let gl = this.context.gl;
        this.tmp4x4.set(value);
        gl.uniformMatrix4fv(this.getUniformLocation(uniform), false, this.tmp4x4);
    }

    public getUniformLocation(uniform: string): Nullable<WebGLUniformLocation> {
        let gl = this.context.gl;
        let location: Nullable<WebGLUniformLocation> = null;
        if (this.program) {
            let location = gl.getUniformLocation(this.program, uniform);
            if (!location && !gl.isContextLost()) {
                throw new Error(`Couldn't find location for uniform ${uniform}`);
            }
        }
        return location;
    }

    public getAttributeLocation(attribute: string): number {
        let gl = this.context.gl;
        let location = -1;
        if (this.program) {
            gl.getAttribLocation(this.program, attribute);
        }
        if (location == -1 && !gl.isContextLost()) {
            throw new Error(`Couldn't find location for attribute ${attribute}`);
        }
        gl.enableVertexAttribArray(location);
        return location;
    }

    public dispose() {
        this.context.removeRestorable(this);

        let gl = this.context.gl;
        if (this.vs) {
            gl.deleteShader(this.vs);
            this.vs = null;
        }

        if (this.fs) {
            gl.deleteShader(this.fs);
            this.fs = null;
        }

        if (this.program) {
            gl.deleteProgram(this.program);
            this.program = null;
        }
    }

}