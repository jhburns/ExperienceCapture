/*eslint no-process-env: "off"*/

const htmlWebpackInjectAttributesPlugin = require('html-webpack-inject-attributes-plugin');

// This override is needed to allow cypress to report
// Errors from the site's scripts
module.exports = function override(config, env) {
  if (process.env.NODE_ENV === 'development') {
    config.plugins.push(
      new htmlWebpackInjectAttributesPlugin({
        crossorigin: "anonymous",
      })
    );
  }

  return config;
};