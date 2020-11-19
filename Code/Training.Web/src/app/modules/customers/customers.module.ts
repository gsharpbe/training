import { SharedModule } from './../shared/shared.module';
import { NgModule } from '@angular/core';
import { CustomerOverviewComponent } from './components/customer-overview/customer-overview.component';

@NgModule({
  declarations: [
    CustomerOverviewComponent
  ],
  imports: [
    SharedModule
  ]
})
export class CustomersModule { }
