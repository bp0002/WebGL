/******/ (function(modules) { // webpackBootstrap
/******/ 	// The module cache
/******/ 	var installedModules = {};
/******/
/******/ 	// The require function
/******/ 	function __webpack_require__(moduleId) {
/******/
/******/ 		// Check if module is in cache
/******/ 		if(installedModules[moduleId]) {
/******/ 			return installedModules[moduleId].exports;
/******/ 		}
/******/ 		// Create a new module (and put it into the cache)
/******/ 		var module = installedModules[moduleId] = {
/******/ 			i: moduleId,
/******/ 			l: false,
/******/ 			exports: {}
/******/ 		};
/******/
/******/ 		// Execute the module function
/******/ 		modules[moduleId].call(module.exports, module, module.exports, __webpack_require__);
/******/
/******/ 		// Flag the module as loaded
/******/ 		module.l = true;
/******/
/******/ 		// Return the exports of the module
/******/ 		return module.exports;
/******/ 	}
/******/
/******/
/******/ 	// expose the modules object (__webpack_modules__)
/******/ 	__webpack_require__.m = modules;
/******/
/******/ 	// expose the module cache
/******/ 	__webpack_require__.c = installedModules;
/******/
/******/ 	// define getter function for harmony exports
/******/ 	__webpack_require__.d = function(exports, name, getter) {
/******/ 		if(!__webpack_require__.o(exports, name)) {
/******/ 			Object.defineProperty(exports, name, { enumerable: true, get: getter });
/******/ 		}
/******/ 	};
/******/
/******/ 	// define __esModule on exports
/******/ 	__webpack_require__.r = function(exports) {
/******/ 		if(typeof Symbol !== 'undefined' && Symbol.toStringTag) {
/******/ 			Object.defineProperty(exports, Symbol.toStringTag, { value: 'Module' });
/******/ 		}
/******/ 		Object.defineProperty(exports, '__esModule', { value: true });
/******/ 	};
/******/
/******/ 	// create a fake namespace object
/******/ 	// mode & 1: value is a module id, require it
/******/ 	// mode & 2: merge all properties of value into the ns
/******/ 	// mode & 4: return value when already ns object
/******/ 	// mode & 8|1: behave like require
/******/ 	__webpack_require__.t = function(value, mode) {
/******/ 		if(mode & 1) value = __webpack_require__(value);
/******/ 		if(mode & 8) return value;
/******/ 		if((mode & 4) && typeof value === 'object' && value && value.__esModule) return value;
/******/ 		var ns = Object.create(null);
/******/ 		__webpack_require__.r(ns);
/******/ 		Object.defineProperty(ns, 'default', { enumerable: true, value: value });
/******/ 		if(mode & 2 && typeof value != 'string') for(var key in value) __webpack_require__.d(ns, key, function(key) { return value[key]; }.bind(null, key));
/******/ 		return ns;
/******/ 	};
/******/
/******/ 	// getDefaultExport function for compatibility with non-harmony modules
/******/ 	__webpack_require__.n = function(module) {
/******/ 		var getter = module && module.__esModule ?
/******/ 			function getDefault() { return module['default']; } :
/******/ 			function getModuleExports() { return module; };
/******/ 		__webpack_require__.d(getter, 'a', getter);
/******/ 		return getter;
/******/ 	};
/******/
/******/ 	// Object.prototype.hasOwnProperty.call
/******/ 	__webpack_require__.o = function(object, property) { return Object.prototype.hasOwnProperty.call(object, property); };
/******/
/******/ 	// __webpack_public_path__
/******/ 	__webpack_require__.p = "";
/******/
/******/
/******/ 	// Load entry module and return exports
/******/ 	return __webpack_require__(__webpack_require__.s = "./test/linear_dynamic_system.ts");
/******/ })
/************************************************************************/
/******/ ({

/***/ "./src/display/canvas2d_display.ts":
/*!*****************************************!*\
  !*** ./src/display/canvas2d_display.ts ***!
  \*****************************************/
/*! exports provided: canvas2DDisplay */
/*! exports used: canvas2DDisplay */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
eval("/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, \"a\", function() { return canvas2DDisplay; });\nfunction canvas2DDisplay(ctx, x, y, color) {\r\n    ctx.fillStyle = color;\r\n    ctx.fillRect(x, y, 1, 1);\r\n}\r\n\n\n//# sourceURL=webpack:///./src/display/canvas2d_display.ts?");

/***/ }),

/***/ "./src/display/html_display.ts":
/*!*************************************!*\
  !*** ./src/display/html_display.ts ***!
  \*************************************/
/*! exports provided: display */
/*! exports used: display */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
eval("/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, \"a\", function() { return display; });\nfunction display(str) {\r\n    let node = document.createElement('span');\r\n    node.innerHTML = str;\r\n    document.body.style.whiteSpace = \"pre\";\r\n    document.body.style.fontFamily = \"serif\";\r\n    document.body.appendChild(node);\r\n}\r\n\n\n//# sourceURL=webpack:///./src/display/html_display.ts?");

/***/ }),

/***/ "./src/linear_dynamic_system/markov.ts":
/*!*********************************************!*\
  !*** ./src/linear_dynamic_system/markov.ts ***!
  \*********************************************/
