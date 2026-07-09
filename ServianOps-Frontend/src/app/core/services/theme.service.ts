import { Injectable, signal, effect } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class ThemeService {
  readonly isDarkMode = signal<boolean>(true); // Default to premium dark theme

  constructor() {
    // Check local storage or system preference
    const storedTheme = localStorage.getItem('so-theme');
    if (storedTheme) {
      this.isDarkMode.set(storedTheme === 'dark');
    } else {
      const prefersDark = window.matchMedia('(prefers-color-scheme: dark)').matches;
      this.isDarkMode.set(prefersDark);
    }

    // Effect to apply theme class to document body
    effect(() => {
      const dark = this.isDarkMode();
      if (dark) {
        document.body.classList.add('dark');
        localStorage.setItem('so-theme', 'dark');
      } else {
        document.body.classList.remove('dark');
        localStorage.setItem('so-theme', 'light');
      }
    });
  }

  toggleTheme(): void {
    this.isDarkMode.update(val => !val);
  }
}
