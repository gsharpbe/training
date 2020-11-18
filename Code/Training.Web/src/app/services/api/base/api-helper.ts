import { ServiceRequestOptions } from '../model/service-request-options';
import { HttpHeaders, HttpParams } from '@angular/common/http';

export class ApiHelper {
    public static convert(serviceRequestOptions: ServiceRequestOptions, parameters: any = null) {
        if (!serviceRequestOptions) {
            serviceRequestOptions = new ServiceRequestOptions();
        }

        let httpHeaders = new HttpHeaders();
        const headers = serviceRequestOptions.getHeaders();

        headers.forEach(element => {
            httpHeaders = httpHeaders.append(element.key, element.value);
        });

        const options = {
            headers: httpHeaders,
            params: this.serialize(parameters)
        };

        return options;
    }

    public static serialize(obj: any) {
        let params: HttpParams = new HttpParams();

        if (obj) {
            for (const key in obj) {
                if (obj.hasOwnProperty(key)) {
                    const value = obj[key];

                    params = this.appendParams(params, key, value);
                }
            }
        }

        return params;
    }

    private static appendParams(params: HttpParams, key: string, value: any): HttpParams {
        /// Convert dates to ISO format string
        if (value instanceof Date) {
            return params.append(key, value.toISOString());
        }

        if (typeof value === 'object') {
            /// Convert object and arrays to query params
            for (const k in value) {
                if (value.hasOwnProperty(k)) {
                    params = this.appendParams(params, key + '[' + k + ']', value[k]);
                }
            }
            return params;
        }

        return params.append(key, value);
    }
}