/*! exports provided: MarkovOne */
/*! exports used: MarkovOne */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
eval("/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, \"a\", function() { return MarkovOne; });\n/* harmony import */ var _math_matrix__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! ../math/matrix */ \"./src/math/matrix.ts\");\n/* harmony import */ var _math_vector2__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! ../math/vector2 */ \"./src/math/vector2.ts\");\n\r\n\r\nclass MarkovOne {\r\n    constructor() {\r\n        this.x = 0;\r\n        this.m = 1;\r\n        this.F = 1;\r\n        this.r = 0.2;\r\n        this.h = 0.0;\r\n        this.deltaH = 0.01;\r\n        this.pv = new _math_vector2__WEBPACK_IMPORTED_MODULE_1__[/* Vector2 */ \"a\"](0, 0);\r\n        this.A = new _math_matrix__WEBPACK_IMPORTED_MODULE_0__[/* Matrix */ \"a\"](2, 2);\r\n        this.B = new _math_matrix__WEBPACK_IMPORTED_MODULE_0__[/* Matrix */ \"a\"](2, 1);\r\n    }\r\n    compute(display) {\r\n        this.h += this.deltaH;\r\n        let h = this.h;\r\n        let r = this.r;\r\n        let m = this.m;\r\n        _math_matrix__WEBPACK_IMPORTED_MODULE_0__[/* Matrix */ \"a\"].ModifyCellToRef(this.A, 1, 1, 1);\r\n        _math_matrix__WEBPACK_IMPORTED_MODULE_0__[/* Matrix */ \"a\"].ModifyCellToRef(this.A, 1, 2, h);\r\n        _math_matrix__WEBPACK_IMPORTED_MODULE_0__[/* Matrix */ \"a\"].ModifyCellToRef(this.A, 2, 1, 0);\r\n        _math_matrix__WEBPACK_IMPORTED_MODULE_0__[/* Matrix */ \"a\"].ModifyCellToRef(this.A, 2, 2, 1 - h * r / m);\r\n        _math_matrix__WEBPACK_IMPORTED_MODULE_0__[/* Matrix */ \"a\"].ModifyCellToRef(this.B, 1, 1, 0);\r\n        _math_matrix__WEBPACK_IMPORTED_MODULE_0__[/* Matrix */ \"a\"].ModifyCellToRef(this.B, 2, 1, h / m);\r\n        let result = this.A.multiply(this.pv.transpose()).add(this.B.scale(this.F));\r\n        _math_matrix__WEBPACK_IMPORTED_MODULE_0__[/* Matrix */ \"a\"].ModifyCellToRef(this.pv, 1, 1, result.m[0]);\r\n        _math_matrix__WEBPACK_IMPORTED_MODULE_0__[/* Matrix */ \"a\"].ModifyCellToRef(this.pv, 1, 2, result.m[1]);\r\n        display(result.m[0], result.m[1]);\r\n    }\r\n}\r\n\n\n//# sourceURL=webpack:///./src/linear_dynamic_system/markov.ts?");

/***/ }),

/***/ "./src/math/matrix.ts":
/*!****************************!*\
  !*** ./src/math/matrix.ts ***!
  \****************************/
