import { Routes } from '@angular/router';
import { MainComponent } from './pages/main/main.component';
import { LoginComponent } from './pages/login/login.component';
import { RegisterComponent } from './pages/register/register.component';
import { MenuComponent } from './pages/menu/menu.component';
import { AdminComponent } from './pages/admin/admin.component';
import { AgendaComponent } from './pages/agenda/agenda.component';
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
        path: 'menu',
        component: MenuComponent
    },
    {
        path: 'admin',
        component: AdminComponent
    },
    {
        path: 'agenda',
    component: AgendaComponent
},
    
];
