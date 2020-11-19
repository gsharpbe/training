import { CustomerApiService } from './../../../../services/api/customer-api.service';
import { Component, OnInit } from '@angular/core';
import { Customer } from 'src/app/models/customer';
import { SearchParameters } from 'src/app/services/api/model/search-parameters';
import { ServiceRequestOptions } from 'src/app/services/api/model/service-request-options';

@Component({
  selector: 'app-customer-overview',
  templateUrl: './customer-overview.component.html',
  styleUrls: ['./customer-overview.component.scss']
})
export class CustomerOverviewComponent implements OnInit {

  customers: Customer[];

  constructor(private customerApiService: CustomerApiService) { }

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

}
