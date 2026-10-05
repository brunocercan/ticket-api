import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { HttpParams } from '@angular/common/http';
import { ApiService } from './api.service';
import { Ticket, TicketDetail, TicketFilters, CreateTicketRequest, UpdateTicketRequest, CreateCommentRequest, TicketComment, PagedResponse } from '../models';

@Injectable({
  providedIn: 'root'
})
export class TicketService {
  private endpoint = '/api/tickets';

  constructor(private api: ApiService) {}

  getTickets(filters?: TicketFilters): Observable<PagedResponse<Ticket>> {
    let params = new HttpParams();
    if (filters) {
      if (filters.id) params = params.set('Id', filters.id.toString());
      if (filters.titulo) params = params.set('Titulo', filters.titulo);
      if (filters.prioridade) params = params.set('Prioridade', filters.prioridade);
      if (filters.status) params = params.set('Status', filters.status);
      if (filters.pageNumber) params = params.set('pageNumber', filters.pageNumber.toString());
      if (filters.pageSize) params = params.set('pageSize', filters.pageSize.toString());
    }
    return this.api.get<PagedResponse<Ticket>>(this.endpoint, params);
  }

  getTicketById(id: number): Observable<Ticket> {
    return this.api.get<Ticket>(`${this.endpoint}/${id}`);
  }

  getTicketDetails(filters?: TicketFilters): Observable<TicketDetail[]> {
    let params = new HttpParams();
    if (filters) {
      if (filters.id) params = params.set('Id', filters.id.toString());
      if (filters.titulo) params = params.set('Titulo', filters.titulo);
      if (filters.prioridade) params = params.set('Prioridade', filters.prioridade);
      if (filters.status) params = params.set('Status', filters.status);
    }
    return this.api.get<TicketDetail[]>(`${this.endpoint}/details`, params);
  }

  createTicket(ticket: CreateTicketRequest): Observable<string> {
    return this.api.post<string>(this.endpoint, ticket);
  }

  updateTicket(id: number, ticket: UpdateTicketRequest): Observable<string> {
    return this.api.put<string>(`${this.endpoint}/${id}`, ticket);
  }

  deleteTicket(id: number): Observable<string> {
    return this.api.delete<string>(`${this.endpoint}/${id}`);
  }

  addComment(comment: CreateCommentRequest): Observable<string> {
    return this.api.post<string>(`${this.endpoint}/comment`, comment);
  }
}