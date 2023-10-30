using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Advanced;
Console.WriteLine("123");

Image CropWhitespace(Image<Rgba32> sourceImage)
{
    //find whitespace
    int scanHeight = sourceImage.Height / 2;
    int scanWidth = sourceImage.Width / 2;
    int xLhs = 0;
    while (xLhs < sourceImage.Width)
    {
        if (sourceImage[xLhs, scanHeight].R < 120) break;
        xLhs++;
    }
    int xRhs = sourceImage.Width;
    while (xRhs > 0)
    {
        if (sourceImage[xRhs-1, scanHeight].R < 120) break;
        xRhs--;
    }
    int yLhs = 0;
    while (yLhs < sourceImage.Height)
    {
        if (sourceImage[scanHeight, yLhs].R < 120) break;
        yLhs++;
    }
    int yRhs = sourceImage.Height;
    while (yRhs > 0)
    {
        if (sourceImage[scanHeight - 1, yRhs - 1].R < 120) break;
        yRhs--;
    }
    Rectangle cropRectangle = new Rectangle(xLhs, yLhs, xRhs-xLhs, yRhs-yLhs);
    sourceImage.Mutate(x => x.Crop(cropRectangle));
    return sourceImage;
}
using (var image = Image.Load<Rgba32>("C:/Users/kangu/source/repos/WFAISchedule/WFAISchedule/sources/calendar.png"))
{
    //image.Mutate(x => x.Crop(new Rectangle(0, 0, 100, 100)));
    Console.WriteLine(CropWhitespace(image));
    //image[4, 10] = Color.FromRgba(1, 1, 1, 1);
    image.Save("amogus.png");
}