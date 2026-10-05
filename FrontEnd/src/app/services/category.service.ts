import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';
import { Category } from '../models';

@Injectable({
  providedIn: 'root'
})
export class CategoryService {
  private endpoint = '/api/categories';

  constructor(private api: ApiService) {}

  getCategories(): Observable<Category[]> {
    return this.api.get<Category[]>(this.endpoint);
  }

  getCategoryById(id: number): Observable<Category> {
    return this.api.get<Category>(`${this.endpoint}/${id}`);
  }
}