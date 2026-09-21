// Used by the containerised build (docker-compose "web" service), where nginx
// serves the SPA and proxies /api to the API on the same origin — so there is
// no cross-origin call and no CORS involved.
// environment.prod.ts is left alone for the Azure Static Web Apps deploy.
export const environment = {
  apiUrl: '/api',
  auth0: {
    domain: 'dev-sizppb5m3zuup43h.us.auth0.com',
    clientId: 'e14p6kIA0CoAm6vzFm5e5dPq0iXcYs8V',
    audience: 'https://api.expensetracker',
  },
  // Publishable key — safe to ship in the browser bundle. Never put a logo.dev sk_ key here.
  logoDev: {
    token: 'pk_XwdHh7UrRWaC9z3If7rFpg',
  },
};
