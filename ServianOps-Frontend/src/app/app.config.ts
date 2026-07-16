import { ApplicationConfig, provideBrowserGlobalErrorListeners, provideZoneChangeDetection, APP_INITIALIZER } from '@angular/core';
import { provideRouter, withComponentInputBinding } from '@angular/router';
import { provideHttpClient, withInterceptors } from '@angular/common/http';

import { routes } from './app.routes';
import { jwtInterceptor } from './core/interceptors/jwt.interceptor';
import { 
  AuthService,
  CustomersService, 
  CustomerTypesService, 
  JobsService, 
  PlansService, 
  RolesService, 
  SitesService, 
  TenantsService, 
  TradesService, 
  UsersService,
  API_BASE_URL 
} from './core/api/service-proxies';
import { environment } from '../environments/environment';
import { firstValueFrom } from 'rxjs';
import { AuthService as AppAuthService } from './core/services/auth.service';

function initializeApp(authService: AppAuthService) {
  return () => firstValueFrom(authService.initSession());
}

export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideZoneChangeDetection({ eventCoalescing: true }),
    provideRouter(routes, withComponentInputBinding()),
    provideHttpClient(withInterceptors([jwtInterceptor])),
    { provide: API_BASE_URL, useValue: environment.apiUrl },
    {
      provide: APP_INITIALIZER,
      useFactory: initializeApp,
      deps: [AppAuthService],
      multi: true
    },
    AuthService,
    CustomersService,
    CustomerTypesService,
    JobsService,
    PlansService,
    RolesService,
    SitesService,
    TenantsService,
    TradesService,
    UsersService
  ]
};