/*! exports provided: N1, N2, N3, N4, Matrix */
/*! exports used: Matrix */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
eval("/* unused harmony export N1 */\n/* unused harmony export N2 */\n/* unused harmony export N3 */\n/* unused harmony export N4 */\n/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, \"a\", function() { return Matrix; });\n/* harmony import */ var _pool__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! ./pool */ \"./src/math/pool.ts\");\n/* harmony import */ var _scalar__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! ./scalar */ \"./src/math/scalar.ts\");\n\r\n\r\nconst N1 = 1;\r\nconst N2 = 2;\r\nconst N3 = 3;\r\nconst N4 = 4;\r\n/**\r\n * 行主序 存储\r\n * * 0  1  2  3\r\n * * 4  5  6  7\r\n * * 8  9 10 11\r\n * * ......\r\n */\r\nclass Matrix {\r\n    /**\r\n     * 创建一个矩阵\r\n     * @param row 行数目\r\n     * @param col 列数目\r\n     * @param value 元素值\r\n     */\r\n    constructor(row, col, value = 0) {\r\n        /** @hidden */\r\n        this._isDirty = true;\r\n        this._size = col * row;\r\n        this.row = row;\r\n        this.col = col;\r\n        this._m = Object(_pool__WEBPACK_IMPORTED_MODULE_0__[/* createFloat32Array */ \"a\"])(this._size);\r\n        for (let i = 0; i < this._size; i++) {\r\n            this._m[i] = value;\r\n        }\r\n    }\r\n    get m() { return this._m; }\r\n    isEqual(b) {\r\n        let size = this._m.length;\r\n        let result = true;\r\n        for (let i = 0; i < size; i++) {\r\n            if (!_scalar__WEBPACK_IMPORTED_MODULE_1__[/* FloatScalar */ \"a\"].Equal(this._m[i], b._m[i])) {\r\n                result = false;\r\n                break;\r\n            }\r\n        }\r\n        return result;\r\n    }\r\n    dispose() {\r\n        Object(_pool__WEBPACK_IMPORTED_MODULE_0__[/* recycleFloat32Array */ \"b\"])(this._m);\r\n        this._m = undefined;\r\n    }\r\n    /**\r\n     * 获取矩阵数据副本\r\n     * @returns Float32Array\r\n     */\r\n    toArray() {\r\n        return new Float32Array(this._m.buffer.slice(0));\r\n    }\r\n    /**\r\n     * 获取矩阵格式化显示字符串\r\n     * @param length 每个元素显示时的字符总长度\r\n     * @param decimalLenght 小数部分显示字符数目\r\n     * @returns 格式化字符串\r\n     */\r\n    toFormatString(length = 12, decimalLenght = 4, breakString = '\\r\\n') {\r\n        let result = '';\r\n        for (let j = 0; j < this.row; j++) {\r\n            for (let i = 0; i < this.col; i++) {\r\n                let sourceIndex = j * this.col + i;\r\n                result += _scalar__WEBPACK_IMPORTED_MODULE_1__[/* FloatScalar */ \"a\"].FormatString(this._m[sourceIndex], length, decimalLenght) + ',';\r\n            }\r\n            result += breakString;\r\n        }\r\n        return result;\r\n    }\r\n    /**\r\n     * 行主序存储的索引计算\r\n     * @param rowIndex 行序号\r\n     * @param colIndex 列序号\r\n     * @param row 行数目\r\n     * @param col 列数目\r\n     * @returns 数组索引\r\n     */\r\n    static RowMajorOrder(rowIndex, colIndex, row, col) {\r\n        return (rowIndex - 1) * col + (colIndex - 1);\r\n    }\r\n    /**\r\n     * 列主序存储的索引计算\r\n     * @param rowIndex 行序号\r\n     * @param colIndex 列序号\r\n     * @param row 行数目\r\n     * @param col 列数目\r\n     * @returns 数组索引\r\n     */\r\n    static ColMajorOrder(rowIndex, colIndex, row, col) {\r\n        return (rowIndex - 1) + (colIndex - 1) * row;\r\n    }\r\n    /**\r\n     * 获取矩阵指定 列 的子矩阵\r\n     * @param source source matrix\r\n     * @param colIndex index of column\r\n     * @param result result matrix\r\n     */\r\n    static GetColumn(source, colIndex, result) {\r\n        let rowCount = source.row;\r\n        for (let i = 0; i < rowCount; i++) {\r\n            result._m[i] = source._m[i * source.col + colIndex];\r\n        }\r\n    }\r\n    /**\r\n     * 获取矩阵指定 行 的子矩阵\r\n     * @param source source matrix\r\n     * @param rowIndex index of row\r\n     * @param result result matrix\r\n     */\r\n    static GetRow(source, rowIndex, result) {\r\n        let colCount = source.col;\r\n        for (let i = 0; i < colCount; i++) {\r\n            result._m[i] = source._m[rowIndex * source.col + i];\r\n        }\r\n    }\r\n    /**\r\n     * 创建元素全为 0 的矩阵\r\n     * @param row 行数目\r\n     * @param col 列数目\r\n     * @return Zero Matrix\r\n     */\r\n    static Zero(row, col) {\r\n        let result = new Matrix(row, col, 0.);\r\n        return result;\r\n    }\r\n    /**\r\n     * 修改元素为 指定数值\r\n     * @param target target matrix\r\n     * @param value value number\r\n     */\r\n    static ModifyToRef(target, value) {\r\n        for (let i = 0; i < target._size; i++) {\r\n            target._m[i] = value;\r\n        }\r\n        target._isDirty = true;\r\n    }\r\n    /**\r\n     * 修改矩阵指定元素为 指定数值 - 行主序\r\n     * @param target target matrix\r\n     * @param rowIndex index of row\r\n     * @param colIndex index of col\r\n     * @param value value number\r\n     */\r\n    static ModifyCellToRef(target, rowIndex, colIndex, value) {\r\n        if (rowIndex <= target.row && colIndex <= target.col) {\r\n            target._m[Matrix.RowMajorOrder(rowIndex, colIndex, target.row, target.col)] = value;\r\n            target._isDirty = true;\r\n        }\r\n        else {\r\n            throw new Error(`Matrix ModifyToRef Error: rowIndex or colIndex is out of bound.`);\r\n        }\r\n    }\r\n    /**\r\n     * 创建元素全为 1 的矩阵\r\n     * @param row 行数目\r\n     * @param col 列数目\r\n     * @return One Matrix\r\n     */\r\n    static One(row, col) {\r\n        let result = new Matrix(row, col, 1.);\r\n        return result;\r\n    }\r\n    /**\r\n     * 矩阵复制\r\n     * @returns result matrix\r\n     */\r\n    clone() {\r\n        let result = new Matrix(this.row, this.col, 0.);\r\n        Matrix.CopyTo(this, result);\r\n        return result;\r\n    }\r\n    /**\r\n     * 矩阵复制\r\n     * @param source source matrix\r\n     * @param result result matrix\r\n     */\r\n    static CopyTo(source, result) {\r\n        for (let i = 0; i < result._size; i++) {\r\n            result._m[i] = source._m[i];\r\n        }\r\n        result._isDirty = true;\r\n    }\r\n    /**\r\n     * 检查矩阵行列是否相同\r\n     * @param left left matrix\r\n     * @param right right matrix\r\n     * @returns bool\r\n     */\r\n    static SameColRow(left, right) {\r\n        return left.col === right.col && left.row === right.row;\r\n    }\r\n    /**\r\n     * 矩阵加法\r\n     * @param right right matrix\r\n     * @returns result matrix\r\n     */\r\n    add(right) {\r\n        let result = new Matrix(this.row, this.col, 0.);\r\n        Matrix.AddToRef(this, right, result);\r\n        return result;\r\n    }\r\n    /**\r\n     * 矩阵加法\r\n     * @param left left matrix\r\n     * @param right right matrix\r\n     * @param result result matrix\r\n     */\r\n    static AddToRef(left, right, result) {\r\n        let size = left._size;\r\n        for (let i = 0; i < size; i++) {\r\n            result._m[i] = left._m[i] + right._m[i];\r\n        }\r\n        result._isDirty = true;\r\n    }\r\n    /**\r\n     * 矩阵减法\r\n     * @param right right matrix\r\n     * @returns result matrix\r\n     */\r\n    substract(right) {\r\n        let result = new Matrix(this.row, this.col);\r\n        Matrix.SubstractToRef(this, right, result);\r\n        return result;\r\n    }\r\n    /**\r\n     * 矩阵减法\r\n     * @param left left matrix\r\n     * @param right right matrix\r\n     * @param result result matrix\r\n     */\r\n    static SubstractToRef(left, right, result) {\r\n        let size = left._size;\r\n        for (let i = 0; i < size; i++) {\r\n            result._m[i] = left._m[i] - right._m[i];\r\n        }\r\n        result._isDirty = true;\r\n    }\r\n    /**\r\n     * 标量乘法\r\n     * @param scalar scale value\r\n     * @returns new matrix\r\n     */\r\n    scale(scalar) {\r\n        let result = new Matrix(this.row, this.col);\r\n        Matrix.ScaleToRef(this, scalar, result);\r\n        return result;\r\n    }\r\n    /**\r\n     * 标量乘法\r\n     * @param left target matrix\r\n     * @param scalar scale value\r\n     * @param result result matrix\r\n     */\r\n    static ScaleToRef(left, scalar, result) {\r\n        let size = left._size;\r\n        for (let i = 0; i < size; i++) {\r\n            result._m[i] = left._m[i] * scalar;\r\n        }\r\n        result._isDirty = true;\r\n    }\r\n    /**\r\n     * 转置\r\n     */\r\n    transpose() {\r\n        let result = new Matrix(this.col, this.row);\r\n        Matrix.TransposeToRef(this, result);\r\n        return result;\r\n    }\r\n    /**\r\n     * 转置: 只能对行列匹配的矩阵使用\r\n     * @param source source matrix\r\n     * @param result Transpose result\r\n     * @tip T(T(A))     = A\r\n     * @tip T(xA)       = xT(A)\r\n     * @tip T(A + B)    = T(A) + T(B)\r\n     * @tip T(AB)       = T(B)T(A)\r\n     */\r\n    static TransposeToRef(source, result) {\r\n        let temp = source.toArray();\r\n        for (let j = 0; j < source.row; j++) {\r\n            for (let i = 0; i < source.col; i++) {\r\n                let sourceIndex = j * source.col + i;\r\n                let resultIndex = i * result.col + j;\r\n                result._m[resultIndex] = temp[sourceIndex];\r\n            }\r\n        }\r\n        result._isDirty = true;\r\n    }\r\n    /**\r\n     * 矩阵乘法 Left * Right = Result\r\n     * @param right right matrix\r\n     * @returns result\r\n     */\r\n    multiply(right) {\r\n        let resultRow = this.row;\r\n        let resultCol = right.col;\r\n        let result = new Matrix(resultRow, resultCol);\r\n        Matrix.MultiplyToRef(this, right, result);\r\n        return result;\r\n    }\r\n    /**\r\n     * 矩阵乘法 Left * Right = Result\r\n     * @param left left matrix\r\n     * @param right right matrix\r\n     * @param result result matrix\r\n     * @tip AB !== BA; eg: A: Matrix<3, 2>, B: Matrix<2, 3>\r\n     * @tip result.row = left.row\r\n     * @tip result.col = right.col\r\n     * @tip compute number of calculate iteration == left.col == right.row\r\n     */\r\n    static MultiplyToRef(left, right, result) {\r\n        // if (left.row === left.col && left.isIdentity()) {\r\n        //     Matrix.CopyFrom(right, result);\r\n        // } else if (right.row === right.col && right.isIdentity()) {\r\n        //     Matrix.CopyFrom(left, result);\r\n        // } else {\r\n        let size = left.m.length;\r\n        let temp = new Float32Array(size);\r\n        let tempLength = left.col;\r\n        let resultRow = left.row;\r\n        let resultCol = right.col;\r\n        let tempLeftRow = new Matrix(tempLength, 1);\r\n        let tempRightCol = new Matrix(tempLength, 1);\r\n        let tempCount = tempLength;\r\n        for (let i = 0; i < resultRow; i++) {\r\n            for (let j = 0; j < resultCol; j++) {\r\n                let resultIndex = i * resultCol + j;\r\n                Matrix.GetRow(left, i, tempLeftRow);\r\n                Matrix.GetColumn(right, j, tempRightCol);\r\n                temp[resultIndex] = 0;\r\n                for (let k = 0; k < tempCount; k++) {\r\n                    temp[resultIndex] += tempLeftRow._m[k] * tempRightCol._m[k];\r\n                }\r\n            }\r\n        }\r\n        for (let i = 0; i < size; i++) {\r\n            result._m[i] = temp[i];\r\n        }\r\n        result._isDirty = true;\r\n        // }\r\n    }\r\n    // public static Dot(right: Matrix) {\r\n    // }\r\n    // public static Cross(right: Matrix, result: Matrix) {\r\n    // }\r\n    /**\r\n     * Sum Of Quared Residuals 残差平方和\r\n     * @param left left matrix\r\n     * @param right right matrix\r\n     */\r\n    static SumOfQuaredResiduals(left, right) {\r\n        let len = left._size;\r\n        let result = 0;\r\n        let tempDiff = 0;\r\n        for (let i = 0; i < len; i++) {\r\n            tempDiff = left._m[i] - right._m[i];\r\n            result += tempDiff * tempDiff;\r\n        }\r\n        return result;\r\n    }\r\n    /**\r\n     * 各项平方和\r\n     * @param left matrix\r\n     * @returns Sum Of Quared\r\n     */\r\n    static SumOfQuared(left) {\r\n        let len = left._size;\r\n        let result = 0;\r\n        let tempDiff = 0;\r\n        for (let i = 0; i < len; i++) {\r\n            tempDiff = left._m[i];\r\n            result += tempDiff * tempDiff;\r\n        }\r\n        return result;\r\n    }\r\n    /**\r\n     * 归一化各项 - 如果为 0 矩阵, 则返回第一项为 1 的结果\r\n     * @param left matrix\r\n     * @param result matrix\r\n     */\r\n    static NormalizeToRef(left, result) {\r\n        let len = left._size;\r\n        let length = Math.sqrt(Matrix.SumOfQuared(left));\r\n        if (length == 0) {\r\n            result._m[0] = 1;\r\n            for (let i = 1; i < len; i++) {\r\n                result._m[i] = 0;\r\n            }\r\n        }\r\n        else {\r\n            for (let i = 0; i < len; i++) {\r\n                result._m[i] = left._m[i] / length;\r\n            }\r\n        }\r\n        result._isDirty = true;\r\n    }\r\n    static FractToRef(target, result) {\r\n        let len = target._size;\r\n        for (let i = 0; i < len; i++) {\r\n            result._m[i] = target._m[i] % 1;\r\n        }\r\n        result._isDirty = true;\r\n    }\r\n    static FloorToRef(target, result) {\r\n        let len = target._size;\r\n        for (let i = 0; i < len; i++) {\r\n            result._m[i] = Math.floor(target._m[i]);\r\n        }\r\n        result._isDirty = true;\r\n    }\r\n    static CeilToRef(target, result) {\r\n        let len = target._size;\r\n        for (let i = 0; i < len; i++) {\r\n            result._m[i] = Math.ceil(target._m[i]);\r\n        }\r\n        result._isDirty = true;\r\n    }\r\n    static RoundToRef(target, result) {\r\n        let len = target._size;\r\n        for (let i = 0; i < len; i++) {\r\n            result._m[i] = Math.round(target._m[i]);\r\n        }\r\n        result._isDirty = true;\r\n    }\r\n    /**\r\n     * 在两个矩阵间 线性 插值\r\n     * @param from 起始矩阵\r\n     * @param to 结束矩阵\r\n     * @param amount 插值因子\r\n     * @param result 结果矩阵\r\n     */\r\n    static LerpToRef(from, to, amount, result) {\r\n        let len = from._size;\r\n        let tempDiff = 0;\r\n        for (let i = 0; i < len; i++) {\r\n            tempDiff = to._m[i] - from._m[i];\r\n            result._m[i] = from._m[i] + tempDiff * amount;\r\n        }\r\n        result._isDirty = true;\r\n    }\r\n    /**\r\n     * divide every item\r\n     * @param left Matrix\r\n     * @param right Matrix\r\n     * @param result Matrix\r\n     */\r\n    static DivideItemsToRef(left, right, result) {\r\n        let len = left._size;\r\n        for (let i = 0; i < len; i++) {\r\n            result._m[i] = left._m[i] / right._m[i];\r\n        }\r\n        result._isDirty = true;\r\n    }\r\n    /**\r\n     * 在两个矩阵间 Hermite 插值\r\n     * @param value1 起始数值矩阵\r\n     * @param tangent1 起始 tangent 数值\r\n     * @param value2 结束数值矩阵\r\n     * @param tangent2 结束 tangent 数值\r\n     * @param amount 插值因子\r\n     * @param result 结果矩阵\r\n     */\r\n    static Hermite(value1, tangent1, value2, tangent2, amount, result) {\r\n        let squared = amount * amount;\r\n        let cubed = amount * squared;\r\n        let part1 = ((2.0 * cubed) - (3.0 * squared)) + 1.0;\r\n        let part2 = (-2.0 * cubed) + (3.0 * squared);\r\n        let part3 = (cubed - (2.0 * squared)) + amount;\r\n        let part4 = cubed - squared;\r\n        let len = value1._size;\r\n        for (let i = 0; i < len; i++) {\r\n            result._m[i] = (((value1._m[i] * part1) + (value2._m[i] * part2)) + (tangent1._m[i] * part3)) + (tangent2._m[i] * part4);\r\n        }\r\n        result._isDirty = true;\r\n    }\r\n}\r\n\n\n//# sourceURL=webpack:///./src/math/matrix.ts?");

