import { Component, OnInit, Input, OnDestroy } from '@angular/core';
import { Router, ActivatedRoute, NavigationEnd  } from '@angular/router';
import { Location } from '@angular/common';
import { ApiHelpService } from '../api-help.service';
import { ClassInfo } from './ClassInfo';
import { Result } from '../Result';
import { Observable } from 'rxjs';
import { Subscription } from 'rxjs';
import { switchMap } from 'rxjs/operators';
import { isNullOrUndefined } from 'util';
import { SvrInfoService } from '../svr-info.service';
import { ServiceInfo } from '../ServiceInfo';
import { SvrInfoParam } from '../svrInfoParam';
import { ApiInfoService } from '../api-info.service';


@Component({
  selector: 'app-api-list',
  templateUrl: './api-list.component.html',
  styleUrls: ['./api-list.component.css'],
  providers: [ ApiInfoService]
})
export class ApiListComponent implements OnInit, OnDestroy  {
  // @Input() defaultId: string;
  constructor(private router: Router,
    private apiHelpService: ApiHelpService,
    private location: Location,
    private info: SvrInfoService
  ) {
    this.subscription = info.svrInfo$.subscribe(
      o => {
        this.svrName = o.SvrName;
        this.host = o.Uri;
        this.apiInfos = o.Info;
        // console.log(this.apiInfos);
        // this.error = '';
        // this.info.postApiInfo(this.apiInfos);
        this.GetApiList(o.SvrName, o.Uri);
    });
  }
    title = '服务接口列表';
    error: string;
    apiInfos: ClassInfo[];
    svrName: string;
    host: string;
    apiName: string;
    subscription: Subscription;
    private GetApiList(sid: string, host: string) {
      // const sInfo = this.apiHelpService.getSvrList();
      const id = sid;
      let rst: Observable<ClassInfo[]>;
      rst = this.apiHelpService.getApiHelp(`${id}`);
      rst.subscribe(o => {
        this.apiInfos = o;
        // this.error = o.c !== 200 ? o.msg : null;
        this.info.postApiInfo(this.apiInfos);
      });
    }

  ngOnInit() {
    // this.result = this.route.paramMap.pipe(
    //   switchMap((params: ParamMap) =>
    //   this.GetApiListObservable(params.get('id'), this.route.queryParams['host'])
    // ));
  }

  ngOnDestroy() {
    // prevent memory leak when component destroyed
    this.subscription.unsubscribe();
  }

}
