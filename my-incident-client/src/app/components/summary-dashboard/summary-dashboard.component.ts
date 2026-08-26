import { Component, Input } from '@angular/core';
import { AggregationDto } from '../../models/request.model';

interface ChartItem {
  key: string;
  value: number;
  percent: number;
  color: string;
}

@Component({
  selector: 'app-summary-dashboard',
  templateUrl: './summary-dashboard.component.html',
  styleUrls: ['./summary-dashboard.component.css']
})
export class SummaryDashboardComponent {
  @Input() aggregation: AggregationDto | null = null;

  private statusColors: Record<string, string> = {
    'New': '#90CAF9',
    'InProgress': '#FFE082',
    'Waiting': '#CE93D8',
    'Completed': '#A5D6A7',
    'Rejected': '#EF9A9A'
  };

  private priorityColors: Record<string, string> = {
    'Low': '#A5D6A7',
    'Medium': '#FFE082',
    'High': '#EF9A9A'
  };

  private subjectColors: Record<string, string> = {
    'רכב': '#90CAF9',
    'מחשוב': '#80DEEA',
    'תשתיות': '#FFE082',
    'הרשאות': '#CE93D8',
    'אבטחה': '#EF9A9A',
    'כספים': '#A5D6A7',
    'הדרכה': '#BCAAA4'
  };

  get statusItems(): ChartItem[] {
    return this.buildItems(this.aggregation?.byStatus, this.statusColors);
  }

  get priorityItems(): ChartItem[] {
    return this.buildItems(this.aggregation?.byPriority, this.priorityColors);
  }

  get subjectItems(): ChartItem[] {
    return this.buildItems(this.aggregation?.bySubject, this.subjectColors);
  }

  private buildItems(data: Record<string, number> | undefined, colors: Record<string, string>): ChartItem[] {
    if (!data) return [];
    const max = Math.max(...Object.values(data), 1);
    return Object.entries(data).map(([key, value]) => ({
      key,
      value,
      percent: (value / max) * 100,
      color: colors[key] || '#90A4AE'
    }));
  }
}
