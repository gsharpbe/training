import { Observable } from 'rxjs';
import { JsonUtils } from './../../../../utils/json-utils';
import { Component, OnInit } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { Customer } from 'src/app/models/customer';
import { CustomerEditParameters } from './customer-edit-parameters';
import { CustomerApiService } from 'src/app/services/api/customer-api.service';
import { mergeMap } from 'rxjs/operators';

@Component({
  selector: 'app-customer-edit',
  templateUrl: './customer-edit.component.html',
  styleUrls: ['./customer-edit.component.scss']
})
export class CustomerEditComponent implements OnInit {

  customer: Customer;
  callback: (customer: Customer) => void;
  isNew = false;

  constructor(private bsModalRef: BsModalRef, private customerApiService: CustomerApiService) { }

  ngOnInit(): void {
  }

  initialize(parameters: CustomerEditParameters) {
    if (parameters.customer === null) {
      this.isNew = true;
      this.customer = new Customer();
    }
    else {
      this.customer = JsonUtils.deepClone(parameters.customer);
    }
    this.callback = parameters.callback;
  }

  save() {
    if (this.isNew) {
      this.customerApiService.create$(this.customer)
      .pipe(mergeMap(result => {
        return this.customerApiService.get$(result.guid);
      }))
      .subscribe(customer => this.customer = customer, e => this.onSaveError(e), () => this.onSaveComplete());
    }
    else {
      this.customerApiService.update$(this.customer).subscribe(() => null, e => this.onSaveError(e), () => this.onSaveComplete());
    }
  }

  onSaveError(error) {

  }

  onSaveComplete() {
    if (this.callback) {
      this.callback(this.customer);
    }
    this.bsModalRef.hide();
  }

  cancel() {
    this.bsModalRef.hide();
  }

}
