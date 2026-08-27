# מערכת ניהול פניות (MyIncident)

מערכת Full Stack לניהול פניות/בקשות המתקבלות מארגונים ממשלתיים.  
המערכת כוללת שרת REST API מבוסס .NET 8 וממשק לקוח Angular 15.

---

## 🔗 קישורים

| רכיב | קישור |
|-------|--------|
| **אתר חי (API)** | https://myincident-api.onrender.com |
| **Swagger (תיעוד API)** | https://myincident-api.onrender.com/swagger |
| **קליינט (Angular)** | https://myincident.vercel.app |
| **קוד מקור — GitHub** | https://github.com/RachelShlezinger/MyIncident |
| **בסיס נתונים** | PostgreSQL (Render) — נוצר אוטומטית בהרצה ראשונה |

> **הערה**: ב-Free tier של Render, הבקשה הראשונה אחרי חוסר פעילות לוקחת ~50 שניות (ה-instance "מתעורר"). לאחר מכן הכל מהיר.

---

## 📁 מיקום הקוד

| רכיב | נתיב ב-Repository |
|-------|-------------------|
| שרת C# (.NET 8 API) | `/MyIncident.API/` |
| קליינט Angular | `/my-incident-client/` |
| ניתוח ארכיטקטורה | `/ARCHITECTURE.md` |
| Dockerfile (deploy) | `/MyIncident.API/Dockerfile` |

---

## 🏗️ סביבות

### ייצור (Production) — Render + Vercel
- **API**: Docker container על Render (auto-deploy מ-branch `main`)
- **DB**: PostgreSQL על Render (Free tier, 256MB RAM, 1GB storage)
- **Client**: Vercel (static hosting)
- ה-DB נוצר אוטומטית (`EnsureCreated`) בעלייה הראשונה ומתמלא ב-10,000 רשומות

### פיתוח (Development) — Local
- **API**: `dotnet run` על `localhost:5114`
- **DB**: מתחבר ל-PostgreSQL ב-Render (External URL)
- **Client**: `npm start` על `localhost:4200`

---

## 🚀 הוראות הרצה (Development)

