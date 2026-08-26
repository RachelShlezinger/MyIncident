import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { PagedResponse, AggregationDto, Request, UpdateStatusRequest, CreateRequestPayload } from '../models/request.model';
import { QueryParams } from '../models/query-params.model';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class RequestService {
  private readonly apiUrl = `${environment.apiUrl}/requests`;

  constructor(private http: HttpClient) {}

  getRequests(params: QueryParams): Observable<PagedResponse<Request>> {
    const httpParams = this.buildParams(params);
    return this.http.get<PagedResponse<Request>>(this.apiUrl, { params: httpParams });
  }

  getAggregations(params: QueryParams): Observable<AggregationDto> {
    const httpParams = this.buildParams(params);
    return this.http.get<AggregationDto>(`${this.apiUrl}/aggregations`, { params: httpParams });
  }

  updateStatus(id: number, status: string, rowVersion: string): Observable<Request> {
    const body: UpdateStatusRequest = { status, rowVersion };
    return this.http.patch<Request>(`${this.apiUrl}/${id}/status`, body);
  }

  createRequest(payload: CreateRequestPayload): Observable<Request> {
    return this.http.post<Request>(this.apiUrl, payload);
  }

  private buildParams(params: QueryParams): HttpParams {
    let httpParams = new HttpParams();

    if (params.page) httpParams = httpParams.set('page', params.page.toString());
    if (params.pageSize) httpParams = httpParams.set('pageSize', params.pageSize.toString());
    if (params.status) httpParams = httpParams.set('status', params.status);
    if (params.priority) httpParams = httpParams.set('priority', params.priority);
    if (params.organizationName) httpParams = httpParams.set('organizationName', params.organizationName);
    if (params.handlerName) httpParams = httpParams.set('handlerName', params.handlerName);
    if (params.fromDate) httpParams = httpParams.set('fromDate', params.fromDate);
    if (params.toDate) httpParams = httpParams.set('toDate', params.toDate);
    if (params.search) httpParams = httpParams.set('search', params.search);
    if (params.sortBy) httpParams = httpParams.set('sortBy', params.sortBy);
    if (params.sortDirection) httpParams = httpParams.set('sortDirection', params.sortDirection);

    return httpParams;
  }
}
