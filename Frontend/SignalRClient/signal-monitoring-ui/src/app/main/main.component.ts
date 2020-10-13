import { Component } from '@angular/core';
import { HttpClient } from '@aspnet/signalr';
import { Subscription } from 'rxjs';
import { UserMessageModel } from '../models/user-message-model';
import { AdminMessageModel } from '../models/admin-message-model';
import { SignalRService } from '../services/signal-r.service';
import { FormBuilder } from '@angular/forms';
import { TimeOuts } from '../models/timeout-model';

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
  subscription: Subscription;
  userName:string;
  timeOuts: TimeOuts[] = [{value:5, name:"5 sec."} , {value:10, name:"10 sec."}, {value:15, name:"15 sec."}, {value:20, name:"20 sec."}];

  constructor(private signalRService: SignalRService, public fb: FormBuilder){

  }

  setTimeOutComboBox = this.fb.group({
    setTimeOut: ['']
  })

  async send(user: any){

    if (user.status==0)
    {
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
        this.isAdmin = false;
      }
    }
  }

  addToListUsersMessages(usermessage: UserMessageModel) {
    let tempMess = new UserMessageModel();
    tempMess.rand=usermessage.rand;
    this.userMessagesList.push(tempMess);
  };

  onSubmit()
  {
    this.signalRService.setTimeOutMessage(this.setTimeOutComboBox.value);
    // this.setTimeOutComboBox.value
  }

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
