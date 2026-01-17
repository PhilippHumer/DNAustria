export const environment = {
  production: false,
  backendUrl: (['localhost','127.0.0.1'].includes(window.location.hostname) ? 'http://localhost:5100' : 'http://backend:5100'),
  apiKey: 'dev-key'
};

