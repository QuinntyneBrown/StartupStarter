export interface LoginRequest {
  email: string;
  password: string;
}

export interface LoginResponse {
  accessToken: string;
  refreshToken: string;
  expiresAt: Date;
  user: AuthenticatedUser;
  requiresMfa: boolean;
}

export interface AuthenticatedUser {
  userId: string;
  email: string;
  firstName: string;
  lastName: string;
  accountId: string;
  roles: string[];
  permissions: string[];
}

export interface UserSession {
  sessionId: string;
  userId: string;
  accountId: string;
  ipAddress: string;
  userAgent: string;
  loginMethod: string;
  createdAt: Date;
  expiresAt: Date;
  isActive: boolean;
}

export interface MfaSetup {
  mfaId: string;
  userId: string;
  method: MfaMethod;
  isEnabled: boolean;
  backupCodes?: string[];
  qrCodeUrl?: string;
}

export enum MfaMethod {
  SMS = 'SMS',
  Email = 'Email',
  AuthenticatorApp = 'AuthenticatorApp'
}

export interface VerifyMfaRequest {
  code: string;
  sessionId: string;
}

export interface ForgotPasswordRequest {
  email: string;
}

export interface ResetPasswordRequest {
  token: string;
  newPassword: string;
  confirmPassword: string;
}

export interface ChangePasswordRequest {
  currentPassword: string;
  newPassword: string;
  confirmPassword: string;
}

export interface RefreshTokenRequest {
  refreshToken: string;
}