### דרישות מקדימות
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Node.js 16+](https://nodejs.org/) (כולל npm)
- גישת אינטרנט (ה-DB רץ ב-Render)

### שרת (API)

```bash
cd MyIncident.API
dotnet run
```

השרת יעלה על `http://localhost:5114`  
Swagger: `http://localhost:5114/swagger`

> בהרצה ראשונה, בסיס הנתונים נוצר אוטומטית ומתמלא ב-10,000 רשומות לדוגמה.

### צד לקוח (Angular)

```bash
cd my-incident-client
npm install
npm start
```

האפליקציה תעלה על `http://localhost:4200`

---

## 🗄️ בסיס נתונים

### סוג: PostgreSQL (Render)

בסיס הנתונים **נוצר אוטומטית** בהרצה ראשונה — אין צורך בסקריפט יצירה ידני.

### יצירת מבנה הטבלאות
הקוד משתמש ב-`EnsureCreated()` — מה שיוצר את כל הטבלאות אוטומטית לפי ההגדרות ב-Entity Framework Core.

### נתונים ראשוניים (Seed)
הקובץ `MyIncident.API/Data/DatabaseSeeder.cs` מכיל את כל הלוגיקה ליצירת נתונים ראשוניים:
- **10 ארגונים** עם גורמים מטפלים (טבלת `Organizations`)
- **10,000 פניות** עם נתוני דמו מגוונים (טבלת `Requests`)

ה-Seed רץ אוטומטית אם הטבלאות ריקות.

### Connection String
מוגדר ב-`appsettings.json`:
```
Host=dpg-da7k1ihsrm7s73esgoa0-a.frankfurt-postgres.render.com
Database=myincidentdb
Username=myincident_user
SSL Mode=Require
```

### טבלאות

| טבלה | תיאור |
|-------|--------|
| `Organizations` | ארגונים וגורמים מטפלים (Id, Name, HandlerName) |
| `Requests` | פניות — מקושרות לארגון ב-FK (OrganizationId) |

---

## 📋 תיאור כללי

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

## 🛠️ טכנולוגיות

| רכיב | טכנולוגיה |
|-------|-----------|
| שרת (API) | .NET 8, ASP.NET Core Web API |
| בסיס נתונים | PostgreSQL, Entity Framework Core 8 |
| צד לקוח | Angular 15, TypeScript |
| תיעוד API | Swagger / OpenAPI |
| Deploy — API | Render (Docker) |
| Deploy — Client | Vercel |

---

## 📐 מבנה הפרויקט

```
MyIncident/
├── MyIncident.API/          # שרת REST API (.NET 8)
│   ├── Controllers/         # נקודות קצה (Endpoints)
│   │   ├── RequestsController.cs
│   │   └── OrganizationsController.cs
│   ├── Services/            # שכבת לוגיקה עסקית
│   ├── Repositories/        # שכבת גישה לנתונים
│   ├── Models/              # מודלי Domain
│   │   ├── Request.cs
│   │   └── Organization.cs
│   ├── DTOs/                # אובייקטי העברת נתונים
│   ├── Data/                # DbContext, Seeder, Configurations
│   ├── Middleware/          # טיפול גלובלי בשגיאות
│   └── Dockerfile           # Build & Deploy configuration
├── my-incident-client/      # אפליקציית Angular
│   └── src/app/
│       ├── components/
│       │   ├── request-table/       # טבלה ראשית + pagination
│       │   ├── filter-panel/        # פאנל סינון וחיפוש
│       │   ├── summary-dashboard/   # דאשבורד גרפי
│       │   └── create-request/      # טופס יצירת פנייה
│       ├── services/        # שירותי HTTP
│       └── models/          # ממשקי TypeScript
├── README.md                # הוראות הפעלה (קובץ זה)
└── ARCHITECTURE.md          # ניתוח ארכיטקטורה מעמיק
```

---

## 🔌 נקודות קצה (API Endpoints)

| Method | Endpoint | תיאור |
|--------|----------|--------|
| GET | `/api/requests` | אחזור פניות בדפים (עם סינון, חיפוש, מיון) |
| GET | `/api/requests/aggregations` | נתונים מצטברים (ספירות לפי סטטוס, עדיפות, ונושא) |
| POST | `/api/requests` | יצירת פנייה חדשה |
| PATCH | `/api/requests/{id}/status` | עדכון סטטוס פנייה |
| GET | `/api/organizations` | רשימת ארגונים וגורמים מטפלים |

---

## 🔒 אתגר נבחר: Concurrency (בקרת מקביליות)

### הבעיה
כאשר שני משתמשים מנסים לעדכן את אותה פנייה במקביל — ללא מנגנון הגנה, העדכון השני ידרוס את הראשון.

### הפתרון: Optimistic Concurrency עם RowVersion (xmin)

- כל רשומה מכילה `RowVersion` (מנוהל אוטומטית ע"י PostgreSQL)
- בעדכון, השרת משווה את ה-RowVersion שנשלח לערך הנוכחי ב-DB
- אם לא תואמים → **409 Conflict** + הודעה למשתמש + טעינה מחדש

---

## ⚡ שיפורי ביצועים

- **אינדקסים** על Status, Priority, CreatedAt, OrganizationName
- **דפדוף בצד השרת** (Skip/Take) — רק 20 רשומות בכל פעם
- **AsNoTracking** — שאילתות קריאה ללא Change Tracking
- **Debounce בחיפוש** — 300ms השהיה בצד הקליינט

---

## 🏢 ארגונים וגורמים מטפלים

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

## 📝 נושאי פניות

רכב, מחשוב, תשתיות, הרשאות, אבטחה, כספים, הדרכה

כותרת הפנייה בפורמט: **"נושא - תיאור"** (לדוגמה: "רכב - אחזקת רכב")
