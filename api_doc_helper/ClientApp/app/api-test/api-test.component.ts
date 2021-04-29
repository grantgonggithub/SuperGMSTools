import { Component, OnInit, OnDestroy, AfterViewInit  } from '@angular/core';
import * as $ from 'jquery';
import { TestApiObject } from '../TestApiObject';
import { SvrInfoParam } from '../svrInfoParam';
import { SvrInfoService } from '../svr-info.service';
import { Observable } from 'rxjs';
import { Subscription } from 'rxjs';
import { ApiInfoService } from '../api-info.service';
import { ClassInfo } from '../api-list/ClassInfo';
import { isNullOrUndefined } from 'util';
import { Routes, RouterModule, Router } from '@angular/router';
import { Args } from '../Args';

declare var PR: any; // 相信我，这个类型肯定被加载了，哦哈哈!
@Component({
  selector: 'app-api-test',
  templateUrl: './api-test.component.html',
  styleUrls: ['./api-test.component.css']
})
export class ApiTestComponent implements OnInit, OnDestroy {

  constructor(private info: SvrInfoService,
     private router: Router) {
    this.subscription = info.svrInfo$.subscribe(
      o => {
        if (isNullOrUndefined(o) || isNullOrUndefined(o.Info)) {
          return;
        }
        this.api = new TestApiObject();
        this.api.Name = o.ApiName;
        this.svrName = o.SvrName;
        this.api.ApiUri = o.Uri + '' + o.SvrName + '/' + o.ApiName;
        const tmpInfo = o.Info.find(m => m.Name === this.api.Name);
        this.api.Desc = tmpInfo.Desc;
        const arg = new Args({}, 'zh_cn', '', '');
        const argString = JSON.stringify(arg, null, 2);
        this.api.InputParam = argString.replace(/{}/, tmpInfo.PropertyInfo[0].JsonDesc);
        console.log(this.api);
      });
   }

  svrName: string;
  api: TestApiObject;
  subscription: Subscription;
  ngOnInit() {
  }
  Back() {
    this.router.navigate(['svr', this.svrName, this.api.Name]);
    return false;
  }
  Test() {
    const uri = $('#ApiUri').val().toString();
    const args = $('#InputParam').val().toString();
    let dtStart: Date = new Date();
    let dtEnd: Date = new Date();

    $.ajax({url: uri,
        type: 'POST', // GET
        async: true, // 或false,是否异步
        contentType: 'application/json; charset=utf-8',
        data: args,
        timeout: 5000, // 超时时间
        dataType: 'json', // 返回的数据格式：json/xml/html/script/jsonp/text
        beforeSend: function() {
            $('#loading').show();
            $('#saveToken').hide();
            dtStart = new Date();
        },
        success: function (data, status, xhr) {
            dtEnd = new Date();
            const runms = dtEnd.valueOf() - dtStart.valueOf();
            $('#httpTime').html(runms + ' ms');
            $('#httpStatus').html('成功:<strong>' + xhr.status + '</strong>,' + status);
            try {
                $('#apiResult').html(JSON.stringify(data, null, 4));

                $('#apiResult').removeClass('prettyprinted');
                PR.prettyPrint();
                try {
                    $('#saveToken').attr('href', $('#saveToken').attr('href') + data.v.Token);

                    $('#saveToken').show();
                } catch (err1) {
                }
            } catch (err) {
                $('#apiResult').html(err.message);
            }
        },
        error: function(xhr, status) {

            $('#httpStatus').html('<span class="error">错误</span>:<strong>' + xhr.status + '</strong>,' + status);
            $('#apiResult').html(xhr.statusText);
        },
        complete: function() {
            // console.log('结束');
            $('#loading').hide();
        }
    });
    // layer.full(index);
    return false;
  }
  get diagnostic() { return JSON.stringify(this.api); }
  ngOnDestroy() {
    // prevent memory leak when component destroyed
    this.subscription.unsubscribe();
  }
}
