import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { Pessoa } from '../models/pessoa';

@Injectable({
  providedIn: 'root'
})
export class PessoaService {
  private http = inject(HttpClient);
  private apiUrl = 'http://localhost:5120/api/Pessoas';

  getPessoas(): Observable<Pessoa[]> {
    return this.http.get<Pessoa[]>(this.apiUrl).pipe(catchError(this.handleError));
  }

  createPessoa(pessoa: Pessoa): Observable<Pessoa> {
    return this.http.post<Pessoa>(this.apiUrl, pessoa).pipe(catchError(this.handleError));
  }

  updatePessoa(id: number, pessoa: Pessoa): Observable<any> {
    return this.http.put(`${this.apiUrl}/${id}`, pessoa).pipe(catchError(this.handleError));
  }

  deletePessoa(id: number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/${id}`).pipe(catchError(this.handleError));
  }

  // Novo método de validação de CPF
  validarCpf(cpf: string): boolean {
    if (!cpf) return false;
    const numeros = cpf.replace(/\D/g, '');
    return numeros.length === 11; // apenas verifica quantidade de dígitos
  }

  private handleError(error: HttpErrorResponse) {
    let errorMessage = 'Ocorreu um erro desconhecido!';
    if (error.error instanceof ErrorEvent) {
      errorMessage = `Erro do lado do cliente: ${error.error.message}`;
    } else {
      errorMessage = `Código do erro: ${error.status}, mensagem: ${error.message}`;
    }
    console.error(errorMessage);
    return throwError(() => new Error('Algo deu errado; por favor, tente novamente mais tarde.'));
  }
}
