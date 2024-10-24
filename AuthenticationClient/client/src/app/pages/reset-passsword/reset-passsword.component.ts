import { Component, inject, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { ResetPasswordRequest } from '../../interfaces/reset-password-request';
import { AuthService } from '../../services/auth.service';
import { ActivatedRoute, Router } from '@angular/router';
import { HttpErrorResponse } from '@angular/common/http';

@Component({
  selector: 'app-reset-passsword',
  standalone: true,
  imports: [FormsModule, MatSnackBarModule],
  templateUrl: './reset-passsword.component.html',
  styleUrl: './reset-passsword.component.css'
})
export class ResetPassswordComponent implements OnInit {

  // resetPassword = {} as ResetPasswordRequest;
  resetPassword: ResetPasswordRequest = {
    email: 'your_email@example.com',
    token: 'your_token', //get this from the params from the URL
    newPassword: 'your_new_password'
  } as ResetPasswordRequest;

  authService = inject(AuthService);
  router = inject(Router);
  route = inject(ActivatedRoute);
  matSnackBar = inject(MatSnackBar);
  ngOnInit(): void {
    this.route.queryParams.subscribe(params => {
      //Crucial: Check for email and token presence and validity
      if (params['email'] && params['token']) {
        this.resetPassword.email = params['email'];
        this.resetPassword.token = params['token'];
      } else {
        this.matSnackBar.open('Invalid or missing email/token.', 'Close', { duration: 5000 });
        // Consider navigating back or to an error page
        this.router.navigate(['/error']); // Example
      }
    });
  }
  resetPasswordHandle =()=>{
    if (!this.resetPassword.email || !this.resetPassword.newPassword) {
      this.matSnackBar.open("Email and new password are required.", "OK");
      return;
    }
    this.authService.resetPassword(this.resetPassword).subscribe({
      next: (response: any) => { // Important: Ensure type matches the response
        if (response.isSuccess) {  // Check for success (crucial)
          this.matSnackBar.open(response.message, 'Close', { duration: 5000 });
          this.router.navigate(['/login']);
        }
        else {
          this.matSnackBar.open(response.message || "Password reset failed.", "OK"); // Or more specific message
        }
      },
      error: (error: HttpErrorResponse) => {
        let errorMessage = "An error occurred.";
        if (error.error && typeof error.error === 'object' && error.error.message) {
          errorMessage = error.error.message; // Extract message from API response
        }
          this.matSnackBar.open(errorMessage, 'Close', { duration: 5000 });
      },
    });
  }
}
