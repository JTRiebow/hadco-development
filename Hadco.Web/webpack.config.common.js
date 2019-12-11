const path = require('path');
const webpack = require('webpack');

const { ProvidePlugin } = webpack;

module.exports = {
  context: path.resolve(__dirname, 'src'),

  entry: {
    index: './index.ts',
  },

  output: {
    path: path.resolve(__dirname, 'dist'),
    filename: '[name].[chunkhash].bundle.js',
    sourceMapFilename: '[name].[chunkhash].bundle.map',
    chunkFilename: '[id].[chunkhash].chunk.js',
  },
  
  optimization: {
    splitChunks: {
      name: 'common',
    },
  },

  module: {
    rules: [
      {
        test: /\.ts$/,
        exclude: /node_modules/,
        use: [
          {
            loader: 'awesome-typescript-loader',
          },
        ],
      },
      {
        test: /\.html$/,
        use: [
          {
            loader: 'html-loader',
          },
        ],
      },
      {
        test: /\.css$|\.scss$/,
        loader: [ 'style-loader', 'css-loader', 'sass-loader' ],
      },
      {
        test: /\.(svg|png|jp(e)?g|gif|eot|ttf)(\?v=[0-9]\.[0-9]\.[0-9])?$/,
        loader: [ 'file-loader' ],
      },
      {
        test: /\.woff(2)?(\?v=[0-9]\.[0-9]\.[0-9])?$/,
        loader: 'url-loader?limit=10000&mimetype=application/font-woff',
      },
    ],
  },

  plugins: [
    new ProvidePlugin({
      _: 'lodash',
      $: 'jquery',
      'window.jQuery': 'jquery',
    }),
  ],
  
  resolve: {
    extensions: [ '.ts', '.js', '.html', '.css', '.svg', '.png', '.jpg', '.jpeg', '.gif' ],
  },
};