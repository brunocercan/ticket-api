import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { TicketService, Ticket, TicketFilters, PagedResponse } from '../../../services';
import { UserService, User } from '../../../services';
import { AuthService } from '../../../services';

@Component({
  selector: 'app-ticket-list',
  standalone: true,
  imports: [CommonModule, RouterLink, FormsModule],
  template: `
    <div class="d-flex justify-content-between align-items-center mb-3">
      <h2>Tickets</h2>
      <a routerLink="/tickets/new" class="btn btn-primary">
        <i class="bi bi-plus-circle me-1"></i> Novo Ticket
      </a>
    </div>

    <!-- Filtros -->
    <div class="card mb-4">
      <div class="card-body">
        <form (ngSubmit)="applyFilters()" #filterForm="ngForm">
          <div class="row g-3">
            <div class="col-md-3">
              <label class="form-label">ID</label>
              <input type="number" class="form-control" name="id" [(ngModel)]="filters.id" placeholder="ID">
            </div>
            <div class="col-md-3">
              <label class="form-label">Título</label>
              <input type="text" class="form-control" name="titulo" [(ngModel)]="filters.titulo" placeholder="Título">
            </div>
            <div class="col-md-2">
              <label class="form-label">Prioridade</label>
              <select class="form-select" name="prioridade" [(ngModel)]="filters.prioridade">
                <option value="">Todas</option>
                <option value="Low">Baixa</option>
                <option value="Medium">Média</option>
                <option value="High">Alta</option>
                <option value="Critical">Crítica</option>
              </select>
            </div>
            <div class="col-md-2">
              <label class="form-label">Status</label>
              <select class="form-select" name="status" [(ngModel)]="filters.status">
                <option value="">Todos</option>
                <option value="Open">Aberto</option>
                <option value="InProgress">Em Progresso</option>
                <option value="Resolved">Resolvido</option>
              </select>
            </div>
            <div class="col-md-2 d-flex align-items-end">
              <button type="submit" class="btn btn-outline-primary me-2">
                <i class="bi bi-funnel me-1"></i> Filtrar
              </button>
              <button type="button" class="btn btn-outline-secondary" (click)="clearFilters()">
                <i class="bi bi-x-circle me-1"></i> Limpar
              </button>
            </div>
          </div>
        </form>
      </div>
    </div>

    <!-- Lista de Tickets -->
    <div class="card">
      <div class="card-body">
        <div *ngIf="loading" class="text-center py-4">
          <div class="spinner-border text-primary" role="status">
            <span class="visually-hidden">Carregando...</span>
          </div>
        </div>

        <div *ngIf="!loading && tickets.length === 0" class="text-center py-4 text-muted">
          Nenhum ticket encontrado
        </div>

        <div class="table-responsive" *ngIf="!loading && tickets.length > 0">
          <table class="table table-hover">
            <thead class="table-light">
              <tr>
                <th>ID</th>
                <th>Título</th>
                <th>Prioridade</th>
                <th>Status</th>
                <th>Solicitante</th>
                <th>Responsável</th>
                <th>Criado em</th>
                <th class="text-end">Ações</th>
              </tr>
            </thead>
            <tbody>
              <tr *ngFor="let ticket of tickets" (click)="goToDetail(ticket.id)" style="cursor: pointer;">
                <td>{{ ticket.id }}</td>
                <td>{{ ticket.titulo }}</td>
                <td>
                  <span class="badge" [ngClass]="getPriorityClass(ticket.prioridade)">
                    {{ getPriorityLabel(ticket.prioridade) }}
                  </span>
                </td>
                <td>
                  <span class="badge" [ngClass]="getStatusClass(ticket.status)">
                    {{ getStatusLabel(ticket.status) }}
                  </span>
                </td>
                <td>{{ getUserName(ticket.idSolicitante) }}</td>
                <td>{{ ticket.idVinculado ? getUserName(ticket.idVinculado) : '-' }}</td>
                <td>{{ formatDate(ticket.dataCriacao) }}</td>
                <td class="text-end">
                  <div class="btn-group btn-group-sm">
                    <a [routerLink]="['/tickets', ticket.id]" class="btn btn-outline-primary" (click)="$event.stopPropagation()">
                      <i class="bi bi-eye"></i>
                    </a>
                    <a *ngIf="authService.getUserRole() === 'Admin' || authService.getUserRole() === 'Support'" [routerLink]="['/tickets', ticket.id, 'edit']" class="btn btn-outline-secondary" (click)="$event.stopPropagation()">
                      <i class="bi bi-pencil"></i>
                    </a>
                  </div>
                </td>
              </tr>
            </tbody>
          </table>
        </div>

        <!-- Paginação -->
        <nav *ngIf="pagination.totalPages > 1" aria-label="Pagination">
          <ul class="pagination justify-content-center">
            <li class="page-item" [class.disabled]="pagination.currentPage === 1">
              <a class="page-link" href="#" (click)="$event.preventDefault(); goToPage(pagination.currentPage - 1)">Anterior</a>
            </li>
            <li class="page-item" *ngFor="let page of getPagesArray()" [class.active]="page === pagination.currentPage">
              <a class="page-link" href="#" (click)="$event.preventDefault(); goToPage(page)">{{ page }}</a>
            </li>
            <li class="page-item" [class.disabled]="pagination.currentPage === pagination.totalPages">
              <a class="page-link" href="#" (click)="$event.preventDefault(); goToPage(pagination.currentPage + 1)">Próximo</a>
            </li>
          </ul>
        </nav>
        <p class="text-center text-muted small" *ngIf="pagination.totalCount > 0">
          Mostrando {{ (pagination.currentPage - 1) * pagination.pageSize + 1 }} a {{ Math.min(pagination.currentPage * pagination.pageSize, pagination.totalCount) }} de {{ pagination.totalCount }} tickets
        </p>
      </div>
    </div>
  `,
  styles: [`
    .badge { font-size: 0.75rem; padding: 0.35em 0.65em; }
    .badge.bg-low { background-color: #6c757d !important; }
    .badge.bg-medium { background-color: #0dcaf0 !important; color: #000; }
    .badge.bg-high { background-color: #fd7e14 !important; }
    .badge.bg-critical { background-color: #dc3545 !important; }
    .badge.bg-open { background-color: #198754 !important; }
    .badge.bg-inprogress { background-color: #0d6efd !important; }
    .badge.bg-resolved { background-color: #6c757d !important; }
  `]
})
export class TicketListComponent implements OnInit {
  tickets: Ticket[] = [];
  loading = false;
  filters: TicketFilters = { pageNumber: 1, pageSize: 10 };
  pagination = {
    currentPage: 1,
    pageSize: 10,
    totalCount: 0,
    totalPages: 0
  };
  users: User[] = [];
  Math = Math;

