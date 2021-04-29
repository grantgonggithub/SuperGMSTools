import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Routes, RouterModule } from '@angular/router';
import { SvrListComponent } from './svr-list.component';
import { ApiListComponent } from '../api-list/api-list.component';
import { ApiInfoComponent } from '../api-info/api-info.component';
import { ApiTestComponent } from '../api-test/api-test.component';
import { FormsModule } from '@angular/forms';
import { HttpClientModule, HttpClient} from '@angular/common/http';
import { TranslateModule, TranslateLoader } from '@ngx-translate/core';
import { TranslateHttpLoader } from '@ngx-translate/http-loader';
import {MissingTranslationHandler, MissingTranslationHandlerParams} from '@ngx-translate/core';
import { isNullOrUndefined } from 'util';
// 使用TranslateHttpLoader加载项目的本地语言json配置文件
export function createTranslateHttpLoader(http: HttpClient) {
  return new TranslateHttpLoader(http, './assets/i18n/', '.json');
}
export class MyMissingTranslationHandler implements MissingTranslationHandler {
  handle(params: MissingTranslationHandlerParams) {
        // console.log(params);
        // const tValue =  params.translateService.parser.interpolate(params.key, params.interpolateParams);
        // return isNullOrUndefined(tValue) ? params.key : tValue;
        return params.key;
  }
}

export const ROUTES: Routes = [
  {
    path: '',
    component: SvrListComponent,
    children: [
      { path: 'test/:id/:name', component: ApiTestComponent },
      { path: ':id/:name', component: ApiInfoComponent },
      { path: ':id', component: ApiListComponent },
      { path: '', component: ApiListComponent, pathMatch: 'full' },
    ]
  }
];

@NgModule({
  imports: [
    CommonModule,
    FormsModule,
    RouterModule.forChild(ROUTES),
    HttpClientModule,
    TranslateModule.forRoot({
      loader: {
        provide: TranslateLoader,
                useFactory: createTranslateHttpLoader,
                deps: [HttpClient]
      },
      missingTranslationHandler: {provide: MissingTranslationHandler, useClass: MyMissingTranslationHandler},
  })
  ],
  declarations: [
    SvrListComponent,
    ApiListComponent,
    ApiInfoComponent,
    ApiTestComponent
  ]
})
export class SvrListModule { }
