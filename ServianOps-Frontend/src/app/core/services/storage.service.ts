import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class StorageService {
  constructor() {}

  /**
   * Set item in SessionStorage
   */
  setSessionItem(key: string, value: any): void {
    try {
      const serializedValue = typeof value === 'string' ? value : JSON.stringify(value);
      sessionStorage.setItem(key, serializedValue);
    } catch (e) {
      console.warn('StorageService: failed to write to sessionStorage', e);
    }
  }

  /**
   * Get item from SessionStorage
   */
  getSessionItem<T>(key: string): T | null {
    try {
      const data = sessionStorage.getItem(key);
      if (!data) return null;
      try {
        return JSON.parse(data) as T;
      } catch {
        return data as unknown as T;
      }
    } catch {
      return null;
    }
  }

  /**
   * Remove item from SessionStorage
   */
  removeSessionItem(key: string): void {
    try {
      sessionStorage.removeItem(key);
    } catch (e) {
      console.warn('StorageService: failed to delete from sessionStorage', e);
    }
  }

  /**
   * Clear SessionStorage
   */
  clearSession(): void {
    try {
      sessionStorage.clear();
    } catch (e) {
      console.warn('StorageService: failed to clear sessionStorage', e);
    }
  }

  /**
   * Set item in LocalStorage (used for Remember Me)
   */
  setLocalItem(key: string, value: any): void {
    try {
      const serializedValue = typeof value === 'string' ? value : JSON.stringify(value);
      localStorage.setItem(key, serializedValue);
    } catch (e) {
      console.warn('StorageService: failed to write to localStorage', e);
    }
  }

  /**
   * Get item from LocalStorage
   */
  getLocalItem<T>(key: string): T | null {
    try {
      const data = localStorage.getItem(key);
      if (!data) return null;
      try {
        return JSON.parse(data) as T;
      } catch {
        return data as unknown as T;
      }
    } catch {
      return null;
    }
  }

  /**
   * Remove item from LocalStorage
   */
  removeLocalItem(key: string): void {
    try {
      localStorage.removeItem(key);
    } catch (e) {
      console.warn('StorageService: failed to delete from localStorage', e);
    }
  }
}