  constructor(
    private ticketService: TicketService,
    private userService: UserService,
    public authService: AuthService
  ) {}

  ngOnInit(): void {
    this.loadUsers();
    this.loadTickets();
  }

  loadUsers(): void {
    this.userService.getUsers(1, 1000).subscribe({
      next: (response) => {
        this.users = response.data || response;
      }
    });
  }

  loadTickets(page: number = 1): void {
    this.loading = true;
    this.filters.pageNumber = page;
    this.filters.pageSize = this.pagination.pageSize;
    this.ticketService.getTickets(this.filters).subscribe({
      next: (response: PagedResponse<Ticket>) => {
        this.tickets = response.data;
        this.pagination.currentPage = response.currentPage;
        this.pagination.totalPages = response.totalPages;
        this.pagination.pageSize = response.pageSize;
        this.pagination.totalCount = response.totalCount;
        this.loading = false;
      },
      error: () => {
        this.loading = false;
      }
    });
  }

  applyFilters(): void {
    this.loadTickets(1);
  }

  clearFilters(): void {
    this.filters = { pageNumber: 1, pageSize: 10 };
    this.loadTickets(1);
  }

  goToPage(page: number): void {
    if (page >= 1 && page <= this.pagination.totalPages) {
      this.loadTickets(page);
    }
  }

  getPagesArray(): number[] {
    const pages = [];
    const start = Math.max(1, this.pagination.currentPage - 2);
    const end = Math.min(this.pagination.totalPages, start + 4);
    for (let i = start; i <= end; i++) {
      pages.push(i);
    }
    return pages;
  }

  goToDetail(id: number): void {
    // Navigation handled by routerLink
  }

  getPriorityClass(priority: string): string {
    switch (priority) {
      case 'Low': return 'bg-low';
      case 'Medium': return 'bg-medium';
      case 'High': return 'bg-high';
      case 'Critical': return 'bg-critical';
      default: return 'bg-secondary';
    }
  }

  getPriorityLabel(priority: string): string {
    switch (priority) {
      case 'Low': return 'Baixa';
      case 'Medium': return 'Média';
      case 'High': return 'Alta';
      case 'Critical': return 'Crítica';
      default: return priority;
    }
  }

  getStatusClass(status: string): string {
    switch (status) {
      case 'Open': return 'bg-open';
      case 'InProgress': return 'bg-inprogress';
      case 'Resolved': return 'bg-resolved';
      default: return 'bg-secondary';
    }
  }

  getStatusLabel(status: string): string {
    switch (status) {
      case 'Open': return 'Aberto';
      case 'InProgress': return 'Em Progresso';
      case 'Resolved': return 'Resolvido';
      default: return status;
    }
  }

  getUserName(userId: number): string {
    const user = this.users.find(u => u.idUsuario === userId);
    return user ? user.nome : `ID: ${userId}`;
  }

  formatDate(date: Date | string | undefined): string {
    if (!date) return '-';
    const d = new Date(date);
    return d.toLocaleDateString('pt-BR', { day: '2-digit', month: '2-digit', year: 'numeric', hour: '2-digit', minute: '2-digit' });
  }
}