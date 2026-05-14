import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-user-list',
  imports: [RouterLink],
  templateUrl: './user-list.html',
})
export class UserList {
  readonly users = [
    {
      id: 1,
      firstName: 'Jan',
      lastName: 'Kowalski',
      email: 'jan@example.com',
      permissions: [
        { permission: 'users.read' },
        { permission: 'users.write' },
        { permission: 'settings.read' },
      ],
    },
    {
      id: 2,
      firstName: 'Anna',
      lastName: 'Nowak',
      email: 'anna@example.com',
      permissions: [
        { permission: 'users.read' },
      ],
    },
    {
      id: 3,
      firstName: 'Piotr',
      lastName: 'Wiśniewski',
      email: 'piotr@example.com',
      permissions: [],
    },
  ];
}
