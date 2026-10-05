import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { UserService, User } from '../../../services';
import { AuthService } from '../../../services';

@Component({
  selector: 'app-user-list',
  standalone: true,
  imports: [CommonModule, RouterLink, FormsModule],
  template: `
    <div class="d-flex justify-content-between align-items-center mb-3">
      <h2>Usuários</h2>
      <a *ngIf="authService.getUserRole() === 'Admin'" routerLink="/users/new" class="btn btn-primary">
        <i class="bi bi-plus-circle me-1"></i> Novo Usuário
      </a>
    </div>

    <!-- Lista de Usuários -->
    <div class="card">
      <div class="card-body">
        <div *ngIf="loading" class="text-center py-4">
          <div class="spinner-border text-primary" role="status">
            <span class="visually-hidden">Carregando...</span>
          </div>
        </div>

        <div *ngIf="!loading && users.length === 0" class="text-center py-4 text-muted">
          Nenhum usuário encontrado
        </div>

        <div class="table-responsive" *ngIf="!loading && users.length > 0">
          <table class="table table-hover">
            <thead class="table-light">
              <tr>
                <th>ID</th>
                <th>Nome</th>
                <th>Email</th>
                <th>Função</th>
                <th>Criado em</th>
                <th class="text-end">Ações</th>
              </tr>
            </thead>
            <tbody>
              <tr *ngFor="let user of users">
                <td>{{ user.idUsuario }}</td>
                <td>{{ user.nome }}</td>
                <td>{{ user.emailUsuario }}</td>
                <td>
                  <span class="badge" [ngClass]="getRoleClass(user.funcao)">
                    {{ getRoleLabel(user.funcao) }}
                  </span>
                </td>
                <td>{{ formatDate(user.dataCriacao) }}</td>
                <td class="text-end">
                  <div class="btn-group btn-group-sm">
                    <a [routerLink]="['/users', user.idUsuario]" class="btn btn-outline-primary">
                      <i class="bi bi-eye"></i>
                    </a>
                    <a *ngIf="authService.getUserRole() === 'Admin'" [routerLink]="['/users', user.idUsuario, 'edit']" class="btn btn-outline-secondary">
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
          Mostrando {{ (pagination.currentPage - 1) * pagination.pageSize + 1 }} a {{ Math.min(pagination.currentPage * pagination.pageSize, pagination.totalCount) }} de {{ pagination.totalCount }} usuários
        </p>
      </div>
    </div>
  `,
  styles: [`
    .badge { font-size: 0.75rem; padding: 0.35em 0.65em; }
    .badge.bg-user { background-color: #6c757d !important; }
    .badge.bg-support { background-color: #0d6efd !important; }
    .badge.bg-admin { background-color: #dc3545 !important; }
  `]
})
export class UserListComponent implements OnInit {
  users: User[] = [];
  loading = false;
  pagination = {
    currentPage: 1,
    pageSize: 10,
    totalCount: 0,
    totalPages: 0
  };
  Math = Math;

  constructor(
    private userService: UserService,
    public authService: AuthService
  ) {}

  ngOnInit(): void {
    this.loadUsers();
  }

  loadUsers(page: number = 1): void {
    this.loading = true;
    this.pagination.currentPage = page;
    this.userService.getUsers(page, this.pagination.pageSize).subscribe({
      next: (response) => {
        this.users = response.data || response;
        this.loading = false;
      },
      error: () => {
        this.loading = false;
      }
    });
  }

  goToPage(page: number): void {
    if (page >= 1 && page <= this.pagination.totalPages) {
      this.loadUsers(page);
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

  getRoleClass(role: string): string {
    switch (role) {
      case 'User': return 'bg-user';
      case 'Support': return 'bg-support';
      case 'Admin': return 'bg-admin';
      default: return 'bg-secondary';
    }
  }

  getRoleLabel(role: string): string {
    switch (role) {
      case 'User': return 'Usuário';
      case 'Support': return 'Suporte';
      case 'Admin': return 'Admin';
      default: return role;
    }
  }

  formatDate(date: Date | string | null | undefined): string {
    if (!date) return '-';
    const d = new Date(date);
    return d.toLocaleDateString('pt-BR', { day: '2-digit', month: '2-digit', year: 'numeric' });
  }
}