import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { AuthService } from '../services/auth.service';
import { catchError, throwError } from 'rxjs';
import { Router } from '@angular/router';
// Clone the request to add the authentication header.

export const tokenInterceptor: HttpInterceptorFn = (req, next) => {
  const authService = inject(AuthService);
  const router = inject(Router);

  const token = authService.getToken(); // Lấy token từ AuthService
  if (token) {
    console.log('Request URL:', req.url);

    // const cloned = req.clone({

    //   headers: req.headers.append('Authorization',  `Bearer ${token}`),
    // });

    const cloned = req.clone({
      headers: req.headers.set('Authorization', `Bearer ${token}`), // Set Bearer token
      withCredentials: true
    // setHeaders: {
    //   Authorization: `Bearer ${authToken}`
    // }
    });
    return next(cloned).pipe(
      catchError((err: HttpErrorResponse) => {
        if (err.status === 401) {
          authService
            .refreshToken({
              email: authService.getUserDetail()?.email,
              token: authService.getToken() || '',
              refreshToken: authService.getRefreshToken() || '',
            })
            .subscribe({
              next: (response) => {
                if (response.isSuccess) {
                  localStorage.setItem('user', JSON.stringify(response));
                  const cloned = req.clone({
                    setHeaders: {
                      Authorization: `Bearer ${response.token}`,
                    },
                  });
                  location.reload();
                }
              },
              error: () => {
                authService.logout();
                router.navigate(['/login']);
              },
            });
        }
        return throwError(err);
      }
      )
    )
  }
  return next(req);
};
