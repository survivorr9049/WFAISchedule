using Tesseract;
namespace WFAISchedule {
    public class CalendarScraper {
        public Dictionary<char, char> specialCharacters = new Dictionary<char, char>() {
            {'ą', 'a'},
            {'ę', 'e'},
            {'ć', 'c'},
            {'ó', 'o'},
            {'ń', 'n'},
            {'ł', 'l'},
            {'ś', 's'},
            {'ż', 'z'}
        };
        public Dictionary<string, int> months = new Dictionary<string, int>()
        {
            {"styczen", 1},
            {"luty", 2},
            {"marzec", 3},
            {"kwiecien", 4},
            {"maj", 5},
            {"czerwiec", 6},
            {"lipiec", 7},
            {"sierpien", 8},
            {"wrzesien", 9},
            {"pazdziernik", 10},
            {"listopad", 11},
            {"grudzien", 12},
        };
        public CalendarCell GetCellData(Image<Rgba32> sourceImage, int index) {
            CalendarCell cell;
            CalendarProcessor calendarProcessor = new();
            int calendarOutline = 0;
            calendarProcessor.CropWhitespace(sourceImage);
            Vector2 calendarCellDimensions = calendarProcessor.FindCellDimensions(sourceImage, out calendarOutline);
            Vector2 pivotCell = calendarProcessor.FindPivotCell(sourceImage, calendarCellDimensions, calendarOutline);
            Vector2 pivotCellPosition = calendarProcessor.DetermineCellPosition(sourceImage, pivotCell, calendarCellDimensions, calendarOutline);
            Console.WriteLine($"pivot point: {pivotCell.x} {pivotCell.y}");
            Vector2 dimensions = calendarProcessor.GetTableDimensions(sourceImage, calendarCellDimensions);
            Vector2 position = new Vector2((index - 1) % dimensions.x, ((index - 1) / dimensions.x));
            position = new Vector2(dimensions.x - position.x, dimensions.y - position.y);
            Rectangle rect = calendarProcessor.GetCellRect(sourceImage, position, pivotCell, pivotCellPosition, calendarCellDimensions, calendarOutline, dimensions);
            Image<Rgba32> c = sourceImage.Clone();
            Console.WriteLine($"x: {rect.X} y: {rect.Y}; {rect.Width} x {rect.Height}");
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
        public string GetCalendarMonth(Image<Rgba32> sourceImage) {
            string month;
            CalendarProcessor calendarProcessor = new();
            Image<Rgba32> i = sourceImage.Clone();
            i.Mutate(x => x.Crop(calendarProcessor.GetMonthRect(i)));
            MemoryStream stream = new();
            i.Save(stream, sourceImage.Metadata.DecodedImageFormat!);
            byte[] bytes = stream.ToArray();
            using(var engine = new TesseractEngine(@"../../../tessdata", "eng", EngineMode.LstmOnly)) {
                engine.DefaultPageSegMode = PageSegMode.SingleBlock;
                using(var img = Pix.LoadFromMemory(bytes))
                using(var page = engine.Process(img)) {
                    month = page.GetText().Split()[0].ToLower();
                }
            }
            return month;
        }
        public string RemoveSpecialCharacters(string input) {
            return new String(input.Select(c => {
                if(specialCharacters.ContainsKey(c))
                    return specialCharacters[c];
                else return c;
            }).ToArray());
        }
    }
}
