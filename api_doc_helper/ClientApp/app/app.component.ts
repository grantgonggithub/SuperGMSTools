import { Component, OnInit, Injectable} from '@angular/core';
import { Http } from '@angular/http';
import { ServiceInfo } from './ServiceInfo';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})

@Injectable()
export class AppComponent implements OnInit {
  title = '微服务接口文档';
  constructor() {

  }
  ngOnInit() {

  }

}
