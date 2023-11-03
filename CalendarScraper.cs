using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tesseract;
namespace WFAISchedule {
    public class CalendarScraper {
        public CalendarCell GetCellData(Image<Rgba32> sourceImage, int index) {
            CalendarCell cell;
            CalendarProcessor calendarProcessor = new();
            int calendarOutline = 0;
            calendarProcessor.CropWhitespace(sourceImage);
            Vector2 calendarCellDimensions = calendarProcessor.FindCellDimensions(sourceImage, out calendarOutline);
            Vector2 pivotCell = calendarProcessor.FindPivotCell(sourceImage, calendarCellDimensions, calendarOutline);
            Vector2 pivotCellPosition = calendarProcessor.DetermineCellPosition(sourceImage, pivotCell, calendarCellDimensions);
            Vector2 dimensions = calendarProcessor.GetTableDimensions(sourceImage, calendarCellDimensions);
            Vector2 position = new Vector2((index % dimensions.x - 1), (index / dimensions.x));
            position = new Vector2(dimensions.x - position.x, dimensions.y - position.y);
            Rectangle rect = calendarProcessor.GetCellRect(sourceImage, position, pivotCell, pivotCellPosition, calendarCellDimensions, calendarOutline, dimensions);
            Image<Rgba32> c = sourceImage.Clone();
            c.Mutate(x => x.Crop(rect));
            MemoryStream stream = new();
            c.Save(stream, sourceImage.Metadata.DecodedImageFormat!);
            byte[] bytes = stream.ToArray();
            using(var engine = new TesseractEngine(@"../../../tessdata", "eng", EngineMode.LstmOnly)) {
                engine.DefaultPageSegMode = PageSegMode.SingleBlock;
                using(var img = Pix.LoadFromMemory(bytes))
                using(var page = engine.Process(img)) {
                    var text = page.GetText();
                    int day = 0;
                    cell = new CalendarCell(calendarProcessor.GetCellSubjects(text, out day), day);
                }
            }
            return cell;
        }
    }
}
