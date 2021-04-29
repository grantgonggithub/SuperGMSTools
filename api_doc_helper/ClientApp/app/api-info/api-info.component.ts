import { Component, OnInit, OnDestroy, AfterViewInit  } from '@angular/core';
import { SvrInfoParam } from '../svrInfoParam';
import { SvrInfoService } from '../svr-info.service';
import { Observable } from 'rxjs';
import { Subscription } from 'rxjs';
import { ApiInfoService } from '../api-info.service';
import { ClassInfo } from '../api-list/ClassInfo';
import { isNullOrUndefined } from 'util';
import { Routes, RouterModule, Router } from '@angular/router';

declare var PR: any;

@Component({
  selector: 'app-api-info',
  templateUrl: './api-info.component.html',
  styleUrls: ['./api-info.component.css']
})
export class ApiInfoComponent implements OnInit, OnDestroy, AfterViewInit   {

  constructor(private info: SvrInfoService ,
              private router: Router) {
    this.subscription = info.svrInfo$.subscribe(
      o => {
        if (isNullOrUndefined(o) || isNullOrUndefined(o.Info)) {
          return;
        }

        const api = o.ApiName;
        const tmpInfo = o.Info.find(m => m.Name === api);
        this.apiInfo = [ tmpInfo ];
        this.host = o.Uri;
        this.svrName = o.SvrName;
    });
  }

  subscription: Subscription;
  apiInfo: ClassInfo[];
  host: string;
  svrName: string;

   GetCommonErrorCode(err: string) {
        return  '<span class="alert-success">200:业务操作成功</span><br />' +
        '-1:系统框架未定义的异常情况<br />' +
        '400:非法请求<br />' +
        '401:访问需要授权的资源，但是提供的token无接口权限<br />' +
        '403:参数错误，服务器无法解析<br />' +
        '404:请求的方法不存在<br />' +
        '405:错误码未定义<br />' +
        '500:服务器内部错误，业务接口处理未捕获的所有异常<br />' +
        '505:Token授权已超时，需要登录<br />' +
        '600:业务层返回错误，业务层未定义错误码，但定义了返回错误信息 <br />'
        + err;
    }
  GetApiUri() {
    return this.host + this.svrName + '/' + this.apiInfo[0].Name;
  }

  Back() {
    this.router.navigate(['svr', this.svrName]);
    return false;
  }

  Debug(svr: string, api: string) {
    this.router.navigate(['svr', 'test', svr, api]);
    return false;
  }

  ngOnInit() {

  }

  ngOnDestroy() {
    // prevent memory leak when component destroyed
    this.subscription.unsubscribe();
  }
  public ngAfterViewInit() {
    console.log('PR.prettyPrint();');
    PR.prettyPrint();
    console.log(PR);
  }
}
