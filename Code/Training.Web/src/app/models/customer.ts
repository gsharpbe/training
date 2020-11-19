import { Address } from './address';
import { ModelBase } from './base/model-base';
import { Project } from './project';

export class Customer extends ModelBase {
    name: string;
    addresses: Address[];
    projects: Project[];
}