/***/ }),

/***/ "./src/math/pool.ts":
/*!**************************!*\
  !*** ./src/math/pool.ts ***!
  \**************************/
/*! exports provided: createFloat32Array, recycleFloat32Array */
/*! exports used: createFloat32Array, recycleFloat32Array */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
eval("/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, \"a\", function() { return createFloat32Array; });\n/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, \"b\", function() { return recycleFloat32Array; });\nconst float32arrayPool = [];\r\nconst float32arrayPool3 = [];\r\nconst float32arrayPool4 = [];\r\nconst float32arrayPool16 = [];\r\nfunction createFloat32Array(size) {\r\n    let result;\r\n    if (size == 3) {\r\n        result = float32arrayPool3.pop();\r\n    }\r\n    else if (size == 4) {\r\n        result = float32arrayPool4.pop();\r\n    }\r\n    else if (size == 16) {\r\n        result = float32arrayPool16.pop();\r\n    }\r\n    else {\r\n        let len = float32arrayPool.length;\r\n        if (len > 0) {\r\n            let index = -1;\r\n            for (let i = len - 1; i >= 0; i--) {\r\n                if (float32arrayPool[i].length == size) {\r\n                    index = i;\r\n                    result = float32arrayPool[i];\r\n                    break;\r\n                }\r\n            }\r\n            if (len > 1) {\r\n                float32arrayPool[index] = float32arrayPool[len - 1];\r\n            }\r\n            float32arrayPool.length = len - 1;\r\n        }\r\n    }\r\n    if (!result) {\r\n        result = new Float32Array(size);\r\n    }\r\n    return result;\r\n}\r\nfunction recycleFloat32Array(data) {\r\n    let len = data.length;\r\n    if (len == 3) {\r\n        float32arrayPool3.push(data);\r\n    }\r\n    else if (len == 4) {\r\n        float32arrayPool4.push(data);\r\n    }\r\n    else if (len == 16) {\r\n        float32arrayPool16.push(data);\r\n    }\r\n    else {\r\n        float32arrayPool.push(data);\r\n    }\r\n}\r\n\n\n//# sourceURL=webpack:///./src/math/pool.ts?");

