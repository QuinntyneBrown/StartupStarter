import { HttpInterceptorFn, HttpErrorResponse } from '@angular/common/http';
import { inject } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { catchError, throwError } from 'rxjs';

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const snackBar = inject(MatSnackBar);

  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      let errorMessage = 'An unexpected error occurred';

      if (error.error instanceof ErrorEvent) {
        errorMessage = error.error.message;
      } else {
        switch (error.status) {
          case 400:
            errorMessage = error.error?.message || 'Bad request';
            break;
          case 401:
            errorMessage = 'Unauthorized. Please login again.';
            break;
          case 403:
            errorMessage = 'You do not have permission to perform this action';
            break;
          case 404:
            errorMessage = 'Resource not found';
            break;
          case 409:
            errorMessage = error.error?.message || 'Conflict occurred';
            break;
          case 422:
            errorMessage = error.error?.message || 'Validation failed';
            break;
          case 500:
            errorMessage = 'Server error. Please try again later.';
            break;
          case 503:
            errorMessage = 'Service temporarily unavailable';
            break;
          default:
            errorMessage = error.error?.message || `Error: ${error.status}`;
        }
      }

      if (error.status !== 401) {
        snackBar.open(errorMessage, 'Close', {
          duration: 5000,
          panelClass: ['error-snackbar']
        });
      }

      return throwError(() => error);
    })
  );
};
