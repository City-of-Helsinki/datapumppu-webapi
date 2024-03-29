const path = require('path')
const HtmlWebpackPlugin = require('html-webpack-plugin')
const MiniCssExtractPlugin = require("mini-css-extract-plugin");
const stylesHandler = MiniCssExtractPlugin.loader;

module.exports = {
  // Where files should be sent once they are bundled
  output: {
    path: path.join(__dirname, '/dist'),
    filename: 'bundle.js'
  },
  plugins: [
    new HtmlWebpackPlugin({
      template: path.join(__dirname, "test-page", "test-page.html"),
    }),
    new MiniCssExtractPlugin(),

  ],
  // webpack 5 comes with devServer which loads in development mode
  devServer: {
    static: {
      directory: path.join(__dirname, "/dist"),
    },
    port: 3000,
  },
  // Rules of how webpack will take our files, complie & bundle them for the browser 
  module: {
    rules: [
      {
        test: /\.(js|jsx)$/,
        exclude: /node_modules/,
        use: {
          loader: 'babel-loader'
        }
      },
      {
        test: /\.css$/i,
        use: [stylesHandler, "css-loader"],
      }
    ]
  }
}
