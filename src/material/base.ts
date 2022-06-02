import { Matrix4x4 } from "../math/matrix4x4";
import { Vector2 } from "../math/vector2";
import { Vector3 } from "../math/vector3";
import { Vector4 } from "../math/vector4";

export interface IShaderEffectFlag {
    [key: string]: boolean;
}

export interface IShaderEffectFloat {
    [key: string]: number;
}

export interface IShaderEffectVector2 {
    [key: string]: Vector2;
}

export interface IShaderEffectVector3 {
    [key: string]: Vector3;
}

export interface IShaderEffectVector4 {
    [key: string]: Vector4;
}

export interface IShaderEffectMat2 {
    [key: string]: Vector4;
}

export interface IShaderEffectMat4 {
    [key: string]: Matrix4x4;
}

export interface IShaderEffectSampler2D {
    [key: string]: string;
}

export interface IShader {
    passes: IMaterialSubPass[];
    apply(config: IShaderApplyConfig): void;
    compiler(config: IShaderApplyConfig): void;
}

export interface IShaderApplyConfig {
    flagEffect: IShaderEffectFlag;
    floatEffect: IShaderEffectFlag;
    vec2Effect: IShaderEffectVector2;
    vec3Effect: IShaderEffectVector3;
    vec4Effect: IShaderEffectVector4;
    mat2Effect: IShaderEffectMat2;
    mat4Effect: IShaderEffectMat4;
    sampler2DEffect: IShaderEffectSampler2D;
}

export interface IMaterialSubPass {
    tags: string[];
}

export interface IMaterial {
    shader: IShader;
    shaderConfig: IShaderApplyConfig;
}