import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { QueryParams } from '../../models/query-params.model';
import { RequestStatus, RequestPriority } from '../../models/request.model';
import { RequestService, OrganizationDto } from '../../services/request.service';
import { Subject, debounceTime, distinctUntilChanged } from 'rxjs';

@Component({
  selector: 'app-filter-panel',
  templateUrl: './filter-panel.component.html',
  styleUrls: ['./filter-panel.component.css']
})
export class FilterPanelComponent implements OnInit {
  @Output() filtersChanged = new EventEmitter<QueryParams>();

  statuses = Object.values(RequestStatus);
  priorities = Object.values(RequestPriority);
  handlers: string[] = [];

  selectedStatus = '';
  selectedPriority = '';
  selectedHandler = '';
  fromDate = '';
  toDate = '';
  searchTerm = '';

  private searchSubject = new Subject<string>();

  constructor(private requestService: RequestService) {
    this.searchSubject.pipe(
      debounceTime(300),
      distinctUntilChanged()
    ).subscribe(() => this.emitFilters());
  }

  ngOnInit(): void {
    this.requestService.getOrganizations().subscribe({
      next: (orgs) => this.handlers = orgs.map(o => o.handlerName)
    });
  }

  onSearchInput(value: string): void {
    this.searchTerm = value;
    this.searchSubject.next(value);
  }

  onFilterChange(): void {
    this.emitFilters();
  }

  clearFilters(): void {
    this.selectedStatus = '';
    this.selectedPriority = '';
    this.selectedHandler = '';
    this.fromDate = '';
    this.toDate = '';
    this.searchTerm = '';
    this.emitFilters();
  }

  private emitFilters(): void {
    const filters: QueryParams = {};
    if (this.selectedStatus) filters.status = this.selectedStatus;
    if (this.selectedPriority) filters.priority = this.selectedPriority;
    if (this.selectedHandler) filters.handlerName = this.selectedHandler;
    if (this.fromDate) filters.fromDate = this.fromDate;
    if (this.toDate) filters.toDate = this.toDate;
    if (this.searchTerm) filters.search = this.searchTerm;
    this.filtersChanged.emit(filters);
  }
}
