import { Component } from '@angular/core';
import { HttpClient } from '@aspnet/signalr';
import { Subscription } from 'rxjs';
import { UserMessageModel } from '../models/user-message-model';
import { AdminMessageModel } from '../models/admin-message-model';
import { SignalRService } from '../services/signal-r.service';

@Component({
  selector: 'app-main',
  templateUrl: './main.component.html',
  styleUrls: ['./main.component.scss']
})
export class MainComponent {
  public cokumns: string[]=["Name", "status"];
  isAdmin:boolean = false;
  msgDto: UserMessageModel = new UserMessageModel();
  userMessagesList: UserMessageModel[] = [];
  adminMessagesList: AdminMessageModel[] = [];
  // title = 'message-ui';
  disconnect: boolean = true;
  connected: boolean = false;
  subscription: Subscription;
  // button:string = "Connect";
  userName:string;
  // userDto: AdminMessageModel= new AdminMessageModel();

  constructor(private signalRService: SignalRService){

  }

  async send(user: any){
    if (this.connected)
    {
      // this.button="Connect";
      this.signalRService.broadcastMessage(new AdminMessageModel(user.name, user.status));
      this.subscription.unsubscribe();
      this.signalRService.stopConnection();
    }
    else
    {
      this.signalRService.startConnection(user.name);
      this.signalRService.broadcastMessage(new AdminMessageModel(user.name, user.status));
      if (user.name == "Admin")
      {
        this.subscription=this.signalRService.adminMessageReceived().subscribe((adminmessage) => { this.workWithListUsersNames(adminmessage); });
        this.isAdmin = true;
      }
      else
      {
        this.subscription=this.signalRService.usersMessageReceived().subscribe((usermessage) => { this.addToListUsersMessages(usermessage); });
      }
      // this.button="Disconnect";
    }
    this.connected=!this.connected;
  }

  addToListUsersMessages(usermessage: UserMessageModel) {
    let tempMess = new UserMessageModel();
    tempMess.rand=usermessage.rand;
    this.userMessagesList.push(tempMess);
  };

  workWithListUsersNames(adminmessage: AdminMessageModel) {
    if(adminmessage.status==1)
    {
      this.adminMessagesList.push(adminmessage);
    }
    else
    {
      let temp = this.adminMessagesList.findIndex(element=> element.name==adminmessage.name);
      this.adminMessagesList.splice(temp, 1);
    }
  }
}
