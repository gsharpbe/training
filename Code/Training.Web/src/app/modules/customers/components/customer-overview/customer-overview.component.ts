import { CustomerApiService } from './../../../../services/api/customer-api.service';
import { Component, OnInit } from '@angular/core';
import { Customer } from 'src/app/models/customer';
import { SearchParameters } from 'src/app/services/api/model/search-parameters';
import { ServiceRequestOptions } from 'src/app/services/api/model/service-request-options';
import { CustomerEditParameters } from '../customer-edit/customer-edit-parameters';
import { BsModalService, ModalOptions } from 'ngx-bootstrap/modal';
import { CustomerEditComponent } from '../customer-edit/customer-edit.component';

@Component({
  selector: 'app-customer-overview',
  templateUrl: './customer-overview.component.html',
  styleUrls: ['./customer-overview.component.scss']
})
export class CustomerOverviewComponent implements OnInit {

  customers: Customer[];

  constructor(private customerApiService: CustomerApiService, private modalService: BsModalService) { }

  ngOnInit(): void {
    this.loadCustomers();
  }

  loadCustomers(): void {
    const options = new ServiceRequestOptions();
    options.includes.add('customer', 'addresses');

    const searchParameters = new SearchParameters();
    this.customerApiService.search$(searchParameters, options).subscribe(result => {
      this.customers = result.data;
    });
  }

  addCustomer(): void {
    this.editCustomer(null);
  }

  editCustomer(customer: Customer): void {
    const parameters = new CustomerEditParameters();
    parameters.customer = customer;
    parameters.callback = c => this.onEditCustomerComplete(c);

    const options = new ModalOptions<CustomerEditComponent>();
    options.ignoreBackdropClick = true;
    options.class = 'modal-lg';

    const bsModalRef = this.modalService.show(CustomerEditComponent, options);
    bsModalRef.content.initialize(parameters);
  }

  onEditCustomerComplete(customer: Customer): void {
    const index = this.customers.map(x => x.guid).indexOf(customer.guid);
    if (index >= 0) {
      this.customers[index] = customer;
    }
    else {
      this.customers.push(customer);
    }
  }

}
