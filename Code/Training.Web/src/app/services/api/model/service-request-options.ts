import { ServiceRequestIncludes } from './service-request-includes';

export class ServiceRequestOptions {
    public includes = new ServiceRequestIncludes();

    public getHeaders() {
        const includeHeaders = this.includes.getHeaders();

        return includeHeaders;
    }
}
