export interface UserSession {
  sessionId: string;
  userId: string;
  accountId: string;
  ipAddress: string;
  userAgent: string;
  loginMethod: LoginMethod;
  createdAt: Date;
  expiresAt?: Date;
  loggedOutAt?: Date;
  logoutType?: LogoutType;
  isActive: boolean;
}

export interface LoginAttempt {
  loginAttemptId: string;
  userId?: string;
  email: string;
  ipAddress: string;
  userAgent: string;
  loginMethod: LoginMethod;
  success: boolean;
  failureReason?: FailureReason;
  attemptCount: number;
  timestamp: Date;
}

export interface MultiFactorAuthentication {
  mfaId: string;
  userId: string;
  accountId: string;
  method: MfaMethod;
  isEnabled: boolean;
  enabledBy: string;
  enabledAt: Date;
  disabledAt?: Date;
  disabledBy?: string;
  disabledReason?: string;
}

export interface PasswordResetRequest {
  resetRequestId: string;
  userId: string;
  email: string;
  ipAddress: string;
  requestedAt: Date;
  expiresAt: Date;
  completedAt?: Date;
  resetMethod?: ResetMethod;
  isCompleted: boolean;
}

export enum LoginMethod {
  Password = 'Password',
  SSO = 'SSO',
  OAuth = 'OAuth',
  MFA = 'MFA'
}

export enum LogoutType {
  Manual = 'Manual',
  SessionExpired = 'SessionExpired',
  ForcedLogout = 'ForcedLogout'
}

export enum FailureReason {
  InvalidCredentials = 'InvalidCredentials',
  AccountLocked = 'AccountLocked',
  AccountDisabled = 'AccountDisabled',
  MFAFailed = 'MFAFailed'
}

export enum MfaMethod {
  SMS = 'SMS',
  Email = 'Email',
  AuthenticatorApp = 'AuthenticatorApp',
  HardwareToken = 'HardwareToken'
}

export enum ResetMethod {
  Email = 'Email',
  AdminReset = 'AdminReset',
  SecurityQuestions = 'SecurityQuestions'
}

export interface LoginRequest {
  email: string;
  password: string;
}

export interface LoginResponse {
  session: UserSession;
  user: AuthenticatedUser;
  token: string;
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

export interface PasswordResetInitRequest {
  email: string;
}

export interface PasswordResetCompleteRequest {
  token: string;
  newPassword: string;
}

export interface EnableMfaRequest {
  method: MfaMethod;
}

export interface VerifyMfaRequest {
  code: string;
}
