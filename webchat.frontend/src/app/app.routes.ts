import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth.guard';
import { ChatComponent } from './features/chat/chat.component';
import { HomeComponent } from './features/home/home.component';

export const routes: Routes = [
    { path: '', component: HomeComponent },
    { path: 'chat', component: ChatComponent, canActivate:[authGuard] }
];
