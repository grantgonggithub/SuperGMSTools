import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ApiListComponent } from './api-list/api-list.component';
import { SvrListComponent } from './svr-list/svr-list.component';
import { SvrListModule } from './svr-list/svr-list.module';


const routes: Routes = [
  { path: 'index.html', redirectTo: '/svr', pathMatch: 'full' },
  { path: '', redirectTo: '/svr', pathMatch: 'full' },
  { path: 'svr' , loadChildren: './svr-list/svr-list.module#SvrListModule'}
  // { path: 'svr', component: SvrListComponent  },
  // { path: 'svr/:id', component: ApiListComponent }
];

@NgModule({
  exports: [ RouterModule ],
  imports: [ RouterModule.forRoot(routes, { enableTracing: true }) ],
})
export class AppRoutingModule {
 }
