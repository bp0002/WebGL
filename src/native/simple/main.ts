import { RenderLauncher } from "./render_launcher";
import { GeometryTools } from "./geometry";

const createButton = (tag: string, clikCall: (arg: string) => void) => {
    const div = document.createElement('div');
    div.innerHTML = `
        <span style="color: black;">${tag}</span><input id="${tag}" type="number" value="edgeCount:"/>
    `;
    div.style.width = '200px';
    div.style.height = 'auto';
    div.style.backgroundColor = 'green';
    div.addEventListener(
        'pointerdown',
        (e) => {
            const arg = <HTMLInputElement>document.getElementById(tag);
            clikCall && clikCall(arg.value);
        }
    );

    createPanel().appendChild(div);
};

const createPanel = () => {
    if (!panel) {
        panel = document.createElement('div');
        panel.style.width = '200px';
        panel.style.height = '400px';
        panel.style.overflowY = 'auto';
        panel.style.position = 'absolute';
        document.body.appendChild(panel);
    }

    return panel;
};

let panel: HTMLDivElement;
const canvas = <HTMLCanvasElement>document.getElementById('your_canvas');

canvas.width = window.innerWidth;
canvas.height = window.innerHeight;

canvas.style.transform = 'scale(0.5)';

RenderLauncher.active(canvas, null);

createButton(
    'polygon',
    (arg: string) => {
        // const reg = /edgeCount:(.+)-()/;
        // const resResult = arg.match(reg);
        if (arg && RenderLauncher.mesh) {
            const dataBuffer01 = RenderLauncher.mesh.dataBufferCfg;

            dataBuffer01.clearVertex();
            dataBuffer01.clearColor();
            dataBuffer01.clearFace();

            const edgeCount = (<any>arg) - 0;

            const sphere = GeometryTools.polygon(edgeCount, true);

            if (sphere) {
                if (sphere.vertexs3D) {
                    sphere.vertexs3D.forEach((vertex, index, arr) => {
                        dataBuffer01.addVertex(vertex[0], vertex[1], vertex[2]);
                        dataBuffer01.addColor(Math.abs(vertex[2]), 0, 0, 1);
                    });
                }

                sphere.faces.forEach((face) => {
                    dataBuffer01.addFace(face[0], face[1], face[2]);
                });
            }

            dataBuffer01.update(<WebGLRenderingContext>RenderLauncher.webgldemo.gl);
        }
    }
);

createButton(
    'pyramid',
    (arg: string) => {
        // const reg = /edgeCount:(.+)-()/;
        // const resResult = arg.match(reg);
        if (arg && RenderLauncher.mesh) {
            const dataBuffer01 = RenderLauncher.mesh.dataBufferCfg;

            dataBuffer01.clearVertex();
            dataBuffer01.clearColor();
            dataBuffer01.clearFace();

            const edgeCount = (<any>arg) - 0;

            const sphere = GeometryTools.pyramid(edgeCount);

            if (sphere) {
                if (sphere.vertexs3D) {
                    sphere.vertexs3D.forEach((vertex, index, arr) => {
                        dataBuffer01.addVertex(vertex[0], vertex[2], vertex[1]);
                        dataBuffer01.addColor(Math.abs(vertex[2]), 0, 0, 1);
                    });
                }

                sphere.faces.forEach((face) => {
                    dataBuffer01.addFace(face[0], face[1], face[2]);
                });
            }

            dataBuffer01.update(<WebGLRenderingContext>RenderLauncher.webgldemo.gl);
        }
    }
);

