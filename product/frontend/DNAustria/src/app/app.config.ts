import { provideRouter } from '@angular/router';
import {
  ApplicationConfig,
  importProvidersFrom,
  provideBrowserGlobalErrorListeners,
  provideZoneChangeDetection
} from '@angular/core';

import {routes} from './app.routes';
import {ApiModule, Configuration} from './api';
import {environment} from './environment';
import {provideHttpClient} from '@angular/common/http';

export const appConfig: ApplicationConfig = {
    providers: [
      provideBrowserGlobalErrorListeners(),
      provideZoneChangeDetection({eventCoalescing: true}),
      provideRouter(routes),
      provideHttpClient(),
      importProvidersFrom(
        ApiModule.forRoot(() =>
          new Configuration({
            basePath: environment.apiUrl
          })
        )
      )
    ],
  }
;
