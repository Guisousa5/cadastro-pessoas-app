import { ChangeDetectionStrategy, Component, OnInit, inject, signal, ChangeDetectorRef } from '@angular/core';
import { CommonModule, SlicePipe, DatePipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Pessoa } from '../../models/pessoa';
import { PessoaService } from '../../services/pessoa';
import { ConfirmationService, MessageService } from 'primeng/api';

// Componentes standalone do PrimeNG
import { Toast } from 'primeng/toast';
import { Table } from 'primeng/table';
import { Button } from 'primeng/button';
import { Dialog } from 'primeng/dialog';
import { InputText } from 'primeng/inputtext';
import { Toolbar } from 'primeng/toolbar';
import { ConfirmDialog } from 'primeng/confirmdialog';
import { DatePicker } from 'primeng/datepicker';
import { InputMask } from 'primeng/inputmask';

@Component({
  selector: 'app-pessoas-crud',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    Table,
    Button,
    Dialog,
    InputText,
    Toolbar,
    ConfirmDialog,
    Toast,
    DatePicker,
    InputMask,
    SlicePipe,
    DatePipe
  ],
  providers: [MessageService, ConfirmationService],
  templateUrl: './pessoas-crud.component.html',
  styleUrls: ['./pessoas-crud.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class PessoasCrudComponent implements OnInit {
  private pessoaService = inject(PessoaService);
  private messageService = inject(MessageService);
  private confirmationService = inject(ConfirmationService);
  private cdr = inject(ChangeDetectorRef);

  pessoas = signal<Pessoa[]>([]);
  loading = signal<boolean>(true);

  pessoaDialog: boolean = false;
  pessoa: Partial<Pessoa> = {};

  ngOnInit() {
    this.carregarPessoas();
  }

  carregarPessoas() {
    this.loading.set(true);
    this.pessoaService.getPessoas().subscribe({
      next: (data) => {
        this.pessoas.set(data);
        this.loading.set(false);
        this.cdr.markForCheck();
      },
      error: () => {
        this.messageService.add({ severity: 'error', summary: 'Erro', detail: 'Falha ao carregar pessoas.', life: 3000 });
        this.loading.set(false);
        this.cdr.markForCheck();
      }
    });
  }

  abrirModalNovaPessoa() {
    this.pessoa = {};
    this.pessoaDialog = true;
  }

  editarPessoa(pessoa: Pessoa) {
    this.pessoa = {
      ...pessoa}
    this.pessoaDialog = true;
  }

  esconderModal() {
    this.pessoaDialog = false;
  }

  salvarPessoa() {
    const dadosParaApi: Pessoa = {
    ...this.pessoa,
    cpf: this.pessoa.cpf as string,
    nome: this.pessoa.nome as string,
    email: this.pessoa.email as string,
    dataNascimento: this.pessoa.dataNascimento as string,
  };

    const operation$ = dadosParaApi.id
      ? this.pessoaService.updatePessoa(dadosParaApi.id, dadosParaApi)
      : this.pessoaService.createPessoa(dadosParaApi);

    const successMsg = dadosParaApi.id ? 'Pessoa Atualizada' : 'Pessoa Criada';

    operation$.subscribe({
      next: () => {
        this.messageService.add({ severity: 'success', summary: 'Sucesso', detail: successMsg, life: 3000 });
        this.carregarPessoas();
      },
      error: () => {
        this.messageService.add({ severity: 'error', summary: 'Erro', detail: `Não foi possível salvar a pessoa.`, life: 3000 });
      }
    });

    this.pessoaDialog = false;
  }

  deletarPessoa(pessoa: Pessoa) {
    this.confirmationService.confirm({
      message: 'Tem certeza que deseja deletar ' + pessoa.nome + '?',
      header: 'Confirmar',
      icon: 'pi pi-exclamation-triangle',
      accept: () => {
        if (!pessoa.id) return;
        this.pessoaService.deletePessoa(pessoa.id).subscribe({
          next: () => {
            this.messageService.add({ severity: 'success', summary: 'Sucesso', detail: 'Pessoa Deletada', life: 3000 });
            this.carregarPessoas();
          },
          error: () => {
            this.messageService.add({ severity: 'error', summary: 'Erro', detail: `Não foi possível deletar a pessoa.`, life: 3000 });
          }
        });
      }
    });
  }
}
