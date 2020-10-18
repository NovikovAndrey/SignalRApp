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
  readonly POST_URL_SetTimeOut: string  = "https://localhost:44389/api/admin/setTimeOut";
  readonly POST_URL_GetUserRole: string = "https://localhost:44353/api/main/getRoles";

  private adminConfig: ConfigModel;
  private userConfig: ConfigModel;
  private imagesConfig: ConfigModel;
  private nextImageConfig: ConfigModel;
  private userActivitiesConfig: ConfigModel;

  messageObject: UserMessageModel;
  adminMessagesObject: AdminMessageModel = new AdminMessageModel();
  isConnect = false;

  usersObj = new Subject<UserMessageModel>();
  blobObject = new Subject<BlobModel>();
  blobNextObject = new Subject<BlobModel>();
  activeUserObj = new Subject<ActiveUsersModel[]>();
  activitiesUserObj = new Subject<AdminMessagesLog>();

  constructor(private http: HttpClient) {

  }
  public async startConnection(user: MessageModel, isAdmin: boolean) {
    this.setConfigurations();
    try{
      if (isAdmin)
      {
        this.connection = new signalR.HubConnectionBuilder().withUrl(this.adminConfig.chanelUrl).build();
        this.connection.on(this.adminConfig.methodName, (mess) => this.mapActiveUsers(mess));
        this.connection.on(this.userActivitiesConfig.methodName, (mess) => this.mapUsersActivities(mess));
        this.connection.on(this.nextImageConfig.methodName, (image)=> this.fromBase64ToNextImage(image));
      }
      else
      {
        this.connection = new signalR.HubConnectionBuilder().withUrl(this.userConfig.chanelUrl).build();
        this.connection.on(this.userConfig.methodName, (mess) => {this.mapMessageUser(mess); console.log(mess);});
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


  setConfigurations() {

    this.adminConfig = new ConfigModel("https://localhost:44353/Admins", "SendActiveClients");

    this.nextImageConfig = new ConfigModel("https://localhost:44353/Admins", "GetNextImages");

    this.userActivitiesConfig = new ConfigModel("https://localhost:44353/Admins", "SendActivitiesClient");

    this.userConfig = new ConfigModel("https://localhost:44353/Clients", "GetMessages");
    
    this.imagesConfig = new ConfigModel("https://localhost:44353/Images", "GetImages");
  };

  private mapActiveUsers(mess: ActiveUsersModel[]): void {
    this.activeUserObj.next(mess);
 };

  public async stopConnection(user:MessageModel) {
    if(user.role=="Admin")
    {
      this.cleaningAdminssSubscribes(this.activeUserObj, this.activitiesUserObj, this.blobNextObject, this.blobObject)
      this.connection.off("SendActiveClients");
      this.connection.off("GetNextImages");
      this.connection.off("SendActivitiesClient");
    }
    else
    {
      this.cleaningClientsSubscribes(this.usersObj, this.blobObject)
      this.connection.off(this.userConfig.methodName);
    }
    this.connectionImages.off("GetImages");
    this.connectionImages.stop();
    this.connection.stop();
    this.connectionImages = null;
    this.connection = null;
    this.isConnect = false;
  };
  
  cleaningAdminssSubscribes(activeUserObj: Subject<ActiveUsersModel[]>, activitiesUserObj: Subject<AdminMessagesLog>, blobNextObject: Subject<BlobModel>, blobObject: Subject<BlobModel>) {
    activeUserObj.observers.splice(0, activeUserObj.observers.length)
    activitiesUserObj.observers.splice(0, activitiesUserObj.observers.length)
    blobNextObject.observers.splice(0, blobNextObject.observers.length)
    blobObject.observers.splice(0, blobObject.observers.length)
  };

  private mapMessageUser(mess: UserMessageModel): void {
    this.messageObject = new UserMessageModel(mess.rand);
    this.usersObj.next(this.messageObject);
 };

  public usersMessageReceived(): Observable<UserMessageModel> {
      return this.usersObj.asObservable();
  };
  
  cleaningClientsSubscribes(firstObj: Subject<any>, secondObj: Subject<any> ) {
    firstObj.observers.splice(0, firstObj.observers.length)
    secondObj.observers.splice(0, secondObj.observers.length)
  };

  public activeUsersReceived(): Observable<ActiveUsersModel[]> {
    return this.activeUserObj.asObservable();
  };

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
