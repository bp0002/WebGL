/**
 * WEBGL 基本处理
 */

export interface WebGLInstanceOpt {
    canvas: HTMLCanvasElement;
}

export class ShaderCfg {
    public readonly sname: string;
    public readonly vs: string;
    public readonly fs: string;
    public vshader: WebGLShader | undefined    ;
    public fshader: WebGLShader | undefined    ;
    public programe: WebGLProgram | undefined  ;
    public u_time_loc: WebGLUniformLocation | undefined        ;
    public u_mouse_loc: WebGLUniformLocation | undefined       ;
    public u_resolution_loc: WebGLUniformLocation | undefined  ;
    public u_translate_loc: WebGLUniformLocation | undefined   ;
    public u_scale_loc: WebGLUniformLocation | undefined   ;
    public u_rotate_loc: WebGLUniformLocation | undefined   ;
    public u_float_loc: WebGLUniformLocation | undefined   ;
    public a_position_loc: number | undefined                  ;
    public a_uv: number | undefined                  ;
    public u_texture: WebGLUniformLocation | undefined  ;
    public u_texture1: WebGLUniformLocation | undefined  ;
    private shader_program: WebGLProgram | undefined;
    public texActive: boolean = false  ;
    constructor(sname: string, vs: string, fs: string) {
        this.sname = sname;
        this.fs = fs;
        this.vs = vs;
    }
    public getPrograme(gl: WebGLRenderingContext) {

        const shader_fragment   = <WebGLShader>this.getFSShader(gl);
        const shader_vertex     = <WebGLShader>this.getVSShader(gl);

        if (this.shader_program === undefined && gl.getShaderParameter(shader_fragment, gl.COMPILE_STATUS)) {

            const shader_program  = <WebGLProgram>gl.createProgram();

            this.shader_program = shader_program;

            gl.attachShader(<WebGLProgram>this.shader_program, shader_vertex);
            gl.attachShader(<WebGLProgram>this.shader_program, shader_fragment);

            gl.linkProgram(<WebGLProgram>this.shader_program);
        }

        this.u_mouse_loc        = <WebGLUniformLocation>gl.getUniformLocation(<WebGLProgram>this.shader_program, `u_mouse`);

        this.u_time_loc         = <WebGLUniformLocation>gl.getUniformLocation(<WebGLProgram>this.shader_program, `u_time`);

        this.u_resolution_loc   = <WebGLUniformLocation>gl.getUniformLocation(<WebGLProgram>this.shader_program, `u_resolution`);

        this.u_translate_loc    = <WebGLUniformLocation>gl.getUniformLocation(<WebGLProgram>this.shader_program, `u_translate`);

        this.u_scale_loc        = <WebGLUniformLocation>gl.getUniformLocation(<WebGLProgram>this.shader_program, `u_scale`);

        this.u_rotate_loc       = <WebGLUniformLocation>gl.getUniformLocation(<WebGLProgram>this.shader_program, `u_rotate`);

        this.u_float_loc        = <WebGLUniformLocation>gl.getUniformLocation(<WebGLProgram>this.shader_program, `u_float`);

        this.a_position_loc     = gl.getAttribLocation(<WebGLProgram>this.shader_program, 'position');

        this.a_uv               = gl.getAttribLocation(<WebGLProgram>this.shader_program, 'a_uv');

        this.u_texture          = <WebGLUniformLocation>gl.getUniformLocation(<WebGLProgram>this.shader_program, 'u_sampler');

        this.u_texture1          = <WebGLUniformLocation>gl.getUniformLocation(<WebGLProgram>this.shader_program, 'u_sampler1');

        if (this.a_position_loc >= 0) {
            gl.enableVertexAttribArray(this.a_position_loc);
        }

        if (this.a_uv >= 0) {
            gl.enableVertexAttribArray(this.a_uv);
        }

        gl.useProgram(<WebGLProgram>this.shader_program);

        if (this.texActive) {
            this.u_texture && gl.uniform1i(this.u_texture, 0);
            this.u_texture1 && gl.uniform1i(this.u_texture1, 1);
        }

    }
    public getVSShader(gl: WebGLRenderingContext) {
        if (gl === null) { return this.vshader; }

        if (this.vshader) { return this.vshader; }

        this.vshader  = <WebGLShader>gl.createShader(gl.VERTEX_SHADER);

        if (this.vshader === null) { return this.vshader; }

        if (this.vs === undefined) { return this.vshader; }

        gl.shaderSource(this.vshader, this.vs);
        gl.compileShader(this.vshader);

        if (!gl.getShaderParameter(this.vshader, gl.COMPILE_STATUS)) {
            console.error(`ERROR IN 'VERTEX_SHADER' SHADER: ${ gl.getShaderInfoLog(this.vshader) }`);
            return this.vshader;
        }

        return this.vshader;
    }
    public getFSShader(gl: WebGLRenderingContext) {
        if (gl === null) { return this.fshader; }

        if (this.fshader) { return this.fshader; }

        this.fshader  = <WebGLShader>gl.createShader(gl.FRAGMENT_SHADER);

        if (this.fshader === null) { return this.fshader; }

        if (this.fs === undefined) { return this.fshader; }

        gl.shaderSource(this.fshader, this.fs);
        gl.compileShader(this.fshader);

        if (!gl.getShaderParameter(this.fshader, gl.COMPILE_STATUS)) {
            console.error(`ERROR IN 'FRAGMENT_SHADER' SHADER: ${ gl.getShaderInfoLog(this.fshader) }`);
            return this.fshader;
        }

        return this.fshader;
    }
}

