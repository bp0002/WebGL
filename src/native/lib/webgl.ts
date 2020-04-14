
import { Vector3, Matrix } from '../../babylon_math/Maths/math';
/**
 * WEBGL 基本处理
 */

export interface WebGLInstanceOpt {
    canvas: HTMLCanvasElement;
}

export class Camera {
    public position: Vector3 = new Vector3(0, 0, 0);
    public target: Vector3 = new Vector3(0, 0, -1);
    public up: Vector3 = new Vector3(0, 1, 0);
    private _matrix: Matrix = new Matrix();
    public getMatrix() {
        Matrix.LookAtLHToRef(this.position, this.target, this.up, this._matrix);
        return this._matrix;
    }
    public static Default: Camera = new Camera();
}

export class ShaderCfg {
    public readonly sname: string;
    public readonly vs: string;
    public readonly fs: string;
    public vshader: WebGLShader | undefined    ;
    public fshader: WebGLShader | undefined    ;
    public programe: WebGLProgram | undefined  ;
    public u_view_matrix_loc: WebGLUniformLocation | undefined ;
    public u_time_loc: WebGLUniformLocation | undefined        ;
    public u_mouse_loc: WebGLUniformLocation | undefined       ;
    public u_resolution_loc: WebGLUniformLocation | undefined  ;
    public u_translate_loc: WebGLUniformLocation | undefined   ;
    public u_scale_loc: WebGLUniformLocation | undefined   ;
    public u_rotate_loc: WebGLUniformLocation | undefined   ;
    public u_float_loc: WebGLUniformLocation | undefined   ;
    public a_position_loc: number | undefined                  ;
    public a_color_loc: number | undefined                  ;
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

        gl.useProgram(<WebGLProgram>this.shader_program);

        this.u_view_matrix_loc  = <WebGLUniformLocation>gl.getUniformLocation(<WebGLProgram>this.shader_program, `u_ViewMatrix`);

        this.u_mouse_loc        = <WebGLUniformLocation>gl.getUniformLocation(<WebGLProgram>this.shader_program, `u_mouse`);

        this.u_time_loc         = <WebGLUniformLocation>gl.getUniformLocation(<WebGLProgram>this.shader_program, `u_time`);

        this.u_resolution_loc   = <WebGLUniformLocation>gl.getUniformLocation(<WebGLProgram>this.shader_program, `u_resolution`);

        this.u_translate_loc    = <WebGLUniformLocation>gl.getUniformLocation(<WebGLProgram>this.shader_program, `u_translate`);

        this.u_scale_loc        = <WebGLUniformLocation>gl.getUniformLocation(<WebGLProgram>this.shader_program, `u_scale`);

        this.u_rotate_loc       = <WebGLUniformLocation>gl.getUniformLocation(<WebGLProgram>this.shader_program, `u_rotate`);

        this.u_float_loc        = <WebGLUniformLocation>gl.getUniformLocation(<WebGLProgram>this.shader_program, `u_float`);

        this.a_position_loc     = gl.getAttribLocation(<WebGLProgram>this.shader_program, 'a_position');

        this.a_color_loc        = gl.getAttribLocation(<WebGLProgram>this.shader_program, 'a_color');

        this.a_uv               = gl.getAttribLocation(<WebGLProgram>this.shader_program, 'a_uv');

        this.u_texture          = <WebGLUniformLocation>gl.getUniformLocation(<WebGLProgram>this.shader_program, 'u_sampler');

        this.u_texture1         = <WebGLUniformLocation>gl.getUniformLocation(<WebGLProgram>this.shader_program, 'u_sampler1');

        if (this.a_position_loc >= 0) {
            gl.enableVertexAttribArray(this.a_position_loc);
        }

        if (this.a_color_loc >= 0) {
            gl.enableVertexAttribArray(this.a_color_loc);
        }

