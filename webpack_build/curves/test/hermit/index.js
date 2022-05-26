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
/******/ 	return __webpack_require__(__webpack_require__.s = "./src/curves/test/hermit.ts");
/******/ })
/************************************************************************/
/******/ ({

/***/ "./src/curves/test/hermit.ts":
/*!***********************************!*\
  !*** ./src/curves/test/hermit.ts ***!
  \***********************************/
/*! no exports provided */
/*! all exports used */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
eval("__webpack_require__.r(__webpack_exports__);\n/* harmony import */ var _display_canvas2d_display__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! ../../display/canvas2d_display */ \"./src/display/canvas2d_display.ts\");\n/* harmony import */ var _math_scalar__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! ../../math/scalar */ \"./src/math/scalar.ts\");\n\r\n\r\n// Hermit\r\nlet v0 = 0, t0 = 8.728395, f0 = 0.0;\r\nlet v1 = 1, t1 = 0, f1 = 0.2517014;\r\nfunction hermiteTest(x) {\r\n    return _math_scalar__WEBPACK_IMPORTED_MODULE_1__[/* FloatScalar */ \"a\"].Hermite(v0, t0 * (f1 - f0), v1, t1 * (f1 - f0), x);\r\n}\r\nlet canvas = document.createElement('canvas');\r\ndocument.body.appendChild(canvas);\r\ncanvas.width = 500;\r\ncanvas.height = 1000;\r\nlet ctx = canvas.getContext('2d');\r\nctx.fillStyle = '#0f0';\r\nctx.fillRect(0, canvas.height - 500, canvas.width, 1);\r\nfor (let i = 0; i < 500; i++) {\r\n    let x = i / 500;\r\n    let v = hermiteTest(x);\r\n    Object(_display_canvas2d_display__WEBPACK_IMPORTED_MODULE_0__[/* canvas2DDisplay */ \"a\"])(ctx, x * canvas.width, canvas.height - v * 500, '#f00');\r\n}\r\n\n\n//# sourceURL=webpack:///./src/curves/test/hermit.ts?");

/***/ }),

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

/***/ "./src/math/scalar.ts":
/*!****************************!*\
  !*** ./src/math/scalar.ts ***!
  \****************************/
