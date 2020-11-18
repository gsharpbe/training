import { ServiceRequestIncludes } from './service-request-includes';
import { ServiceRequestExcludes } from './service-request-excludes';

export class ServiceRequestOptions {
    public includes = new ServiceRequestIncludes();
    public excludes = new ServiceRequestExcludes();

    public getHeaders() {
        const includeHeaders = this.includes.getHeaders();
        const excludeHeaders = this.excludes.getHeaders();

        return includeHeaders.concat(excludeHeaders);
    }
}
