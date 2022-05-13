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
        gl.bindBuffer(gl.ARRAY_BUFFER, null);
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
        gl.bindBuffer(gl.ELEMENT_ARRAY_BUFFER, null);
    }
}
