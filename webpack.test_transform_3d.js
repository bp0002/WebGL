const path = require('path');
const { CleanWebpackPlugin } = require('clean-webpack-plugin');
const HTMLWebpackPlugin = require('html-webpack-plugin');

module.exports = {
    // entry: './src/logic_webgl/index.ts',
    entry: {
        logic_webgl_vao: './test/transform_3d.ts'
    },
    output: {
        filename: 'index.js',
        path: path.resolve(__dirname, 'webpack_build/test/transform_3d')
    },
    // devtool: 'inline-source-map',                                  // 告诉 webpack 提取这些 source map，并内联到最终的 文件 中。
    module: {
        rules:[ 
            {
                test: /\.ts?$/,
                use: 'ts-loader',
                exclude: /node_modules/
            }
        ]
    },
    resolve: {
        extensions: [ '.tsx', '.ts', '.js' ]
    },
    mode: 'development',
    // mode: 'production',
    optimization: {
        usedExports: true
    },
    plugins: [
        new CleanWebpackPlugin(),
        new HTMLWebpackPlugin({
            template: './src/index.html',
        })
    ],
}