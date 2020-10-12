import { Component } from '@angular/core';
import { HttpClient } from '@aspnet/signalr';
import { Subscription } from 'rxjs';
import { UserMessageModel } from './models/user-message-model';
import { AdminMessageModel } from './models/admin-message-model';
import { SignalRService } from './services/signal-r.service';
import { MatTableDataSource } from '@angular/material/table';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent {
  public columns: string[] = ["name", "status"];
  public activeUsers: MatTableDataSource<AdminMessageModel> = new MatTableDataSource<AdminMessageModel>();
  isAdmin: boolean = false;
  msgDto: UserMessageModel = new UserMessageModel();
  userMessagesList: UserMessageModel[] = [];
  adminMessagesList: AdminMessageModel[] = [];
  title = 'message-ui';
  disconnect: boolean = true;
  connected: boolean = false;
  subscription: Subscription;
  button: string = "Connect";
  userDto: AdminMessageModel = new AdminMessageModel();

  constructor(private signalRService: SignalRService) {

  }

  async send(user: string) {
    if (this.connected) {

      this.button = "Connect";
      this.signalRService.broadcastMessage(this.userDto.name = user, this.userDto.status=0);
      this.subscription.unsubscribe();
      this.signalRService.stopConnection();
      if (user != "Admin")
      {
        this.signalRService.removeFromActiveUsers(user);
      }
    }
    else {
      this.signalRService.startConnection(user);
      this.signalRService.broadcastMessage(this.userDto);
      if (user == "Admin") {
        this.subscription = this.signalRService.adminMessageReceived().subscribe((adminmessage) => { this.addToLIstAdmin(adminmessage); });
        this.isAdmin = true;
      }
      else {
        this.subscription = this.signalRService.usersMessageReceived().subscribe((usermessage) => { this.addToLIstUser(usermessage); });
      }
      this.button = "Disconnect";
    }
    this.connected = !this.connected;
  };

  removeFromActiveUsers(user: string) {

    let index = this.activeUsers.data.findIndex(x=> x.name == user);
  };

  addToLIstUser(usermessage: UserMessageModel) {
    let tempMess = new UserMessageModel();
    tempMess.rand = usermessage.rand;
    this.userMessagesList.push(tempMess);
  };

  addToLIstAdmin(adminmessage: AdminMessageModel) {

    // this.activeUsers._updateChangeSubscription();
    let tempMess = new AdminMessageModel();
    tempMess.name = adminmessage.name;
    this.adminMessagesList.push(tempMess);
    this.activeUsers.data.push(tempMess);
    this.activeUsers._updateChangeSubscription();
  }
}
