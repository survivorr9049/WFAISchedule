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

        //try {
            ScheduleScraper scraper = new();
            List<ScheduleCell> cells = scraper.GetDaySchedule(image, coordinates[0], coordinates[1]);
            foreach(ScheduleCell cell in cells) {
                Console.WriteLine(cell.textData.Split("\n")[0] + " " + cell.occupation.y + "h" + " " + cell.type);
                Console.WriteLine($" {(cell.always ? "always" : "according to calendar")}");
                Console.WriteLine($"Cell Code: {scheduleProcessor.GetCellCode(cell.textData, cell.type)} \n");
            }
            /*Vector2 requestedCell = new(coordinates[0], coordinates[1]);
            Vector2 tableDimensions = scheduleProcessor.GetTableDimensions(image, cellDimensions);
            Vector2 cellOccupation = new();
            Rectangle rect = scheduleProcessor.GetCellRect(image, cellDimensions, requestedCell, outlineSize, out cellOccupation);
            Image<Rgba32> i = image.Clone();
            i.Mutate(x => x.Crop(rect));
            //i.Save("output.png");
            MemoryStream stream = new();
            i.Save(stream, i.Metadata.DecodedImageFormat!);
            byte[] bytes = stream.ToArray();
            using(var engine = new TesseractEngine(@"../../../tessdata", "eng", EngineMode.LstmOnly))
            using(var img = Pix.LoadFromMemory(bytes)) 
            using(var page = engine.Process(img)) {
                var text = page.GetText();
                //Console.WriteLine(text);
                ScheduleCell cell = new ScheduleCell(cellOccupation, text, scheduleProcessor.GetCellType(cellOccupation, text), scheduleProcessor.GetCellFrequency(text));
                Console.WriteLine($"duration: {cell.occupation.y}h");
                Console.WriteLine(cell.textData.Split("\n")[0]);
                Console.WriteLine(cell.type);
            }  */
       /* } catch {
            Console.WriteLine($"Unable to get cell ({coordinates[0]}, {coordinates[1]})");
            continue;
        }*/

        //Console.WriteLine("Success! Saved to `output.png`");
    }

}