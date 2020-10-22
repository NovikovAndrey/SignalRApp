import { Component, Inject } from '@angular/core';


@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent {

  title = 'angular-signalR';
  language: string;

  constructor(@Inject('DEFAULT_LANGUAGE') private defaultLanguage: string){
    this.language = defaultLanguage;
  }

}
