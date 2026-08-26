export interface QueryParams {
  page?: number;
  pageSize?: number;
  status?: string;
  priority?: string;
  organizationName?: string;
  handlerName?: string;
  fromDate?: string;
  toDate?: string;
  search?: string;
  sortBy?: string;
  sortDirection?: 'asc' | 'desc';
}
