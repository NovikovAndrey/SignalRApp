export class UserRoleModel{
  public userName: string;
  public userRole: string;

  constructor(name?: string, role?: string)
  {
    this.userName = name;
    this.userRole = role;
  }
}