        if (this.a_uv >= 0) {
            gl.enableVertexAttribArray(this.a_uv);
        }

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
    /**
     * 一组 FLOAT 数据的大小
     */
    public FLOAT_SIZE:       number = 0;
    /**
     * 一组 INT 数据的大小
     * * 没有这个处理必要 - INT 数据为 绘制的ELEMENT
     * * 所有线数据 - 所有面数据 - 所有点数据 没有混合保存
     */
    public INT_SIZE:         number = 0;
    /**
     * 一个点的 坐标数据 数据量
     */
    public VERTEX_SIZE:      number = 3;
    /**
     * 一个点的 颜色数据 数据量
     */
    public COLOR_SIZE:       number = 4;
    /**
     * 一个点的 UV数据 数据量
     */
    public UV_SIZE:          number = 2;
    /**
     * 一个面的 点数目数据 数据量
     */
    public FACE_SIZE:        number = 3;
    /**
     * 一个点的 点数目数据 数据量
     */
    public LINE_SIZE:        number = 2;
    /**
     * 一条线的 点数目数据 数据量
     */
    public POINT_SIZE:       number = 1;
    public readonly vname:          string;

    public float_buffer:            WebGLBuffer | undefined;
    public float_data:              number[]    = [];
    public int_buffer:              WebGLBuffer | undefined;
    public int_data:                number[]    = [];

    public readonly vertex_data:    [number, number, number][]    = [];
    // public vertex_buffer:           WebGLBuffer | undefined;
    public vertex_offset:           number = 0;

    public readonly color_data:     [number, number, number, number][]    = [];
    // public color_buffer:            WebGLBuffer | undefined;
    public color_offset:            number = 0;

    public readonly face_data:      [number, number, number][]    = [];
    // public face_buffer:             WebGLBuffer | undefined;
    public face_offset:             number = 0;

    public readonly point_data:      number[]    = [];
    // public face_buffer:             WebGLBuffer | undefined;
    public point_offset:             number = 0;

    public readonly line_data:      [number, number][]    = [];
    // public line_buffer:             WebGLBuffer | undefined;
    public line_offset:             number = 0;

    public readonly uv_data:        [number, number][]    = [];
    // public uv_buffer:               WebGLBuffer | undefined;
    public uv_offset:               number = 0;
    constructor(vname: string) {
        this.vname = vname;
    }
    public addVertex(x: number, y: number, z: number = 0) {
        this.vertex_data.push([x, y, z]);
    }
    public addFace(a: number, b: number, c: number) {
        this.face_data.push([a, b, c]);
    }
    public addPoint(data: number) {
        this.point_data.push(data);
    }
    public addColor(r: number, g: number, b: number, a: number) {
        this.color_data.push([r, g, b, a]);
    }
    public addUV(u: number, v: number) {
        this.uv_data.push([u, v]);
    }
    public addVertex2(data: [number, number, number]) {
        this.vertex_data.push(data);
    }
    public addFace2(data: [number, number, number]) {
        this.face_data.push(data);
    }
    public addPoint2(data: number) {
        this.point_data.push(data);
    }
    public addColor2(data: [number, number, number, number]) {
        this.color_data.push(data);
    }
    public addUV2(data: [number, number]) {
        this.uv_data.push(data);
    }
    public clearVertex() {
        this.vertex_data.length = 0;
    }
    public clearFace() {
        this.face_data.length = 0;
    }
    public clearPoint() {
        this.point_data.length = 0;
    }
    public clearColor() {
        this.color_data.length = 0;
    }
    public clearUV() {
        this.uv_data.length = 0;
    }
    public update(gl: WebGLRenderingContext) {
        this.activeFloatBuffer(gl);
        this.activeIntBuffer(gl);
    }
    public activeFloatBuffer(gl: WebGLRenderingContext) {
        if (!this.float_buffer) {
            this.float_buffer   = <WebGLBuffer>gl.createBuffer();
        }

        this.float_data.length = 0;
        const count = this.vertex_data.length;
        let vertex: [number, number, number], color: [number, number, number, number], uv: [number, number];

        for (let i = 0; i < count; i++) {
            vertex    = this.vertex_data[i];
            color     = this.color_data[i];
            uv        = this.uv_data[i];

            vertex  && this.float_data.push(...vertex);
            color   && this.float_data.push(...color);
            uv      && this.float_data.push(...uv);
        }

        let offset = 0;
        if (this.vertex_data.length > 0) {
            this.vertex_offset = offset;
            offset += this.VERTEX_SIZE;
        }

        if (this.color_data.length > 0) {
            this.color_offset = offset;
            offset += this.COLOR_SIZE;
        }

        if (this.uv_data.length > 0) {
            this.uv_offset = offset;
            offset += this.UV_SIZE;
        }

        this.FLOAT_SIZE     = offset;

        gl.bindBuffer(gl.ARRAY_BUFFER, this.float_buffer);
        gl.bufferData(gl.ARRAY_BUFFER,
                        new Float32Array(this.float_data),
                        gl.STATIC_DRAW
                    );
    }
    public activeIntBuffer(gl: WebGLRenderingContext) {
        if (!this.int_buffer) {
            this.int_buffer  = <WebGLBuffer>gl.createBuffer();
        }

        this.int_data.length = 0;

        let face: [number, number, number], line: [number, number];
        let faces: number[] = [], lines: number[] = [];

        this.line_data.length = 0;

        const faceCount = this.face_data.length;
        for (let i = 0; i < faceCount; i++) {
            face    = this.face_data[i];
            face    && faces.push(...face);

            if (face) {
                for (let j = 0; j < 3; j++) {
                    if (j === 0) {
                        this.line_data.push([face[2], face[0]]);
                    } else {
                        this.line_data.push([face[j - 1], face[j]]);
                    }
                }
            }
        }

        const lineCount = this.line_data.length;
        for (let i = 0; i < lineCount; i++) {
            line    = this.line_data[i];
            line    && lines.push(...line);
        }

        let offset = 0;
        if (faces.length > 0) {
            // this.int_data.push(...faces);
            faces.forEach((face) => {
                this.int_data.push(face);
            });

            this.face_offset = offset;
            offset += faces.length;
        }

        if (lines.length > 0) {
            // this.int_data.push(...lines);
            lines.forEach((line) => {
                this.int_data.push(line);
            });

            this.line_offset = offset;
            offset += lines.length;
        }

        if (this.vertex_data.length > 0) {
            this.vertex_data.forEach((point, index) => {
                this.int_data.push(index);
            });

            this.point_offset = offset;
            offset += this.vertex_data.length;
        }

        this.INT_SIZE     = offset;

        gl.bindBuffer(gl.ELEMENT_ARRAY_BUFFER, this.int_buffer);
        gl.bufferData(gl.ELEMENT_ARRAY_BUFFER,
                        new Uint16Array(this.int_data),
                        gl.STATIC_DRAW
                    );
    }
}

