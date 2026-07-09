import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule, AbstractControl, ValidationErrors } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';
import { StorageService } from '../../../core/services/storage.service';
import { ThemeService } from '../../../core/services/theme.service';
import { ToastService } from '../../../core/services/toast.service';
import { LoginDto } from '../../../core/models/auth.models';

type AuthView = 'LOGIN' | 'SIGNUP' | 'FORGOT' | 'OTP';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss'
})
export class LoginComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly authService = inject(AuthService);
  private readonly storage = inject(StorageService);
  private readonly router = inject(Router);
  private readonly route = inject(ActivatedRoute);
  readonly themeService = inject(ThemeService);
  private readonly toastService = inject(ToastService);

  // View state signals
  readonly activeView = signal<AuthView>('LOGIN');

  // Form group definitions
  loginForm!: FormGroup;
  signUpForm!: FormGroup;
  forgotForm!: FormGroup;
  otpForm!: FormGroup;

  // Reactivity state signals
  readonly isLoading = signal(false);
  readonly apiError = signal<string | null>(null);
  readonly sessionExpiredMessage = signal<string | null>(null);
  readonly showPassword = signal(false);
  readonly showConfirmPassword = signal(false);

  // Email storage for forgot password -> OTP flow transition
  readonly otpEmail = signal<string>('');

  // Keys for storage autofill
  private readonly REMEMBER_EMAIL_KEY = 'so_remember_email';
  private readonly REMEMBER_TENANCY_KEY = 'so_remember_tenancy';

  ngOnInit(): void {
    // 1. Parse route query params for session reasons (expired / unauthorized)
    this.route.queryParams.subscribe((params) => {
      const reason = params['reason'];
      if (reason === 'expired') {
        this.sessionExpiredMessage.set('Your session has expired. Please log in again.');
      } else if (reason === 'unauthorized') {
        this.sessionExpiredMessage.set('Access denied. Please sign in with authorized credentials.');
      } else {
        this.sessionExpiredMessage.set(null);
      }
    });

    // 2. Load cached Remember Me values
    const savedEmail = this.storage.getLocalItem<string>(this.REMEMBER_EMAIL_KEY) || '';
    const savedTenancy = this.storage.getLocalItem<string>(this.REMEMBER_TENANCY_KEY) || '';
    const shouldRemember = savedEmail !== '' || savedTenancy !== '';

    // 3. Initialize Reactive Forms
    this.loginForm = this.fb.group({
      tenancyName: [savedTenancy],
      email: [savedEmail, [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]],
      rememberMe: [shouldRemember]
    });

    this.signUpForm = this.fb.group({
      tenancyName: ['', [Validators.required]],
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]],
      confirmPassword: ['', [Validators.required]],
      agreeTerms: [false, [Validators.requiredTrue]]
    }, { validators: this.passwordMatchValidator });

    this.forgotForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]]
    });

    this.otpForm = this.fb.group({
      code1: ['', [Validators.required, Validators.pattern('^[0-9]$')]],
      code2: ['', [Validators.required, Validators.pattern('^[0-9]$')]],
      code3: ['', [Validators.required, Validators.pattern('^[0-9]$')]],
      code4: ['', [Validators.required, Validators.pattern('^[0-9]$')]],
      code5: ['', [Validators.required, Validators.pattern('^[0-9]$')]],
      code6: ['', [Validators.required, Validators.pattern('^[0-9]$')]]
    });
  }

  passwordMatchValidator(g: AbstractControl): ValidationErrors | null {
    const password = g.get('password')?.value;
    const confirm = g.get('confirmPassword')?.value;
    return password === confirm ? null : { mismatch: true };
  }

  togglePasswordVisibility(): void {
    this.showPassword.update((val) => !val);
  }

  toggleConfirmPasswordVisibility(): void {
    this.showConfirmPassword.update((val) => !val);
  }

  isFieldInvalid(form: FormGroup, fieldName: string): boolean {
    const field = form.get(fieldName);
    return !!(field && field.invalid && (field.dirty || field.touched));
  }

  switchView(view: AuthView): void {
    this.activeView.set(view);
    this.apiError.set(null);
    this.sessionExpiredMessage.set(null);
  }

  /**
   * Submit login details to server
   */
  onSubmit(): void {
    if (this.loginForm.invalid) {
      this.loginForm.markAllAsTouched();
      return;
    }

    this.isLoading.set(true);
    this.apiError.set(null);
    this.sessionExpiredMessage.set(null);

    const formValues = this.loginForm.value;
    const loginPayload: LoginDto = {
      email: formValues.email.trim(),
      password: formValues.password,
      tenancyName: formValues.tenancyName?.trim() || undefined
    };

    this.authService.login(loginPayload).subscribe({
      next: (res) => {
        // 1. Process Remember Me credentials cache
        if (formValues.rememberMe) {
          this.storage.setLocalItem(this.REMEMBER_EMAIL_KEY, loginPayload.email);
          if (loginPayload.tenancyName) {
            this.storage.setLocalItem(this.REMEMBER_TENANCY_KEY, loginPayload.tenancyName);
          } else {
            this.storage.removeLocalItem(this.REMEMBER_TENANCY_KEY);
          }
        } else {
          this.storage.removeLocalItem(this.REMEMBER_EMAIL_KEY);
          this.storage.removeLocalItem(this.REMEMBER_TENANCY_KEY);
        }

        // 2. Alert success & redirect
        this.toastService.success(`Welcome back! Successfully logged in as ${res.role}.`);
        const returnUrl = this.route.snapshot.queryParams['returnUrl'] || '/dashboard';
        this.router.navigateByUrl(returnUrl);
        this.isLoading.set(false);
      },
      error: (err) => {
        const errMsg = err.error?.error || 'Authentication failed. Please verify credentials.';
        this.apiError.set(errMsg);
        this.toastService.error(errMsg);
        this.isLoading.set(false);
      }
    });
  }

  /**
   * Submit Sign Up Details
   */
  onSignUpSubmit(): void {
    if (this.signUpForm.invalid) {
      this.signUpForm.markAllAsTouched();
      return;
    }

    this.isLoading.set(true);
    // Simulate API registration delay
    setTimeout(() => {
      this.isLoading.set(false);
      this.toastService.success('Registration request received. Verification email dispatched.');
      this.otpEmail.set(this.signUpForm.value.email);
      this.switchView('OTP');
      this.signUpForm.reset();
    }, 1500);
  }

  /**
   * Submit Forgot Password Link Request
   */
  onForgotSubmit(): void {
    if (this.forgotForm.invalid) {
      this.forgotForm.markAllAsTouched();
      return;
    }

    this.isLoading.set(true);
    const email = this.forgotForm.value.email;

    setTimeout(() => {
      this.isLoading.set(false);
      this.toastService.info(`Password recovery link dispatched to ${email}`);
      this.otpEmail.set(email);
      this.switchView('OTP');
      this.forgotForm.reset();
    }, 1200);
  }

  /**
   * Handle OTP Inputs and paste operations
   */
  onOtpKeyUp(event: KeyboardEvent, index: number): void {
    const input = event.target as HTMLInputElement;
    const value = input.value;
    
    // Focus next input automatically
    if (value && index < 6) {
      const nextInput = document.getElementById(`otp-${index + 1}`) as HTMLInputElement;
      if (nextInput) nextInput.focus();
    }
  }

  onOtpKeyDown(event: KeyboardEvent, index: number): void {
    // Focus previous input on backspace
    if (event.key === 'Backspace' && !((event.target as HTMLInputElement).value) && index > 1) {
      const prevInput = document.getElementById(`otp-${index - 1}`) as HTMLInputElement;
      if (prevInput) {
        prevInput.focus();
        prevInput.value = '';
      }
    }
  }

  onOtpSubmit(): void {
    if (this.otpForm.invalid) {
      this.toastService.error('Please enter the full 6-digit OTP code.');
      return;
    }

    this.isLoading.set(true);
    const code = Object.values(this.otpForm.value).join('');
    
    setTimeout(() => {
      this.isLoading.set(false);
      if (code === '123456' || code.length === 6) {
        this.toastService.success('Identity verification successful. You can now access your account.');
        this.switchView('LOGIN');
        this.otpForm.reset();
      } else {
        this.toastService.error('Invalid OTP code. Please enter the correct code.');
      }
    }, 1500);
  }

  resendOtp(): void {
    this.toastService.info('Dispatched a new verification OTP code.');
  }

  /**
   * OAuth Social Login triggers
   */
  triggerSocialLogin(provider: 'Google' | 'Apple' | 'GitHub'): void {
    this.isLoading.set(true);
    this.toastService.info(`Connecting to ${provider} authentication servers...`);

    setTimeout(() => {
      this.isLoading.set(false);
      // Simulate successful admin authentication for oauth demo
      this.toastService.success(`Oauth verification with ${provider} succeeded.`);
      
      // Simulate direct dashboard redirection
      // Note: We bypass normal API credentials here for visual feedback convenience
      const returnUrl = this.route.snapshot.queryParams['returnUrl'] || '/dashboard';
      this.router.navigateByUrl(returnUrl);
    }, 1800);
  }
}