export class DataBufferCfg {
    public readonly vname:          string;
    public vertex_loc:              number | undefined;
    public readonly vertex_data:    number[]    = [];
    public vertex_buffer:           WebGLBuffer | undefined;
    public face_loc:                number | undefined;
    public readonly face_data:      number[]    = [];
    public face_buffer:             WebGLBuffer | undefined;
    public readonly uv_data:        number[]    = [];
    public uv_buffer:               WebGLBuffer | undefined;
    constructor(vname: string) {
        this.vname = vname;
    }
    public addVertex(x: number, y: number, z: number) {
        this.vertex_data.push(x, y);
    }
    public addFace(a: number, b: number, c: number) {
        this.face_data.push(a, b, c);
    }
    public addUV(u: number, v: number) {
        this.uv_data.push(u, v);
    }
    public update(gl: WebGLRenderingContext) {
        this.activeVertex(gl);
        this.activeUV(gl);
        this.activeFace(gl);
    }
    public activeVertex(gl: WebGLRenderingContext) {
        if (!this.vertex_buffer) {
            this.vertex_buffer  = <WebGLBuffer>gl.createBuffer();

            gl.bindBuffer(gl.ARRAY_BUFFER, this.vertex_buffer);
            gl.bufferData(gl.ARRAY_BUFFER,
                            new Float32Array(this.vertex_data),
                            gl.STATIC_DRAW
                        );
        }
    }
    public activeFace(gl: WebGLRenderingContext) {
        if (!this.face_buffer) {
            this.face_buffer  = <WebGLBuffer>gl.createBuffer();

            gl.bindBuffer(gl.ELEMENT_ARRAY_BUFFER, this.face_buffer);
            gl.bufferData(gl.ELEMENT_ARRAY_BUFFER,
                            new Uint16Array(this.face_data),
                            gl.STATIC_DRAW
                        );
        }
    }
    public activeUV(gl: WebGLRenderingContext) {
        if (!this.uv_buffer) {
            this.uv_buffer  = <WebGLBuffer>gl.createBuffer();

            gl.bindBuffer(gl.ARRAY_BUFFER, this.uv_buffer);
            gl.bufferData(gl.ARRAY_BUFFER,
                            new Float32Array(this.uv_data),
                            gl.STATIC_DRAW
                        );
        }
    }
}

