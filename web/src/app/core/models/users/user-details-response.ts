export interface UserPermissionResponse {
  permission: string;
}

export interface UserDetailsResponse {
  id: number;
  firstName: string;
  lastName: string;
  email: string;
  lastLoginUtc: string | null;
  createdAtUtc: string;
  permissions: UserPermissionResponse[];
}
