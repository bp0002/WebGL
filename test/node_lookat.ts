import { LeftHandCoordinateSys3D } from "../src/coordinate_system/left_coordinate_sys_3d";
import { display } from "../src/display/html_display";
import { ESpace } from "../src/math/axis";
import { Quaternion } from "../src/math/quaternion";
import { Vector3 } from "../src/math/vector3";
import { ModifierLookAt } from "../src/modifier/3d/transform/look_at";
import { Node } from "../src/transform/3d/node";

function breakRow() {
    display('--------------------------------------------------------------<br>');
}

let coordinate_sys = new LeftHandCoordinateSys3D();

let node = new Node(`01`, coordinate_sys);
display(node.absoluteRotationQuaternion.toFormatString(10, 4) + `<br>`);

let lookatModifier = new ModifierLookAt();
lookatModifier.spcae = ESpace.WORLD;
node.modifierLocalList.push(lookatModifier);

let testQuaternion = new Quaternion();

display(`Look at <0, 0, 1> <br>`);
lookatModifier.target.copyFromFloats(0, 0, 1);
node.computeWorldMatrix();
display(`rotationQuaternion <br>`);
display(node.rotationQuaternion.toFormatString(10, 4) + `<br>`);
display(`absoluteRotationQuaternion <br>`);
display(node.absoluteRotationQuaternion.toFormatString(10, 4) + `<br>`);

display(`Look at <0, -1, 0> <br>`);
lookatModifier.target.copyFromFloats(0, -1, 0);
node.computeWorldMatrix();
display(`rotationQuaternion <br>`);
display(node.rotationQuaternion.toFormatString(10, 4) + `<br>`);
display(`absoluteRotationQuaternion <br>`);
display(node.absoluteRotationQuaternion.toFormatString(10, 4) + `<br>`);
display(node.computeWorldMatrix().toFormatString(10, 4) + `<br>`);

display(`Look at <0, -1, 0> 与 旋转角度 (-90,0,0) 不同 <br>`);
coordinate_sys.eulerAnglesToQuaternion(-90 / 180 * Math.PI, 0, 0, testQuaternion);
display(testQuaternion.toFormatString(10, 4) + `<br>`);

display(`Look at <0, -1, 1> <br>`);
lookatModifier.target.copyFromFloats(0, -1, 1);
node.computeWorldMatrix();
display(`rotationQuaternion <br>`);
display(node.rotationQuaternion.toFormatString(10, 4) + `<br>`);
display(`absoluteRotationQuaternion <br>`);
display(node.absoluteRotationQuaternion.toFormatString(10, 4) + `<br>`);

display(`Look at <0, -1, 1> 与 旋转角度 (-45,0,0) 是相同的 <br>`);
coordinate_sys.eulerAnglesToQuaternion(-45 / 180 * Math.PI, 0, 0, testQuaternion);
display(testQuaternion.toFormatString(10, 4) + `<br>`);
