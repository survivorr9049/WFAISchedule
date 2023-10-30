using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Advanced;
using WFAISchedule;
Console.WriteLine("123");

using (var image = Image.Load<Rgba32>("C:/Users/kangu/source/repos/WFAISchedule/WFAISchedule/sources/calendar2.png"))
{
    //image.Mutate(x => x.Crop(new Rectangle(0, 0, 100, 100)));
    ImageProcessor imageProcessor = new ImageProcessor();
    Console.WriteLine(imageProcessor.CropWhitespace(image));
    Vector2 cellDimensions = imageProcessor.FindCellDimensions(image);
    Console.WriteLine(cellDimensions.y);
    Console.WriteLine(cellDimensions.x);
    Console.WriteLine(imageProcessor.FindPivotCell(image, cellDimensions).x + "s");
    Console.WriteLine(imageProcessor.FindPivotCell(image, cellDimensions).y);

    //image[4, 10] = Color.FromRgba(1, 1, 1, 1);
    image.Save("amogus.png");
}