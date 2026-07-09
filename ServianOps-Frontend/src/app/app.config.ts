import { ApplicationConfig, provideBrowserGlobalErrorListeners, APP_INITIALIZER, inject } from '@angular/core';
import { provideRouter } from '@angular/router';
import { provideHttpClient, withInterceptors } from '@angular/common/http';

import { routes } from './app.routes';
import { jwtInterceptor } from './core/interceptors/jwt.interceptor';
import { AuthService } from './core/services/auth.service';
import { SessionService } from './core/services/session.service';

export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideRouter(routes),
    provideHttpClient(
      withInterceptors([jwtInterceptor])
    ),
    {
      provide: APP_INITIALIZER,
      useFactory: () => {
        return () => {
          // Initialize user session on application startup
          const authService = inject(AuthService);
          authService.initSession();
          
          // Instantiate SessionService so it begins tracking token exp
          inject(SessionService);
        };
      },
      multi: true
    }
  ]
};

