import { Injectable } from '@angular/core';
import { Subject } from 'rxjs';
import { SvrInfoParam } from './svrInfoParam';
import { ClassInfo } from './api-list/ClassInfo';


@Injectable({
  providedIn: 'root'
})
export class SvrInfoService {

  private svrInfoSource = new Subject<SvrInfoParam>();
  private apiInfoSource = new Subject<ClassInfo[]>();

  svrInfo$ = this.svrInfoSource.asObservable();
  apiInfo$ = this.apiInfoSource.asObservable();
  constructor() { }

  // Service message commands
  postSvrInfo(info: SvrInfoParam) {
    this.svrInfoSource.next(info);
    console.log('post:' + info.SvrName);
  }

  postApiInfo(info: ClassInfo[]) {
    this.apiInfoSource.next(info);
  }
}