createButton(
    'antiPrism',
    (arg: string) => {
        // const reg = /edgeCount:(.+)-()/;
        // const resResult = arg.match(reg);
        if (arg && RenderLauncher.mesh) {
            const dataBuffer01 = RenderLauncher.mesh.dataBufferCfg;

            dataBuffer01.clearVertex();
            dataBuffer01.clearColor();
            dataBuffer01.clearFace();

            const edgeCount = (<any>arg) - 0;

            const sphere = GeometryTools.antiPrism(edgeCount);

            if (sphere) {
                if (sphere.vertexs3D) {
                    sphere.vertexs3D.forEach((vertex, index, arr) => {
                        dataBuffer01.addVertex(vertex[0], vertex[2], vertex[1]);
                        dataBuffer01.addColor(Math.abs(vertex[2]), 0, 0, 1);
                    });
                }

                sphere.faces.forEach((face) => {
                    dataBuffer01.addFace(face[0], face[1], face[2]);
                });
            }

            dataBuffer01.update(<WebGLRenderingContext>RenderLauncher.webgldemo.gl);
        }
    }
);

createButton(
    'column',
    (arg: string) => {
        // const reg = /edgeCount:(.+)-()/;
        // const resResult = arg.match(reg);
        if (arg && RenderLauncher.mesh) {
            const dataBuffer01 = RenderLauncher.mesh.dataBufferCfg;

            dataBuffer01.clearVertex();
            dataBuffer01.clearColor();
            dataBuffer01.clearFace();

            const edgeCount = (<any>arg) - 0;

            const sphere = GeometryTools.column(edgeCount);

            if (sphere) {
                if (sphere.vertexs3D) {
                    sphere.vertexs3D.forEach((vertex, index, arr) => {
                        dataBuffer01.addVertex(vertex[0], vertex[2], vertex[1]);
                        dataBuffer01.addColor(Math.abs(vertex[2]), 0, 0, 1);
                    });
                }

                sphere.faces.forEach((face) => {
                    dataBuffer01.addFace(face[0], face[1], face[2]);
                });
            }

            dataBuffer01.update(<WebGLRenderingContext>RenderLauncher.webgldemo.gl);
        }
    }
);

createButton(
    'sphere',
    (arg: string) => {
        // const reg = /edgeCount:(.+)-()/;
        // const resResult = arg.match(reg);
        if (arg && RenderLauncher.mesh) {
            const dataBuffer01 = RenderLauncher.mesh.dataBufferCfg;

            dataBuffer01.clearVertex();
            dataBuffer01.clearColor();
            dataBuffer01.clearFace();

            const edgeCount = (<any>arg) - 0;

            const sphere = GeometryTools.sphere(edgeCount, edgeCount);

            if (sphere) {
                if (sphere.vertexs3D) {
                    sphere.vertexs3D.forEach((vertex, index, arr) => {
                        dataBuffer01.addVertex(vertex[0], vertex[2], vertex[1]);
                        dataBuffer01.addColor(Math.abs(vertex[2]), 0, 0, 1);
                    });
                }

                sphere.faces.forEach((face) => {
                    dataBuffer01.addFace(face[0], face[1], face[2]);
                });
            }

            dataBuffer01.update(<WebGLRenderingContext>RenderLauncher.webgldemo.gl);
        }
    }
);

createButton(
    'ribbon',
    (arg: string) => {
        // const reg = /edgeCount:(.+)-()/;
        // const resResult = arg.match(reg);
        if (RenderLauncher.mesh) {
            const dataBuffer01 = RenderLauncher.mesh.dataBufferCfg;

            dataBuffer01.clearVertex();
            dataBuffer01.clearColor();
            dataBuffer01.clearFace();

            // const sphere = GeometryTools.sphere(100, 100);
            const sphere = GeometryTools.ribbon2(100, Date.now() % 1000 / 1000);

            if (sphere) {
                if (sphere.vertexs3D) {
                    sphere.vertexs3D.forEach((vertex, index, arr) => {
                        dataBuffer01.addVertex(vertex[0], vertex[1], vertex[2]);
                        dataBuffer01.addColor(0.8, 0, 0, 1);
                    });
                }

                sphere.faces.forEach((face) => {
                    dataBuffer01.addFace(face[0], face[1], face[2]);
                });
            }

            dataBuffer01.update(<WebGLRenderingContext>RenderLauncher.webgldemo.gl);
        }
    }
);