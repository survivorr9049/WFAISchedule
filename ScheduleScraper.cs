using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tesseract;
namespace WFAISchedule {
    public class ScheduleScraper {
        public List<ScheduleCell> GetDaySchedule(Image<Rgba32> sourceImage, int day) {
            ScheduleProcessor scheduleProcessor = new();
            int outlineSize = 0;
            Vector2 cellDimensions = scheduleProcessor.FindCellDimensions(sourceImage, out outlineSize);
            List<ScheduleCell> cells = new List<ScheduleCell>();
            Vector2 position = new Vector2((6 - day) * 3, 12);
            while(position.y > 0) {
                ScheduleCell currentCell = GetScheduleCell(sourceImage, position, cellDimensions, outlineSize, scheduleProcessor);
                position.y -= currentCell.occupation.y;
                cells.Add(currentCell);
            }
            //cells.Add(GetScheduleCell(sourceImage, new Vector2(1, 9), cellDimensions, outlineSize, scheduleProcessor));
            return cells;
        }
        public ScheduleCell GetScheduleCell(Image<Rgba32> sourceImage, Vector2 requestedCell, Vector2 cellDimensions, int outlineSize, ScheduleProcessor scheduleProcessor) {
            Vector2 cellOccupation = new();
            ScheduleCell cell;
            Rectangle rect = scheduleProcessor.GetCellRect(sourceImage, cellDimensions, requestedCell, outlineSize, out cellOccupation);
            Image<Rgba32> image = sourceImage.Clone();
            image.Mutate(x => x.Crop(rect));
            MemoryStream stream = new();
            image.Save(stream, image.Metadata.DecodedImageFormat!);
            byte[] bytes = stream.ToArray();
            using(var engine = new TesseractEngine(@"../../../tessdata", "eng", EngineMode.LstmOnly))
            using(var img = Pix.LoadFromMemory(bytes))
            using(var page = engine.Process(img)) {
                var text = page.GetText();
                cell = new ScheduleCell(cellOccupation, text, scheduleProcessor.GetCellType(cellOccupation, text));
            }
            return cell;
        }
    }
}
