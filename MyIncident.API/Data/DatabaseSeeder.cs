using MyIncident.API.Models;

namespace MyIncident.API.Data;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(AppDbContext context)
    {
        if (context.Requests.Any())
            return;

        var random = new Random(42);
        var statuses = Enum.GetValues<RequestStatus>();
        var priorities = Enum.GetValues<RequestPriority>();

        var orgHandlerMap = new Dictionary<string, string>
        {
            { "פרקליטות", "יוסי כהן" },
            { "הון אנושי", "מירב לוי" },
            { "תקשוב", "אבי ישראלי" },
            { "כספים", "דנה שמעוני" },
            { "לשכה משפטית", "רונית אברהם" },
            { "ביטחון פנים", "עמית גולן" },
            { "מינהל", "שרה דוד" },
            { "דוברות", "נועם פרץ" },
            { "רכש ולוגיסטיקה", "יעל מזרחי" },
            { "הדרכה והשתלמויות", "אורן חיים" }
        };

        var orgs = orgHandlerMap.Keys.ToArray();

        var titles = new Dictionary<string, string[]>
        {
            { "רכב", new[] { "אחזקת רכב", "חידוש רישיון רכב", "בקשה להחלפת רכב", "תיקון רכב שירות", "הקצאת רכב חדש" } },
            { "מחשוב", new[] { "התקנת תוכנה", "תקלה בתחנת עבודה", "בקשת ציוד היקפי", "שדרוג מחשב", "תקלת רשת" } },
            { "תשתיות", new[] { "תקלת חשמל", "תיקון מזגן", "בקשת שיפוץ", "תחזוקת מבנה", "בעיית ניקיון" } },
            { "הרשאות", new[] { "בקשת גישה למערכת", "שינוי הרשאות", "הסרת משתמש", "איפוס סיסמה", "הוספת משתמש" } },
            { "אבטחה", new[] { "דיווח אירוע אבטחה", "בקשת בדיקת חדירה", "עדכון מדיניות", "תקלה במצלמות", "בקשת תג כניסה" } },
            { "כספים", new[] { "בקשת אישור תקציב", "דיווח חריגה", "בקשת החזר", "עדכון פרטי ספק", "בקשת תשלום" } },
            { "הדרכה", new[] { "בקשת הדרכה", "רישום לקורס", "בקשת הנחיה מקצועית", "בקשת חומרי לימוד", "הדרכת עובד חדש" } }
        };

        var subjects = titles.Keys.ToArray();

        var openers = new[]
        {
            "דניאל לוי", "רחל כהן", "יוסף אברהם", "מיכל שמעון", "אורי דוד",
            "נעמי פרץ", "אלון גולן", "שירה מזרחי", "תומר חיים", "ליאת ביטון",
            "עידו שלום", "הילה ברק", "גיא עוז", "ענבל רוזן", "ניר אדרי"
        };

        var detailedDescriptions = new[]
        {
            "הבעיה מתרחשת באופן קבוע מאז תחילת השבוע. ניסיתי לפנות למחלקה המתאימה אך לא קיבלתי מענה.",
            "דרוש טיפול דחוף. הנושא משפיע על עבודה שוטפת של הצוות ומעכב תהליכים.",
            "הבעיה חוזרת על עצמה מספר פעמים ביום. נדרש פתרון קבוע ולא עוקף.",
            "בעקבות שינוי ארגוני יש צורך בעדכון המערכת בהתאם. אנא טפלו בהקדם.",
            "הבקשה הוגשה בעקבות הנחיית מנהל המחלקה. יש לתאם מול הגורם הרלוונטי.",
            "מדובר בתקלה שחוזרת כבר שבועיים. פתחתי פנייה קודמת שלא טופלה.",
            "נדרש אישור תקציבי לפני ביצוע. מצורף אישור מנהל.",
            "הבעיה משפיעה על כלל העובדים בקומה. מבקשת טיפול בעדיפות גבוהה.",
            "לאחר בדיקה מול הספק, התברר שנדרשת התערבות פנימית. מפרט מצורף.",
            "הנושא עלה בישיבת הנהלה ונקבע שיש לטפל בו תוך שבוע."
        };

        var requests = new List<Request>(10000);

        for (int i = 0; i < 10000; i++)
        {
            var createdAt = DateTime.UtcNow.AddDays(-random.Next(1, 365)).AddHours(-random.Next(0, 24));
            var org = orgs[random.Next(orgs.Length)];
            var subject = subjects[random.Next(subjects.Length)];
            var titleDesc = titles[subject][random.Next(titles[subject].Length)];
            requests.Add(new Request
            {
                Title = $"{subject} - {titleDesc}",
                Description = detailedDescriptions[random.Next(detailedDescriptions.Length)],
                OpenedBy = openers[random.Next(openers.Length)],
                OrganizationName = org,
                HandlerName = orgHandlerMap[org],
                Status = statuses[random.Next(statuses.Length)],
                Priority = priorities[random.Next(priorities.Length)],
                CreatedAt = createdAt,
                UpdatedAt = createdAt.AddHours(random.Next(0, 168))
            });
        }

        context.Requests.AddRange(requests);
        await context.SaveChangesAsync();
    }
}
