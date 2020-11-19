import { Injectable } from '@angular/core';
import { Customer } from '../../models/customer';
import { ApiService } from './base/api.service';

@Injectable({ providedIn: 'root' })
export class CustomerApiService extends ApiService<Customer> {

    getRoute(): string {
        return 'customers';
    }
}
