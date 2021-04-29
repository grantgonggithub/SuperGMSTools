import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';

import { FormsModule } from '@angular/forms';
import { HttpModule } from '@angular/http';
import { HttpClientModule } from '@angular/common/http';


import { AppComponent } from './app.component';
import { AppRoutingModule } from './/app-routing.module';
import { ApiListComponent } from './api-list/api-list.component';
import { SvrListComponent } from './svr-list/svr-list.component';
import { ApiHelpService } from './api-help.service';
import { MessagesComponent } from './messages/messages.component';
import { MessageService } from './message.service';

@NgModule({
  declarations: [
    AppComponent,
    // ApiListComponent,
    // SvrListComponent,
    MessagesComponent
  ],
  imports: [
    BrowserModule,
    FormsModule,
    HttpModule,
    AppRoutingModule,
    HttpClientModule
  ],
  providers: [ApiHelpService, MessageService],
  bootstrap: [AppComponent]
})
export class AppModule { }
