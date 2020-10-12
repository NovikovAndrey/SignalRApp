import { EventEmitter, Injectable } from '@angular/core';
import * as signalR from '@aspnet/signalr'
import { HttpClient } from '@angular/common/http';
import { Observable, Subject } from 'rxjs';
import { UserMessageModel } from '../models/user-message-model';
import { AdminMessageModel } from '../models/admin-message-model';
import { ConfigModel } from '../models/config-mogel';


@Injectable({
  providedIn: 'root'
})
export class SignalRService {

  // private connection: signalR.HubConnection = new signalR.HubConnectionBuilder().withUrl("https://localhost:44398/chat").build();
  // readonly POST_URL: string  = "https://localhost:44398/api/chat/sendUser";
  private connection: signalR.HubConnection;
  readonly POST_URL: string  = "https://localhost:44389/api/users/sendUser";

  private adminConfig: ConfigModel = new ConfigModel();
  private userConfig: ConfigModel = new ConfigModel();

  usersObj = new Subject<UserMessageModel>();
  messageObject: UserMessageModel = new UserMessageModel();
  adminMessagesObject: AdminMessageModel = new AdminMessageModel();
  adminObj = new Subject<AdminMessageModel>();

  constructor(private http: HttpClient) {

  }
  public async startConnection(user: string) {
    this.setConfigurations(this.adminConfig, this.userConfig);
    if (user == "Admin")
    {
      this.connection = new signalR.HubConnectionBuilder().withUrl(this.adminConfig.chanelUrl).build();
      this.connection.on(this.adminConfig.methodName, (mess) => this.mapMessageAdmin(mess));
    }
    else
    {
      this.connection = new signalR.HubConnectionBuilder().withUrl(this.userConfig.chanelUrl).build();
      this.connection.on(this.userConfig.methodName, (mess) => this.mapMessageUser(mess));
    }
    if(this.connection.onclose.length==0)
    {
      this.connection.onclose(async (flag)=> {if (flag){await this.startConnection(user);}});
    }
    // this.connectionConfig(user, this.adminConfig, this.userConfig);
    // if(user=="Admin")
    // {

    //   this.connection = new signalR.HubConnectionBuilder().withUrl("https://localhost:44389/admin").build();
    //   this.connection.on("GetUsers", (mess) => this.mapMessageAdmin(mess));
    // }
    // else
    // {
    //   this.connection = new signalR.HubConnectionBuilder().withUrl("https://localhost:44389users").build();
    //   this.connection.on("GetMessages", (mess) => this.mapMessageUser(mess));
    // }
    try{
      this.connection.start();
      console.log("connected");
    }
    catch(err){
      console.log(err)
      setTimeout(() => {
        this.startConnection(user);
      }, 3000);
    }
  };

  setConfigurations(adminConfig: ConfigModel, userConfig: ConfigModel) {
    adminConfig.chanelUrl = "https://localhost:44389/admin";
    adminConfig.methodName = "GetUsers";

    userConfig.chanelUrl = "https://localhost:44389/users";
    userConfig.methodName = "GetMessages";
  };

  // connectionConfig(user: string, adminConfig: ConfigModel, userConfig: ConfigModel) {
  //   throw new Error('Method not implemented.');
  // };

  private mapMessageAdmin(mess: AdminMessageModel): void {
    this.adminMessagesObject.name = mess.name;
    this.adminObj.next(this.adminMessagesObject);
 };

  public async stopConnection() {
    this.connection.off("GetMessage");
    this.connection.off("GetUsers");
    this.connection.stop();
  };

  private mapMessageUser(mess: UserMessageModel): void {
    this.messageObject.rand = mess.rand;
    this.usersObj.next(this.messageObject);
 };

  public usersMessageReceived(): Observable<UserMessageModel> {
    return this.usersObj.asObservable();
  };

  public adminMessageReceived(): Observable<AdminMessageModel> {
    return this.adminObj.asObservable();
  };

  public broadcastMessage(msgDto: AdminMessageModel) {
    this.http.post(this.POST_URL, msgDto).subscribe(data => console.log("fdgdsfgdfg"));
    // this.connection.invoke("SendMessage1", msgDto.user, msgDto.msgText).catch(err => console.error(err));    // This can invoke the server method named as "SendMethod1" directly.
  };

  removeFromActiveUsers(user: string) {
    throw new Error('Method not implemented.');
  }
}
