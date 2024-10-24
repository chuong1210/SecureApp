import { Component, inject } from '@angular/core';
import { AuthService } from '../../services/auth.service';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { FormsModule } from '@angular/forms';
import { MatIconModule } from '@angular/material/icon';
import { HttpErrorResponse } from '@angular/common/http';

@Component({
  selector: 'app-forget-passsword',
  standalone: true,
  imports: [FormsModule, MatIconModule, MatSnackBarModule],
  templateUrl: './forget-passsword.component.html',
  styleUrl: './forget-passsword.component.css'
})
export class ForgetPassswordComponent {
  email!: string;
  authService = inject(AuthService);
  matSnackbar = inject(MatSnackBar);
  showEmailSent = false;
  isSubmitting = false;

  forgetPassword=()=>
  {
    this.isSubmitting=true;
    this.authService.forgotPassword(this.email).subscribe({
      next:(res)=>
      {
        if(res.isSuccess){
          this.matSnackbar.open(res.message,"Close",{
            duration:5000
          })
          this.showEmailSent=true;
        }
        else
        {
          this.matSnackbar.open(res.message,"Close",{duration:5000})
        }
      },
      error: (error: HttpErrorResponse) => {
        this.matSnackbar.open(error.message, 'Close', {
          duration: 5000,
        });
      },
      complete: () => {
        this.isSubmitting = false;
      },
    })
  }
}
