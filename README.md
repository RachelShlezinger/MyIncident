# מערכת ניהול פניות (MyIncident)

מערכת Full Stack לניהול פניות/בקשות המתקבלות מארגונים ממשלתיים. המערכת כוללת שרת REST API מבוסס .NET 8 וממשק לקוח Angular 15.

---

## תיאור כללי

המערכת מאפשרת:
- **צפייה בפניות** — טבלה עם דפדוף (Pagination) התומכת ב-10,000+ רשומות
- **שורה מתרחבת** — לחיצה על פנייה מציגה תיאור הבעיה ופותח הפנייה
- **יצירת פנייה חדשה** — טופס עם בחירת נושא, ארגון (גורם מטפל אוטומטי), עדיפות, ותיאור
- **סינון** — לפי סטטוס, עדיפות, גורם מטפל, וטווח תאריכים
- **חיפוש חופשי** — בכותרת הפנייה ושם הארגון
- **מיון** — לפי כל עמודה בטבלה (עולה/יורד)
- **עדכון סטטוס** — ישירות מהטבלה, עם טיפול ב-Concurrency
- **דאשבורד גרפי** — גרפי עמודות: פילוח לפי סטטוס, עדיפות, ונושא
- **טיפול בשגיאות** — הודעות ברורות למשתמש + אפשרות ניסיון חוזר

---

## טכנולוגיות

| רכיב | טכנולוגיה |
|-------|-----------|
| שרת (API) | .NET 8, ASP.NET Core Web API |
| בסיס נתונים | SQL Server LocalDB, Entity Framework Core 8 |
| צד לקוח | Angular 15, TypeScript |
| תיעוד API | Swagger / OpenAPI |

---

## מבנה הפרויקט

```
MyIncident/
├── MyIncident.API/          # שרת REST API
│   ├── Controllers/         # נקודות קצה (Endpoints)
│   ├── Services/            # שכבת לוגיקה עסקית
│   ├── Repositories/        # שכבת גישה לנתונים
│   ├── Models/              # מודלי Domain
│   ├── DTOs/                # אובייקטי העברת נתונים
│   ├── Data/                # DbContext, Seeder, Configurations
│   └── Middleware/          # טיפול גלובלי בשגיאות
├── my-incident-client/      # אפליקציית Angular
│   └── src/app/
│       ├── components/      # קומפוננטות UI
│       │   ├── request-table/       # טבלה ראשית + pagination
│       │   ├── filter-panel/        # פאנל סינון וחיפוש
│       │   ├── summary-dashboard/   # דאשבורד גרפי
│       │   └── create-request/      # טופס יצירת פנייה
│       ├── services/        # שירותי HTTP
│       └── models/          # ממשקי TypeScript
├── README.md
└── ARCHITECTURE.md          # ניתוח ארכיטקטורה מעמיק
```

---

