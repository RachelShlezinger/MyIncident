import { Component, EventEmitter, Output } from '@angular/core';
import { QueryParams } from '../../models/query-params.model';
import { RequestStatus, RequestPriority } from '../../models/request.model';
import { Subject, debounceTime, distinctUntilChanged } from 'rxjs';

@Component({
  selector: 'app-filter-panel',
  templateUrl: './filter-panel.component.html',
  styleUrls: ['./filter-panel.component.css']
})
export class FilterPanelComponent {
  @Output() filtersChanged = new EventEmitter<QueryParams>();

  statuses = Object.values(RequestStatus);
  priorities = Object.values(RequestPriority);
  handlers = [
    'יוסי כהן',
    'מירב לוי',
    'אבי ישראלי',
    'דנה שמעוני',
    'רונית אברהם',
    'עמית גולן',
    'שרה דוד',
    'נועם פרץ',
    'יעל מזרחי',
    'אורן חיים'
  ];

  selectedStatus = '';
  selectedPriority = '';
  selectedHandler = '';
  fromDate = '';
  toDate = '';
  searchTerm = '';

  private searchSubject = new Subject<string>();

  constructor() {
    this.searchSubject.pipe(
      debounceTime(300),
      distinctUntilChanged()
    ).subscribe(() => this.emitFilters());
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