/***/ }),

/***/ "./src/math/row.ts":
/*!*************************!*\
  !*** ./src/math/row.ts ***!
  \*************************/
/*! exports provided: Row */
/*! exports used: Row */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
eval("/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, \"a\", function() { return Row; });\n/* harmony import */ var _matrix__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! ./matrix */ \"./src/math/matrix.ts\");\n\r\nclass Row extends _matrix__WEBPACK_IMPORTED_MODULE_0__[/* Matrix */ \"a\"] {\r\n    constructor(col) {\r\n        super(1, col);\r\n    }\r\n    /**\r\n     * 向量点积 (内积 inner product) v ● w\r\n     * @tip 如果 v,w 均为单位向量 v ● w = cosΘ, Θ = arccos((v ● w)/(║v║║w║))\r\n     * @tip w 在 单位向量 v 上的投影 v' = (v ● w)v\r\n     * @param left v\r\n     * @param right w\r\n     * @returns v ● w\r\n     */\r\n    static Dot(left, right) {\r\n        let result = 0;\r\n        for (let i = 0; i < left.col; i++) {\r\n            result += left._m[i] * right._m[i];\r\n        }\r\n        return result;\r\n    }\r\n    static DistanceSquared(start, end) {\r\n        return Row.SumOfQuaredResiduals(end, start);\r\n    }\r\n    static Distance(start, end) {\r\n        return Math.sqrt(Row.SumOfQuaredResiduals(end, start));\r\n    }\r\n    static Length(target) {\r\n        return Math.sqrt(Row.SumOfQuared(target));\r\n    }\r\n}\r\n\n\n//# sourceURL=webpack:///./src/math/row.ts?");

