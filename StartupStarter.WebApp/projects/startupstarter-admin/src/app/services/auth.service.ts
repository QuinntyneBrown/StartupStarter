import { Injectable, inject, signal, computed } from '@angular/core';
import { Router } from '@angular/router';
import { Observable, tap, of } from 'rxjs';
import { ApiService } from './api.service';
import {
  LoginRequest,
  LoginResponse,
  AuthenticatedUser,
  UserSession,
  MfaSetup,
  VerifyMfaRequest,
  ForgotPasswordRequest,
  ResetPasswordRequest,
  ChangePasswordRequest,
  RefreshTokenRequest
} from '../models';

const TOKEN_KEY = 'auth_token';
const REFRESH_TOKEN_KEY = 'refresh_token';
const USER_KEY = 'auth_user';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private readonly api = inject(ApiService);
  private readonly router = inject(Router);

  private readonly _user = signal<AuthenticatedUser | null>(this.loadUser());
  private readonly _token = signal<string | null>(this.loadToken());

  readonly user = this._user.asReadonly();
  readonly token = this._token.asReadonly();
  readonly isAuthenticated = computed(() => !!this._token() && !!this._user());

  private loadUser(): AuthenticatedUser | null {
    const stored = localStorage.getItem(USER_KEY);
    return stored ? JSON.parse(stored) : null;
  }

  private loadToken(): string | null {
    return localStorage.getItem(TOKEN_KEY);
  }

  login(request: LoginRequest): Observable<LoginResponse> {
    return this.api.post<LoginResponse>('auth/login', request).pipe(
      tap(response => {
        if (!response.requiresMfa) {
          this.setAuthData(response);
        }
      })
    );
  }

  verifyMfa(request: VerifyMfaRequest): Observable<LoginResponse> {
    return this.api.post<LoginResponse>('auth/verify-mfa', request).pipe(
      tap(response => this.setAuthData(response))
    );
  }

  logout(): void {
    this.api.post('auth/logout', {}).subscribe({
      complete: () => this.clearAuth()
    });
    this.clearAuth();
  }

  refreshToken(): Observable<LoginResponse> {
    const refreshToken = localStorage.getItem(REFRESH_TOKEN_KEY);
    if (!refreshToken) {
      this.clearAuth();
      return of(null as unknown as LoginResponse);
    }

    const request: RefreshTokenRequest = { refreshToken };
    return this.api.post<LoginResponse>('auth/refresh', request).pipe(
      tap(response => this.setAuthData(response))
    );
  }

  forgotPassword(request: ForgotPasswordRequest): Observable<void> {
    return this.api.post<void>('auth/forgot-password', request);
  }

  resetPassword(request: ResetPasswordRequest): Observable<void> {
    return this.api.post<void>('auth/reset-password', request);
  }

  changePassword(request: ChangePasswordRequest): Observable<void> {
    return this.api.post<void>('auth/change-password', request);
  }

  enableMfa(): Observable<MfaSetup> {
    return this.api.post<MfaSetup>('auth/enable-mfa', {});
  }

  disableMfa(): Observable<void> {
    return this.api.post<void>('auth/disable-mfa', {});
  }

  getSessions(): Observable<UserSession[]> {
    return this.api.get<UserSession[]>('auth/sessions');
  }

  revokeSession(sessionId: string): Observable<void> {
    return this.api.delete<void>(`auth/sessions/${sessionId}`);
  }

  hasPermission(permission: string): boolean {
    const user = this._user();
    if (!user) return false;
    return user.permissions.includes(permission) || user.permissions.includes('*:*');
  }

  hasAnyPermission(permissions: string[]): boolean {
    return permissions.some(p => this.hasPermission(p));
  }

  hasRole(role: string): boolean {
    const user = this._user();
    if (!user) return false;
    return user.roles.includes(role);
  }

  private setAuthData(response: LoginResponse): void {
    localStorage.setItem(TOKEN_KEY, response.accessToken);
    localStorage.setItem(REFRESH_TOKEN_KEY, response.refreshToken);
    localStorage.setItem(USER_KEY, JSON.stringify(response.user));
    this._token.set(response.accessToken);
    this._user.set(response.user);
  }

  private clearAuth(): void {
    localStorage.removeItem(TOKEN_KEY);
    localStorage.removeItem(REFRESH_TOKEN_KEY);
    localStorage.removeItem(USER_KEY);
    this._token.set(null);
    this._user.set(null);
    this.router.navigate(['/login']);
  }
}
