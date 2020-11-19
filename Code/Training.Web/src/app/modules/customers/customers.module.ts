import { SharedModule } from './../shared/shared.module';
import { NgModule } from '@angular/core';
import { CustomerOverviewComponent } from './components/customer-overview/customer-overview.component';
import { CustomerEditComponent } from './components/customer-edit/customer-edit.component';

@NgModule({
  declarations: [
    CustomerOverviewComponent,
    CustomerEditComponent
  ],
  imports: [
    SharedModule
  ]
})
export class CustomersModule { }
