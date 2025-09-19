import { ChangeDetectionStrategy, Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Pessoa } from '../../models/pessoa';
import { PessoaService } from '../../services/pessoa';
import { ConfirmationService, MessageService } from 'primeng/api';
import { Header } from '../../layout/header/header';
import { Footer } from '../../layout/footer/footer';
import { MenuComponent } from '../../layout/menu/menu';

// PrimeNG Modules
import { ToastModule } from 'primeng/toast';
import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { DialogModule } from 'primeng/dialog';
import { InputTextModule } from 'primeng/inputtext';
import { ToolbarModule } from 'primeng/toolbar';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { InputMaskModule } from 'primeng/inputmask';
import { DatePickerModule } from 'primeng/datepicker';

@Component({
  selector: 'app-pessoas-crud',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    TableModule,
    ButtonModule,
    DialogModule,
    InputTextModule,
    ToolbarModule,
    ConfirmDialogModule,
    ToastModule,
    InputMaskModule,
    DatePickerModule,
    Header,
    Footer,
    MenuComponent
  ],
  providers: [MessageService, ConfirmationService],
  templateUrl: './pessoas-crud.html',
  styleUrls: ['./pessoas-crud.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class PessoasCrudComponent implements OnInit {
  private pessoaService = inject(PessoaService);
  private messageService = inject(MessageService);
  private confirmationService = inject(ConfirmationService);

  pessoas = signal<Pessoa[]>([]);
  loading = signal<boolean>(true);
  private _pessoaDialog = signal<boolean>(false);
  pessoa = signal<Partial<Pessoa>>({});

  get pessoaDialogValue(): boolean { return this._pessoaDialog(); }
  set pessoaDialogValue(val: boolean) { this._pessoaDialog.set(val); }

  ngOnInit() {
    this.carregarPessoas();
  }

  carregarPessoas() {
    this.loading.set(true);
    this.pessoaService.getPessoas().subscribe({
      next: (data) => {
        // Garantir que cada pessoa tenha id
        this.pessoas.set(data.map(p => ({ ...p, id: p.id })));
        this.loading.set(false);
      },
      error: (error) => {
        console.error('Erro ao carregar pessoas:', error);
        this.messageService.add({severity:'error', summary:'Erro', detail:'Falha ao carregar pessoas.', life:3000});
        this.loading.set(false);
      }
    });
  }

  abrirModalNovaPessoa() {
    this.pessoa.set({ nome: '', email: '', cpf: '', dataNascimento: '' });
    this.pessoaDialogValue = true;
  }

  editarPessoa(p: Pessoa) {
    if (!p.id) {
      this.messageService.add({severity:'error', summary:'Erro', detail:'Pessoa inválida para edição.', life:3000});
      return;
    }

    const dataFormatada = p.dataNascimento ? this.converterDataParaInput(p.dataNascimento) : '';
    const cpfNumeros = p.cpf ? p.cpf.replace(/\D/g, '') : '';
    this.pessoa.set({ ...p, dataNascimento: dataFormatada, cpf: cpfNumeros });
    this.pessoaDialogValue = true;
  }

  esconderModal() {
    this.pessoaDialogValue = false;
  }

  setPessoaField<K extends keyof Pessoa>(field: K, value: Pessoa[K]) {
    this.pessoa.set({ ...this.pessoa(), [field]: value });
  }

  get nomeValue(): string { return this.pessoa().nome || ''; }
  set nomeValue(val: string) { this.setPessoaField('nome', val); }

  get emailValue(): string { return this.pessoa().email || ''; }
  set emailValue(val: string) { this.setPessoaField('email', val); }

  get cpfValue(): string { return this.pessoa().cpf || ''; }
  set cpfValue(val: string) { this.setPessoaField('cpf', val); }

  get dataNascimento(): string { return this.pessoa().dataNascimento || ''; }
  set dataNascimento(val: string) { this.setPessoaField('dataNascimento', val); }

  private formatarDataParaApi(data: string): string {
    if (!data) return '';
    const partes = data.includes('/') ? data.split('/') : data.split('-');
    if (partes.length !== 3) return data;
    if (partes[0].length === 4) return data; // Já está YYYY-MM-DD
    const [dia, mes, ano] = partes;
    return `${ano}-${mes.padStart(2,'0')}-${dia.padStart(2,'0')}`;
  }

  private converterDataParaInput(data: string): string {
    if (!data) return '';
    const partes = data.split('-');
    if (partes.length !== 3) return '';
    return `${partes[0]}-${partes[1].padStart(2,'0')}-${partes[2].padStart(2,'0')}`;
  }

  salvarPessoa() {
    const pessoaAtual = this.pessoa();

    // Remove máscara do CPF
    const cpfNumeros = pessoaAtual.cpf ? (pessoaAtual.cpf as string).replace(/\D/g, '') : '';

    if (!pessoaAtual.nome || !cpfNumeros || !pessoaAtual.email || !pessoaAtual.dataNascimento) {
      this.messageService.add({ severity: 'warn', summary: 'Aviso', detail: 'Preencha todos os campos obrigatórios.', life: 3000 });
      return;
    }

    const dadosParaApi: Pessoa = {
      ...pessoaAtual,
      nome: pessoaAtual.nome as string,
      cpf: cpfNumeros,
      email: pessoaAtual.email as string,
      dataNascimento: this.formatarDataParaApi(pessoaAtual.dataNascimento as string)
    };

    const operation$ = dadosParaApi.id
      ? this.pessoaService.updatePessoa(dadosParaApi.id, dadosParaApi)
      : this.pessoaService.createPessoa(dadosParaApi);

    const successMsg = dadosParaApi.id ? 'Pessoa atualizada com sucesso!' : 'Pessoa criada com sucesso!';

    operation$.subscribe({
      next: () => {
        this.messageService.add({ severity: 'success', summary: 'Sucesso', detail: successMsg, life: 3000 });
        this.carregarPessoas();
        this.esconderModal();
      },
      error: (error) => {
        console.error('Erro ao salvar pessoa:', error);
        const detalhes = error.error?.errors?.join(', ') || error.error?.message || 'Não foi possível salvar a pessoa.';
        this.messageService.add({ severity: 'error', summary: 'Erro', detail: detalhes, life: 3000 });
      }
    });
  }

  deletarPessoa(p: Pessoa) {
    if (!p.id) {
      this.messageService.add({severity:'error', summary:'Erro', detail:'Pessoa inválida para exclusão.', life:3000});
      return;
    }

    this.confirmationService.confirm({
      message: `Tem certeza que deseja deletar ${p.nome}?`,
      header: 'Confirmar Exclusão',
      icon: 'pi pi-exclamation-triangle',
      acceptLabel: 'Sim',
      rejectLabel: 'Não',
      accept: () => {
        this.pessoaService.deletePessoa(p.id!).subscribe({
          next: () => {
            this.messageService.add({severity:'success', summary:'Sucesso', detail:'Pessoa deletada com sucesso!', life:3000});
            this.carregarPessoas();
          },
          error: (error) => {
            console.error('Erro ao deletar pessoa:', error);
            const detalhes = error.error?.errors?.join(', ') || error.error?.message || 'Não foi possível deletar a pessoa.';
            this.messageService.add({severity:'error', summary:'Erro', detail:detalhes, life:3000});
          }
        });
      }
    });
  }

  validarCpf(cpf: string): boolean {
    return this.pessoaService.validarCpf(cpf);
  }
}
