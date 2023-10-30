using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Advanced;
using WFAISchedule;
Console.WriteLine("123");

using (var image = Image.Load<Rgba32>("C:/Users/kangu/source/repos/WFAISchedule/WFAISchedule/sources/calendar1.png"))
{
    //image.Mutate(x => x.Crop(new Rectangle(0, 0, 100, 100)));
    ImageProcessing imageProcessor = new ImageProcessing();
    Console.WriteLine(imageProcessor.CropWhitespace(image));
    Console.WriteLine(imageProcessor.FindCellDimensions(image).y);
    Console.WriteLine(imageProcessor.FindCellDimensions(image).x);
    //image[4, 10] = Color.FromRgba(1, 1, 1, 1);
    image.Save("amogus.png");
}