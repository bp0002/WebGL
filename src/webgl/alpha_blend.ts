import { BlendMode } from "./base";


export class WebGLBlendModeConverter {
    static ZERO = 0;
    static ONE = 1;
    static SRC_COLOR = 0x0300;
    static ONE_MINUS_SRC_COLOR = 0x0301;
    static SRC_ALPHA = 0x0302;
    static ONE_MINUS_SRC_ALPHA = 0x0303;
    static DST_ALPHA = 0x0304;
    static ONE_MINUS_DST_ALPHA = 0x0305;
    static DST_COLOR = 0x0306;

    static getDestGLBlendMode(blendMode: BlendMode) {
        switch (blendMode) {
            case BlendMode.Normal: return WebGLBlendModeConverter.ONE_MINUS_SRC_ALPHA;
            case BlendMode.Additive: return WebGLBlendModeConverter.ONE;
            case BlendMode.Multiply: return WebGLBlendModeConverter.ONE_MINUS_SRC_ALPHA;
            case BlendMode.Screen: return WebGLBlendModeConverter.ONE_MINUS_SRC_ALPHA;
            default: throw new Error("Unknown blend mode: " + blendMode);
        }
    }

    static getSourceGLBlendMode(blendMode: BlendMode, premultipliedAlpha: boolean = false) {
        switch (blendMode) {
            case BlendMode.Normal: return premultipliedAlpha ? WebGLBlendModeConverter.ONE : WebGLBlendModeConverter.SRC_ALPHA;
            case BlendMode.Additive: return premultipliedAlpha ? WebGLBlendModeConverter.ONE : WebGLBlendModeConverter.SRC_ALPHA;
            case BlendMode.Multiply: return WebGLBlendModeConverter.DST_COLOR;
            case BlendMode.Screen: return WebGLBlendModeConverter.ONE;
            default: throw new Error("Unknown blend mode: " + blendMode);
        }
    }

    static alphaBlendState(gl: WebGLRenderingContext, alphaMode: number) {

        switch (alphaMode) {
            case (2): {
                gl.blendFuncSeparate(gl.SRC_ALPHA, gl.ONE_MINUS_SRC_ALPHA, gl.SRC_ALPHA, gl.ONE_MINUS_SRC_ALPHA);
                break;
            }
            case (7): {
                gl.blendFuncSeparate(gl.ONE, gl.ONE_MINUS_SRC_ALPHA, gl.ONE, gl.ONE);
                break;
            }
            case (8): {
                gl.blendFuncSeparate(gl.ONE, gl.ONE_MINUS_SRC_ALPHA, gl.ONE, gl.ONE_MINUS_SRC_ALPHA);
                break;
            }
            default: {
                gl.blendFuncSeparate(gl.SRC_ALPHA, gl.ONE_MINUS_SRC_ALPHA, gl.SRC_ALPHA, gl.ONE_MINUS_SRC_ALPHA);
                break;
            }
        }
    }
}