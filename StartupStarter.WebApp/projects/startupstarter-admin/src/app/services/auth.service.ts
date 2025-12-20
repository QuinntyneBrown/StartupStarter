import { Injectable, inject, signal, computed } from '@angular/core';
import { Router } from '@angular/router';
import { Observable, tap, catchError, of, BehaviorSubject } from 'rxjs';
import { ApiService } from './api.service';
import {
  LoginRequest,
  LoginResponse,
  AuthenticatedUser,
  PasswordResetInitRequest,
  PasswordResetCompleteRequest,
  EnableMfaRequest,
  VerifyMfaRequest,
  UserSession
} from '../models';

const TOKEN_KEY = 'auth_token';
const USER_KEY = 'auth_user';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private readonly api = inject(ApiService);
  private readonly router = inject(Router);

  private readonly _user = signal<AuthenticatedUser | null>(this.getStoredUser());
  private readonly _token = signal<string | null>(this.getStoredToken());
  private readonly _isLoading = signal(false);

  readonly user = this._user.asReadonly();
  readonly currentUser = this._user.asReadonly();
  readonly token = this._token.asReadonly();
  readonly isLoading = this._isLoading.asReadonly();
  readonly isAuthenticated = computed(() => !!this._token());
  readonly userFullName = computed(() => {
    const u = this._user();
    return u ? `${u.firstName} ${u.lastName}` : '';
  });

  login(request: LoginRequest): Observable<LoginResponse> {
    this._isLoading.set(true);
    return this.api.post<LoginResponse>('authentication/login', request).pipe(
      tap(response => {
        this.setSession(response);
        this._isLoading.set(false);
      }),
      catchError(error => {
        this._isLoading.set(false);
        throw error;
      })
    );
  }

  logout(): Observable<boolean> {
    return this.api.post<boolean>('authentication/logout', {}).pipe(
      tap(() => this.clearSession()),
      catchError(() => {
        this.clearSession();
        return of(true);
      })
    );
  }

  requestPasswordReset(request: PasswordResetInitRequest): Observable<boolean> {
    return this.api.post<boolean>('authentication/password-reset/request', request);
  }

  completePasswordReset(request: PasswordResetCompleteRequest): Observable<boolean> {
    return this.api.post<boolean>('authentication/password-reset/complete', request);
  }

  enableMfa(request: EnableMfaRequest): Observable<{ secretKey: string; qrCode: string }> {
    return this.api.post<{ secretKey: string; qrCode: string }>('authentication/mfa/enable', request);
  }

  verifyMfa(request: VerifyMfaRequest): Observable<boolean> {
    return this.api.post<boolean>('authentication/mfa/verify', request);
  }

  disableMfa(): Observable<boolean> {
    return this.api.post<boolean>('authentication/mfa/disable', {});
  }

  changePassword(currentPassword: string, newPassword: string): Observable<boolean> {
    return this.api.post<boolean>('authentication/change-password', { currentPassword, newPassword });
  }

  getSessions(): Observable<UserSession[]> {
    return this.api.get<UserSession[]>('authentication/sessions');
  }

  terminateSession(sessionId: string): Observable<boolean> {
    return this.api.delete<boolean>(`authentication/sessions/${sessionId}`);
  }

  hasPermission(permission: string): boolean {
    const user = this._user();
    return user?.permissions?.includes(permission) ?? false;
  }

  hasRole(role: string): boolean {
    const user = this._user();
    return user?.roles?.includes(role) ?? false;
  }

  hasAnyPermission(permissions: string[]): boolean {
    return permissions.some(p => this.hasPermission(p));
  }

  hasAllPermissions(permissions: string[]): boolean {
    return permissions.every(p => this.hasPermission(p));
  }

  private setSession(response: LoginResponse): void {
    this._token.set(response.token);
    this._user.set(response.user);
    localStorage.setItem(TOKEN_KEY, response.token);
    localStorage.setItem(USER_KEY, JSON.stringify(response.user));
  }

  private clearSession(): void {
    this._token.set(null);
    this._user.set(null);
    localStorage.removeItem(TOKEN_KEY);
    localStorage.removeItem(USER_KEY);
    this.router.navigate(['/login']);
  }

  private getStoredToken(): string | null {
    return localStorage.getItem(TOKEN_KEY);
  }

  private getStoredUser(): AuthenticatedUser | null {
    const userJson = localStorage.getItem(USER_KEY);
    return userJson ? JSON.parse(userJson) : null;
  }
}
