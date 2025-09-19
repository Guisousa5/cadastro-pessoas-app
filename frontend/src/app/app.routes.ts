// app.routes.ts
import { Routes } from '@angular/router';
import { HomeComponent } from './features/home/home';
import { PessoasCrudComponent } from './features/pessoas-crud/pessoas-crud'

export const routes: Routes = [
  { path: '', component: HomeComponent }, // rota inicial -> Home
  { path: 'pessoas', component: PessoasCrudComponent }, // CRUD
  { path: '**', redirectTo: '' } // qualquer rota inválida volta para home
];
