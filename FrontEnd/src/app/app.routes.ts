import { Routes } from '@angular/router';
import { TicketListComponent } from './components/tickets/ticket-list/ticket-list.component';
import { TicketDetailComponent } from './components/tickets/ticket-detail/ticket-detail.component';
import { TicketFormComponent } from './components/tickets/ticket-form/ticket-form.component';
import { UserListComponent } from './components/users/user-list/user-list.component';
import { UserFormComponent } from './components/users/user-form/user-form.component';
import { LoginComponent } from './components/auth/login/login.component';
import { adminGuard, supportGuard, userGuard } from './guards/role.guard';

export const routes: Routes = [
  { path: '', redirectTo: '/tickets', pathMatch: 'full' },
  { path: 'login', component: LoginComponent },
  
  // Ticket routes - all authenticated users can view
  { path: 'tickets', component: TicketListComponent, canActivate: [userGuard] },
  { path: 'tickets/new', component: TicketFormComponent, canActivate: [userGuard] },
  { path: 'tickets/:id', component: TicketDetailComponent, canActivate: [userGuard] },
  
  // Ticket edit - only Support and Admin
  { path: 'tickets/:id/edit', component: TicketFormComponent, canActivate: [supportGuard] },
  
  // User routes - only Admin
  { path: 'users', component: UserListComponent, canActivate: [adminGuard] },
  { path: 'users/new', component: UserFormComponent, canActivate: [adminGuard] },
  { path: 'users/:id/edit', component: UserFormComponent, canActivate: [adminGuard] },
  
  { path: '**', redirectTo: '/tickets' }
];