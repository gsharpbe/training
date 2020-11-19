import { ModelBase } from './base/model-base';
import { Customer } from './customer';

export class Project extends ModelBase {
    name: string;
    customer: Customer;
}
