using WFAISchedule;

using (var image = Image.Load<Rgba32>("sources/schedule.png"))
{
    ScheduleProcessor scheduleProcessor = new();
    scheduleProcessor.CropWhitespace(image);
    int outlineSize = 0;
    Vector2 cellDimensions = scheduleProcessor.FindCellDimensions(image, out outlineSize);
    Console.WriteLine($"Detected cell dimensions: {cellDimensions.x} x {cellDimensions.y}");
    Console.WriteLine($"Detected outline size: {outlineSize}");
    while (true) {
        Console.Write("Provide the coordinates of the requested cell (x, y): ");
        string coordinateString = Console.ReadLine() ?? "0, 0";
        int[] coordinates = coordinateString.Split(',').Select(c => {
            try {
                int parsed = int.Parse(c);
                return parsed;
            } catch {
                return -1;
            }
        }).ToArray();

        if (coordinates.Length != 2 || coordinates.Contains(-1)){
            Console.WriteLine("Format error");
            continue;
        }

        try {
            Vector2 requestedCell = new(coordinates[0], coordinates[1]);
            Vector2 tableDimensions = scheduleProcessor.GetTableDimensions(image, cellDimensions);
            Rectangle rect = scheduleProcessor.GetCellRect(image, cellDimensions, requestedCell, tableDimensions, outlineSize);
            Image i = image.Clone();
            i.Mutate(x => x.Crop(rect));
            i.Save("output.png");
        } catch {
            Console.WriteLine($"Unable to get cell ({coordinates[0]}, {coordinates[1]})");
            continue;
        }

        Console.WriteLine("Success! Saved to `output.png`");
    }
}