export class Mesh {
    public texture: TextureInstance | null;
    public maskTexture: TextureInstance | null;
    public readonly dataBufferCfg: DataBufferCfg;
    public readonly shaderCfg: ShaderCfg;
    public readonly id: string;
    public readonly translate: number[] = [0, 0, 0];
    public readonly scale: number[]     = [1, 1, 1];
    public readonly rotate: number[]    = [0, 0, 0];
    public ufloat: number      = 0.0;
    constructor(id: string, geo: DataBufferCfg, material: ShaderCfg) {
        this.id             = id;
        this.dataBufferCfg  = geo;
        this.shaderCfg      = material;
        this.texture        = null;
        this.maskTexture    = null;
    }
    public render(scene: Scene) {

        const gl = <WebGLRenderingContext>scene.engine.gl;

        const shader = <ShaderCfg>this.shaderCfg;

        if (this.texture) {
            this.shaderCfg.texActive = this.texture.active();

            if (this.maskTexture) {
                this.shaderCfg.texActive = this.maskTexture.active();
            }

            if (!this.shaderCfg.texActive) {
                return;
            }
        }

        shader.getPrograme(gl);

        <WebGLUniformLocation>shader.u_mouse_loc    && gl.uniform2fv(<WebGLUniformLocation>shader.u_mouse_loc,    scene.engine.u_mouse);
        <WebGLUniformLocation>shader.u_time_loc     && gl.uniform1f(<WebGLUniformLocation>shader.u_time_loc,      scene.engine.timestamp * 0.001);
        <WebGLUniformLocation>shader.u_float_loc    && gl.uniform1f(<WebGLUniformLocation>shader.u_float_loc,      this.ufloat);

        <WebGLUniformLocation>shader.u_resolution_loc   && gl.uniform2f(<WebGLUniformLocation>shader.u_resolution_loc, scene.engine.width,  scene.engine.height);
        <WebGLUniformLocation>shader.u_translate_loc    && gl.uniform3f(<WebGLUniformLocation>shader.u_translate_loc,  this.translate[0],  this.translate[1],  this.translate[2]);
        <WebGLUniformLocation>shader.u_scale_loc        && gl.uniform3f(<WebGLUniformLocation>shader.u_scale_loc,      this.scale[0],      this.scale[1],      this.scale[2]);
        <WebGLUniformLocation>shader.u_rotate_loc       && gl.uniform3f(<WebGLUniformLocation>shader.u_rotate_loc,     this.rotate[0],     this.rotate[1],     this.rotate[2]);

        if (<number>shader.a_position_loc >= 0) {
            gl.bindBuffer(gl.ARRAY_BUFFER, <WebGLBuffer>this.dataBufferCfg.vertex_buffer);
            gl.vertexAttribPointer(<number>shader.a_position_loc,
                                        2,
                                        gl.FLOAT,
                                        false,
                                        4 * 2,
                                        0
                                    );
        }

        if (<number>shader.a_uv >= 0) {
            gl.bindBuffer(gl.ARRAY_BUFFER, <WebGLBuffer>this.dataBufferCfg.uv_buffer);
            gl.vertexAttribPointer(<number>shader.a_uv,
                                        2,
                                        gl.FLOAT,
                                        false,
                                        4 * 2,
                                        0
                                    );
        }

        if (<WebGLBuffer>this.dataBufferCfg.face_buffer) {
            gl.bindBuffer(gl.ELEMENT_ARRAY_BUFFER, <WebGLBuffer>this.dataBufferCfg.face_buffer);
            gl.drawElements(gl.TRIANGLES,
                                this.dataBufferCfg.face_data.length,
                                gl.UNSIGNED_SHORT,
                                0
                            );
        }

        gl.flush();
    }
}

export class Scene {
    public readonly sname:  string;
    public readonly engine: WebGLInstance;
    public readonly viewport:   number[] = [0, 0, 0, 0];
    public readonly meshMap:    Map<string, Mesh> = new Map();
    constructor(sname: string, engine: WebGLInstance) {
        this.sname  = sname;
        this.engine = engine;
    }
    public addMesh(mesh: Mesh) {
        this.meshMap.set(mesh.id, mesh);
    }
    public render(isClear: boolean) {
        const gl = <WebGLRenderingContext>this.engine.gl;

        gl.viewport(this.viewport[0], this.viewport[1], this.viewport[2], this.viewport[3]);
        if (isClear) {
            gl.clear(gl.COLOR_BUFFER_BIT);
        }

        gl.enable(gl.BLEND);
        gl.blendFuncSeparate(gl.SRC_ALPHA, gl.ONE_MINUS_SRC_ALPHA, gl.ONE, gl.ONE);
        this.meshMap.forEach((mesh) => {
            mesh.render(this);
        });
    }
}

