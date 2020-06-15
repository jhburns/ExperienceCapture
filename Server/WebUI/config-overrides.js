const SriPlugin = require('webpack-subresource-integrity');

module.exports = function override(config, env) {
  config.output.crossOriginLoading = 'anonymous';

  console.log(process.env.NODE_ENV === 'development');
  config.plugins.push(
    new SriPlugin({
      hashFuncNames: ['sha256', 'sha384'],
      enabled: process.env.NODE_ENV === 'development',
    })
  );
  
  return config;
}