/*! exports provided: FloatScalar */
/*! exports used: FloatScalar */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
eval("/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, \"a\", function() { return FloatScalar; });\nclass FloatScalar {\r\n    static isNaN(value) {\r\n        return isNaN(value);\r\n    }\r\n    /**\r\n     * Boolean: 检查两个数值是否相等\r\n     * @param a left value\r\n     * @param b right value\r\n     * @returns a == b\r\n     */\r\n    static Equal(a, b) {\r\n        return a == b;\r\n    }\r\n    /**\r\n     * Boolean: 检查两个数值是否在指定误差范围内相等\r\n     * @param a left value\r\n     * @param b right value\r\n     * @param epsilon (default = 1.401298E-45)\r\n     * @returns if the absolute diffrence between a and b is lower than epsilon\r\n     */\r\n    static EqualWithEpsilon(a, b, epsilon = 1.401298E-45) {\r\n        let diff = a - b;\r\n        return -epsilon < diff && diff < epsilon;\r\n    }\r\n    /**\r\n     * Returens -1 if value is negative, returns +1 if value is positive, or reuturn 0\r\n     * @param value the value\r\n     */\r\n    static Sign(value) {\r\n        value = +value;\r\n        if (value === 0 || this.isNaN(value)) {\r\n            return 0;\r\n        }\r\n        return value > 0 ? 1 : -1;\r\n    }\r\n    /**\r\n     * Returns the value it self if it's between minimum and maximum\r\n     * Returns minimum if the value is lower than minimum\r\n     * Returns maximum if the value is greater than maximum\r\n     * @param value the value to clamp\r\n     * @param minimum the minimum (default 0)\r\n     * @param maximum the maximum (default 1)\r\n     * @returns the clamped value\r\n     */\r\n    static Clamp(value, minimum = 0, maximum = 1) {\r\n        return Math.min(maximum, Math.max(minimum, value));\r\n    }\r\n    /**\r\n     * Normalized the value between 0.0 and 1.0 using min and max value\r\n     * @param value the value\r\n     * @param min min to normalize between\r\n     * @param max max to normalize between\r\n     * @returns the normalize value\r\n     */\r\n    static Normalize(value, min, max) {\r\n        if (max != min) {\r\n            return this.Clamp((value - min) / (max - min));\r\n        }\r\n        else {\r\n            return 0.;\r\n        }\r\n    }\r\n    /**\r\n     * Denormalize the value between 0.0 and 1.0 using min and max value\r\n     * @param normalised the normalised value\r\n     * @param min min to normalize between\r\n     * @param max max to normalize between\r\n     * @returns the normalize value\r\n     */\r\n    static Denormalize(normalised, min, max) {\r\n        return normalised * (max - min) + min;\r\n    }\r\n    /**\r\n     * Detransform the value to new coordinate using min and max value\r\n     * @param transformed the transformed value\r\n     * @param min min to transform between\r\n     * @param max max to transform between\r\n     * @returns the transform value\r\n     */\r\n    static Detransform(transformed, min, max) {\r\n        return min * (1.0 - transformed) + max * transformed;\r\n    }\r\n    /**\r\n     * Transform the value to new coordinate using min and max value\r\n     * @param value the value\r\n     * @param min min to transform between\r\n     * @param max max to transform between\r\n     * @returns the transform value\r\n     */\r\n    static Transform(value, min, max) {\r\n        if (max != min) {\r\n            return (value - min) / (max - min);\r\n        }\r\n        else {\r\n            return 0.;\r\n        }\r\n    }\r\n    /**\r\n     * Diffrent with modulo operator\r\n     * Repeat(-5, 3) = 1\r\n     * -Infinity to Infinity\r\n     * -5 mod 3 = -2\r\n     * 0 to -Infinity or Infinity\r\n     * @param value the value\r\n     * @param length step length\r\n     */\r\n    static Repeat(value, length) {\r\n        return value - Math.floor(value / length) * length;\r\n    }\r\n    /**\r\n     * PingPongs the value t\r\n     * @param value the value\r\n     * @param length the step length\r\n     */\r\n    static PingPong(value, length) {\r\n        let t = FloatScalar.Repeat(value, length * 2.0);\r\n        return length - Math.abs(t - length);\r\n    }\r\n    /**\r\n     * Calculate the shortest diffrence between two angles in degrees.\r\n     * @param current current degrees\r\n     * @param target target degrees\r\n     * @returns delta degrees\r\n     */\r\n    static DeltaAngle(current, target) {\r\n        let num = FloatScalar.Repeat(target - current, 360.0);\r\n        if (num > 180.0) {\r\n            num -= 360.0;\r\n        }\r\n        return num;\r\n    }\r\n    static Step(x1, x2) {\r\n        return x1 <= x2 ? 1. : 0.;\r\n    }\r\n    static SmoothStep(from, to, tx) {\r\n        let t = FloatScalar.Clamp(tx, 0., 1.);\r\n        let tt = t * t;\r\n        t = -2.0 * tt * t + 3.0 * tt;\r\n        return FloatScalar.Detransform(t, from, to);\r\n    }\r\n    static Lerp(from, to, tx) {\r\n        return FloatScalar.Detransform(tx, from, to);\r\n    }\r\n    /**\r\n     * Hermite interplate\r\n     * @param value1 spline value\r\n     * @param tangent1 spline tangent\r\n     * @param value2 spline value\r\n     * @param tangent2 spline tangent\r\n     * @param amount amount\r\n     */\r\n    static Hermite(value1, tangent1, value2, tangent2, amount) {\r\n        let squared = amount * amount;\r\n        let cubed = squared * amount;\r\n        let part1 = ((2.0 * cubed) - (3.0 * squared)) + 1.0;\r\n        let part2 = (-2.0 * cubed) + (3.0 * squared);\r\n        let part3 = (cubed - (2.0 * squared)) + amount;\r\n        let part4 = cubed - squared;\r\n        return value1 * part1 + value2 * part2 + tangent1 * part3 + tangent2 * part4;\r\n    }\r\n    static RandomRange(min, max) {\r\n        if (min == max) {\r\n            return min;\r\n        }\r\n        else {\r\n            return FloatScalar.Lerp(min, max, Math.random());\r\n        }\r\n    }\r\n    static FormatString(value, length, decimalLenght, char = \" \") {\r\n        let result = value.toFixed(decimalLenght);\r\n        let tempLength = result.length;\r\n        for (let i = length - tempLength; i >= 0; i--) {\r\n            result = char + result;\r\n        }\r\n        return result;\r\n    }\r\n}\r\nFloatScalar.Pi = Math.PI;\r\nFloatScalar.TwoPi = Math.PI * 2;\r\n\n\n//# sourceURL=webpack:///./src/math/scalar.ts?");

/***/ })

/******/ });