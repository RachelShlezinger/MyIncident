import { Component, EventEmitter, Output } from '@angular/core';
import { RequestService } from '../../services/request.service';
import { RequestPriority } from '../../models/request.model';

interface OrgHandler {
  org: string;
  handler: string;
}

@Component({
  selector: 'app-create-request',
  templateUrl: './create-request.component.html',
  styleUrls: ['./create-request.component.css']
})
export class CreateRequestComponent {
  @Output() requestCreated = new EventEmitter<void>();

  showForm = false;
  submitting = false;
  error: string | null = null;
  success = false;

  selectedSubject = '';
  titleDescription = '';
  selectedOrg = '';
  selectedPriority = '';
  handlerName = '';
  openedBy = '';
  description = '';

  priorities = Object.values(RequestPriority);

  subjects = ['רכב', 'מחשוב', 'תשתיות', 'הרשאות', 'אבטחה', 'כספים', 'הדרכה'];

  orgHandlers: OrgHandler[] = [
    { org: 'פרקליטות', handler: 'יוסי כהן' },
    { org: 'הון אנושי', handler: 'מירב לוי' },
    { org: 'תקשוב', handler: 'אבי ישראלי' },
    { org: 'כספים', handler: 'דנה שמעוני' },
    { org: 'לשכה משפטית', handler: 'רונית אברהם' },
    { org: 'ביטחון פנים', handler: 'עמית גולן' },
    { org: 'מינהל', handler: 'שרה דוד' },
    { org: 'דוברות', handler: 'נועם פרץ' },
    { org: 'רכש ולוגיסטיקה', handler: 'יעל מזרחי' },
    { org: 'הדרכה והשתלמויות', handler: 'אורן חיים' }
  ];

  constructor(private requestService: RequestService) {}

  toggleForm(): void {
    this.showForm = !this.showForm;
    if (!this.showForm) this.resetForm();
  }

  onOrgChange(): void {
    const found = this.orgHandlers.find(o => o.org === this.selectedOrg);
    this.handlerName = found ? found.handler : '';
  }

  onSubmit(): void {
    this.error = null;
    this.success = false;

    if (!this.selectedSubject) {
      this.error = 'יש לבחור נושא';
      return;
    }
    if (!this.titleDescription.trim()) {
      this.error = 'יש להזין תיאור כותרת';
      return;
    }
    if (!this.selectedOrg) {
      this.error = 'יש לבחור ארגון';
      return;
    }
    if (!this.selectedPriority) {
      this.error = 'יש לבחור עדיפות';
      return;
    }
    if (!this.openedBy.trim()) {
      this.error = 'יש להזין שם פותח הפנייה';
      return;
    }

    this.submitting = true;

    const title = `${this.selectedSubject} - ${this.titleDescription.trim()}`;

    this.requestService.createRequest({
      title,
      organizationName: this.selectedOrg,
      priority: this.selectedPriority,
      description: this.description.trim(),
      openedBy: this.openedBy.trim()
    }).subscribe({
      next: () => {
        this.success = true;
        this.submitting = false;
        this.requestCreated.emit();
        setTimeout(() => {
          this.resetForm();
          this.showForm = false;
        }, 1500);
      },
      error: (err) => {
        this.submitting = false;
        this.error = err.error?.message || 'שגיאה ביצירת הפנייה';
      }
    });
  }

  private resetForm(): void {
    this.selectedSubject = '';
    this.titleDescription = '';
    this.selectedOrg = '';
    this.selectedPriority = '';
    this.handlerName = '';
    this.openedBy = '';
    this.description = '';
    this.error = null;
    this.success = false;
  }
}
