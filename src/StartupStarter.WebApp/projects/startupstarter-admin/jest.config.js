/** @type {import('jest').Config} */
const config = {
  displayName: 'startupstarter-admin',
  preset: 'jest-preset-angular',
  setupFilesAfterEnv: ['<rootDir>/setup-jest.ts'],
  testPathIgnorePatterns: [
    '/node_modules/',
    '/e2e/'
  ],
  moduleNameMapper: {
    '^@app/(.*)$': '<rootDir>/src/app/$1',
    '^@env/(.*)$': '<rootDir>/src/environments/$1'
  },
  transformIgnorePatterns: [
    'node_modules/(?!.*\\.mjs$|@angular|rxjs)'
  ],
  testMatch: [
    '**/*.spec.ts'
  ],
  collectCoverageFrom: [
    'src/**/*.ts',
    '!src/**/*.spec.ts',
    '!src/main.ts',
    '!src/**/*.routes.ts'
  ]
};

module.exports = config;