/***/ }),

/***/ "./src/math/scalar.ts":
/*!****************************!*\
  !*** ./src/math/scalar.ts ***!
  \****************************/
/*! exports provided: FloatScalar */
/*! exports used: FloatScalar */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
eval("/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, \"a\", function() { return FloatScalar; });\nclass FloatScalar {\r\n    static isNaN(value) {\r\n        return isNaN(value);\r\n    }\r\n    /**\r\n     * Boolean: 检查两个数值是否相等\r\n     * @param a left value\r\n     * @param b right value\r\n     * @returns a == b\r\n     */\r\n    static Equal(a, b) {\r\n        return a == b;\r\n    }\r\n    /**\r\n     * Boolean: 检查两个数值是否在指定误差范围内相等\r\n     * @param a left value\r\n     * @param b right value\r\n     * @param epsilon (default = 1.401298E-45)\r\n     * @returns if the absolute diffrence between a and b is lower than epsilon\r\n     */\r\n    static EqualWithEpsilon(a, b, epsilon = 1.401298E-45) {\r\n        let diff = a - b;\r\n        return -epsilon < diff && diff < epsilon;\r\n    }\r\n    /**\r\n     * Returens -1 if value is negative, returns +1 if value is positive, or reuturn 0\r\n     * @param value the value\r\n     */\r\n    static Sign(value) {\r\n        value = +value;\r\n        if (value === 0 || this.isNaN(value)) {\r\n            return 0;\r\n        }\r\n        return value > 0 ? 1 : -1;\r\n    }\r\n    /**\r\n     * Returns the value it self if it's between minimum and maximum\r\n     * Returns minimum if the value is lower than minimum\r\n     * Returns maximum if the value is greater than maximum\r\n     * @param value the value to clamp\r\n     * @param minimum the minimum (default 0)\r\n     * @param maximum the maximum (default 1)\r\n     * @returns the clamped value\r\n     */\r\n    static Clamp(value, minimum = 0, maximum = 1) {\r\n        return Math.min(maximum, Math.max(minimum, value));\r\n    }\r\n    /**\r\n     * Normalized the value between 0.0 and 1.0 using min and max value\r\n     * @param value the value\r\n     * @param min min to normalize between\r\n     * @param max max to normalize between\r\n     * @returns the normalize value\r\n     */\r\n    static Normalize(value, min, max) {\r\n        if (max != min) {\r\n            return this.Clamp((value - min) / (max - min));\r\n        }\r\n        else {\r\n            return 0.;\r\n        }\r\n    }\r\n    /**\r\n     * Denormalize the value between 0.0 and 1.0 using min and max value\r\n     * @param normalised the normalised value\r\n     * @param min min to normalize between\r\n     * @param max max to normalize between\r\n     * @returns the normalize value\r\n     */\r\n    static Denormalize(normalised, min, max) {\r\n        return normalised * (max - min) + min;\r\n    }\r\n    /**\r\n     * Detransform the value to new coordinate using min and max value\r\n     * @param transformed the transformed value\r\n     * @param min min to transform between\r\n     * @param max max to transform between\r\n     * @returns the transform value\r\n     */\r\n    static Detransform(transformed, min, max) {\r\n        return min * (1.0 - transformed) + max * transformed;\r\n    }\r\n    /**\r\n     * Transform the value to new coordinate using min and max value\r\n     * @param value the value\r\n     * @param min min to transform between\r\n     * @param max max to transform between\r\n     * @returns the transform value\r\n     */\r\n    static Transform(value, min, max) {\r\n        if (max != min) {\r\n            return (value - min) / (max - min);\r\n        }\r\n        else {\r\n            return 0.;\r\n        }\r\n    }\r\n    /**\r\n     * Diffrent with modulo operator\r\n     * Repeat(-5, 3) = 1\r\n     * -Infinity to Infinity\r\n     * -5 mod 3 = -2\r\n     * 0 to -Infinity or Infinity\r\n     * @param value the value\r\n     * @param length step length\r\n     */\r\n    static Repeat(value, length) {\r\n        return value - Math.floor(value / length) * length;\r\n    }\r\n    /**\r\n     * PingPongs the value t\r\n     * @param value the value\r\n     * @param length the step length\r\n     */\r\n    static PingPong(value, length) {\r\n        let t = FloatScalar.Repeat(value, length * 2.0);\r\n        return length - Math.abs(t - length);\r\n    }\r\n    /**\r\n     * Calculate the shortest diffrence between two angles in degrees.\r\n     * @param current current degrees\r\n     * @param target target degrees\r\n     * @returns delta degrees\r\n     */\r\n    static DeltaAngle(current, target) {\r\n        let num = FloatScalar.Repeat(target - current, 360.0);\r\n        if (num > 180.0) {\r\n            num -= 360.0;\r\n        }\r\n        return num;\r\n    }\r\n    static Step(x1, x2) {\r\n        return x1 <= x2 ? 1. : 0.;\r\n    }\r\n    static SmoothStep(from, to, tx) {\r\n        let t = FloatScalar.Clamp(tx, 0., 1.);\r\n        let tt = t * t;\r\n        t = -2.0 * tt * t + 3.0 * tt;\r\n        return FloatScalar.Detransform(t, from, to);\r\n    }\r\n    static Lerp(from, to, tx) {\r\n        return FloatScalar.Detransform(tx, from, to);\r\n    }\r\n    /**\r\n     * Hermite interplate\r\n     * @param value1 spline value\r\n     * @param tangent1 spline tangent\r\n     * @param value2 spline value\r\n     * @param tangent2 spline tangent\r\n     * @param amount amount\r\n     */\r\n    static Hermite(value1, tangent1, value2, tangent2, amount) {\r\n        let squared = amount * amount;\r\n        let cubed = squared * amount;\r\n        let part1 = ((2.0 * cubed) - (3.0 * squared)) + 1.0;\r\n        let part2 = (-2.0 * cubed) + (3.0 * squared);\r\n        let part3 = (cubed - (2.0 * squared)) + amount;\r\n        let part4 = cubed - squared;\r\n        return value1 * part1 + value2 * part2 + tangent1 * part3 + tangent2 * part4;\r\n    }\r\n    static RandomRange(min, max) {\r\n        if (min == max) {\r\n            return min;\r\n        }\r\n        else {\r\n            return FloatScalar.Lerp(min, max, Math.random());\r\n        }\r\n    }\r\n    static FormatString(value, length, decimalLenght, char = \" \") {\r\n        let result = value.toFixed(decimalLenght);\r\n        let tempLength = result.length;\r\n        for (let i = length - tempLength; i >= 0; i--) {\r\n            result = char + result;\r\n        }\r\n        return result;\r\n    }\r\n}\r\nFloatScalar.Pi = Math.PI;\r\nFloatScalar.TwoPi = Math.PI * 2;\r\n\n\n//# sourceURL=webpack:///./src/math/scalar.ts?");

