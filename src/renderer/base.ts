
export interface IRenderContext {
    viewport(x: number, y: number, w: number, h: number): void;
    numberTypeSet: NumberTypeSet;
}

export type NumberType = number;

export interface NumberTypeSet {
    FLOAT: NumberType;
    UNSIGNED_SHORT: NumberType;
    BYTE: NumberType;
}

export interface IRenderBuffer {
    format: NumberType;
    buffer: any;
    data: ArrayLike<number>;
    chunkCount: number;
    chunkSize: number;
    update(chunk: ArrayLike<number>): void;
    dispose(): void;
}

export interface IRenderer {
    passTagQuene: string[];
}