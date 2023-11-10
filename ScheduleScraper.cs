using Tesseract;
namespace WFAISchedule {
    public class ScheduleScraper {
        public List<ScheduleCell> GetDaySchedule(Image<Rgba32> sourceImage, int day, int group) {
            ScheduleProcessor scheduleProcessor = new();
            int outlineSize = 0;
            scheduleProcessor.CropWhitespace(sourceImage);
            Vector2 cellDimensions = scheduleProcessor.FindCellDimensions(sourceImage, out outlineSize);
            List<ScheduleCell> cells = new List<ScheduleCell>();
            Vector2 position = new Vector2((6 - day) * 3, 12);
            while(position.y > 0) {
                ScheduleCell currentCell = GetScheduleCell(sourceImage, position, cellDimensions, outlineSize, scheduleProcessor);
                if(currentCell.type != CellType.Lecture)
                    if(group > 1) currentCell = GetScheduleCell(sourceImage, position - new Vector2(group - 1, 0), cellDimensions, outlineSize, scheduleProcessor);
                position.y -= currentCell.occupation.y;
                //omit tri-colored group and overlapping cells
                if(currentCell.type == CellType.Lecture && currentCell.textData.Contains("lab") || currentCell.occupation.y < 2 && currentCell.type != CellType.Empty) {
                    ScheduleCell dummyCell = new ScheduleCell(new Vector2(1, 1), "", CellType.Empty, true);
                    for(int i = 0; i < currentCell.occupation.y; i++) {
                        cells.Add(dummyCell);
                        Console.WriteLine("omiting " + position.y);
                    }
                    continue;
                }
                cells.Add(currentCell);
            }
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
                cell = new ScheduleCell(cellOccupation, text, scheduleProcessor.GetCellType(cellOccupation, text), scheduleProcessor.GetCellFrequency(text));
            }
            return cell;
        }
    }
}
