using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Advanced;
using WFAISchedule;
Console.WriteLine("123");

using (var image = Image.Load<Rgba32>("C:/Users/kangu/source/repos/WFAISchedule/WFAISchedule/sources/calendar1.png"))
{
    //image.Mutate(x => x.Crop(new Rectangle(0, 0, 100, 100)));
    ImageProcessor imageProcessor = new ImageProcessor();
    Console.WriteLine(imageProcessor.CropWhitespace(image));
    int outline = 0;
    Vector2 cellDimensions = imageProcessor.FindCellDimensions(image, out outline);
    Console.WriteLine(cellDimensions.y);
    Console.WriteLine(cellDimensions.x);
    Vector2 pivotCell = imageProcessor.FindPivotCell(image, cellDimensions, outline);
    Console.WriteLine($"pivot: x {pivotCell.x} y {pivotCell.y}");
    Console.WriteLine(outline);
    Vector2 pivotCellPosition = imageProcessor.DetermineCellPosition(image, pivotCell, cellDimensions);
    Console.WriteLine($"{pivotCellPosition.x} X {pivotCellPosition.y}");
    Vector2 dimensions = imageProcessor.GetTableDimensions(image, cellDimensions);
    Console.WriteLine($"{dimensions.x} X {dimensions.y}");


    Rectangle rect = imageProcessor.GetCellRect(image, new Vector2(5,5), pivotCell, pivotCellPosition, cellDimensions, outline, dimensions);
    image.Mutate(x => x.Crop(rect));

    //image[4, 10] = Color.FromRgba(1, 1, 1, 1);
    image.Save("amogus.png");
}