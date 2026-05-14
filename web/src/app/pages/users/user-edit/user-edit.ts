import { Component, inject } from '@angular/core';
import { RouterLink, ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-user-edit',
  imports: [RouterLink],
  templateUrl: './user-edit.html',
})
export class UserEdit {
  readonly id = inject(ActivatedRoute).snapshot.paramMap.get('id');
}
