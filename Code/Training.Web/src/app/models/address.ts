import { ModelBase } from './base/model-base';

export class Address extends ModelBase {
    addressLine1: string;
    addressLine2: string;
    zip: string;
    city: string;
}
