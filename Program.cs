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
CalendarCell[,] calendarDays = ScrapeAllCalendars("../../../calendars/practices");
CalendarCell[,] lectureCalendarDays = ScrapeAllCalendars("../../../calendars/lectures");


ScheduleProcessor scheduleProcessor = new();
string csv = GenerateCsvHeader();
Console.WriteLine(string.Join("\n", lectureCalendarDays[24, 10].subjects));
for(int month = 0; month < 12; month++) {
    for(int day = 1; day < 32; day++) {
        if(calendarDays[day, month].day == 0) continue;
        DateTime date = new DateTime(2023, month + 1, calendarDays[day, month].day);
        List<ScheduleCell> scheduleDay = scheduleDays[(int)date.DayOfWeek - 1];
        int time = Config.startTime;
        foreach(ScheduleCell cell in scheduleDay) {
            if(cell.type != CellType.Practice) {
                time += cell.occupation.y;
            } else {
                if(cell.always) {
                    csv += GenerateCsvEntry(cell, time, day, month + 1, 2023);
                    time += cell.occupation.y;
                } else {
                    if(calendarDays[day, month].subjects.Contains(scheduleProcessor.GetCellCode(cell.textData, cell.type))){
                        csv += GenerateCsvEntry(cell, time, day, month + 1, 2023);
                    }
                    time += cell.occupation.y;
                }
            }
        }
    }
}
for(int month = 0; month < 12; month++) {
    for(int day = 1; day < 32; day++) {
        if(lectureCalendarDays[day, month].day == 0) continue;
        Console.WriteLine(lectureCalendarDays[day, month].day);
        Console.WriteLine($"{day} of {month}");
        DateTime date = new DateTime(2023, month + 1, lectureCalendarDays[day, month].day);
        Console.WriteLine(date);
        if((int)date.DayOfWeek > 5) continue;
        List<ScheduleCell> scheduleDay = scheduleDays[(int)date.DayOfWeek - 1];
        int time = Config.startTime;
        foreach(ScheduleCell cell in scheduleDay) {
            if(cell.type != CellType.Lecture) {
                time += cell.occupation.y;
            } else {
                if(cell.always) {
                    csv += GenerateCsvEntry(cell, time, day, month + 1, 2023);
                    time += cell.occupation.y;
                } else {
                    if(lectureCalendarDays[day, month].subjects.Contains(scheduleProcessor.GetCellCode(cell.textData, cell.type))) {
                        csv += GenerateCsvEntry(cell, time, day, month + 1, 2023);
                    }
                    time += cell.occupation.y;
                }
            }
        }
    }
}
using(FileStream output = File.Create("output.csv")) {
    byte[] FT = new UTF8Encoding(true).GetBytes(csv);
    output.Write(FT, 0, FT.Length);
}


string GenerateCsvEntry(ScheduleCell cell, int hour, int day, int month, int year) {
    string description;
    if(cell.type == CellType.Lecture) description = "Lecture";
    else description = "Practice";
    return $"{scheduleProcessor.GetCellCode(cell.textData, cell.type)} - {description}, {day}/{month}/{year}, {hour}:00, {hour + cell.occupation.y}:00, {description} \n";
}
string GenerateCsvHeader() {
    return $"Subject, Start Date, Start time, End Time, Description,\n";
}
CalendarCell[,] ScrapeAllCalendars(string directory) {
    string[] calendars = Directory.GetFiles(directory);
    CalendarCell[,] calendarDays = new CalendarCell[32, 12];
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
            Console.WriteLine($"{cellDimensions.x} x {cellDimensions.y}");
            Console.WriteLine($"{dimensions.x} x {dimensions.y}");
            for(int i = 1; i <= dimensions.x * dimensions.y; i++) {
                CalendarCell cell = scraper.GetCellData(image, i);
                calendarDays[cell.day, month - 1] = cell;
            }
        }
    }
    return calendarDays;
}
