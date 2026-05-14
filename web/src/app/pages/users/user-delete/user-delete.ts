import { Component, inject } from '@angular/core';
import { RouterLink, ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-user-delete',
  imports: [RouterLink],
  templateUrl: './user-delete.html',
})
export class UserDelete {
  readonly id = inject(ActivatedRoute).snapshot.paramMap.get('id');
}
