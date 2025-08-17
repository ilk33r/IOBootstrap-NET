const rewire = require('rewire');

// Pointing to file which we want to re-wire — this is original build script
const defaults = rewire('react-scripts/scripts/build.js');

// Getting configuration from original build script
let config = defaults.__get__('config');

// Get rid of hash for js files
const currentDate = new Date();
const dateString = `${currentDate.getFullYear()}.${currentDate.getMonth() + 1}.${currentDate.getDate()}`;
config.output.filename = `static/js/[name].${dateString}.js`
config.output.chunkFilename = `static/js/[name].chunk.${dateString}.js`

// Get rid of hash for css files
const miniCssExtractPlugin = config.plugins.find(element => element.constructor.name === "MiniCssExtractPlugin");
miniCssExtractPlugin.options.filename = `static/css/[name].${dateString}.css`
miniCssExtractPlugin.options.chunkFilename = `static/css/[name].${dateString}.css`