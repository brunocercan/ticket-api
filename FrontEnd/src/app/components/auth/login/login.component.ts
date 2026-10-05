import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService, LoginRequest, CreateUserRequest } from '../../../services';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  template: `
    <div class="container">
      <div class="row justify-content-center mt-5">
        <div class="col-md-6 col-lg-4">
          <div class="card">
            <div class="card-header text-center">
              <h3><i class="bi bi-ticket-perforated me-2"></i>Ticket HelpDesk</h3>
              <p class="text-muted mb-0">{{ isRegisterMode ? 'Criar conta' : 'Entrar na sua conta' }}</p>
            </div>
            <div class="card-body">
              <div *ngIf="error" class="alert alert-danger alert-dismissible fade show" role="alert">
                {{ error }}
                <button type="button" class="btn-close" (click)="error = ''"></button>
              </div>

              <div *ngIf="success" class="alert alert-success alert-dismissible fade show" role="alert">
                {{ success }}
                <button type="button" class="btn-close" (click)="success = ''"></button>
              </div>

              <form (ngSubmit)="onSubmit()" #authForm="ngForm" novalidate>
                <div class="mb-3">
                  <label for="email" class="form-label">Email <span class="text-danger">*</span></label>
                  <input type="email" class="form-control" id="email" name="email" [(ngModel)]="credentials.email" required #email="ngModel">
                  <div *ngIf="email.invalid && (email.dirty || email.touched)" class="text-danger small">
                    <div *ngIf="email.errors?.['required']">Email é obrigatório</div>
                    <div *ngIf="email.errors?.['email']">Email inválido</div>
                  </div>
                </div>

                <div class="mb-3">
                  <label for="senha" class="form-label">{{ isRegisterMode ? 'Senha' : 'Senha' }} <span class="text-danger">*</span></label>
                  <input type="password" class="form-control" id="senha" name="senha" [(ngModel)]="credentials.senha" [required]="!isRegisterMode || credentials.senha" [minlength]="isRegisterMode ? 6 : null" #senha="ngModel">
                  <div *ngIf="senha.invalid && (senha.dirty || senha.touched)" class="text-danger small">
                    <div *ngIf="senha.errors?.['required']">Senha é obrigatória</div>
                    <div *ngIf="senha.errors?.['minlength']">Senha deve ter no mínimo 6 caracteres</div>
                  </div>
                </div>

                <div class="mb-3" *ngIf="isRegisterMode">
                  <label for="nome" class="form-label">Nome <span class="text-danger">*</span></label>
                  <input type="text" class="form-control" id="nome" name="nome" [(ngModel)]="registerData.nome" required maxlength="150" #nome="ngModel">
                  <div *ngIf="nome.invalid && (nome.dirty || nome.touched)" class="text-danger small">
                    <div *ngIf="nome.errors?.['required']">Nome é obrigatório</div>
                    <div *ngIf="nome.errors?.['maxlength']">Nome deve ter no máximo 150 caracteres</div>
                  </div>
                </div>

                <div class="mb-3" *ngIf="isRegisterMode">
                  <label for="funcao" class="form-label">Função <span class="text-danger">*</span></label>
                  <select class="form-select" id="funcao" name="funcao" [(ngModel)]="registerData.funcao" required #funcao="ngModel">
                    <option value="">Selecione...</option>
                    <option value="User">Usuário</option>
                    <option value="Support">Suporte</option>
                  </select>
                  <div *ngIf="funcao.invalid && (funcao.dirty || funcao.touched)" class="text-danger small">
                    Função é obrigatória
                  </div>
                </div>

                <div class="d-grid">
                  <button type="submit" class="btn btn-primary" [disabled]="authForm.invalid || submitting">
                    <span *ngIf="submitting" class="spinner-border spinner-border-sm me-2"></span>
                    {{ isRegisterMode ? 'Registrar' : 'Entrar' }}
                  </button>
                </div>
              </form>

              <div class="text-center mt-3">
                <p class="mb-0">
                  {{ isRegisterMode ? 'Já tem uma conta?' : 'Não tem uma conta?' }}
                  <a href="#" class="ms-1" (click)="toggleMode($event)">{{ isRegisterMode ? 'Entrar' : 'Registrar' }}</a>
                </p>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  `
})
export class LoginComponent implements OnInit {
  isRegisterMode = false;
  submitting = false;
  error = '';
  success = '';

  credentials: LoginRequest = {
    email: '',
    senha: ''
  };

  registerData: CreateUserRequest = {
    nome: '',
    email: '',
    senha: '',
    funcao: 'User'
  };

  constructor(
    private authService: AuthService,
    private router: Router
  ) {}

  ngOnInit(): void {
    if (this.authService.isLoggedIn()) {
      this.router.navigate(['/tickets']);
    }
  }

  toggleMode(event: Event): void {
    event.preventDefault();
    this.isRegisterMode = !this.isRegisterMode;
    this.error = '';
    this.success = '';
  }

  onSubmit(): void {
    this.submitting = true;
    this.error = '';
    this.success = '';

    if (this.isRegisterMode) {
      this.authService.register(this.registerData).subscribe({
        next: () => {
          // Auto-login after registration
          this.authService.login({
            email: this.registerData.email,
            senha: this.registerData.senha
          }).subscribe({
            next: () => {
              this.router.navigate(['/tickets']);
            },
            error: (err) => {
              this.error = err.error?.detail || 'Registro realizado, mas falha ao fazer login automático';
              this.submitting = false;
            }
          });
        },
        error: (err) => {
          this.error = err.error?.detail || 'Erro ao registrar';
          this.submitting = false;
        }
      });
    } else {
      this.authService.login(this.credentials).subscribe({
        next: () => {
          this.success = 'Login realizado com sucesso!';
          setTimeout(() => {
            this.router.navigate(['/tickets']);
          }, 1000);
        },
        error: (err) => {
          if (err.status === 401) {
            this.error = 'Email ou senha incorretos. Verifique suas credenciais.';
          } else {
            this.error = err.error?.detail || 'Erro ao fazer login';
          }
          this.submitting = false;
        }
      });
    }
  }
}