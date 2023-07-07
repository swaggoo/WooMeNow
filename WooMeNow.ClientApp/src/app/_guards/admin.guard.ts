import { inject } from '@angular/core';
import { CanActivateFn } from '@angular/router';
import { AccountService } from '../_services/account.service';
import { map } from 'rxjs/operators';
import { ToastrService } from 'ngx-toastr';

export const adminGuard: CanActivateFn = () => {
  const accountService = inject(AccountService);
  const toastrService = inject(ToastrService);

  return accountService.currentUser$.pipe(
    map(user => {
      if (!user) return false;
      if (user.roles.includes('Admin') || user.roles.includes('Moderator')) {
        return true;
      } else {
        toastrService.error('You cannot enter this area');
        return false;
      }
    })
  );
};
