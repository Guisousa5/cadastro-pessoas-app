import { bootstrapApplication } from '@angular/platform-browser';
import { provideHttpClient } from '@angular/common/http';
import { provideRouter } from '@angular/router';
import { provideAnimationsAsync } from '@angular/platform-browser/animations/async'; // Importa o provedor de animações
import { AppComponent } from './app/app';

bootstrapApplication(AppComponent, {
  providers: [
    provideHttpClient(),
    provideRouter([]),
    provideAnimationsAsync() // Adiciona o provedor de animações ao array
  ]
}).catch(err => console.error(err));
