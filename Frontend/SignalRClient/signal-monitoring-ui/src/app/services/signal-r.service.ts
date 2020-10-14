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


@Injectable({
  providedIn: 'root'
})
export class SignalRService {

  private connection: signalR.HubConnection;
  private connectionImages: signalR.HubConnection;
  readonly POST_URL_UserName: string  = "https://localhost:44389/api/users/sendUser";
  readonly POST_URL_SetTimeOut: string  = "https://localhost:44389/api/admin/setTimeOut";
  readonly POST_URL_GetUserRole: string = "https://localhost:44389/api/work/getUserRole";

  private adminConfig: ConfigModel = new ConfigModel();
  private userConfig: ConfigModel = new ConfigModel();
  private imagesConfig: ConfigModel = new ConfigModel();

  usersObj = new Subject<UserMessageModel>();
  messageObject: UserMessageModel = new UserMessageModel();
  adminMessagesObject: AdminMessageModel = new AdminMessageModel();
  adminObj = new Subject<AdminMessageModel>();

  blobObject = new Subject<BlobModel>();


  constructor(private http: HttpClient) {

  }
  public async startConnection(user: any) {
    this.setConfigurations(this.adminConfig, this.userConfig, this.imagesConfig);
    if (user)
    {
      this.connection = new signalR.HubConnectionBuilder().withUrl(this.adminConfig.chanelUrl).build();
      this.connection.on(this.adminConfig.methodName, (mess) => this.mapMessageAdmin(mess));
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
      this.connection.onclose(async (flag)=> {if (flag){await this.startConnection(user);}});
      this.connectionImages.onclose(async (flag)=> {if (flag){await this.startConnection(user);}});
    }
    try{
      this.connection.start();
      this.connectionImages.start();
      console.log("connected");
    }
    catch(err){
      console.log(err)
      setTimeout(() => {
        this.startConnection(user);
      }, 3000);
    }
  };

  setConfigurations(adminConfig: ConfigModel, userConfig: ConfigModel, imagesConfig: ConfigModel) {
    adminConfig.chanelUrl = "https://localhost:44389/admin";
    adminConfig.methodName = "GetUsers";

    userConfig.chanelUrl = "https://localhost:44389/users";
    userConfig.methodName = "GetMessages";

    imagesConfig.chanelUrl = "https://localhost:44389/images";
    imagesConfig.methodName = "GetImages";
  };

  private mapMessageAdmin(mess: AdminMessageModel): void {
    this.adminObj.next(new AdminMessageModel(mess.name, mess.status));
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

  public broadcastMessage(msgDto: MessageModel) {
    this.http.post(this.POST_URL_UserName, msgDto).subscribe();
  };

  public setTimeOutMessage(sec:number){
    this.http.post(this.POST_URL_SetTimeOut, sec).subscribe();
  };

  getUserRole(user: AdminMessageModel) {
    return this.http.post(this.POST_URL_GetUserRole, user);
  };

  private fromBase64ToImage(image: BlobModel): void {
    this.blobObject.next(new BlobModel(image.blob, image.type));
  };

  public blobMessageReceived(): Observable<BlobModel>{
    return this.blobObject.asObservable();
  };
}
