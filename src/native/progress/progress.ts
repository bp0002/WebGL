import { RenderLauncher } from "./render_launcher";

/**
 * 本地进度条
 */

export class Bar {
    // 加载的总文件长度
    public total: number = 0;
    // 加载的当前进度
    public loaded: number = 0;
    private old: number = 0;
    private last: number = 0;
    private opacity: number = 0;
    private timeRef: number = 0;

    constructor(cfg: { canvas: HTMLCanvasElement, bg: string }) {
        RenderLauncher.active(cfg.canvas, 'progress', cfg);
    }
    private calc = () => {
        setTimeout(this.calc, 100);

        if (this.last === this.loaded) {
            return;
        }

        this.last = this.loaded;
        RenderLauncher.updateProgress((this.last - this.old) / (this.total - this.old));
    }
    // 显示文字和进度条， 文字有呼吸灯的效果
    public show(text: string, total: number, loaded: number) {
        this.total = total;
        this.old = loaded;
        if (this.timeRef) {
            return;
        }

        this.timeRef = <any>setTimeout(this.calc, 100);
    }
    public onProcess(url: string, type: string, total: number, loaded: number, data?: Uint8Array) {
        this.loaded = loaded;
    }
    public clear() {
        this.total = 0;
        this.loaded = 0;
        this.old = 0;
        if (this.timeRef) {
            clearTimeout(this.timeRef);
        }
        this.timeRef = 0;
        RenderLauncher.destroy();
    }
}