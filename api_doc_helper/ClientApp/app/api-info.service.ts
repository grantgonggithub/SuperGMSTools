import { Injectable } from '@angular/core';
import { Subject } from 'rxjs';
import { SvrInfoParam } from './svrInfoParam';

@Injectable({
  providedIn: 'root'
})
export class ApiInfoService {

  private apiInfoSource = new Subject<SvrInfoParam>();
  apiInfo$ = this.apiInfoSource.asObservable();
  constructor() { }

  // Service message commands
  postApiInfo(info: SvrInfoParam) {
    this.apiInfoSource.next(info);
  }
}