export class Mesh {
    public texture: TextureInstance | null;
    public wireFrame: boolean = false;
    public pointFrame: boolean = false;
    public triangleFrame: boolean = true;
    public maskTexture: TextureInstance | null;
    public alphaMode: number = 0;
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
        } else {
            gl.bindTexture(gl.TEXTURE_2D, null);
        }

        shader.getPrograme(gl);

        const camera = scene.camera || Camera.Default;
        <WebGLUniformLocation>shader.u_view_matrix_loc  && gl.uniformMatrix4fv(<WebGLUniformLocation>shader.u_view_matrix_loc,  false,  <any>camera.getMatrix().m);
        <WebGLUniformLocation>shader.u_mouse_loc        && gl.uniform2fv(<WebGLUniformLocation>shader.u_mouse_loc,    scene.engine.u_mouse);
        <WebGLUniformLocation>shader.u_time_loc         && gl.uniform1f(<WebGLUniformLocation>shader.u_time_loc,      scene.engine.timestamp * 0.001);
        <WebGLUniformLocation>shader.u_float_loc        && gl.uniform1f(<WebGLUniformLocation>shader.u_float_loc,      this.ufloat);

        <WebGLUniformLocation>shader.u_resolution_loc   && gl.uniform2f(<WebGLUniformLocation>shader.u_resolution_loc, scene.engine.width,  scene.engine.height);
        <WebGLUniformLocation>shader.u_translate_loc    && gl.uniform3f(<WebGLUniformLocation>shader.u_translate_loc,  this.translate[0],  this.translate[1],  this.translate[2]);
        <WebGLUniformLocation>shader.u_scale_loc        && gl.uniform3f(<WebGLUniformLocation>shader.u_scale_loc,      this.scale[0],      this.scale[1],      this.scale[2]);
        <WebGLUniformLocation>shader.u_rotate_loc       && gl.uniform3f(<WebGLUniformLocation>shader.u_rotate_loc,     this.rotate[0],     this.rotate[1],     this.rotate[2]);

        if (<number>shader.a_position_loc >= 0) {
            gl.bindBuffer(gl.ARRAY_BUFFER, <WebGLBuffer>this.dataBufferCfg.float_buffer);
            gl.vertexAttribPointer(<number>shader.a_position_loc,
                                        this.dataBufferCfg.VERTEX_SIZE,
                                        gl.FLOAT,
                                        false,
                                        this.dataBufferCfg.FLOAT_SIZE * 4,
                                        this.dataBufferCfg.vertex_offset * 4
                                    );
        }

        if (<number>shader.a_color_loc >= 0) {
            gl.bindBuffer(gl.ARRAY_BUFFER, <WebGLBuffer>this.dataBufferCfg.float_buffer);
            gl.vertexAttribPointer(<number>shader.a_color_loc,
                                        this.dataBufferCfg.COLOR_SIZE,
                                        gl.FLOAT,
                                        false,
                                        this.dataBufferCfg.FLOAT_SIZE * 4,
                                        this.dataBufferCfg.color_offset * 4
                                    );
        }

        if (<number>shader.a_uv >= 0) {
            gl.bindBuffer(gl.ARRAY_BUFFER, <WebGLBuffer>this.dataBufferCfg.float_buffer);
            gl.vertexAttribPointer(<number>shader.a_uv,
                                        this.dataBufferCfg.UV_SIZE,
                                        gl.FLOAT,
                                        false,
                                        this.dataBufferCfg.FLOAT_SIZE * 4,
                                        this.dataBufferCfg.uv_offset * 4
                                    );
        }

        if (<WebGLBuffer>this.dataBufferCfg.int_buffer) {
            if (this.wireFrame) {
                if (this.dataBufferCfg.line_data.length > 0) {
                    gl.bindBuffer(gl.ELEMENT_ARRAY_BUFFER, <WebGLBuffer>this.dataBufferCfg.int_buffer);
                    gl.drawElements(gl.LINES,
                                        this.dataBufferCfg.line_data.length * this.dataBufferCfg.LINE_SIZE,
                                        gl.UNSIGNED_SHORT,
                                        this.dataBufferCfg.line_offset * 2
                                    );
                }
            }

            if (this.triangleFrame) {
                if (this.dataBufferCfg.face_data.length > 0) {
                    gl.bindBuffer(gl.ELEMENT_ARRAY_BUFFER, <WebGLBuffer>this.dataBufferCfg.int_buffer);
                    gl.drawElements(gl.TRIANGLES,
                                        this.dataBufferCfg.face_data.length * this.dataBufferCfg.FACE_SIZE,
                                        gl.UNSIGNED_SHORT,
                                        this.dataBufferCfg.face_offset * 2
                                    );
                }
            }

            if (this.pointFrame) {
                if (this.dataBufferCfg.vertex_data.length > 0) {
                    gl.drawElements(gl.POINTS,
                                        this.dataBufferCfg.vertex_data.length * this.dataBufferCfg.POINT_SIZE,
                                        gl.UNSIGNED_SHORT,
                                        this.dataBufferCfg.point_offset * 2
                                    );
                }
            }
        }

        gl.flush();
    }
}

