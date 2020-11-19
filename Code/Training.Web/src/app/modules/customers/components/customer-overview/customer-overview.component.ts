import { CustomerApiService } from './../../../../services/api/customer-api.service';
import { Component, OnInit } from '@angular/core';
import { Customer } from 'src/app/models/customer';
import { SearchParameters } from 'src/app/services/api/model/search-parameters';

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
    const searchParameters = new SearchParameters();
    this.customerApiService.search$(searchParameters).subscribe(result => {
      this.customers = result.data;
    });
  }

}