export class TextureInstance {
    public static loadCall = (path: string, engine: WebGLInstance, cb: (img: HTMLImageElement, fname: string, engine: WebGLInstance) => void) => {
        try {
            // const img = new Image();
            // img.onload = () => {
            //     cb(img, path, engine);
            // };
            // img.src = path;
        } catch (e) {
            console.error(e);
        }
    }
    public static loaded = (img: HTMLImageElement, fname: string, engine: WebGLInstance) => {
        const texIns = <TextureInstance>engine.getTexture(fname);
        if (texIns) {
            const GL = <WebGLRenderingContext>engine.gl;
            const tex   = <WebGLTexture>GL.createTexture();
            GL.pixelStorei(GL.UNPACK_FLIP_Y_WEBGL, true);
            GL.bindTexture(GL.TEXTURE_2D, tex);
            GL.texImage2D(GL.TEXTURE_2D, 0, GL.RGBA, GL.RGBA, GL.UNSIGNED_BYTE, img);
            GL.texParameteri(GL.TEXTURE_2D, GL.TEXTURE_MAG_FILTER, GL.LINEAR);
            GL.texParameteri(GL.TEXTURE_2D, GL.TEXTURE_MIN_FILTER, GL.NEAREST_MIPMAP_LINEAR);
            GL.generateMipmap(GL.TEXTURE_2D);
            GL.bindTexture(GL.TEXTURE_2D, null);
            texIns._tex = tex;
        }

    }
    public readonly fname: string;
    private _index: number;
    private _tex: WebGLTexture | null;
    private _engine: WebGLInstance;
    constructor(name: string, engine: WebGLInstance, index?: number) {
        this.fname      = name;
        this._engine    = engine;
        this._tex       = null;
        this._index     = index || 0;

        engine.addTexture(this);
        TextureInstance.loadCall(name, engine, TextureInstance.loaded);
    }
    public active() {
        let result: boolean = false;

        const GL    = <WebGLRenderingContext>this._engine.gl;

        if (this._tex) {
            if (this._index === 0) {
                GL.activeTexture(GL.TEXTURE0);
            } else if (this._index === 1) {
                GL.activeTexture(GL.TEXTURE1);
            }
            GL.bindTexture(GL.TEXTURE_2D, this._tex);
            result = true;
        }

        return result;
    }
    public remove() {
        this._engine.delTexture(this);
    }
}

export class WebGLInstance {
    public readonly canvas: HTMLCanvasElement;
    public readonly gl: WebGLRenderingContext | null;
    public readonly width: number;
    public readonly height: number;
    public static readonly uniforms_1f: string[]    = ['u_time'];
    public static readonly uniforms_2fv: string[]   = ['u_mouse'];
    public static readonly uniforms_2f: string[]    = ['u_resolution'];
    public static readonly contentModes = ["webgl", "experimental-webgl", "webgl2", "webkit-3d", "moz-webgl"];
    public readonly u_mouse: number[]        = [0, 0];
    public timestamp: number = 0;
    private sceneMap: Map<string, Scene> = new Map();
    private textureMap: Map<string, TextureInstance> = new Map();
    private _isDestroy: boolean = false;
    public get isDestroy() {
        return this._isDestroy;
    }
    constructor(opt: WebGLInstanceOpt) {
        this.canvas = opt.canvas;
        this.width  = this.canvas.width;
        this.height = this.canvas.height;
        this.gl     = WebGLInstance.ctxInitFunc(this.canvas);
    }
    private static ctxInitFunc(canvas: HTMLCanvasElement): WebGLRenderingContext | null {
        let gl: WebGLRenderingContext | null = null;
        try {
            for (var ii = 0; ii < WebGLInstance.contentModes.length; ++ii) {
                try {
                    gl = <WebGLRenderingContext>canvas.getContext(WebGLInstance.contentModes[ii], { alpha : true, antialias : false });
                } catch (e) {
                    //
                }

                if (gl) {
                    break;
                }
            }
        } catch (error) {
            console.warn(`There is not webgl compatible :( `);
        }

        return gl;
    }
    public createTexture(fname: string, index?: number) {
        let tex: TextureInstance = <TextureInstance>this.textureMap.get(fname);

        if (tex === undefined) {
            tex = new TextureInstance(fname, this, index);
        }

        return tex;
    }
    public addTexture(tex: TextureInstance) {
        this.textureMap.set(tex.fname, tex);
    }
    public getTexture(fname: string) {
        return this.textureMap.get(fname);
    }
    public delTexture(tex: TextureInstance) {
        this.textureMap.delete(tex.fname);
        (<WebGLRenderingContext>this.gl).deleteTexture(tex);
    }
    public addScene(cfg: Scene) {
        this.sceneMap.set(cfg.sname, cfg);
    }
    public clearColor() {
        const gl = (<WebGLRenderingContext>this.gl);
        gl.viewport(0, 0, this.width, this.height);
        gl.clearColor(0.0, 0.0, 0.0, 0.0);
    }
    public loop = (timestamp: number) => {
        this.timestamp = timestamp;

        this.renderLoop(timestamp);

        requestAnimationFrame(this.loop);
    }
    public renderLoop(timestamp: number) {}
    public destroy() {
        this._isDestroy = true;
        this.textureMap.forEach((tex) => {
            this.delTexture(tex);
        });
    }
}