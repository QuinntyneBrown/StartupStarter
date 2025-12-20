/** @type {import('jest').Config} */
const config = {
  projects: [
    '<rootDir>/projects/startupstarter-admin'
  ],
  testPathIgnorePatterns: [
    '/node_modules/',
    '/e2e/'
  ]
};

module.exports = config;
