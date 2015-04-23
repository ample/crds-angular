var webpack = require("webpack");
var ExtractTextPlugin = require("extract-text-webpack-plugin");
var path = require("path");

var endpoint = {
    'url': 'http://localhost:49380'
};

var definePlugin = new webpack.DefinePlugin({
    __API_ENDPOINT__: JSON.stringify(process.env.CRDS_API_ENDPOINT || "http://mp-int-web.cloudapp.net/gateway/"),
    __CMS_ENDPOINT__: JSON.stringify(process.env.CRDS_CMS_ENDPOINT || "http://content.crossroads.net/")
});

module.exports = {
    entry: './app/app.js',
    context: __dirname,
    output: {
        path: './assets',
        publicPath: '/assets/',
        filename: '[name].js'
    },
    module: {
        loaders: [
            {
                test: /\.css$/,
                loader: "style-loader!css-loader"
            },
            {
                test: /\.js$/,
                exclude: /node_modules/,
                loader: 'babel-loader'
            },
            {
                test: /\.scss$/,
                loader: ExtractTextPlugin.extract('style-loader', 'css-loader!sass-loader')
            },
            {
                test: /\.(jpe?g|png|gif|svg)$/i,
                loaders: ['image?bypassOnDebug&optimizationLevel=7&interlaced=false']
            },
            {
                test: /\.woff(2)?(\?v=[0-9]\.[0-9]\.[0-9])?$/,
                loader: "url-loader?limit=10000&minetype=application/font-woff"
            },
            {
                test: /\.(ttf|eot|svg)(\?v=[0-9]\.[0-9]\.[0-9])?$/,
                loader: "file-loader"
            },
            {
                test: /\.html$/,
                loader: "ng-cache?prefix=[dir]"
            }

      //{ test: /\.html$/,loader: "ngtemplate?module=myTemplates&relativeTo=^" + (path.resolve(__dirname, 'app/')) + "!html"}
    ]
    },
    plugins: [
    new ExtractTextPlugin("[name].css"),
    definePlugin
  ]
};