## דרישות מקדימות

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Node.js 16+](https://nodejs.org/) (כולל npm)
- SQL Server LocalDB (מגיע עם Visual Studio או ניתן להתקנה בנפרד)

---

## הוראות הרצה

### שרת (API)

```bash
cd MyIncident.API
dotnet run
```

השרת יעלה על `http://localhost:5114`.  
ממשק Swagger זמין בכתובת: `http://localhost:5114/swagger`

> בהרצה ראשונה, בסיס הנתונים נוצר אוטומטית ומתמלא ב-10,000 רשומות לדוגמה.

### צד לקוח (Angular)

```bash
cd my-incident-client
npm install
npm start
```

האפליקציה תעלה על `http://localhost:4200`.

---

## נקודות קצה עיקריות (API)

| Method | Endpoint | תיאור |
|--------|----------|--------|
| GET | `/api/requests` | אחזור פניות בדפים (עם סינון, חיפוש, מיון) |
| GET | `/api/requests/aggregations` | נתונים מצטברים (ספירות לפי סטטוס, עדיפות, ונושא) |
| POST | `/api/requests` | יצירת פנייה חדשה |
| PATCH | `/api/requests/{id}/status` | עדכון סטטוס פנייה |

### פרמטרי שאילתה (GET /api/requests)

| פרמטר | תיאור | ברירת מחדל |
|--------|--------|------------|
| page | מספר עמוד | 1 |
| pageSize | גודל עמוד | 20 |
| status | סינון לפי סטטוס (New, InProgress, Waiting, Completed, Rejected) | — |
| priority | סינון לפי עדיפות (Low, Medium, High) | — |
| organizationName | סינון לפי שם ארגון (חיפוש חלקי) | — |
| handlerName | סינון לפי גורם מטפל | — |
| fromDate | תאריך התחלה | — |
| toDate | תאריך סיום | — |
| search | חיפוש חופשי בכותרת ושם ארגון | — |
| sortBy | שדה למיון | CreatedAt |
| sortDirection | כיוון מיון (asc/desc) | desc |

---

## מודל נתונים — פנייה (Request)

| שדה | סוג | תיאור |
|------|------|--------|
| Id | int | מזהה ייחודי |
| Title | string (max 200) | כותרת בפורמט "נושא - תיאור" |
| Description | string | תיאור מפורט של הבעיה |
| OpenedBy | string | שם פותח הפנייה |
| OrganizationName | string (max 150) | שם הארגון |
| HandlerName | string | גורם מטפל (נקבע אוטומטית לפי ארגון) |
| Status | enum | New, InProgress, Waiting, Completed, Rejected |
| Priority | enum | Low, Medium, High |
| CreatedAt | datetime | תאריך יצירה |
| UpdatedAt | datetime | תאריך עדכון אחרון |
| RowVersion | byte[] | Concurrency token |

---

## אתגר נבחר: Concurrency (בקרת מקביליות)

### הבעיה
כאשר שני משתמשים פותחים את אותה פנייה ומנסים לעדכן את הסטטוס שלה במקביל — ללא מנגנון הגנה, העדכון השני ידרוס את הראשון ללא התראה.

### הפתרון שנבחר: Optimistic Concurrency עם RowVersion

- **RowVersion** — כל רשומה מכילה שדה `RowVersion` (timestamp) שמתעדכן אוטומטית בכל שינוי.
- **בקליינט** — כשהמשתמש טוען רשומה, הוא מקבל את ה-RowVersion הנוכחי.
- **בעדכון** — הקליינט שולח את ה-RowVersion שקיבל. השרת משווה אותו לערך הנוכחי ב-DB.
- **התנגשות** — אם הערכים לא תואמים (כלומר, מישהו אחר עדכן בינתיים), מוחזרת שגיאת **409 Conflict**.
- **בממשק** — מוצגת הודעה "הרשומה שונתה על ידי משתמש אחר" והנתונים נטענים מחדש.

### חלופות שנשקלו

| חלופה | יתרונות | חסרונות | סיבת פסילה |
|--------|----------|---------|-------------|
| Pessimistic Locking (נעילה) | מונע התנגשויות לחלוטין | ביצועים ירודים, סיכון ל-deadlocks, UX גרוע | לא מתאים לאפליקציית Web |
| Last Write Wins | פשוט למימוש | איבוד נתונים שקט | לא עומד בדרישה |
| Merge Conflicts (כמו Git) | שומר את שני השינויים | מורכבות גבוהה, UX מסובך | Overkill לעדכון סטטוס |

### מימוש טכני

**שרת (Middleware):**
```csharp
catch (DbUpdateConcurrencyException)
{
    // מחזיר 409 Conflict
    await WriteErrorResponse(context, 409, "Conflict",
        "The record was modified by another user. Please reload and try again.");
}
```

**קליינט:**
```typescript
if (err.status === 409) {
    alert('הרשומה שונתה על ידי משתמש אחר. הנתונים יטענו מחדש.');
    this.loadData();
}
```

---

## שיפורי ביצועים

- **אינדקסים** על שדות הסינון הנפוצים: Status, Priority, CreatedAt, OrganizationName
- **דפדוף בצד השרת** — רק העמוד הנדרש נטען מה-DB (Skip/Take)
- **AsNoTracking** — שאילתות קריאה ללא Change Tracking
- **Debounce בחיפוש** — השהיה של 300ms בצד הקליינט למניעת בקשות מיותרות

---

## הרצת בדיקות

### בדיקות שרת
```bash
cd MyIncident.API
dotnet test
```

---

## ארגונים וגורמים מטפלים

| ארגון | גורם מטפל |
|-------|-----------|
| פרקליטות | יוסי כהן |
| הון אנושי | מירב לוי |
| תקשוב | אבי ישראלי |
| כספים | דנה שמעוני |
| לשכה משפטית | רונית אברהם |
| ביטחון פנים | עמית גולן |
| מינהל | שרה דוד |
| דוברות | נועם פרץ |
| רכש ולוגיסטיקה | יעל מזרחי |
| הדרכה והשתלמויות | אורן חיים |

---

## נושאי פניות

רכב, מחשוב, תשתיות, הרשאות, אבטחה, כספים, הדרכה

כותרת הפנייה בפורמט: **"נושא - תיאור"** (לדוגמה: "רכב - אחזקת רכב")
