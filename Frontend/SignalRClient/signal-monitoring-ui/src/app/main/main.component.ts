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
import { ActiveUsersModel } from '../models/active-users-model';
import { MatTableDataSource } from '@angular/material/table';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-main',
  templateUrl: './main.component.html',
  styleUrls: ['./main.component.scss']
})
export class MainComponent {
  public columns: string[]=["Messages"];
  public messages = new MatTableDataSource<ActiveUsersModel>();

  isAdmin:boolean = false;
  isBlob: boolean = false;
  msgDto: UserMessageModel = new UserMessageModel();
  userMessagesList: UserMessageModel[] = [];
  adminMessagesList: AdminMessageModel[] = [];
  activeUsersList: ActiveUsersModel[] = [];
  adminMessagesLog: AdminMessagesLog[] = [];
  subscription: Subscription;
  userName:string;
  userStatus: string = "active";
  timeOuts: TimeOuts[] = [{value:5, name:"5 sec."} , {value:10, name:"10 sec."}, {value:15, name:"15 sec."}, {value:20, name:"20 sec."}];
  blob: string;
  buttonConnect = "Connect";
  isConnect = false;
  isConnectedParent = false;
  isConnected = false;
  isNextBlob: boolean = false;
  blobNext: string;

  constructor(private signalRService: SignalRService, public fb: FormBuilder){ }

  setTimeOutComboBox = this.fb.group({
    setTimeOut: ['']
  })

  async send(user: AdminMessageModel){
    // this.signalRService.mainConnection();
    this.signalRService.getUserRole(user).subscribe((data)=> {this.parseMesageModel(data, user.status);});
    let t = environment.apiUrl;
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
      this.isAdmin = false;
      this.blob=null;
      this.userMessagesList=null;
      this.activeUsersList = null;
      // this.signalRService.broadcastMessage(user);
      this.subscription.unsubscribe();
      this.signalRService.stopConnection(user);
    }
    else
    {
      this.signalRService.startConnection(user, this.isAdmin);
      // this.signalRService.broadcastMessage(user);
      if (this.isAdmin)
      {
        this.subscription=this.signalRService.activeUsersReceived().subscribe((adminmessage) => { this.workWithListActiveUsers(adminmessage); });
        this.subscription=this.signalRService.usersActivitiesReceived().subscribe((adminmessage) => { this.workWithUsersActivities(adminmessage); });
        this.subscription = this.signalRService.blobNextMessageReceived().subscribe((blob)=> {this.workWithNextBlob(blob)});
      }
      else
      {
        this.subscription=this.signalRService.usersMessageReceived().subscribe((usermessage) => { this.addToListUsersMessages(usermessage); });
      }
      this.subscription = this.signalRService.blobMessageReceived().subscribe((blob)=> {this.workWithBlob(blob)});
    }
     this.checkConnect(this.signalRService.isConnect);
  };

  workWithUsersActivities(adminmessage: AdminMessagesLog) {
    this.adminMessagesLog.push(adminmessage);
  };

  workWithNextBlob(blob: any) {
    this.isNextBlob=true;
    this.blobNext = blob.type+blob.blob;
  };

  checkConnect(isConnect: boolean) {
    if(isConnect){
      this.buttonConnect = "Disconnect";
    }
    else{
      this.buttonConnect = "Connect";
    }
    this.isConnect = isConnect;
  };

  workWithBlob(blob: BlobModel) {
    this.isBlob=true;
    this.blob = blob.type+blob.blob;
  };

  addToListUsersMessages(usermessage: UserMessageModel) {
    if(!this.userMessagesList){
      this.userMessagesList = [];
    }
    let tempMess = new UserMessageModel();
    tempMess.rand=usermessage.rand;
    this.userMessagesList.push(tempMess);
  };

  onSubmit()
  {
    this.signalRService.setTimeOutMessage(this.setTimeOutComboBox.value);
  };

  workWithListActiveUsers(adminmessage: ActiveUsersModel[]) {
    if(!this.activeUsersList){
      this.activeUsersList = [];
    }
    this.activeUsersList.splice(0, this.activeUsersList.length);
    adminmessage.forEach((value) => {this.activeUsersList.push(value);});
    this.messages.data =this.activeUsersList;

    // this.activeUsersList.push(adminmessage);
  };

  // workWithListUsersNames(adminmessage: AdminMessageModel) {
  //   if(adminmessage.status==1)
  //   {
  //     this.adminMessagesList.push(adminmessage);
  //   }
  //   else
  //   {
  //     let temp = this.adminMessagesList.findIndex(element=> element.name==adminmessage.name);
  //     this.adminMessagesList.splice(temp, 1);
  //   }
  //   this.adminMessagesLog.push(new AdminMessagesLog(adminmessage.name, adminmessage.status));
  //   // this.adminMessagesLog.reverse();
  // };
}
