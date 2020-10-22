import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { MainComponent } from './main/main.component';
import { NameComponent } from './name/name.component';
import { AngularMaterialModule } from './angular-materials';
import { FlexLayoutModule } from '@angular/flex-layout';
import { environment } from 'src/environments/environment';




@NgModule({
  declarations: [
    AppComponent,
    MainComponent,
    NameComponent

  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    FormsModule,
    HttpClientModule,
    ReactiveFormsModule,
    AngularMaterialModule,
    FlexLayoutModule,
    AppRoutingModule
  ],
  providers: [
    {provide: 'BACKEND_API_URL', useValue: environment.backendApiUrl},
    {provide: 'DEFAULT_LANGUAGE', useValue: environment.defaultLanguage}
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
