module.exports = {
  entry: './app/app.js',
  output: {
    path: './build',
    publicPath: 'www.crossroads.net/',
    filename: 'bundle.js'
  },
  externals: {
    "angular": "angular"
  },
  module: {
    loaders: [
      { test: /\.js$/, exclude: /node_modules/, loader: 'babel-loader'},
      { test: /\.scss$/, loader: 'style!css!sass'},
      {test: /\.(jpe?g|png|gif|svg)$/i, loaders: ['image?bypassOnDebug&optimizationLevel=7&interlaced=false']}
    ]
  }
};