import { Routes } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { MemberListComponent } from './members/member-list/member-list.component';
import { MessagesComponent } from './messages/messages.component';
import { ListComponent } from './list/list.component';
import { AuthGuard } from './_guards/auth.guard';
import { MemberDetailComponent } from './members/member-detail/member-detail.component';
import { MemberDetailResolver } from './_resolvers/member-detail.resolver';
import { MemberListResolver } from './_resolvers/member-list.resolver';
import { MemberEditComponent } from './members/member-edit/member-edit.component';
import { MemberEditResolver } from './_resolvers/member-edit.resolver';
import { PreventUnsavedChanges } from './_guards/prevent-unsaved-changes.guard';

export const appRoutes : Routes = [
    {path: '', component: HomeComponent},
    {
        path: '', 
        runGuardsAndResolvers: 'always',
        canActivate: [AuthGuard],
        children: [
            {path: 'members', component: MemberListComponent,  resolve: {Users: MemberListResolver}},
            {path: 'members/edit', component: MemberEditComponent,
                resolve: {User: MemberEditResolver}, canDeactivate: [PreventUnsavedChanges]},
            {path: 'members/:id', component: MemberDetailComponent, resolve: {User: MemberDetailResolver}},
            {path: 'messages', component: MessagesComponent},
            {path: 'list', component: ListComponent}
        ]
    },
    {path: '**', redirectTo: '', pathMatch: 'full'},
];
