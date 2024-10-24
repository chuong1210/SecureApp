import { Component, inject } from '@angular/core';
import { AuthService } from '../../services/auth.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-account',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './account.component.html',
  styleUrl: './account.component.css',
})
export class AccountComponent {
  authService = inject(AuthService);
  accountDetail$ = this.authService.getDetail();


  // userId!: string; // ID của người dùng
  // userDetail: any; // Thông tin chi tiết người dùng

  // constructor(private route: ActivatedRoute, private authService: AuthService) {}

  // ngOnInit(): void {
  //   // Lấy ID người dùng từ route
  //   this.userId = this.route.snapshot.paramMap.get('id')!;

  //   // Gọi service để lấy thông tin người dùng
  //   this.authService.getUserDetail(this.userId).subscribe({
}
