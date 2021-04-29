import { Injectable } from '@angular/core';
import { MessageService } from './message.service';
import { Observable, of } from 'rxjs';
import { catchError, map, tap } from 'rxjs/operators';
import { Headers, RequestOptions, URLSearchParams } from '@angular/http';
import { ClassInfo } from './api-list/ClassInfo';
import { Result } from './Result';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { ServiceInfo } from './ServiceInfo';
import { ScmConfig } from './ScmConfig';

@Injectable()
export class ApiHelpService {
  private log(message: string) {
    this.messageService.add('apiService: ' + message);
  }

  private handleError<T> (operation = 'operation', result?: T) {
    return (error: any): Observable<T> => {
      // TODO: send the error to remote logging infrastructure
      console.error(error); // log to console instead
      // TODO: better job of transforming error for user consumption
      this.log(`${operation} failed: ${error.message}`);
      // Let the app keep running by returning an empty result.
      return of(result as T);
    };
  }

  constructor(private http: HttpClient,
    private messageService: MessageService) { }
  async getApiUri() {
      const uri =  await this.http.get<ScmConfig>('/assets/scm.json').toPromise();
      return uri.ConstKeyValue[0].Value;
    }
  getApiHelp(svr: string): Observable<ClassInfo[]> {
    const data = {'SvrName': svr};
    const httpOptions = {
      headers: new HttpHeaders({ 'Content-Type': 'application/json' })
    };
    return this.http.post<ClassInfo[]>('/api/GetApiInfo', data, httpOptions)
    .pipe(
            tap(heroes => this.log(`fetched apiHelp`)),
            catchError(this.handleError<ClassInfo[]>('getApiHelp'))
    );
  }
  getSvrList(): Observable<string[]> {
    return this.http.get<string[]>('/api/GetSvrList')
    .pipe(
      tap(val => this.log(`fetched GetSvrList`)),
      catchError(this.handleError<string[]>('getSvrList'))
    );
  }
  getHttpProxy(): Observable<string> {
    return this.http.get<string>('/api/GetHttpProxy')
    .pipe(
      tap(val => this.log(`fetched getHttpProxy`)),
      catchError(this.handleError<string>('getHttpProxy'))
    );
  }

}