/***/ }),

/***/ "./src/math/vector2.ts":
/*!*****************************!*\
  !*** ./src/math/vector2.ts ***!
  \*****************************/
/*! exports provided: Vector2 */
/*! exports used: Vector2 */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
eval("/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, \"a\", function() { return Vector2; });\n/* harmony import */ var _row__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! ./row */ \"./src/math/row.ts\");\n/* harmony import */ var _scalar__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! ./scalar */ \"./src/math/scalar.ts\");\n\r\n\r\nclass Vector2 extends _row__WEBPACK_IMPORTED_MODULE_0__[/* Row */ \"a\"] {\r\n    /** @hidden */\r\n    get x() {\r\n        return this._m[0];\r\n    }\r\n    set x(value) {\r\n        if (!_scalar__WEBPACK_IMPORTED_MODULE_1__[/* FloatScalar */ \"a\"].Equal(this._m[0], value)) {\r\n            this._m[0] = value;\r\n            this._isDirty = true;\r\n        }\r\n    }\r\n    /** @hidden */\r\n    get y() {\r\n        return this._m[1];\r\n    }\r\n    set y(value) {\r\n        if (!_scalar__WEBPACK_IMPORTED_MODULE_1__[/* FloatScalar */ \"a\"].Equal(this._m[1], value)) {\r\n            this._m[1] = value;\r\n            this._isDirty = true;\r\n        }\r\n    }\r\n    constructor(x = 0, y = 0) {\r\n        super(2);\r\n        this._m[0] = x;\r\n        this._m[1] = y;\r\n    }\r\n    static Cross(v) {\r\n        let result = new Vector2();\r\n        Vector2.CrossToRef(v, result);\r\n        return result;\r\n    }\r\n    static CrossToRef(v, result) {\r\n        let x = v.x;\r\n        let y = v.y;\r\n        result.x = -y;\r\n        result.y = x;\r\n    }\r\n    static FF(v, b) {\r\n        return v.x * b.y - b.x * v.y;\r\n    }\r\n}\r\n\n\n//# sourceURL=webpack:///./src/math/vector2.ts?");

