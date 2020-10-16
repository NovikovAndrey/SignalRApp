import { EventEmitter, Injectable } from '@angular/core';
import * as signalR from '@aspnet/signalr'
import { HttpClient } from '@angular/common/http';
import { Observable, Subject } from 'rxjs';
import { UserMessageModel } from '../models/user-message-model';
import { AdminMessageModel } from '../models/admin-message-model';
import { ConfigModel } from '../models/config-mogel';
import { nextTick } from 'process';
import { MessageModel } from '../models/message';
import { BlobModel } from '../models/blob-model';
import { ActiveUsersModel } from '../models/active-users-model';
import { AdminMessagesLog } from '../models/admin-message-log-model';


@Injectable({
  providedIn: 'root'
})
export class SignalRService {

  private connection: signalR.HubConnection;
  private connectionImages: signalR.HubConnection;
  // readonly POST_URL_UserName: string  = "https://localhost:44389/api/users/sendUser";
  readonly POST_URL_SetTimeOut: string  = "https://localhost:44389/api/admin/setTimeOut";
  // readonly POST_URL_GetUserRole: string = "https://localhost:44389/api/work/getUserRole";
  readonly POST_URL_GetUserRole: string = "https://localhost:44353/api/main/getRoles";

  private adminConfig: ConfigModel = new ConfigModel();
  private userConfig: ConfigModel = new ConfigModel();
  private imagesConfig: ConfigModel = new ConfigModel();
  private nextImageConfig: ConfigModel = new ConfigModel();
  private userActivitiesConfig: ConfigModel = new ConfigModel();

  usersObj = new Subject<UserMessageModel>();
  messageObject: UserMessageModel = new UserMessageModel();
  adminMessagesObject: AdminMessageModel = new AdminMessageModel();
  adminObj = new Subject<AdminMessageModel>();
  isConnect = false;
  blobObject = new Subject<BlobModel>();
  blobNextObject = new Subject<BlobModel>();
  activeUserObj = new Subject<ActiveUsersModel[]>();
  activitiesUserObj = new Subject<AdminMessagesLog>();



  constructor(private http: HttpClient) {

  }
  public async startConnection(user: MessageModel, isAdmin: boolean) {
    this.setConfigurations(this.adminConfig, this.userConfig, this.imagesConfig, this.nextImageConfig, this.userActivitiesConfig);
    try{
      if (isAdmin)
      {
        this.connection = new signalR.HubConnectionBuilder().withUrl(this.adminConfig.chanelUrl).build();
        this.connection.on(this.adminConfig.methodName, (mess) => this.mapActiveUsers(mess));
        this.connection.on(this.userActivitiesConfig.methodName, (mess) => this.mapUsersActivities(mess));
        // this.connection.on(this.adminConfig.methodName, (mess) => this.mapMessageAdmin(mess));
        this.connection.on(this.nextImageConfig.methodName, (image)=> this.fromBase64ToNextImage(image));
      }
      else
      {
        this.connection = new signalR.HubConnectionBuilder().withUrl(this.userConfig.chanelUrl).build();
        this.connection.on(this.userConfig.methodName, (mess) => this.mapMessageUser(mess));
      }
      this.connectionImages = new signalR.HubConnectionBuilder().withUrl(this.imagesConfig.chanelUrl).build();
      this.connectionImages.on(this.imagesConfig.methodName, (image)=> this.fromBase64ToImage(image));
      if(this.connection.onclose.length==0)
      {
        this.connection.onclose(async (flag)=> {if (flag){await this.startConnection(user, isAdmin);}});
        this.connectionImages.onclose(async (flag)=> {if (flag){await this.startConnection(user, isAdmin);}});
      }
      this.connection.start().then(()=>this.connection.invoke("Connect", user.name));
      this.connectionImages.start();

      this.isConnect = true;

      await this.delay(3000);
    }
    catch(err){
      console.log(err)
      setTimeout(() => {
        this.startConnection(user, isAdmin);
      }, 3000);
    }
  };

  async delay(ms: number) {
    await new Promise(resolve => setTimeout(()=>resolve(), ms)).then(()=>console.log("fired"));
}

  mapUsersActivities(mess: any): void {
    this.activitiesUserObj.next(new AdminMessagesLog(mess.name, mess.status));
  };

  usersActivitiesReceived(): Observable<AdminMessagesLog>  {
    return this.activitiesUserObj.asObservable();
  };


  setConfigurations(adminConfig: ConfigModel, userConfig: ConfigModel, imagesConfig: ConfigModel, nextImageConfig: ConfigModel, userActivitiesConfig:ConfigModel) {
    // adminConfig.chanelUrl = "https://localhost:44389/admin";
    // adminConfig.methodName = "GetUsers";

    adminConfig.chanelUrl = "https://localhost:44353/Admins";
    adminConfig.methodName = "SendActiveClients";

    nextImageConfig.chanelUrl = "https://localhost:44353/Admins";
    nextImageConfig.methodName = "GetNextImages";

    userActivitiesConfig.chanelUrl = "https://localhost:44353/Admins";
    userActivitiesConfig.methodName = "SendActivitiesClient";

    // userConfig.chanelUrl = "https://localhost:44389/users";
    userConfig.chanelUrl = "https://localhost:44353/Clients";
    userConfig.methodName = "GetMessages";

    imagesConfig.chanelUrl = "https://localhost:44353/Images";
    imagesConfig.methodName = "GetImages";
  };

  private mapMessageAdmin(mess: AdminMessageModel): void {
    this.adminObj.next(new AdminMessageModel(mess.name, mess.status));
 };

  private mapActiveUsers(mess: ActiveUsersModel[]): void {
    // mess.forEach( function (value) )
    this.activeUserObj.next(mess);
 };

  public async stopConnection(user:MessageModel) {
    this.connection.off("SendActiveClients");
    this.connection.off("GetNextImages");
    this.connection.off("SendActivitiesClient");
    this.connection.off(this.userConfig.methodName);
    this.connection.off("GetImages");
    // this.connection.invoke("Disconnect", user.name)
    this.connection.stop();
    this.connection = null;
    this.isConnect = false;
  };

  private mapMessageUser(mess: UserMessageModel): void {
    this.messageObject.rand = mess.rand;
    this.usersObj.next(this.messageObject);
 };

  public usersMessageReceived(): Observable<UserMessageModel> {
    return this.usersObj.asObservable();
  };

  public activeUsersReceived(): Observable<ActiveUsersModel[]> {
    return this.activeUserObj.asObservable();
  }

  public adminMessageReceived(): Observable<AdminMessageModel> {
    return this.adminObj.asObservable();
  };

  // public broadcastMessage(msgDto: MessageModel) {
  //   this.http.post(this.POST_URL_UserName, msgDto).subscribe();
  // };

  public setTimeOutMessage(sec:number){
    this.http.post(this.POST_URL_SetTimeOut, sec).subscribe();
  };

  getUserRole(user: AdminMessageModel) {
    // return this.http.post(this.POST_URL_GetUserRole, user);
    return this.http.get(this.POST_URL_GetUserRole, {params:{name: user.name}});
  };

  private fromBase64ToImage(image: BlobModel): void {
    this.blobObject.next(new BlobModel(image.blob, image.type));
  };

  public blobMessageReceived(): Observable<BlobModel>{
    return this.blobObject.asObservable();
  };

  private fromBase64ToNextImage(image: BlobModel): void {
    this.blobNextObject.next(new BlobModel(image.blob, image.type));
  };

  blobNextMessageReceived(): Observable<BlobModel> {
    return this.blobNextObject.asObservable();
  }
}
