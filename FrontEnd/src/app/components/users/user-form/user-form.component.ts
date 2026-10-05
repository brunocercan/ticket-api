import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { UserService, User, CreateUserRequest, UpdateUserRequest } from '../../../services';

interface UserFormModel extends Omit<CreateUserRequest, 'senha'> {
  id?: number;
  dataCriacao?: Date;
  senha?: string;
}

@Component({
  selector: 'app-user-form',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="card">
      <div class="card-header">
        <h3>{{ isEditMode ? 'Editar Usuário' : 'Novo Usuário' }}</h3>
      </div>
      <div class="card-body">
        <form (ngSubmit)="onSubmit()" #userForm="ngForm" novalidate>
          <div class="row g-3">
            <div class="col-md-6">
              <label for="nome" class="form-label">Nome <span class="text-danger">*</span></label>
              <input type="text" class="form-control" id="nome" name="nome" [(ngModel)]="user.nome" required maxlength="150" #nome="ngModel">
              <div *ngIf="nome.invalid && (nome.dirty || nome.touched)" class="text-danger small">
                <div *ngIf="nome.errors?.['required']">Nome é obrigatório</div>
                <div *ngIf="nome.errors?.['maxlength']">Nome deve ter no máximo 150 caracteres</div>
              </div>
            </div>
            <div class="col-md-6">
              <label for="email" class="form-label">Email <span class="text-danger">*</span></label>
              <input type="email" class="form-control" id="email" name="email" [(ngModel)]="user.email" required maxlength="255" #email="ngModel">
              <div *ngIf="email.invalid && (email.dirty || email.touched)" class="text-danger small">
                <div *ngIf="email.errors?.['required']">Email é obrigatório</div>
                <div *ngIf="email.errors?.['email']">Email inválido</div>
                <div *ngIf="email.errors?.['maxlength']">Email deve ter no máximo 255 caracteres</div>
              </div>
            </div>
            <div class="col-md-6">
              <label for="funcao" class="form-label">Função <span class="text-danger">*</span></label>
              <select class="form-select" id="funcao" name="funcao" [(ngModel)]="user.funcao" required #funcao="ngModel">
                <option value="">Selecione...</option>
                <option value="User">Usuário</option>
                <option value="Support">Suporte</option>
                <option value="Admin">Administrador</option>
              </select>
              <div *ngIf="funcao.invalid && (funcao.dirty || funcao.touched)" class="text-danger small">
                Função é obrigatória
              </div>
            </div>
            <div class="col-md-6" *ngIf="!isEditMode">
              <label for="senha" class="form-label">Senha <span class="text-danger">*</span></label>
              <input type="password" class="form-control" id="senha" name="senha" [(ngModel)]="user.senha" required minlength="6" maxlength="500" #senha="ngModel">
              <div *ngIf="senha.invalid && (senha.dirty || senha.touched)" class="text-danger small">
                <div *ngIf="senha.errors?.['required']">Senha é obrigatória</div>
                <div *ngIf="senha.errors?.['minlength']">Senha deve ter no mínimo 6 caracteres</div>
                <div *ngIf="senha.errors?.['maxlength']">Senha deve ter no máximo 500 caracteres</div>
              </div>
            </div>
            <div class="col-md-6" *ngIf="isEditMode">
              <label for="senha" class="form-label">Nova Senha (deixe em branco para manter)</label>
              <input type="password" class="form-control" id="senha" name="senha" [(ngModel)]="user.senha" minlength="6" maxlength="500" #senha="ngModel">
              <div *ngIf="senha.invalid && (senha.dirty || senha.touched)" class="text-danger small">
                <div *ngIf="senha.errors?.['minlength']">Senha deve ter no mínimo 6 caracteres</div>
                <div *ngIf="senha.errors?.['maxlength']">Senha deve ter no máximo 500 caracteres</div>
              </div>
            </div>
            <div class="col-md-6" *ngIf="isEditMode">
              <label for="dataCriacao" class="form-label">Data de Criação <span class="text-danger">*</span></label>
              <input type="datetime-local" class="form-control" id="dataCriacao" name="dataCriacao" [(ngModel)]="user.dataCriacao" required #dataCriacao="ngModel">
              <div *ngIf="dataCriacao.invalid && (dataCriacao.dirty || dataCriacao.touched)" class="text-danger small">
                Data de criação é obrigatória
              </div>
            </div>
          </div>

          <div class="d-flex justify-content-end gap-2 mt-4">
            <button type="button" class="btn btn-secondary" (click)="goBack()">Cancelar</button>
            <button type="submit" class="btn btn-primary" [disabled]="userForm.invalid || submitting">
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
export class UserFormComponent implements OnInit {
  user: UserFormModel = {
    nome: '',
    email: '',
    senha: '',
    funcao: ''
  };

  isEditMode = false;
  userId: number | null = null;
  submitting = false;
  error = '';

  constructor(
    private userService: UserService,
    private route: ActivatedRoute,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.checkEditMode();
  }

  checkEditMode(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.isEditMode = true;
      this.userId = +id;
      this.loadUser(this.userId);
    }
  }

  loadUser(id: number): void {
    this.userService.getUserById(id).subscribe({
      next: (user) => {
        this.user = {
          id: user.idUsuario,
          nome: user.nome,
          email: user.emailUsuario,
          funcao: user.funcao,
          dataCriacao: user.dataCriacao || new Date()
        };
      }
    });
  }

  onSubmit(): void {
    this.submitting = true;
    this.error = '';

    if (this.isEditMode && this.userId) {
      const updateRequest: UpdateUserRequest = {
        id: this.userId,
        nome: this.user.nome,
        email: this.user.email,
        funcao: this.user.funcao,
        dataCriacao: this.user.dataCriacao!,
        senha: this.user.senha || undefined
      };
      this.userService.updateUser(this.userId, updateRequest).subscribe({
        next: () => {
          this.router.navigate(['/users']);
        },
        error: (err: any) => {
          this.error = err.error?.detail || 'Erro ao atualizar usuário';
          this.submitting = false;
        }
      });
    } else {
      const createRequest: CreateUserRequest = {
        nome: this.user.nome,
        email: this.user.email,
        senha: this.user.senha!,
        funcao: this.user.funcao
      };
      this.userService.createUser(createRequest).subscribe({
        next: () => {
          this.router.navigate(['/users']);
        },
        error: (err: any) => {
          this.error = err.error?.detail || 'Erro ao criar usuário';
          this.submitting = false;
        }
      });
    }
  }

  goBack(): void {
    this.router.navigate(['/users']);
  }
}