/***/ }),

/***/ "./test/linear_dynamic_system.ts":
/*!***************************************!*\
  !*** ./test/linear_dynamic_system.ts ***!
  \***************************************/
/*! no exports provided */
/*! all exports used */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
eval("__webpack_require__.r(__webpack_exports__);\n/* harmony import */ var _src_linear_dynamic_system_markov__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! ../src/linear_dynamic_system/markov */ \"./src/linear_dynamic_system/markov.ts\");\n/* harmony import */ var _src_display_canvas2d_display__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! ../src/display/canvas2d_display */ \"./src/display/canvas2d_display.ts\");\n/* harmony import */ var _src_math_scalar__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! ../src/math/scalar */ \"./src/math/scalar.ts\");\n/* harmony import */ var _src_display_html_display__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! ../src/display/html_display */ \"./src/display/html_display.ts\");\n\r\n\r\n\r\n\r\nfunction breakRow() {\r\n    Object(_src_display_html_display__WEBPACK_IMPORTED_MODULE_3__[/* display */ \"a\"])('--------------------------------------------------------------<br>');\r\n}\r\nlet markov = new _src_linear_dynamic_system_markov__WEBPACK_IMPORTED_MODULE_0__[/* MarkovOne */ \"a\"]();\r\nmarkov.F = 0.001;\r\nmarkov.r = -0.001;\r\nmarkov.deltaH = 0.01;\r\nlet markovResult = [];\r\nlet canvas = document.createElement('canvas');\r\ndocument.body.appendChild(canvas);\r\ncanvas.width = 1000;\r\ncanvas.height = 500;\r\nlet ctx = canvas.getContext('2d');\r\nlet tempDisplay = (p, v) => {\r\n    markovResult.push([p, v]);\r\n    Object(_src_display_canvas2d_display__WEBPACK_IMPORTED_MODULE_1__[/* canvas2DDisplay */ \"a\"])(ctx, markovResult.length * markov.deltaH * 50, 500 - (p / 10000) * 500, '#f00');\r\n    Object(_src_display_canvas2d_display__WEBPACK_IMPORTED_MODULE_1__[/* canvas2DDisplay */ \"a\"])(ctx, 500 + markovResult.length * markov.deltaH * 50, 500 - (v / 100) * 500, '#0f0');\r\n};\r\nfor (let i = 0; i < 1000; i++) {\r\n    markov.compute(tempDisplay);\r\n}\r\nObject(_src_display_html_display__WEBPACK_IMPORTED_MODULE_3__[/* display */ \"a\"])(`<br>`);\r\nfor (let i = 0; i < 1000; i++) {\r\n    Object(_src_display_html_display__WEBPACK_IMPORTED_MODULE_3__[/* display */ \"a\"])(_src_math_scalar__WEBPACK_IMPORTED_MODULE_2__[/* FloatScalar */ \"a\"].FormatString(i * markov.deltaH, 8, 2, \"-\") + \", \" + _src_math_scalar__WEBPACK_IMPORTED_MODULE_2__[/* FloatScalar */ \"a\"].FormatString(markovResult[i][0], 28, 6, \"-\") + \", \" + _src_math_scalar__WEBPACK_IMPORTED_MODULE_2__[/* FloatScalar */ \"a\"].FormatString(markovResult[i][1], 28, 6, \"-\") + '<br>');\r\n}\r\nbreakRow();\r\n\n\n//# sourceURL=webpack:///./test/linear_dynamic_system.ts?");

/***/ })

/******/ });