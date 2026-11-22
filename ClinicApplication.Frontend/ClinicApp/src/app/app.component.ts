import { CommonModule } from '@angular/common';
import { HttpErrorResponse } from '@angular/common/http';
import { Component, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { finalize } from 'rxjs';
import { AuthService } from './core/auth.service';

@Component({
  selector: 'app-root', standalone: true, imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './app.component.html', styleUrl: './app.component.css'
})
export class AppComponent {
  private readonly formBuilder = inject(FormBuilder);
  private readonly authService = inject(AuthService);
  readonly loginForm = this.formBuilder.nonNullable.group({
    email: ['', [Validators.required, Validators.email, Validators.maxLength(100)]],
    password: ['', Validators.required]
  });
  passwordVisible = false;
  isSubmitting = false;
  errorMessage = '';
  welcomeMessage = '';
  mustChangePassword = false;

  togglePasswordVisibility(): void { this.passwordVisible = !this.passwordVisible; }

  submit(): void {
    this.errorMessage = '';
    this.welcomeMessage = '';
    if (this.loginForm.invalid) { this.loginForm.markAllAsTouched(); return; }
    this.isSubmitting = true;
    this.authService.login(this.loginForm.getRawValue())
      .pipe(finalize(() => this.isSubmitting = false))
      .subscribe({
        next: (response) => {
          this.mustChangePassword = response.mustChangePassword;
          this.welcomeMessage = `تم تسجيل الدخول بنجاح — ${response.role}`;
        },
        error: (error: HttpErrorResponse) => {
          this.errorMessage = error.status === 401 ? 'البريد الإلكتروني أو كلمة المرور غير صحيحة.'
            : error.status === 0 ? 'تعذر الاتصال بالخادم. تأكد أن الـ API يعمل على localhost:5249.'
              : error.error?.detail ?? 'حدث خطأ أثناء تسجيل الدخول. حاول مرة أخرى.';
        }
      });
  }
}
