using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Advanced;
using WFAISchedule;
Console.WriteLine("123");

using (var image = Image.Load<Rgba32>("../../../sources/schedule.png"))
{
    //image.Mutate(x => x.Crop(new Rectangle(0, 0, 100, 100)));
    //Image test = image.Clone();
    /* int outline = 0;
     CalendarProcessor calendarProcessor = new CalendarProcessor();
     calendarProcessor.CropWhitespace(image);
     Vector2 cellDimensions = calendarProcessor.FindCellDimensions(image, out outline);
     Console.WriteLine(cellDimensions.y);
     Console.WriteLine(cellDimensions.x);
     Vector2 pivotCell = calendarProcessor.FindPivotCell(image, cellDimensions, outline);
     Console.WriteLine($"pivot: x {pivotCell.x} y {pivotCell.y}");
     Console.WriteLine(outline);
     Vector2 pivotCellPosition = calendarProcessor.DetermineCellPosition(image, pivotCell, cellDimensions);
     Console.WriteLine($"{pivotCellPosition.x} X {pivotCellPosition.y}");
     Vector2 dimensions = calendarProcessor.GetTableDimensions(image, cellDimensions);
     Console.WriteLine($"{dimensions.x} X {dimensions.y}");

     for(int x = 1; x <= 5; x++)
     {
         for(int y = 1; y <= 5; y++)
         {
             Rectangle rect = calendarProcessor.GetCellRect(image, new Vector2(x, y), pivotCell, pivotCellPosition, cellDimensions, outline, dimensions);
             Image test = image.Clone();
             test.Mutate(x => x.Crop(rect));
             test.Save("amogus" + x + " x " + y + ".png");
         }
     }*/
    //calendarProcessor.CropWhitespace(image);
    /*Rectangle rect = calendarProcessor.GetCellRect(image, new Vector2(5,5), pivotCell, pivotCellPosition, cellDimensions, outline, dimensions);
    
    image.Mutate(x => x.Crop(rect));*/
    //Console.WriteLine(test.Width);
    //image[4, 10] = Color.FromRgba(1, 1, 1, 1);
    ScheduleProcessor scheduleProcessor = new ScheduleProcessor();
    scheduleProcessor.CropWhitespace(image);
    int outlineSize = 0;
    Vector2 cellDimensions = scheduleProcessor.FindCellDimensions(image, out outlineSize);
    Console.WriteLine($"{cellDimensions.x} x {cellDimensions.y}");
    Console.WriteLine(outlineSize);
    Vector2 tableDimensions = scheduleProcessor.GetTableDimensions(image, cellDimensions);
    Rectangle rect = scheduleProcessor.GetCellRect(image, cellDimensions, new Vector2(15, 12), tableDimensions, outlineSize);
    image.Mutate(x => x.Crop(rect));
    image.Save("amogus.png");
}