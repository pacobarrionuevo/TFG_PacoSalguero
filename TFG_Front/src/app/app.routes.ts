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
import { InformesComponent } from './pages/informes/informes.component';

export const routes: Routes = [
    {
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
        path: 'admin',
        component: AdminComponent
    },
    ,
    {
        path: 'app',
        component: LayoutComponent,
        children: [
            { path: 'ficheros', component: FicherosComponent },
            { path: 'agenda', component: AgendaComponent },
            { path: 'crearentrada', component: CrearentradaComponent },
            { path: 'calendar', component: CalendarComponent },
            { path: 'facturas', component: FacturasComponent },
            { path: 'informes', component: InformesComponent},
            { path: '', redirectTo: 'ficheros', pathMatch: 'full' }
        ]
    }   
];
