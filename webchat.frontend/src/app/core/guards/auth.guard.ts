import { inject } from '@angular/core';
import { CanActivateFn } from '@angular/router';
import { SessionStorageService } from '../services/session-storage.service';

export const authGuard: CanActivateFn = (route, state) => {
  const _sessionService = inject(SessionStorageService);

  if(_sessionService.verifyExists()) {
    return true;
  }

  return false;
};