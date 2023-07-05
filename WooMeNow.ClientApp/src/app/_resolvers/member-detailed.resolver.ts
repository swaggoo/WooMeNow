import { inject } from '@angular/core';
import { ResolveFn } from '@angular/router';
import { MembersService } from '../_services/members.service';
import { Member } from '../_models/member';

export const memberDetailedResolver: ResolveFn<Member> = (route) => {
  return inject(MembersService).getMember(route.paramMap.get('username')!);
};
