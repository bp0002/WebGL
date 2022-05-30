import { Dim, Matrix } from "../matrix";
import { Matrix4x4 } from "../matrix4x4";
import { Vector2 } from "../vector2";
import { Vector3 } from "../vector3";
import { Vector4 } from "../vector4";
import { display } from "../../display/html_display";
import { LeftHandCoordinateSys3D } from "../../coordinate_system/left_coordinate_sys_3d";
import { Quaternion } from "../quaternion";
import { SquareMatrix } from "../square_matrix";
import { Row } from "../row";
import { ICoordinateSystem } from "../../coordinate_system/coordinate_sys";

function breakRow() {
    display('--------------------------------------------------------------<br>');
}

const coordinate_sys: ICoordinateSystem = new LeftHandCoordinateSys3D();

let mat1x1 = new Matrix(1, 1);
display(mat1x1.toFormatString(undefined, undefined, '<br>'));
breakRow();

let mat1x4 = new Matrix(1, 4);
display(mat1x4.toFormatString(undefined, undefined, '<br>'));
breakRow();

let mat4x2 = new Matrix(4, 2);
display(mat4x2.toFormatString(undefined, undefined, '<br>'));
breakRow();

display(mat1x4.multiply(mat4x2).toFormatString(undefined, undefined, '<br>'));
breakRow();

let mat4x4 = new Matrix4x4();
SquareMatrix.IdentityToRef(mat4x4);
display(mat4x4.toFormatString(undefined, undefined, '<br>'));
breakRow();

let vector2 = new Vector2(4, 4);
display(vector2.toFormatString(undefined, undefined, '<br>'));
breakRow();

let vector3 = new Vector3(4, 4);
display(vector3.toFormatString(undefined, undefined, '<br>'));
breakRow();

Matrix.MultiplyToRef<1, 3, 1>(vector3, vector3.transpose(), mat1x1);
display(mat1x1.toFormatString(undefined, undefined, '<br>'));
breakRow();

let vector4 = new Vector4(4, 4);
display(vector4.toFormatString(undefined, undefined, '<br>'));
breakRow();

let mat3x2 = new Matrix(3, 2);
let mat2x3 = new Matrix(2, 3);
for (let i: Dim = 1; i <= 3; i++) {
    for (let j: Dim = 1; j <= 2; j++) {
        Matrix.ModifyCellToRef(mat3x2, <Dim>i, <Dim>j, (i - 1) * 2 + (j - 1));
    }
}
for (let i: Dim = 1; i <= 2; i++) {
    for (let j: Dim = 1; j <= 3; j++) {
        Matrix.ModifyCellToRef(mat2x3, <Dim>i, <Dim>j, (i - 1) * 3 + (j - 1));
    }
}
display(mat3x2.toFormatString(undefined, undefined, '<br>'));
breakRow();
display(mat2x3.toFormatString(undefined, undefined, '<br>'));
breakRow();
let mat3x3 = mat3x2.multiply<3>(mat2x3);
display(mat3x3.toFormatString(undefined, undefined, '<br>'));
breakRow();
let mat2x2 = mat2x3.multiply<2>(mat3x2);
display(mat2x2.toFormatString(undefined, undefined, '<br>'));
breakRow();

display('3D 矩阵拆解<br>');
let decomposeTranslation = new Vector3(100, 100, 100);
let decomposeScaling = new Vector3(2, 2, 2);
let decomposeQuaternion = Quaternion.Identity();
let decomposeMatrix4x4 = new Matrix4x4();
Quaternion.EulerAnglesToRef(30 / 180 * Math.PI, 60 / 180 * Math.PI, 90 / 180 * Math.PI, decomposeQuaternion);
coordinate_sys.composeToRef(decomposeScaling, decomposeQuaternion, decomposeTranslation, decomposeMatrix4x4);
display(`Scaling:<br>`);
display(decomposeScaling.toFormatString(10, 4, '<br>'));
display(`Rotation:<br>`);
display(decomposeQuaternion.toFormatString(10, 4, '<br>'));
display(`Translation:<br>`);
display(decomposeTranslation.toFormatString(10, 4, '<br>'));
display(`Composed Matrix4x4:<br>`);
display(decomposeMatrix4x4.toFormatString(10, 4, '<br>'));
coordinate_sys.decompose(decomposeMatrix4x4, decomposeScaling, decomposeQuaternion, decomposeTranslation);
display(`Scaling (Decomposed):<br>`);
display(decomposeScaling.toFormatString(10, 4, '<br>'));
display(`Rotation (Decomposed):<br>`);
display(decomposeQuaternion.toFormatString(10, 4, '<br>'));
display(`Translation (Decomposed):<br>`);
display(decomposeTranslation.toFormatString(10, 4, '<br>'));
breakRow();

