import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { PessoasCrudComponent } from './features/pessoas-crud/pessoas-crud';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, PessoasCrudComponent],
  template: `<app-pessoas-crud></app-pessoas-crud>`,
})
export class AppComponent {}
