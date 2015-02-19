var webpack = require("webpack");
var ExtractTextPlugin = require("extract-text-webpack-plugin");
var path = require("path");


module.exports = {
  entry: './app/app.js',
  output: {
    path: './build',
    publicPath: 'www.crossroads.net/',
    filename: '[name].js'
  },
  externals: {
    "angular": "angular"
  },
  module: {
    loaders: [
      { test: /\.js$/, exclude: /node_modules/, loader: 'babel-loader'},
      { test: /\.scss$/, loader: ExtractTextPlugin.extract('style-loader', 'css-loader!sass-loader')},
      { test: /\.(jpe?g|png|gif|svg)$/i, loaders: ['image?bypassOnDebug&optimizationLevel=7&interlaced=false']},
      { test: /\.woff(2)?(\?v=[0-9]\.[0-9]\.[0-9])?$/, loader: "url-loader?limit=10000&minetype=application/font-woff" },
      { test: /\.(ttf|eot|svg)(\?v=[0-9]\.[0-9]\.[0-9])?$/, loader: "file-loader" },
      { test: /\.html$/,loader: "ngtemplate?module=myTemplates&relativeTo=^" + (path.resolve(__dirname, 'app/')) + "!html"}
    ]
  },
  plugins: [
    new ExtractTextPlugin("[name].css")
  ]
};
