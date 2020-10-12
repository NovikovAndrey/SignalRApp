export class AdminMessageModel
{
  public name: string='';
  public status: number=0;

  constructor(userName?: string, userStatus?:number)
  {
    this.name = userName;
    this.status = userStatus;
  }
}