export class Scene {
    public readonly sname:  string;
    public readonly engine: WebGLInstance;
    public readonly viewport:   number[] = [0, 0, 0, 0];
    public readonly meshMap:    Map<string, Mesh> = new Map();
    public camera!: Camera;
    constructor(sname: string, engine: WebGLInstance) {
        this.sname  = sname;
        this.engine = engine;
    }
    public setCamera(cam: Camera) {
        this.camera = cam;
    }
    public addMesh(mesh: Mesh) {
        this.meshMap.set(mesh.id, mesh);
    }
    public render(isClear: boolean) {
        const gl = <WebGLRenderingContext>this.engine.gl;

        gl.viewport(this.viewport[0], this.viewport[1], this.viewport[2], this.viewport[3]);
        if (isClear) {
            gl.clear(gl.COLOR_BUFFER_BIT | gl.DEPTH_BUFFER_BIT | gl.STENCIL_BUFFER_BIT);
        }

        gl.disable(gl.CULL_FACE);       // 不开启背面剔除
        gl.disable(gl.DEPTH_TEST);      // 不开启深度测试
        gl.disable(gl.SCISSOR_TEST);    // 避免渲染范围被前一个渲染过程限制

        gl.enable(gl.BLEND);
        gl.blendFunc(gl.ONE, gl.ONE_MINUS_SRC_ALPHA);
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
                    gl = <WebGLRenderingContext>canvas.getContext(WebGLInstance.contentModes[ii], { alpha : true, antialias : true });
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