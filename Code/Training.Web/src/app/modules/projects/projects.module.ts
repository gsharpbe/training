import { SharedModule } from './../shared/shared.module';
import { NgModule } from '@angular/core';
import { ProjectOverviewComponent } from './components/project-overview/project-overview.component';

@NgModule({
  declarations: [ProjectOverviewComponent],
  imports: [
    SharedModule
  ]
})
export class ProjectsModule { }
