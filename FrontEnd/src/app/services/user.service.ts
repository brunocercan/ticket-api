import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { HttpParams } from '@angular/common/http';
import { ApiService } from './api.service';
import { User, CreateUserRequest, UpdateUserRequest, LoginRequest, LoginResponse } from '../models';

@Injectable({
  providedIn: 'root'
})
export class UserService {
  private endpoint = '/api/users';
  private authEndpoint = '/api/auth';

  constructor(private api: ApiService) {}

  getUsers(pageNumber: number = 1, pageSize: number = 10): Observable<any> {
    let params = new HttpParams()
      .set('pageNumber', pageNumber.toString())
      .set('pageSize', pageSize.toString());
    return this.api.get<any>(this.endpoint, params);
  }

  getUserById(id: number): Observable<User> {
    return this.api.get<User>(`${this.endpoint}/${id}`);
  }

  createUser(user: CreateUserRequest): Observable<User> {
    return this.api.post<User>(this.endpoint, user);
  }

  updateUser(id: number, user: UpdateUserRequest): Observable<User> {
    return this.api.put<User>(`${this.endpoint}/${id}`, user);
  }

  deleteUser(id: number): Observable<void> {
    return this.api.delete<void>(`${this.endpoint}/${id}`);
  }

  login(credentials: LoginRequest): Observable<LoginResponse> {
    return this.api.post<LoginResponse>(`${this.authEndpoint}/login`, credentials);
  }

  register(user: CreateUserRequest): Observable<User> {
    return this.api.post<User>(`${this.authEndpoint}/register`, user);
  }
}