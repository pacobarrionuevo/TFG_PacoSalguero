import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
// Componente raíz de la aplicación. Sirve como punto de entrada principal.
export class AppComponent {
  title = 'SanitariosApp';
}