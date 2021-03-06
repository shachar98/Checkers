﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CheckersEngine
{
    /// <summary>
    /// Interaction logic for RulesWindow.xaml
    /// </summary>
    public partial class RulesWindow : Window
    {
        public RulesWindow()
        {
            InitializeComponent();

            RulesTextBlock.Text =
    @"
דמקה הוא משחק לוח המשוחק עג לוח בגודל 8X8 משבצות.
במשחק משחקים שני יריבים, בעלי כלים בצבעים שונים, ומטרת כל שחקן היא להוריד(לאכול) את כל האבנים של היריב, או לחסום אותן, כלומר לא לאפשר ליריב לקיים מסע(צעד/ מהלך). שחקן שנשאר ללא אבנים או ללא מסע אפשרי מוכרז כמפסיד.
הלוח מונח כך שהמשבצת הימנית ביותר בשורה הקרובה לכל שחקן תהיה לבנה, האבנים מונחות על המשבצות השחורות של הלוח וההתקדמות היא רק על משבצות אלה באלכסון.
בתחילת המשחק אבני השחקן האחד מונחות בשלוש השורות הראשונות של הלוח ואילו אבני השחקן השני מונחות באותו אופן בצד שלו. לפי המוסכם, הלבן מבצע את המהלך הראשון, והשחור משיב עליו. שתי הפעולות גם יחד נחשבות כמסע אחד.
לוח מונח כך שהמשבצת הימנית ביותר בשורה הקרובה לכל שחקן תהיה לבנה(כמו במשחק השחמט), האבנים מונחות על המשבצות השחורות של הלוח וההתקדמות היא רק על משבצות אלה באלכסון.
אבנים:
כל שחקן מניע בתורו אבן-משחק באלכסון, ממשבצת שחורה אחת למשבצת שחורה סמוכה בכיוון היריב. על המשבצת להיות פנויה מכלים, כלומר לכל אבן יש שתי אפשרויות תנועה – לכיוון היריב ימינה ולכיוון היריב שמאלה – וכל אחת מהן עשויה להיות חסומה. 
אכילה:
דילוג(או אכילה) מתבצע כאשר אבן משחק מונחת במשבצת סמוכה לאבן היריב, ומעבר לאבן היריב יש מקום פנוי. כאשר דילוג אפשרי, חובה לבצע אותו.דילוג מבוצע על ידי הנחת האבן במקום הפנוי שמעבר לאבן היריב והסרת אבן היריב מן הלוח. אם בתום הדילוג אפשרי דילוג נוסף חובה לבצע גם אותו(באותו תור), ללא הגבלה על מספר הדילוגים הרצופים או כיוון התנועה, כלומר אפשר לבצע דילוגים גם לאחור אבל רק בשרשרת.ברגע שקיימת האפשרות לבצע דילוגים בכיוונים שונים(קדימה), מותר(אך לא חובה) לבחור בכיוון בו מספר האכילות יהיה רב יותר, ואסור לאכול אחורה בלי שרשרת.
מלכים:
כשאבן משחק מגיעה לשורה האחרונה, היא הופכת להיות מלך(בעברית יש הקוראים לכך מלכה), כאשר בדרך כלל מייצגים מלך על ידי שתי אבני משחק מונחות אחת על השנייה. מלך, בניגוד לאבן רגילה, יכול לנוע לכל הכיוונים באלכסון(כלומר גם אחורה), ללא הגבלה על כמות המשבצות בדרך, וכמו כן הוא יכול לבצע דילוגים בכל כיוון.
אכילה:
בדומה לאבנים הרגילות, גם המלך מחויב לבצע דילוגים כאשר יש לו אפשרות לעשות זו, אך בנוסף, הוא חייב לבצע דילוג גם כאשר אבנו של היריב אינה צמודה אליו, אך באותו האלכסון. כאן נמצא השינוי לעומת דמקה בינלאומית – בעוד בדמקה בינלאומית המלך יכול לעצור בכל אחת מהמשבצות באלכסון אחרי האכילה, במערכת זו המלך חייב לעצור במשבצת שאחרי האכילה.בשני המקרים, במידה והמלך יכול להמשיך לאכול, הוא חייב לעשות כן.
סיום המשחק
המשחק יכול להסתיים בניצחון או בתיקו. ניצחון מושג אם מתקיים אחד מהבאים:
•	לשחקן היריב לא נותרו כלל כלים על הלוח(אבנים או מלכים).
•	השחקן היריב נכנע.
•	לשחקן היריב אין אפשרות לבצע מהלך מאחר שכליו חסומים.
קיימות שתי דרכים להשגת תיקו:
    1. אם במשך 15 מסעים רצופים נעו מלכים בלבד על גבי הלוח(אף אבן לא התקדמה צעד), ולא השתנה מספר הכלים על גבי הלוח(לא התבצעו דילוגים) מסתיים המשחק ללא ניצחון לאף אחד מהצדדים 
    2. בכל שלב במשחק רשאי כל אחד מהצדדים להציע תיקו, והצד השני רשאי להסכים או לסרב.
מכיוון שמדובר במשחק של שחקן מול מחשב, הוחלט על כך שלא יהיה תיקו, והמשחק יכול להימשך עד שהשחקן ייבחר להפסיקו. כמו כן, גם אופציית הכניעה אינה קיימת, והמחשב והשחקן ימשיך לשחק עד לניצחון(או בחירת השחקן להפסיק את המשחק).
";
        }
    }
}
