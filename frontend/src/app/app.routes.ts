import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: 'pessoas',
    // LAZY LOADING: O componente só é carregado quando esta rota é acessada.
    loadComponent: () => import('./features/pessoas-crud/pessoas-crud')
                          .then(m => m.PessoasCrudComponent),
    title: 'Cadastro de Pessoas'
  },
  {
    path: '', // Rota padrão (home)
    redirectTo: 'pessoas', // Redireciona para a página de pessoas
    pathMatch: 'full'
  },
  {
    path: '**', // Rota coringa para qualquer outra URL
    redirectTo: 'pessoas'
  }
];