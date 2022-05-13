import { DataBufferCfg } from "./data_buffer";
import { ShaderCompiler } from "./shader_compiler";
import { ManagedWebGLRenderingContext } from "./webgl";

export class PostProcessDataManager {
    private static pool: Map<ManagedWebGLRenderingContext, PostProcessDataManager> = new Map();
    public static baseDataBufferCfgKey: string = `BaseDataBufferCfg`;

    public static get(context: ManagedWebGLRenderingContext) {
        let result = this.pool.get(context);
        if (!result) {
            result = new PostProcessDataManager(context);
            PostProcessDataManager.pool.set(context, result);
        }

        return result;
    }

    private shaderMap: Map<string, ShaderCompiler> = new Map();
    private dataBufferMap: Map<string, DataBufferCfg> = new Map();

    constructor(
        private context: ManagedWebGLRenderingContext
    ) {
        let dataBufferCfg = new DataBufferCfg(PostProcessDataManager.baseDataBufferCfgKey);
        dataBufferCfg.addVertex(-1, -1, 0);
        dataBufferCfg.addVertex(1, -1, 0);
        dataBufferCfg.addVertex(1,  1, 0);
        dataBufferCfg.addVertex(-1,  1, 0);
        dataBufferCfg.addFace(0, 1, 2);
        dataBufferCfg.addFace(0, 2, 3);
        dataBufferCfg.update(this.context.gl);
        this.setDataBufferCfg(dataBufferCfg.vname, dataBufferCfg);
    }

    public getShaderCompiler(key: string) {
        return this.shaderMap.get(key);
    }

    public getDataBufferCfg(key: string) {
        return this.dataBufferMap.get(key);
    }

    public setShaderCompiler(key: string, v: ShaderCompiler) {
        return this.shaderMap.set(key, v);
    }

    public setDataBufferCfg(key: string, v: DataBufferCfg) {
        return this.dataBufferMap.set(key, v);
    }
}