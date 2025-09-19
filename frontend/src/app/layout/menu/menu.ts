import { Component } from '@angular/core';
import { MenubarModule } from 'primeng/menubar';
import { MenuItem } from 'primeng/api';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-menu',
  standalone: true,
  imports: [MenubarModule, RouterLink],
  templateUrl: './menu.html',
  styleUrls: ['./menu.scss']
})
export class MenuComponent {
  // Declarando items que será usado no p-menubar
  items: MenuItem[] = [
    { label: 'Home', routerLink: '/' },
    { label: 'Pessoas', routerLink: '/pessoas' },
    { label: 'Configurações', routerLink: '/config' },
  ];
}
