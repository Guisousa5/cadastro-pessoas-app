import { Component } from '@angular/core';
import { MenubarModule } from 'primeng/menubar';
import { MenuItem } from 'primeng/api';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-menu',
  standalone: true,
  imports: [MenubarModule, RouterLink],
  templateUrl: './menu.component.html',
})
export class MenuComponent {
  items: MenuItem[] = [
    {
      label: 'Pessoas',
      icon: 'pi pi-fw pi-users',
      routerLink: '/pessoas'
    }
  ];
}