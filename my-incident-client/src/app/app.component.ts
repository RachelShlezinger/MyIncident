import { Component, ViewChild } from '@angular/core';
import { RequestTableComponent } from './components/request-table/request-table.component';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  title = 'מערכת ניהול פניות';

  @ViewChild('requestTable') requestTable!: RequestTableComponent;

  onRequestCreated(): void {
    this.requestTable.retry();
  }
}
