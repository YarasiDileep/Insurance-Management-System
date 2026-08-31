# InsuranceFrontend

This project was generated using [Angular CLI](https://github.com/angular/angular-cli) version 20.3.34.

## Development server

To start a local development server, run:

```bash
ng serve
```

Once the server is running, open your browser and navigate to `http://localhost:4200/`. The application will automatically reload whenever you modify any of the source files.

## Code scaffolding

Angular CLI includes powerful code scaffolding tools. To generate a new component, run:

```bash
ng generate component component-name
```

For a complete list of available schematics (such as `components`, `directives`, or `pipes`), run:

```bash
ng generate --help
```

## Building

To build the project run:

```bash
ng build
```

This will compile your project and store the build artifacts in the `dist/` directory. By default, the production build optimizes your application for performance and speed.

## Running unit tests

To execute unit tests with the [Karma](https://karma-runner.github.io) test runner, use the following command:

```bash
ng test
```

## Running end-to-end tests

For end-to-end (e2e) testing, run:

```bash
ng e2e
```

Angular CLI does not come with an end-to-end testing framework by default. You can choose one that suits your needs.

This project uses Cypress for e2e testing. To run Cypress locally:

1. Install dev dependencies:

```bash
npm install
```

2. Open Cypress Test Runner (interactive):

```bash
npx cypress open
```

3. Run Cypress headless (CI-friendly):

```bash
npx cypress run
```

The repository includes a simple smoke test at `Insurance-Frontend/cypress/e2e/sample.spec.ts` that verifies the login page loads.

### Cypress E2E tests (added)

We've added a small suite of Cypress end-to-end tests under Insurance-Frontend/cypress/e2e:

- login.spec.ts — verifies login flow
- policies.spec.ts — verifies the policies list page
- create-claim.spec.ts — simulates creating a claim

To run them locally:

1. Install dependencies:

```bash
cd Insurance-Frontend
npm ci
```

2. Start the dev server in one terminal:

```bash
ng serve
```

3. Run Cypress in another terminal (interactive):

```bash
npx cypress open
```

Or headless (CI):

```bash
npx cypress run
```

Note: CI is configured to run Cypress with the GitHub Actions workflow at `.github/workflows/ci.yml`.

## Additional Resources

For more information on using the Angular CLI, including detailed command references, visit the [Angular CLI Overview and Command Reference](https://angular.dev/tools/cli) page.
