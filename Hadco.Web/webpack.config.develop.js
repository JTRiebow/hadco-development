const webpackMerge = require('webpack-merge');
const HtmlWebpackPlugin = require('html-webpack-plugin');
const FaviconsWebpackPlugin = require('favicons-webpack-plugin');
const commonConfig = require('./webpack.config.common.js');

module.exports = webpackMerge(commonConfig, {
  devServer: {
    contentBase: './',
    compress: true,
    port: 9898,
    host: 'localhost',
    historyApiFallback: true,
    watchOptions: {
      aggregateTimeout: 300,
      poll: 1000,
    },
    proxy: [
      {
        context: [ '/api', '/token', '/swagger' ],
        target: 'http://localhost:56404',
      },
    ],
  },
  devtool: 'inline-source-map',
  plugins: [
    new HtmlWebpackPlugin({
      template: './index.html',
      chunksSortMode: 'dependency',
    }),
    
    new FaviconsWebpackPlugin({
      logo: './favicon.png',
      emitStats: true,
      prefix: 'icons/',
      statsFilename: 'icons/stats.json',
      inject: true,
      title: 'Hadco Time',
      background: '#ffffff',
      icons: {
        android: true,
        appleIcon: true,
        appleStartup: true,
        coast: false,
        favicons: true,
        firefox: true,
        opengraph: true,
        twitter: true,
        yandex: true,
        windows: true,
      },
    }),
  ],
  mode: 'development',
});