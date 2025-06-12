import { Routes } from '@angular/router';
import { MainComponent } from './pages/main/main.component';
import { LoginComponent } from './pages/login/login.component';
import { RegisterComponent } from './pages/register/register.component';
import { MenuComponent } from './pages/menu/menu.component';
import { AdminComponent } from './pages/admin/admin.component';
import { AgendaComponent } from './pages/agenda/agenda.component';
import { CrearentradaComponent } from './pages/crearentrada/crearentrada.component';
import { FicherosComponent } from './pages/ficheros/ficheros.component';
import { CalendarComponent } from './pages/calendar/calendar.component';
import { LayoutComponent } from './pages/layout/layout.component';
import { FacturasComponent } from './pages/facturas/facturas.component';
import { AmigosComponent } from './pages/amigos/amigos.component';

// Definición de las rutas de la aplicación.
export const routes: Routes = [
    {
        // Ruta raíz, muestra el componente principal.
        path: '',
        component: MainComponent
    },
    {
        path: 'login',
        component: LoginComponent
    },
    {
        path: 'register',
        component: RegisterComponent
    },
    {
        // Ruta para el panel de administración.
        path: 'admin',
        component: AdminComponent
    },
    {
        // Ruta base para la sección principal de la aplicación autenticada.
        // Utiliza un componente Layout que contiene el menú lateral y un router-outlet para las vistas hijas.
        path: 'app',
        component: LayoutComponent,
        children: [
            { path: 'ficheros', component: FicherosComponent },
            { path: 'agenda', component: AgendaComponent },
            { path: 'crearentrada', component: CrearentradaComponent },
            { path: 'calendar', component: CalendarComponent },
            { path: 'facturas', component: FacturasComponent },
            { path: 'amigos', component: AmigosComponent},
            // Redirige a 'ficheros' por defecto cuando se accede a '/app'.
            { path: '', redirectTo: 'ficheros', pathMatch: 'full' }
        ]
    }   
];