exports.config = {
  seleniumServerJar: './node_modules/protractor/selenium/selenium-server-standalone-2.45.0.jar',
  //seleniumAddress: 'http://localhost:4444/wd/hub',
  specs: ['e2e/**/*.e2e.js'],
  multiCapabilities: [
  //{ browserName: 'firefox'}
  { browserName: 'firefox'}
  //,{ browserName: 'safari'}
  ],
  jasmineNodeOpts: {
    showColors: true,
    defaultTimeoutInterval: 30000
  },
  getPageTimeout: 60000
}
