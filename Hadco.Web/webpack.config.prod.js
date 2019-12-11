const commonConfig = require('./webpack.config.common');
const ExtractCssPlugin = require('mini-css-extract-plugin');
const FaviconsWebpackPlugin = require('favicons-webpack-plugin');
const NgAnnotatePlugin = require('ng-annotate-webpack-plugin');
const HtmlWebpackPlugin = require('html-webpack-plugin');
const webpack = require('webpack');
const WebpackMd5Hash = require('webpack-md5-hash');
const webpackMerge = require('webpack-merge');

module.exports = webpackMerge(commonConfig, {

  devtool: 'source-map',

  plugins: [
    new HtmlWebpackPlugin({
      template: './index.html',
      chunksSortMode: 'dependency',
      filename: '../dist/index.html',
    }),
    
    new FaviconsWebpackPlugin({
      logo: './favicon.png',
      emitStats: true,
      prefix: 'icons/',
      statsFilename: 'icons/stats.json',
      inject: true,
      title: 'Hadco Time',
      background: '#efefef',
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

    new NgAnnotatePlugin({
      add: true,
    }),

    new WebpackMd5Hash(),

    new ExtractCssPlugin({
      filename: '[name].[chunkhash].style.css',
    }),

    new webpack.optimize.ModuleConcatenationPlugin(),
  ],
  
  mode: 'production',

});