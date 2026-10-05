import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { TicketService, TicketDetail, TicketComment, CreateCommentRequest } from '../../../services';
import { UserService, User } from '../../../services';
import { AuthService } from '../../../services';

@Component({
  selector: 'app-ticket-detail',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  template: `
    <div *ngIf="loading" class="text-center py-5">
      <div class="spinner-border text-primary" role="status">
        <span class="visually-hidden">Carregando...</span>
      </div>
    </div>

    <div *ngIf="!loading && ticket" class="card">
      <div class="card-header d-flex justify-content-between align-items-center">
        <h3 class="mb-0">{{ ticket.titulo }}</h3>
        <div>
          <a *ngIf="authService.getUserRole() === 'Admin' || authService.getUserRole() === 'Support'" [routerLink]="['/tickets', ticket.id, 'edit']" class="btn btn-outline-secondary btn-sm">
            <i class="bi bi-pencil me-1"></i> Editar
          </a>
          <button *ngIf="authService.getUserRole() === 'Admin' || authService.getUserRole() === 'Support'" class="btn btn-outline-danger btn-sm ms-2" (click)="deleteTicket()">
            <i class="bi bi-trash me-1"></i> Excluir
          </button>
        </div>
      </div>

      <div class="card-body">
        <div class="row mb-4">
          <div class="col-md-3">
            <strong>ID:</strong> {{ ticket.id }}
          </div>
          <div class="col-md-3">
            <strong>Prioridade:</strong>
            <span class="badge ms-2" [ngClass]="getPriorityClass(ticket.prioridade)">
              {{ getPriorityLabel(ticket.prioridade) }}
            </span>
          </div>
          <div class="col-md-3">
            <strong>Status:</strong>
            <span class="badge ms-2" [ngClass]="getStatusClass(ticket.status)">
              {{ getStatusLabel(ticket.status) }}
            </span>
          </div>
          <div class="col-md-3">
            <strong>Categoria:</strong> {{ ticket.nomeCategoria }}
          </div>
        </div>

        <div class="row mb-4">
          <div class="col-md-6">
            <strong>Solicitante:</strong> {{ ticket.nomeSolicitante }}
          </div>
          <div class="col-md-6">
            <strong>Responsável:</strong> {{ ticket.nomeResponsavelChamado || 'Não atribuído' }}
          </div>
        </div>

        <div class="row mb-4">
          <div class="col-md-4">
            <strong>Criado em:</strong> {{ formatDate(ticket.dataCriacao) }}
          </div>
          <div class="col-md-4">
            <strong>Atualizado em:</strong> {{ ticket.dataAtualizacao ? formatDate(ticket.dataAtualizacao) : '-' }}
          </div>
          <div class="col-md-4">
            <strong>Fechado em:</strong> {{ ticket.dataFechamento ? formatDate(ticket.dataFechamento) : '-' }}
          </div>
        </div>

        <hr>

        <h5>Descrição</h5>
        <p class="text-muted">{{ ticket.descricao }}</p>

        <hr>

        <h5>Comentários <span class="badge bg-primary ms-2">{{ ticket.detalhesTicket?.length || 0 }}</span></h5>

        <!-- Add Comment Form -->
        <div class="card mb-3">
          <div class="card-body">
            <form (ngSubmit)="addComment()" #commentForm="ngForm">
              <div class="mb-3">
                <label for="comment" class="form-label">Novo Comentário</label>
                <textarea class="form-control" id="comment" name="comment" [(ngModel)]="newComment" rows="3" required maxlength="500" #commentInput="ngModel"></textarea>
                <div *ngIf="commentInput.invalid && (commentInput.dirty || commentInput.touched)" class="text-danger small">
                  Comentário é obrigatório (máx. 500 caracteres)
                </div>
              </div>
              <button type="submit" class="btn btn-primary" [disabled]="commentForm.invalid || addingComment">
                <span *ngIf="addingComment" class="spinner-border spinner-border-sm me-2"></span>
                Adicionar Comentário
              </button>
            </form>
          </div>
        </div>

        <!-- Comments List -->
        <div *ngIf="ticket.detalhesTicket && ticket.detalhesTicket.length > 0">
          <div class="card mb-2" *ngFor="let comment of ticket.detalhesTicket">
            <div class="card-body">
              <div class="d-flex justify-content-between mb-2">
                <strong>{{ getCommentAuthor(comment.idUsuario) }}</strong>
                <small class="text-muted">{{ formatDate(comment.dataCriacao) }}</small>
              </div>
              <p class="mb-0">{{ comment.conteudo }}</p>
            </div>
          </div>
        </div>

        <div *ngIf="!ticket.detalhesTicket || ticket.detalhesTicket.length === 0" class="text-center text-muted py-3">
          Nenhum comentário ainda. Seja o primeiro a comentar!
        </div>
      </div>
    </div>

    <div *ngIf="!loading && !ticket" class="text-center py-5">
      <div class="alert alert-warning">Ticket não encontrado</div>
      <a routerLink="/tickets" class="btn btn-primary">Voltar para lista</a>
    </div>
  `
})
export class TicketDetailComponent implements OnInit {
  ticket: TicketDetail | null = null;
  loading = true;
  newComment = '';
  addingComment = false;
  users: User[] = [];

  constructor(
    private ticketService: TicketService,
    private userService: UserService,
    private route: ActivatedRoute,
    private router: Router,
    public authService: AuthService
  ) {}

  ngOnInit(): void {
    this.loadUsers();
    this.loadTicket();
  }

  loadUsers(): void {
    this.userService.getUsers(1, 1000).subscribe({
      next: (response) => {
        this.users = response.data || response;
      }
    });
  }

  loadTicket(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.ticketService.getTicketDetails({ id: +id }).subscribe({
        next: (tickets) => {
          this.ticket = tickets[0] || null;
          this.loading = false;
        },
        error: () => {
          this.loading = false;
        }
      });
    }
  }

  addComment(): void {
    if (!this.newComment.trim() || !this.ticket) return;

    this.addingComment = true;
    const comment: CreateCommentRequest = {
      ticketId: this.ticket.id,
      userId: 1, // TODO: Get current user ID from auth
      content: this.newComment
    };

    this.ticketService.addComment(comment).subscribe({
      next: () => {
        this.newComment = '';
        this.addingComment = false;
        this.loadTicket(); // Reload to get updated comments
      },
      error: () => {
        this.addingComment = false;
      }
    });
  }

  deleteTicket(): void {
    if (!this.ticket || !confirm('Tem certeza que deseja excluir este ticket?')) return;

    this.ticketService.deleteTicket(this.ticket.id).subscribe({
      next: () => {
        this.router.navigate(['/tickets']);
      }
    });
  }

  getCommentAuthor(userId: number): string {
    const user = this.users.find(u => u.idUsuario === userId);
    return user ? user.nome : `Usuário ID: ${userId}`;
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

  formatDate(date: Date | string | undefined): string {
    if (!date) return '-';
    const d = new Date(date);
    return d.toLocaleDateString('pt-BR', { day: '2-digit', month: '2-digit', year: 'numeric', hour: '2-digit', minute: '2-digit' });
  }
}