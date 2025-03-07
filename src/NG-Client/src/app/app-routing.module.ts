import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { IndexComponent } from './pages/welcome/index/index.component';
import { WelcomeComponent } from './pages/welcome/welcome.component';
import { AuthGuard } from './shared/routers/auth.guard';

const routes: Routes = [
  { path: '', pathMatch: 'full', redirectTo: '/auth/login' },
  {
    path: 'index', component: IndexComponent, data: {
      auth: 'test-index'
    },
    canActivate: [AuthGuard]
  },
  {
    path: 'admin', component: WelcomeComponent,
    loadChildren: () => import('./pages/welcome/welcome.module').then(m => m.WelcomeModule)
  },
  { path: 'auth', loadChildren: () => import("./pages/auth/auth.module").then(m => m.AuthModule) }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
