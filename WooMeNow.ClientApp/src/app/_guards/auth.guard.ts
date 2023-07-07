import { inject } from '@angular/core';
import { CanActivateFn } from '@angular/router';
import { map } from 'rxjs';
import { AccountService } from '../_services/account.service';
import { ToastrService } from 'ngx-toastr';

export const AuthGuard: CanActivateFn =
    () => {
      const accountService = inject(AccountService);
      const toastrService = inject(ToastrService);

      return accountService.currentUser$.pipe(
        map(user => {
          if (user) return true;
        else {
          toastrService.error('You need to log in for this operation')
          return false;
        }
        })
      )
    };