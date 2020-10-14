import { Component, EventEmitter, Output } from '@angular/core';

@Component({
  selector: 'app-name',
  templateUrl: './name.component.html',
  styleUrls: ['./name.component.scss']
})
export class NameComponent {
  userName:string;
  button: string ="Connect";
  isConnect: boolean=false;
  @Output()
  sendButton: EventEmitter<any> = new EventEmitter();
  constructor(){

  }

  async send(){
    if(this.isConnect)
    {
      this.button = "Connect";
      this.isConnect=!this.isConnect;
      this.sendButton.emit({name:this.userName, status:0});
    }
    else
    {
      this.button = "Disconnect";
      this.isConnect=!this.isConnect;
      this.sendButton.emit({name:this.userName, status:1});
    }
  }
}
