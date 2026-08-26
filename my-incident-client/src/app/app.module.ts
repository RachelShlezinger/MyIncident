import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { HttpClientModule } from '@angular/common/http';
import { FormsModule } from '@angular/forms';

import { AppComponent } from './app.component';
import { RequestTableComponent } from './components/request-table/request-table.component';
import { FilterPanelComponent } from './components/filter-panel/filter-panel.component';
import { SummaryDashboardComponent } from './components/summary-dashboard/summary-dashboard.component';
import { CreateRequestComponent } from './components/create-request/create-request.component';

@NgModule({
  declarations: [
    AppComponent,
    RequestTableComponent,
    FilterPanelComponent,
    SummaryDashboardComponent,
    CreateRequestComponent
  ],
  imports: [
    BrowserModule,
    HttpClientModule,
    FormsModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
