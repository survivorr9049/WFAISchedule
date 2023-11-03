using WFAISchedule;
using Tesseract;

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
        using(var calendarImage = Image.Load<Rgba32>("../../../sources/calendar.png")) {
           
            CalendarScraper calendarScraper = new();
            CalendarCell cell = calendarScraper.GetCellData(calendarImage, 17);
            Console.WriteLine($"{cell.day} \n {string.Join("\n", cell.subjects)}");
        }
 
    }

}
