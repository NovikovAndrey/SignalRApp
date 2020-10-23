export class AdminMessagesLog{
  Name: string;
  Status:string;

  constructor(name?: string, status?: number){
    this.Name = name;
    if (status==1){
      this.Status = "joined"
    }
    else{
      this.Status = "disconnected"
    }
  }
}
