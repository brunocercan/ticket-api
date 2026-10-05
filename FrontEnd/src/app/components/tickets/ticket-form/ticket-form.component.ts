import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { TicketService, CreateTicketRequest, UpdateTicketRequest, Ticket } from '../../../services';
import { UserService, User } from '../../../services';
import { AuthService } from '../../../services';

interface TicketFormModel extends CreateTicketRequest {
  id?: number;
  dataCriacao?: Date;
  dataAlteracao?: Date;
  dataFechamento?: Date;
}

@Component({
  selector: 'app-ticket-form',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="card">
      <div class="card-header">
        <h3>{{ isEditMode ? 'Editar Ticket' : 'Novo Ticket' }}</h3>
      </div>
      <div class="card-body">
        <form (ngSubmit)="onSubmit()" #ticketForm="ngForm" novalidate>
          <div class="row g-3">
            <div class="col-md-8">
              <label for="titulo" class="form-label">Título <span class="text-danger">*</span></label>
              <input type="text" class="form-control" id="titulo" name="titulo" [(ngModel)]="ticket.titulo" required maxlength="200" #titulo="ngModel">
              <div *ngIf="titulo.invalid && (titulo.dirty || titulo.touched)" class="text-danger small">
                <div *ngIf="titulo.errors?.['required']">Título é obrigatório</div>
                <div *ngIf="titulo.errors?.['maxlength']">Título deve ter no máximo 200 caracteres</div>
              </div>
            </div>
            <div class="col-md-4">
              <label for="prioridade" class="form-label">Prioridade <span class="text-danger">*</span></label>
              <select class="form-select" id="prioridade" name="prioridade" [(ngModel)]="ticket.prioridade" required #prioridade="ngModel">
                <option value="">Selecione...</option>
                <option value="Low">Baixa</option>
                <option value="Medium">Média</option>
                <option value="High">Alta</option>
                <option value="Critical">Crítica</option>
              </select>
              <div *ngIf="prioridade.invalid && (prioridade.dirty || prioridade.touched)" class="text-danger small">
                Prioridade é obrigatória
              </div>
            </div>
            <div class="col-md-4">
              <label for="status" class="form-label">Status <span class="text-danger">*</span></label>
              <select class="form-select" id="status" name="status" [(ngModel)]="ticket.status" required #status="ngModel">
                <option value="">Selecione...</option>
                <option value="Open">Aberto</option>
                <option value="InProgress">Em Progresso</option>
                <option value="Resolved">Resolvido</option>
              </select>
              <div *ngIf="status.invalid && (status.dirty || status.touched)" class="text-danger small">
                Status é obrigatório
              </div>
            </div>
            <div class="col-md-4">
              <label for="categoria" class="form-label">Categoria <span class="text-danger">*</span></label>
              <select class="form-select" id="categoria" name="categoria" [(ngModel)]="ticket.idCategoria" required #categoria="ngModel">
                <option value="">Selecione...</option>
                <option *ngFor="let cat of categories" [value]="cat.id">{{ cat.nome }}</option>
              </select>
              <div *ngIf="categoria.invalid && (categoria.dirty || categoria.touched)" class="text-danger small">
                Categoria é obrigatória
              </div>
            </div>
          </div>

          <div class="mt-3">
            <label for="descricao" class="form-label">Descrição <span class="text-danger">*</span></label>
            <textarea class="form-control" id="descricao" name="descricao" [(ngModel)]="ticket.descricao" rows="4" required maxlength="500" #descricao="ngModel"></textarea>
            <div *ngIf="descricao.invalid && (descricao.dirty || descricao.touched)" class="text-danger small">
              <div *ngIf="descricao.errors?.['required']">Descrição é obrigatória</div>
              <div *ngIf="descricao.errors?.['maxlength']">Descrição deve ter no máximo 500 caracteres</div>
            </div>
          </div>

          <!-- Solicitante - User só pode criar para si mesmo, Admin/Support podem escolher -->
          <div class="row g-3 mt-3" *ngIf="!isUserRole">
            <div class="col-md-6">
              <label for="solicitante" class="form-label">Solicitante <span class="text-danger">*</span></label>
              <select class="form-select" id="solicitante" name="solicitante" [(ngModel)]="ticket.idSolicitante" required #solicitante="ngModel">
                <option value="">Selecione...</option>
                <option *ngFor="let user of users" [value]="user.idUsuario">{{ user.nome }} ({{ user.emailUsuario }})</option>
              </select>
              <div *ngIf="solicitante.invalid && (solicitante.dirty || solicitante.touched)" class="text-danger small">
                Solicitante é obrigatório
              </div>
            </div>
          </div>

          <!-- Para User, mostra info mas não permite alterar -->
          <div class="row g-3 mt-3" *ngIf="isUserRole && !isEditMode">
            <div class="col-md-6">
              <label class="form-label">Solicitante <span class="text-danger">*</span></label>
              <input type="text" class="form-control" value="{{ getCurrentUserName() }}" readonly>
            </div>
          </div>

          <div class="row g-3 mt-3">
            <div class="col-md-6">
              <label for="responsavel" class="form-label">Responsável</label>
              <select class="form-select" id="responsavel" name="responsavel" [(ngModel)]="ticket.idVinculado">
                <option value="">Não atribuído</option>
                <option *ngFor="let user of supportUsers" [value]="user.idUsuario">{{ user.nome }} ({{ user.emailUsuario }})</option>
              </select>
            </div>
          </div>

          <div *ngIf="isEditMode" class="row g-3 mt-3">
            <div class="col-md-6">
              <label for="dataCriacao" class="form-label">Data de Criação <span class="text-danger">*</span></label>
              <input type="datetime-local" class="form-control" id="dataCriacao" name="dataCriacao" [(ngModel)]="ticket.dataCriacao" required #dataCriacao="ngModel">
              <div *ngIf="dataCriacao.invalid && (dataCriacao.dirty || dataCriacao.touched)" class="text-danger small">
                Data de criação é obrigatória
              </div>
            </div>
            <div class="col-md-6">
              <label for="dataAlteracao" class="form-label">Data de Alteração <span class="text-danger">*</span></label>
              <input type="datetime-local" class="form-control" id="dataAlteracao" name="dataAlteracao" [(ngModel)]="ticket.dataAlteracao" required #dataAlteracao="ngModel">
              <div *ngIf="dataAlteracao.invalid && (dataAlteracao.dirty || dataAlteracao.touched)" class="text-danger small">
                Data de alteração é obrigatória
              </div>
            </div>
            <div class="col-md-6">
              <label for="dataFechamento" class="form-label">Data de Fechamento <span class="text-danger">*</span></label>
              <input type="datetime-local" class="form-control" id="dataFechamento" name="dataFechamento" [(ngModel)]="ticket.dataFechamento" required #dataFechamento="ngModel">
              <div *ngIf="dataFechamento.invalid && (dataFechamento.dirty || dataFechamento.touched)" class="text-danger small">
                Data de fechamento é obrigatória
              </div>
            </div>
          </div>

          <div class="d-flex justify-content-end gap-2 mt-4">
            <button type="button" class="btn btn-secondary" (click)="goBack()">Cancelar</button>
            <button type="submit" class="btn btn-primary" [disabled]="ticketForm.invalid || submitting">
              <span *ngIf="submitting" class="spinner-border spinner-border-sm me-2" role="status"></span>
              {{ isEditMode ? 'Atualizar' : 'Criar' }}
            </button>
          </div>
        </form>

        <div *ngIf="error" class="alert alert-danger mt-3">
          {{ error }}
        </div>
      </div>
    </div>
  `
})
export class TicketFormComponent implements OnInit {
  ticket: TicketFormModel = {
    titulo: '',
    descricao: '',
    prioridade: '',
    status: '',
    idCategoria: 0,
    idSolicitante: 0,
    idVinculado: null
  };

  isEditMode = false;
  ticketId: number | null = null;
  submitting = false;
  error = '';
  users: User[] = [];
  supportUsers: User[] = [];
  categories: any[] = [];

  get isUserRole(): boolean {
    return this.authService.getUserRole() === 'User';
  }

  getCurrentUserName(): string {
    const user = this.authService.getCurrentUser();
    return user ? user.nome : '';
  }

  getCurrentUserId(): number {
    const user = this.authService.getCurrentUser();
    return user ? user.idUsuario : 0;
  }

  constructor(
    private ticketService: TicketService,
    private userService: UserService,
    private route: ActivatedRoute,
    private router: Router,
    public authService: AuthService
  ) {}

  ngOnInit(): void {
    this.loadUsers();
    this.loadCategories();
    this.checkEditMode();
  }

  loadUsers(): void {
    this.userService.getUsers(1, 1000).subscribe({
      next: (response) => {
        this.users = response.data || response;
        this.supportUsers = this.users.filter(u => u.funcao === 'Support');
      }
    });
  }

  loadCategories(): void {
    this.categories = [
      { id: 1, nome: 'Hardware' },
      { id: 2, nome: 'Software' },
      { id: 3, nome: 'Rede' },
      { id: 4, nome: 'Acesso' }
    ];
  }

  checkEditMode(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.isEditMode = true;
      this.ticketId = +id;
      this.loadTicket(this.ticketId);
    } else if (this.isUserRole) {
      // User creating new ticket - auto-set solicitante to themselves
      this.ticket.idSolicitante = this.getCurrentUserId();
    }
  }

  loadTicket(id: number): void {
    this.ticketService.getTicketById(id).subscribe({
      next: (ticket) => {
        this.ticket = {
          id: ticket.id,
          titulo: ticket.titulo,
          descricao: ticket.descricao,
          prioridade: ticket.prioridade,
          status: ticket.status,
          idCategoria: ticket.idCategoria,
          idSolicitante: ticket.idSolicitante,
          idVinculado: ticket.idVinculado,
          dataCriacao: ticket.dataCriacao,
          dataAlteracao: ticket.dataAtualizacao || new Date(),
          dataFechamento: ticket.dataFechamento || new Date()
        };
      }
    });
  }

  onSubmit(): void {
    this.submitting = true;
    this.error = '';

    if (this.isEditMode && this.ticketId) {
      const updateRequest: UpdateTicketRequest = {
        id: this.ticketId,
        titulo: this.ticket.titulo,
        descricao: this.ticket.descricao,
        prioridade: this.ticket.prioridade,
        status: this.ticket.status,
        idCategoria: this.ticket.idCategoria,
        idSolicitante: this.ticket.idSolicitante,
        idVinculado: this.ticket.idVinculado || 0,
        dataCriacao: this.ticket.dataCriacao!,
        dataAlteracao: this.ticket.dataAlteracao!,
        dataFechamento: this.ticket.dataFechamento!
      };
      this.ticketService.updateTicket(this.ticketId, updateRequest).subscribe({
        next: () => {
          this.router.navigate(['/tickets', this.ticketId]);
        },
        error: (err: any) => {
          this.error = err.error?.detail || 'Erro ao atualizar ticket';
          this.submitting = false;
        }
      });
    } else {
      const createRequest: CreateTicketRequest = {
        titulo: this.ticket.titulo,
        descricao: this.ticket.descricao,
        prioridade: this.ticket.prioridade,
        status: this.ticket.status,
        idCategoria: this.ticket.idCategoria,
        idSolicitante: this.ticket.idSolicitante,
        idVinculado: this.ticket.idVinculado
      };
      this.ticketService.createTicket(createRequest).subscribe({
        next: () => {
          this.router.navigate(['/tickets']);
        },
        error: (err: any) => {
          this.error = err.error?.detail || 'Erro ao criar ticket';
          this.submitting = false;
        }
      });
    }
  }

  goBack(): void {
    if (this.isEditMode && this.ticketId) {
      this.router.navigate(['/tickets', this.ticketId]);
    } else {
      this.router.navigate(['/tickets']);
    }
  }
}