display('3D 向量绕轴旋转<br>');
let aixsDirection = new Vector3(1, 0, 0);
let rotateSource = new Vector3(0, 1, 0);
let rotateResult = new Vector3(1, 0, 0);
let rotateAngle = 90;
coordinate_sys.vector3RotateWithAxisAndAngle(aixsDirection, Math.PI * rotateAngle / 180, rotateSource, rotateResult);
display(`旋转轴:<br>`);
display(aixsDirection.toFormatString(undefined, undefined, '<br>'));
display(`旋转源:<br>`);
display(rotateSource.toFormatString(undefined, undefined, '<br>'));
display(`旋转角度: ${rotateAngle} <br>`);
display(`旋转结果:<br>`);
display(rotateResult.toFormatString(undefined, undefined, '<br>'));
aixsDirection.copyFromFloats(-1, 0, 0);
rotateSource.copyFromFloats(1, 1, 0);
rotateResult.copyFromFloats(1, 0, 0);
rotateAngle = 135;
coordinate_sys.vector3RotateWithAxisAndAngle(aixsDirection, Math.PI * rotateAngle / 180, rotateSource, rotateResult);
display(`旋转轴:<br>`);
display(aixsDirection.toFormatString(undefined, undefined, '<br>'));
display(`旋转源:<br>`);
display(rotateSource.toFormatString(undefined, undefined, '<br>'));
display(`旋转角度: ${rotateAngle} <br>`);
display(`旋转结果:<br>`);
display(rotateResult.toFormatString(undefined, undefined, '<br>'));
breakRow();

display('逆矩阵<br>');
let invertTranslation = new Vector3(100, 100, 100);
let invertScaling = new Vector3(2, 2, 2);
let invertQuaternion = Quaternion.Identity();
Quaternion.EulerAnglesToRef(135 / 180 * Math.PI, 0 / 180 * Math.PI, 0 / 180 * Math.PI, invertQuaternion);
let invertSourcePosition = new Vector3(1, 1, 0);
let invertUsedTransformation = new Matrix4x4();
display(`源坐标:<br>`);
display(invertSourcePosition.toFormatString(undefined, undefined, '<br>'));
coordinate_sys.composeToRef(invertScaling, invertQuaternion, invertTranslation, invertUsedTransformation);
coordinate_sys.transformCoordinatesFromFloatsToRef(invertSourcePosition.x, invertSourcePosition.y, invertSourcePosition.z, invertUsedTransformation, invertSourcePosition);
display(`变换矩阵:<br>`);
display(invertUsedTransformation.toFormatString(undefined, undefined, '<br>'));
display(`应用变换后坐标:<br>`);
display(invertSourcePosition.toFormatString(undefined, undefined, '<br>'));
SquareMatrix.InvertToRef(invertUsedTransformation, invertUsedTransformation);
coordinate_sys.transformCoordinatesFromFloatsToRef(invertSourcePosition.x, invertSourcePosition.y, invertSourcePosition.z, invertUsedTransformation, invertSourcePosition);
display(`变换矩阵的逆:<br>`);
display(invertUsedTransformation.toFormatString(undefined, undefined, '<br>'));
display(`应用变换后坐标进行逆变换的坐标:<br>`);
display(invertSourcePosition.toFormatString(undefined, undefined, '<br>'));
breakRow();
