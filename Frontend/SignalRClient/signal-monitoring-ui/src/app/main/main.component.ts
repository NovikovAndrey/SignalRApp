import { Component } from '@angular/core';
import { HttpClient } from '@aspnet/signalr';
import { Subscription } from 'rxjs';
import { UserMessageModel } from '../models/user-message-model';
import { AdminMessageModel } from '../models/admin-message-model';
import { SignalRService } from '../services/signal-r.service';
import { FormBuilder } from '@angular/forms';
import { TimeOuts } from '../models/timeout-model';
import { UserRoleModel } from '../models/user-role';

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

  constructor(private signalRService: SignalRService, public fb: FormBuilder){ }

  setTimeOutComboBox = this.fb.group({
    setTimeOut: ['']
  })

  async send(user: AdminMessageModel){
    this.signalRService.getUserRole(user).subscribe((data)=> {this.checkUserRole(data, user.status);});
  }
  checkUserRole(checkRole: any, status: number) {
    if(checkRole.role=="Admin")
    {
      this.isAdmin = true;
    }
    else
    {
      this.isAdmin = false;
    }
    this.connections(checkRole, status)
  }
  connections(userCheckedRole: any, status: number) {
    if (status==0)
    {
      this.signalRService.broadcastMessage(new AdminMessageModel(userCheckedRole.name, status));
      this.subscription.unsubscribe();
      this.signalRService.stopConnection();
    }
    else
    {
      this.signalRService.startConnection(this.isAdmin);
      this.signalRService.broadcastMessage(new AdminMessageModel(userCheckedRole.name, status));
      if (this.isAdmin)
      {
        this.subscription=this.signalRService.adminMessageReceived().subscribe((adminmessage) => { this.workWithListUsersNames(adminmessage); });
      }
      else
      {
        this.subscription=this.signalRService.usersMessageReceived().subscribe((usermessage) => { this.addToListUsersMessages(usermessage); });
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
