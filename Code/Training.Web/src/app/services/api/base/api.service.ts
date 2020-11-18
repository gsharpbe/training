import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Command } from 'src/app/models/commands/command';
import { CommandResult } from 'src/app/models/commands/command-result';
import { environment } from 'src/environments/environment';
import { SearchParameters } from '../model/search-parameters';
import { SearchResult } from '../model/search-result';
import { ServiceRequestOptions } from '../model/service-request-options';
import { ApiHelper } from './api-helper';

@Injectable()
export class ApiService<T>  {

    constructor(public http: HttpClient) {
    }

    getUrl(): string {
        return `${environment.apiUrl}/${this.getRoute()}`;
    }

    getRoute(): string {
        return '';
    }

    search$(searchParameters: SearchParameters, serviceRequestOptions: ServiceRequestOptions = null): Observable<SearchResult<T>> {
        const options = ApiHelper.convert(serviceRequestOptions, searchParameters);
        return this.http.get<SearchResult<T>>(this.getUrl(), options);
    }

    get$(id: any, serviceRequestOptions: ServiceRequestOptions = null): Observable<T> {
        const url = `${this.getUrl()}/${id}`;
        const options = ApiHelper.convert(serviceRequestOptions);

        return this.http.get<T>(url, options);
    }

    create$(model: T): Observable<T> {
        return this.http.post<T>(this.getUrl(), model);
    }

    update$(model: T): Observable<T> {
        return this.http.put<T>(this.getUrl(), model);
    }

    delete$(id: any): Observable<T> {
        const url = `${this.getUrl()}/${id}`;

        return this.http.delete<T>(url);
    }

    execute$(command: Command): Observable<CommandResult> {
        const url = `${this.getUrl()}/commands`;

        return this.http.post<CommandResult>(url, command);
    }
}
