import { Dim, Matrix } from "../matrix";
import { Matrix4x4 } from "../matrix4x4";
import { Vector2 } from "../vector2";
import { Vector3 } from "../vector3";
import { Vector4 } from "../vector4";
import { display } from "../../display/html_display";

let mat1x1 = new Matrix(1, 1);
display(mat1x1.toFormatString(undefined, undefined, '<br>'));
display('-------------------------------<br>');

let mat1x4 = new Matrix(1, 4);
display(mat1x4.toFormatString(undefined, undefined, '<br>'));
display('-------------------------------<br>');

let mat4x2 = new Matrix(4, 2);
display(mat4x2.toFormatString(undefined, undefined, '<br>'));
display('-------------------------------<br>');

display(mat1x4.multiply(mat4x2).toFormatString(undefined, undefined, '<br>'));
display('-------------------------------<br>');

let mat4x4 = new Matrix4x4();
Matrix4x4.IdentifyToRef(mat4x4);
display(mat4x4.toFormatString(undefined, undefined, '<br>'));
display('-------------------------------<br>');

let vector2 = new Vector2(4, 4);
display(vector2.toFormatString(undefined, undefined, '<br>'));
display('-------------------------------<br>');

let vector3 = new Vector3(4, 4);
display(vector3.toFormatString(undefined, undefined, '<br>'));
display('-------------------------------<br>');

Matrix.MultiplyToRef<1, 3, 1>(vector3, vector3.transpose(), mat1x1);
display(mat1x1.toFormatString(undefined, undefined, '<br>'));
display('-------------------------------<br>');

let vector4 = new Vector4(4, 4);
display(vector4.toFormatString(undefined, undefined, '<br>'));
display('-------------------------------<br>');

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
display('-------------------------------<br>');
display(mat2x3.toFormatString(undefined, undefined, '<br>'));
display('-------------------------------<br>');
let mat3x3 = mat3x2.multiply<3>(mat2x3);
display(mat3x3.toFormatString(undefined, undefined, '<br>'));
display('-------------------------------<br>');
let mat2x2 = mat2x3.multiply<2>(mat3x2);
display(mat2x2.toFormatString(undefined, undefined, '<br>'));
display('-------------------------------<br>');
