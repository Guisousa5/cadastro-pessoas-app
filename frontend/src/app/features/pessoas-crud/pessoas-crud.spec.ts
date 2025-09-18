import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PessoasCrud } from './pessoas-crud';

describe('PessoasCrud', () => {
  let component: PessoasCrud;
  let fixture: ComponentFixture<PessoasCrud>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PessoasCrud]
    })
    .compileComponents();

    fixture = TestBed.createComponent(PessoasCrud);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
