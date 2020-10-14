import { Component } from '@angular/core';
import { HttpClient } from '@aspnet/signalr';
import { Subscription } from 'rxjs';
import { UserMessageModel } from '../models/user-message-model';
import { AdminMessageModel } from '../models/admin-message-model';
import { SignalRService } from '../services/signal-r.service';
import { FormBuilder } from '@angular/forms';
import { TimeOuts } from '../models/timeout-model';
import { UserRoleModel } from '../models/user-role';
import { MessageModel } from '../models/message';
import { BlobModel } from '../models/blob-model';
import { AdminMessagesLog } from '../models/admin-message-log-model';

@Component({
  selector: 'app-main',
  templateUrl: './main.component.html',
  styleUrls: ['./main.component.scss']
})
export class MainComponent {
  public columns: string[]=["Name", "status"];
  isAdmin:boolean = false;
  isBlob: boolean = false;
  msgDto: UserMessageModel = new UserMessageModel();
  userMessagesList: UserMessageModel[] = [];
  adminMessagesList: AdminMessageModel[] = [];
  adminMessagesLog: AdminMessagesLog[] = [];
  subscription: Subscription;
  userName:string;
  userStatus: string = "active";
  timeOuts: TimeOuts[] = [{value:5, name:"5 sec."} , {value:10, name:"10 sec."}, {value:15, name:"15 sec."}, {value:20, name:"20 sec."}];
  blob: string;

  constructor(private signalRService: SignalRService, public fb: FormBuilder){ }

  setTimeOutComboBox = this.fb.group({
    setTimeOut: ['']
  })

  async send(user: AdminMessageModel){
    this.signalRService.getUserRole(user).subscribe((data)=> {this.parseMesageModel(data, user.status);});
  };

  parseMesageModel(checkRole: any, status: number)
  {
    this.checkUserRole(new MessageModel(checkRole.name, status, checkRole.role))
  };

  checkUserRole(user: MessageModel) {
    if(user.role=="Admin")
    {
      this.isAdmin = true;
    }
    else
    {
      this.isAdmin = false;
    }
    this.connections(user)
  };

  connections(user: MessageModel) {
    if (user.status==0)
    {
      this.signalRService.broadcastMessage(user);
      this.subscription.unsubscribe();
      this.signalRService.stopConnection();
    }
    else
    {
      this.signalRService.startConnection(this.isAdmin);
      this.signalRService.broadcastMessage(user);
      if (this.isAdmin)
      {
        this.subscription=this.signalRService.adminMessageReceived().subscribe((adminmessage) => { this.workWithListUsersNames(adminmessage); });
      }
      else
      {
        this.subscription=this.signalRService.usersMessageReceived().subscribe((usermessage) => { this.addToListUsersMessages(usermessage); });
      }
      this.subscription = this.signalRService.blobMessageReceived().subscribe((blob)=> {this.workWithBlob(blob)});
    }
  };

  workWithBlob(blob: BlobModel) {
    this.isBlob=true;
    this.blob = blob.type+blob.blob;
  }

  addToListUsersMessages(usermessage: UserMessageModel) {
    let tempMess = new UserMessageModel();
    tempMess.rand=usermessage.rand;
    this.userMessagesList.push(tempMess);
  };

  onSubmit()
  {
    this.signalRService.setTimeOutMessage(this.setTimeOutComboBox.value);
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
    this.adminMessagesLog.push(new AdminMessagesLog(adminmessage.name, adminmessage.status))
  };
}
