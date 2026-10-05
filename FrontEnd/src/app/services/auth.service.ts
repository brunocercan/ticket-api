import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, tap } from 'rxjs';
import { UserService } from '../services';
import { LoginRequest, LoginResponse, CreateUserRequest, User } from '../models';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private currentUserSubject = new BehaviorSubject<User | null>(null);
  public currentUser$ = this.currentUserSubject.asObservable();

  private tokenKey = 'auth_token';

  constructor(private userService: UserService) {
    this.loadToken();
  }

  private loadToken(): void {
    const token = localStorage.getItem(this.tokenKey);
    if (token) {
      this.decodeToken(token);
    }
  }

  private decodeToken(token: string): void {
    try {
      const payload = JSON.parse(atob(token.split('.')[1]));
      const user: User = {
        idUsuario: parseInt(payload.nameid || payload.sub) || 0,
        nome: payload.unique_name || '',
        emailUsuario: payload.email || '',
        funcao: payload.role || '',
        dataCriacao: payload.iat ? new Date(payload.iat * 1000) : undefined
      };
      this.currentUserSubject.next(user);
    } catch (e) {
      console.error('Error decoding token:', e);
    }
  }

  login(credentials: LoginRequest): Observable<LoginResponse> {
    return this.userService.login(credentials).pipe(
      tap(response => {
        localStorage.setItem(this.tokenKey, response.token);
        this.decodeToken(response.token);
      })
    );
  }

  register(userData: CreateUserRequest): Observable<User> {
    return this.userService.register(userData);
  }

  logout(): void {
    localStorage.removeItem(this.tokenKey);
    this.currentUserSubject.next(null);
  }

  getToken(): string | null {
    return localStorage.getItem(this.tokenKey);
  }

  isLoggedIn(): boolean {
    return !!this.getToken();
  }

  getUserRole(): string | null {
    const user = this.currentUserSubject.value;
    return user?.funcao || null;
  }

  getCurrentUser(): User | null {
    return this.currentUserSubject.value;
  }
}