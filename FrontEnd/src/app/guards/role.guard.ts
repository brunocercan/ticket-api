import { CanActivateFn, Router } from '@angular/router';
import { inject } from '@angular/core';
import { AuthService } from '../services';

export const roleGuard: CanActivateFn = (route, state) => {
  const authService = inject(AuthService);
  const router = inject(Router);

  const expectedRoles = route.data['roles'] as string[];

  if (!authService.isLoggedIn()) {
    router.navigate(['/login']);
    return false;
  }

  const userRole = authService.getUserRole();
  
  if (!expectedRoles || expectedRoles.length === 0) {
    return true;
  }

  if (userRole && expectedRoles.includes(userRole)) {
    return true;
  }

  // Redirect to tickets page if not authorized
  router.navigate(['/tickets']);
  return false;
};

export const adminGuard: CanActivateFn = (route, state) => {
  const authService = inject(AuthService);
  const router = inject(Router);

  if (!authService.isLoggedIn()) {
    router.navigate(['/login']);
    return false;
  }

  if (authService.getUserRole() === 'Admin') {
    return true;
  }

  router.navigate(['/tickets']);
  return false;
};

export const supportGuard: CanActivateFn = (route, state) => {
  const authService = inject(AuthService);
  const router = inject(Router);

  if (!authService.isLoggedIn()) {
    router.navigate(['/login']);
    return false;
  }

  const role = authService.getUserRole();
  if (role === 'Admin' || role === 'Support') {
    return true;
  }

  router.navigate(['/tickets']);
  return false;
};

export const userGuard: CanActivateFn = (route, state) => {
  const authService = inject(AuthService);
  const router = inject(Router);

  if (!authService.isLoggedIn()) {
    router.navigate(['/login']);
    return false;
  }

  // All authenticated users can access
  return true;
};