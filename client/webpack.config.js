var webpack = require('webpack');
var ExtractTextPlugin = require('extract-text-webpack-plugin');

module.exports = function (env) {

	var tsPath = env == "prod" ? "ts/" : "dist/ts/";
	var lessPath = env == "prod" ? "" : "dist/";
	var outFile = env == "prod" ? ".min" : "";

	var entries = [
		"order",
		"contact",
		"detail",
		"index",
		"global",
		"blog",
		"cart"
	];
	var entriesObj = new Object();
	entries.forEach(function (entry) {
		entriesObj[entry] = './' + tsPath + entry + '.ts';
	});
	entriesObj["less"] = './' + lessPath + 'less/style.less';

	return {
		entry: entriesObj,
		output: {
			filename: '[name]' + outFile + '.js'
		},
		// Turn on sourcemaps
		devtool: 'source-map',
		resolve: {
			extensions: ['.webpack.js', '.web.js', '.ts', '.js', '.less']
		},
		plugins: [
			new webpack.ProvidePlugin({
				jQuery: 'jquery',
				$: 'jquery',
				jquery: 'jquery'
			}),
			new ExtractTextPlugin("app.css")
		],
		module: {
			loaders: [{
					test: /\.ts$/,
					loader: 'ts-loader'
				},
				{
					test: /\.less$/,
					use: ExtractTextPlugin.extract({
						fallback: 'style-loader',
						use: ['css-loader', 'less-loader']
					})
				},
				{
					test: /\.(png|woff|woff2|eot|ttf|svg)$/,
					loader: 'url-loader?limit=100000'
				}
			]
		}
	}
}
