import { SortDescriptor } from './sort-descriptor';
import { FilterDescriptor } from './filter-descriptor';

export class SearchParameters {
    skip?: number;
    take?: number;
    sort?: SortDescriptor[];
    search?: string;
    filter?: FilterDescriptor[];
    skipCount?: boolean;
    skipData?: boolean;
}
