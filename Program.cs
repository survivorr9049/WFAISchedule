using WFAISchedule;
using Tesseract;
using System.Text;
Console.OutputEncoding = System.Text.Encoding.UTF8;

List<ScheduleCell>[] scheduleDays = new List<ScheduleCell>[5];
int group = 2;
using(var image = Image.Load<Rgba32>(Directory.GetFiles("../../../schedule/")[0])) {
    ScheduleScraper scraper = new();
    for(int i = 1; i < 6; i++) {
        Console.WriteLine(i);
        scheduleDays[i - 1] = scraper.GetDaySchedule(image, i, group);
    }
}
string[] calendars = Directory.GetFiles("../../../calendars/");
CalendarCell[,]  calendarDays = new CalendarCell[31, 12];
foreach(string c in calendars) {
    Console.WriteLine(c);
    using(var image = Image.Load<Rgba32>(c)) {
        CalendarProcessor processor = new();
        CalendarScraper scraper = new();
        int outlineSize = 0;
        processor.CropWhitespace(image);
        Vector2 cellDimensions = processor.FindCellDimensions(image, out outlineSize);
        Vector2 dimensions = processor.GetTableDimensions(image, cellDimensions);
        int month = scraper.months[scraper.RemoveSpecialCharacters(scraper.GetCalendarMonth(image))];
        for(int i = 1; i <= dimensions.x * dimensions.y; i++) {
            CalendarCell cell = scraper.GetCellData(image, i);
            calendarDays[cell.day, month - 1] = cell;
        }
    }
}
/*for(int month = 0; month < 12; month++) {
    for(int day = 1; day < 32; day++) {
        try {
            DateTime date = new DateTime(2023, month + 1, calendarDays[day, month].day);
        
            Console.WriteLine((int)date.DayOfWeek);
        } catch { }
    }
}*/
DateTime date = new DateTime(2023, 10, 12);
List<ScheduleCell> day = scheduleDays[(int)date.DayOfWeek - 1];
ScheduleProcessor processor1 = new();
foreach(ScheduleCell cell in day) {
    try {
        Console.Write($"comparing {processor1.GetCellCode(cell.textData, cell.type)} to {string.Join(" ", calendarDays[12, 9].subjects)}");
        if(calendarDays[12, 9].subjects.Contains(processor1.GetCellCode(cell.textData, cell.type))) {
            Console.Write(" FOUND! \n");
            /*Console.WriteLine(cell.textData.Split("\n")[0] + " " + cell.occupation.y + "h" + " " + cell.type);
            Console.WriteLine($" {(cell.always ? "always" : "according to calendar")}");
            Console.WriteLine($"Cell Code: {processor1.GetCellCode(cell.textData, cell.type)} \n");*/
        } else Console.Write("\n");
    } catch {
        Console.WriteLine($"failed at cell {cell.textData}");
    }
}
//Console.WriteLine($"subjects at day {string.Join("\n", calendarDays[12, 11].subjects)}");
//List<ScheduleCell> cells = scraper.GetDaySchedule(image, coordinates[0], coordinates[1]);


/*Console.WriteLine($"Detected cell dimensions: {cellDimensions.x} x {cellDimensions.y}");
Console.WriteLine($"Detected outline size: {outlineSize}");
Console.WriteLine(scheduleProcessor.IsGrayscale(Color.FromRgb(255, 255, 254)));
while(true) {
    Console.Write("Provide week day (x) and group (y) in format (x, y): ");
    string coordinateString = Console.ReadLine() ?? "0, 0";
    int[] coordinates = coordinateString.Split(',').Select(c => {
        try {
            int parsed = int.Parse(c);
            return parsed;
        } catch {
            return -1;
        }
    }).ToArray();

    if(coordinates.Length != 2 || coordinates.Contains(-1)) {
        Console.WriteLine("Format error");
        continue;
    }
    ScheduleScraper scraper = new();
    List<ScheduleCell> cells = scraper.GetDaySchedule(image, coordinates[0], coordinates[1]);
    int time = 8;
    using(FileStream KRYSTUS = File.Create("KRYSTUS.csv")) {
        foreach(ScheduleCell cell in cells) {
            Console.WriteLine(cell.textData.Split("\n")[0] + " " + cell.occupation.y + "h" + " " + cell.type);
            Console.WriteLine($" {(cell.always ? "always" : "according to calendar")}");
            Console.WriteLine($"Cell Code: {scheduleProcessor.GetCellCode(cell.textData, cell.type)} \n");
            byte[] FT = new UTF8Encoding(true).GetBytes($" Subject, Start Date, Start time, End Time,\n{scheduleProcessor.GetCellCode(cell.textData, cell.type)}, 11/11/2023, {time}:00, {time + cell.occupation.y}:00, \n");
            time += cell.occupation.y;
            if(cell.type == CellType.Empty) continue;
            KRYSTUS.Write(FT, 0, FT.Length);
        }
    }

    using(var calendarImage = Image.Load<Rgba32>("../../../sources/calendar1.png")) {
        CalendarCell[] days = new CalendarCell[31];
x        for(int i = 1; i <= 20; i++) {
            CalendarCell cell = calendarScraper.GetCellData(calendarImage, i);
            //Console.WriteLine($"{cell.day} \n {string.Join("\n", cell.subjects)}");
            days[cell.day] = cell;
        }
        Console.WriteLine(calendarScraper.RemoveSpecialCharacters(calendarScraper.GetCalendarMonth(calendarImage)));
        int month = calendarScraper.months[calendarScraper.RemoveSpecialCharacters(calendarScraper.GetCalendarMonth(calendarImage))];
        for(int i = 0; i < 31; i++) {
            try { Console.WriteLine($"subjects at {i}.{month}: \n {string.Join("\n", days[i].subjects)}"); } catch { }
        }
        //CalendarCell cell = calendarScraper.GetCellData(calendarImage, 17);
        //Console.WriteLine($"{month}, w związku z powyższym: {calendarScraper.months[calendarScraper.RemoveSpecialCharacters(month)]}");
    }

}*/


