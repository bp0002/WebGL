import { LeftHandCoordinateSys3D } from "../src/coordinate_system/left_coordinate_sys_3d";
import { display } from "../src/display/html_display";
import { Matrix4x4 } from "../src/math/matrix4x4";
import { Node } from "../src/transform/3d/node";
import { TransformNode } from "../src/transform/3d/transform_node";

function breakRow() {
    display('--------------------------------------------------------------<br>');
}
let tempMat4x4 = new Matrix4x4();

function displayNode(node: Node) {
    display(`Position:<br>`);
    display(node.position.toFormatString(10, 4, `<br>`));
    display(`Scaling:<br>`);
    display((<any>node)._scaling.toFormatString(10, 4, `<br>`));
    display(`Rotation:<br>`);
    display(node.rotationQuaternion.toFormatString(10, 4, `<br>`));
    display(`Local Matrix:<br>`);
    node.getLocalMatrix(tempMat4x4);
    display(tempMat4x4.toFormatString(10, 4, `<br>`));
    display(`World Matrix:<br>`);
    display(node.computeWorldMatrix().toFormatString(10, 4, `<br>`));
    display(`Absolute Position:<br>`);
    display(node.absolutePosition.toFormatString(10, 4, `<br>`));
    display(`Absolute Scaling:<br>`);
    display(node.absoluteScaling.toFormatString(10, 4, `<br>`));
    display(`Absolute Rotation:<br>`);
    display(node.absoluteRotationQuaternion.toFormatString(10, 4, `<br>`));
    breakRow();
}

let coordinate_sys = new LeftHandCoordinateSys3D();
let node0 = new TransformNode(`0`, coordinate_sys);
node0.modifyRotationByEulerAngle(90 * Math.PI / 180, 0, 0);
node0.position.copyFromFloats(100, 0, 0);
node0.scaling.copyFromFloats(2, 2, 2);
node0.computeWorldMatrix();
display(`node0:<br>`);
displayNode(node0);

let node2 = new Node(`2`, coordinate_sys);
node2.setParent(node0);
node2.position.copyFromFloats(100, 100, 100);
node2.computeWorldMatrix();
display(`node2:<br>`);
displayNode(node2);
