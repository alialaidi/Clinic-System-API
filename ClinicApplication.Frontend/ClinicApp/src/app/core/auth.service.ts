import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable, tap } from 'rxjs';
import { environment } from '../../environments/environment';
import { LoginRequest, LoginResponse } from './auth.models';

const TOKEN_KEY = 'clinic_access_token';
const USER_KEY = 'clinic_user';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly http = inject(HttpClient);
  private readonly loginUrl = `${environment.apiBaseUrl}/api/auth/login`;

  login(request: LoginRequest): Observable<LoginResponse> {
    return this.http.post<LoginResponse>(this.loginUrl, request).pipe(tap((response) => {
      if (typeof localStorage !== 'undefined') {
        localStorage.setItem(TOKEN_KEY, response.accessToken);
        localStorage.setItem(USER_KEY, JSON.stringify({
          userId: response.userId, email: response.email, role: response.role,
          expiresAtUtc: response.expiresAtUtc, mustChangePassword: response.mustChangePassword
        }));
      }
    }));
  }

  getAccessToken(): string | null {
    return typeof localStorage === 'undefined' ? null : localStorage.getItem(TOKEN_KEY);
  }

  logout(): void {
    if (typeof localStorage !== 'undefined') {
      localStorage.removeItem(TOKEN_KEY);
      localStorage.removeItem(USER_KEY);
    }
  }
}
