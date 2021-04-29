import { Component, OnInit, ViewChild, AfterViewInit } from '@angular/core';
import { Http } from '@angular/http';
import { ServiceInfo } from '../ServiceInfo';
import { isNullOrUndefined } from 'util';
import { resolveDirective } from '@angular/core/src/render3/instructions';
import {Router, NavigationEnd, ActivatedRoute} from '@angular/router';
import { SvrInfoService } from '../svr-info.service';
import { SvrInfoParam } from '../svrInfoParam';
import { ClassInfo } from '../api-list/ClassInfo';
import { ParseTreeResult } from '@angular/compiler';
import { ScmConfig } from '../ScmConfig';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { async, ninvoke } from 'q';
import { Result } from '../Result';
import { ApiHelpService } from '../api-help.service';
import { TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'app-svr-list',
  templateUrl: './svr-list.component.html',
  styleUrls: ['./svr-list.component.css'],
  providers: [ SvrInfoService]
})
export class SvrListComponent implements OnInit {
  constructor(
    private httpService: ApiHelpService,
    private router: Router,
    private routerInfo: ActivatedRoute,
    private info: SvrInfoService,
    private translate: TranslateService
  ) {
    this.translate.setDefaultLang('zh');
    this.translate.use('zh');
    this.info.apiInfo$.subscribe(o => {
      this.apiInfo = o;
      console.log(this.apiInfo);
    });
  }
  title = '服务接口列表';
  svrInfo: ServiceInfo;
  apiInfo: ClassInfo[];
  ngOnInit() {
    this.httpService.getHttpProxy().subscribe(httpUri => {
      this.svrInfo = new ServiceInfo();
      this.svrInfo.proxyServer = httpUri;
      this.httpService.getSvrList().subscribe(values => {
        this.svrInfo.svrs = values;
        console.log(values);
        if ( !isNullOrUndefined(this.svrInfo.svrs) && this.svrInfo.svrs.length > 0 ) {
          const si = new SvrInfoParam() ;
          const url = location.pathname.toString();
          const arr = this.parseUrlInfo(url, this.svrInfo.svrs[0]);
          si.SvrName = arr[0] ;
          si.ApiName = arr[1];
          if (url.lastIndexOf(si.SvrName) <= 0 ) {
            this.router.navigate(['svr', si.SvrName]);
          }
          si.Uri = this.svrInfo.proxyServer;
          this.httpService.getApiHelp(si.SvrName).subscribe(v => {
            si.Info = v;
            this.apiInfo = v;
            this.info.postSvrInfo(si);
          });
          this.info.postSvrInfo(si);
        }
      });
    });


    this.router.events.subscribe(event => {
      if (event instanceof  NavigationEnd && !isNullOrUndefined(this.svrInfo) ) {
        const si = new SvrInfoParam() ;
        // console.log(event.url);
        const arr = this.parseUrlInfo(event.url, this.svrInfo.svrs[0]);
        // console.log('导航定位服务:' + arr[0]);
        si.SvrName = arr[0];
        si.ApiName = arr[1];
        si.Uri = this.svrInfo.proxyServer;
        si.Info = this.apiInfo;
        this.info.postSvrInfo(si);
      }
  });
  }

  parseUrlInfo(url: string, defVal: string) {
    const si = ['', ''];
    if (url.startsWith('/svr/test/')) {
      const arr = url.substring(10).split('/');
      if ( arr.length >= 2) {
        si[0] = arr[0];
        si[1] = arr[1];
      } else if (arr.length >= 1) {
        si[0] = arr[0];
      }
    } else if ( url.startsWith('/svr/')) {
      const arr = url.substring(5).split('/');
      if ( arr.length >= 2) {
        si[0] = arr[0];
        si[1] = arr[1];
      } else if (arr.length >= 1) {
        si[0] = arr[0];
      }  else {
        si[0] = defVal;
      }
    } else {
      si[0] = defVal;
    }
    return si;
  }
}
