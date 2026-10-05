import { Component } from '@angular/core';
import { RouterOutlet, RouterLink, RouterLinkActive } from '@angular/router';
import { CommonModule } from '@angular/common';
import { AuthService } from './services';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, RouterOutlet, RouterLink, RouterLinkActive],
  template: `
    <nav class="navbar navbar-expand-lg navbar-dark bg-primary mb-4">
      <div class="container">
        <a class="navbar-brand" routerLink="/tickets">
          <i class="bi bi-ticket-perforated me-2"></i>
          Ticket HelpDesk
        </a>
        <button class="navbar-toggler" type="button" data-bs-toggle="collapse" data-bs-target="#navbarNav">
          <span class="navbar-toggler-icon"></span>
        </button>
        <div class="collapse navbar-collapse" id="navbarNav">
          <ul class="navbar-nav me-auto">
            <li class="nav-item" *ngIf="authService.isLoggedIn()">
              <a class="nav-link" routerLink="/tickets" routerLinkActive="active" [routerLinkActiveOptions]="{exact: true}">
                <i class="bi bi-list-ul me-1"></i> Tickets
              </a>
            </li>
            <li class="nav-item" *ngIf="authService.isLoggedIn()">
              <a class="nav-link" routerLink="/tickets/new" routerLinkActive="active">
                <i class="bi bi-plus-circle me-1"></i> Novo Ticket
              </a>
            </li>
            <li class="nav-item" *ngIf="authService.isLoggedIn()">
              <a class="nav-link" routerLink="/users" routerLinkActive="active">
                <i class="bi bi-people me-1"></i> Usuários
              </a>
            </li>
          </ul>
          <ul class="navbar-nav ms-auto" *ngIf="authService.isLoggedIn()">
            <li class="nav-item">
              <button class="nav-link btn btn-link text-white p-0" (click)="logout()">
                <i class="bi bi-box-arrow-right me-1"></i> Sair
              </button>
            </li>
          </ul>
          <ul class="navbar-nav ms-auto" *ngIf="!authService.isLoggedIn()">
            <li class="nav-item">
              <a class="nav-link" routerLink="/login">
                <i class="bi bi-box-arrow-in-right me-1"></i> Entrar
              </a>
            </li>
          </ul>
        </div>
      </div>
    </nav>

    <div class="container">
      <router-outlet></router-outlet>
    </div>

    <div class="footer mt-5 py-3 bg-light border-top">
      <div class="container text-center text-muted">
        <small>Ticket HelpDesk System - Angular + ASP.NET Core</small>
      </div>
  `,
  styles: [`
    .footer {
      position: relative;
      bottom: 0;
      width: 100%;
    }
  `]
})
export class AppComponent {
  title = 'Ticket HelpDesk';

  constructor(public authService: AuthService) {}

  logout(): void {
    this.authService.logout();
  }
}