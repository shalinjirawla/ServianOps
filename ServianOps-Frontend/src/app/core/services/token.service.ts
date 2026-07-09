import { Injectable, inject } from '@angular/core';
import { StorageService } from './storage.service';
import { DecodedToken } from '../models/auth.models';

@Injectable({
  providedIn: 'root'
})
export class TokenService {
  private readonly storage = inject(StorageService);
  private readonly TOKEN_KEY = 'so_auth_token';

  /**
   * Save JWT token to sessionStorage
   */
  setToken(token: string): void {
    this.storage.setSessionItem(this.TOKEN_KEY, token);
  }

  /**
   * Get JWT token from sessionStorage
   */
  getToken(): string | null {
    return this.storage.getSessionItem<string>(this.TOKEN_KEY);
  }

  /**
   * Remove JWT token
   */
  clearToken(): void {
    this.storage.removeSessionItem(this.TOKEN_KEY);
  }

  /**
   * Natively decode JWT token payload without external libraries
   */
  decodeToken(token: string): DecodedToken | null {
    try {
      const parts = token.split('.');
      if (parts.length !== 3) return null;

      // Base64Url decode the payload (second part)
      let payload = parts[1].replace(/-/g, '+').replace(/_/g, '/');
      
      // Pad base64 representation if needed
      const pad = payload.length % 4;
      if (pad) {
        if (pad === 1) return null;
        payload += new Array(5 - pad).join('=');
      }

      // Handle Unicode characters correctly
      const decodedJson = decodeURIComponent(
        atob(payload)
          .split('')
          .map((c) => '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2))
          .join('')
      );

      const parsed = JSON.parse(decodedJson);
      
      // Resiliently resolve key mappings from both standard JWT and ASP.NET Core schemas
      const role = parsed['role'] || parsed['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] || 'User';
      const tenantId = parsed['tenant_id'] !== undefined ? parsed['tenant_id'] : null;
      const userId = parsed['user_id'] || parsed['sub'] || '';

      return {
        sub: parsed.sub || '',
        email: parsed.email || parsed['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress'] || '',
        jti: parsed.jti || '',
        tenant_id: tenantId,
        user_id: userId,
        role: role,
        exp: parsed.exp || 0,
        iss: parsed.iss,
        aud: parsed.aud
      };
    } catch (e) {
      console.error('TokenService: Error decoding JWT token', e);
      return null;
    }
  }

  /**
   * Get decoded token of current active JWT
   */
  getDecodedToken(): DecodedToken | null {
    const token = this.getToken();
    if (!token) return null;
    return this.decodeToken(token);
  }

  /**
   * Validate if token is expired
   */
  isTokenExpired(token: string): boolean {
    const decoded = this.decodeToken(token);
    if (!decoded || !decoded.exp) return true;

    // JWT exp claim is in seconds. JS Date.now() is in milliseconds.
    const currentTime = Math.floor(Date.now() / 1000);
    return decoded.exp < currentTime;
  }

  /**
   * Check if current active token has expired
   */
  isCurrentTokenExpired(): boolean {
    const token = this.getToken();
    if (!token) return true;
    return this.isTokenExpired(token);
  }
}
