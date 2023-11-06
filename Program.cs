using WFAISchedule;
using Tesseract;
Console.OutputEncoding = System.Text.Encoding.UTF8;

using(var image = Image.Load<Rgba32>("../../../sources/schedule.png")) {
    ScheduleProcessor scheduleProcessor = new();
    scheduleProcessor.CropWhitespace(image);
    int outlineSize = 0;
    Vector2 cellDimensions = scheduleProcessor.FindCellDimensions(image, out outlineSize);
    Console.WriteLine($"Detected cell dimensions: {cellDimensions.x} x {cellDimensions.y}");
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
        foreach(ScheduleCell cell in cells) {
            Console.WriteLine(cell.textData.Split("\n")[0] + " " + cell.occupation.y + "h" + " " + cell.type);
            Console.WriteLine($" {(cell.always ? "always" : "according to calendar")}");
            Console.WriteLine($"Cell Code: {scheduleProcessor.GetCellCode(cell.textData, cell.type)} \n");
        }
        using(var calendarImage = Image.Load<Rgba32>("../../../sources/calendar1.png")) {
            CalendarCell[] days = new CalendarCell[31];
            CalendarScraper calendarScraper = new();
            for(int i = 1; i <= 20; i++) {
                CalendarCell cell = calendarScraper.GetCellData(calendarImage, i);
                //Console.WriteLine($"{cell.day} \n {string.Join("\n", cell.subjects)}");
                days[cell.day] = cell;
            }
            Console.WriteLine(calendarScraper.RemoveSpecialCharacters(calendarScraper.GetCalendarMonth(calendarImage)));
            int month = calendarScraper.months[calendarScraper.RemoveSpecialCharacters(calendarScraper.GetCalendarMonth(calendarImage))];
            for(int i = 0; i < 31; i++) {
                try { Console.WriteLine($"your subjects at {i}.{month}: \n {string.Join("\n", days[i].subjects)}"); } catch { }
            }
            //CalendarCell cell = calendarScraper.GetCellData(calendarImage, 17);
            //Console.WriteLine($"{month}, w związku z powyższym: {calendarScraper.months[calendarScraper.RemoveSpecialCharacters(month)]}");
        }
 
    }

}
