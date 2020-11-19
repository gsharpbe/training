import { ProjectOverviewComponent } from './modules/projects/components/project-overview/project-overview.component';
import { CustomerOverviewComponent } from './modules/customers/components/customer-overview/customer-overview.component';
import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

const routes: Routes = [
  { path: 'customers', component: CustomerOverviewComponent },
  { path: 'projects', component: ProjectOverviewComponent },
  { path: '**', redirectTo: 'customers', pathMatch: 'full' }

];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
