import { inject } from '@angular/core';
import { Router, CanActivateFn } from '@angular/router';
import { AuthService } from '../services';

export const permissionGuard = (permissions: string[]): CanActivateFn => {
  return () => {
    const authService = inject(AuthService);
    const router = inject(Router);

    if (authService.hasAnyPermission(permissions)) {
      return true;
    }

    router.navigate(['/dashboard']);
    return false;
  };
};

export const roleGuard = (roles: string[]): CanActivateFn => {
  return () => {
    const authService = inject(AuthService);
    const router = inject(Router);

    const hasRole = roles.some(role => authService.hasRole(role));
    if (hasRole) {
      return true;
    }

    router.navigate(['/dashboard']);
    return false;
  };
};
