export enum RequestStatus {
  New = 'New',
  InProgress = 'InProgress',
  Waiting = 'Waiting',
  Completed = 'Completed',
  Rejected = 'Rejected'
}

export enum RequestPriority {
  Low = 'Low',
  Medium = 'Medium',
  High = 'High'
}

export interface Request {
  id: number;
  title: string;
  description: string;
  openedBy: string;
  organizationName: string;
  handlerName: string;
  status: RequestStatus;
  priority: RequestPriority;
  createdAt: string;
  updatedAt: string;
  rowVersion: string;
}

export interface PagedResponse<T> {
  items: T[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
}

export interface AggregationDto {
  totalCount: number;
  byStatus: Record<string, number>;
  byPriority: Record<string, number>;
  bySubject: Record<string, number>;
}

export interface UpdateStatusRequest {
  status: string;
  rowVersion: string;
}

export interface CreateRequestPayload {
  title: string;
  organizationName: string;
  priority: string;
  description: string;
  openedBy: string;
}

export interface ErrorResponse {
  error: string;
  message: string;
  statusCode: number;
}
