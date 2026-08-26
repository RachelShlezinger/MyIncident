import { Component, OnInit } from '@angular/core';
import { RequestService } from '../../services/request.service';
import { Request, PagedResponse, AggregationDto, RequestStatus } from '../../models/request.model';
import { QueryParams } from '../../models/query-params.model';

@Component({
  selector: 'app-request-table',
  templateUrl: './request-table.component.html',
  styleUrls: ['./request-table.component.css']
})
export class RequestTableComponent implements OnInit {
  requests: Request[] = [];
  aggregation: AggregationDto | null = null;
  totalCount = 0;
  totalPages = 0;
  currentPage = 1;
  pageSize = 20;
  sortBy = 'CreatedAt';
  sortDirection: 'asc' | 'desc' = 'desc';
  filters: QueryParams = {};

  loading = false;
  error: string | null = null;
  expandedId: number | null = null;

  statuses = Object.values(RequestStatus);

  constructor(private requestService: RequestService) {}

  ngOnInit(): void {
    this.loadData();
  }

  onFiltersChanged(filters: QueryParams): void {
    this.filters = filters;
    this.currentPage = 1;
    this.loadData();
  }

  onSort(field: string): void {
    if (this.sortBy === field) {
      this.sortDirection = this.sortDirection === 'asc' ? 'desc' : 'asc';
    } else {
      this.sortBy = field;
      this.sortDirection = 'asc';
    }
    this.loadData();
  }

  onPageChange(page: number): void {
    this.currentPage = page;
    this.loadData();
  }

  onStatusUpdate(request: Request, newStatus: string): void {
    this.requestService.updateStatus(request.id, newStatus, request.rowVersion)
      .subscribe({
        next: (updated) => {
          const index = this.requests.findIndex(r => r.id === updated.id);
          if (index >= 0) this.requests[index] = updated;
          this.loadAggregations();
        },
        error: (err) => {
          if (err.status === 409) {
            alert('הרשומה שונתה על ידי משתמש אחר. הנתונים יטענו מחדש.');
            this.loadData();
          } else {
            this.error = err.error?.message || 'שגיאה בעדכון סטטוס';
          }
        }
      });
  }

  retry(): void {
    this.error = null;
    this.loadData();
  }

  getSortIcon(field: string): string {
    if (this.sortBy !== field) return '↕';
    return this.sortDirection === 'asc' ? '↑' : '↓';
  }

  toggleExpand(id: number): void {
    this.expandedId = this.expandedId === id ? null : id;
  }

  private loadData(): void {
    this.loading = true;
    this.error = null;

    const params: QueryParams = {
      page: this.currentPage,
      pageSize: this.pageSize,
      sortBy: this.sortBy,
      sortDirection: this.sortDirection,
      ...this.filters
    };

    this.requestService.getRequests(params).subscribe({
      next: (response) => {
        this.requests = response.items;
        this.totalCount = response.totalCount;
        this.totalPages = response.totalPages;
        this.loading = false;
      },
      error: (err) => {
        this.loading = false;
        this.error = err.status === 0
          ? 'לא ניתן להתחבר לשרת. בדוק את החיבור לרשת.'
          : err.error?.message || 'שגיאה בטעינת הנתונים';
      }
    });

    this.loadAggregations();
  }

  private loadAggregations(): void {
    this.requestService.getAggregations(this.filters).subscribe({
      next: (agg) => this.aggregation = agg,
      error: () => {}
    });
  }
}
