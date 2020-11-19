import { Customer } from 'src/app/models/customer';

export class CustomerEditParameters {
    customer: Customer;
    callback: (customer: Customer) => void;
}