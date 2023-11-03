namespace WFAISchedule {
    public class ScheduleProcessor : ImageProcessor {
        public Vector2 FindCellDimensions(Image<Rgba32> sourceImage, out int outlineSize) {
            Vector2 cellDimensions = new Vector2(0, 0);
            Vector2 position = new Vector2(sourceImage.Width-1, sourceImage.Height-1);
            int threshold = Config.threshold;
            outlineSize = 1;
            int i = 0;
            while(sourceImage[position.x, position.y].R < threshold) {
                position.x--;
                position.y--;
            }
            //check if color is in grayscale (empty tile)
            while(!IsGrayscale(sourceImage[position.x, position.y]) || sourceImage[position.x, position.y].R < threshold) {
                position.x--;
            }
            while(sourceImage[position.x, position.y].R > threshold) {
                position.x--;
                i++;
            }
            cellDimensions.x = i;
            i = 0;
            position.x++;
            while(sourceImage[position.x, position.y].R > threshold) {
                position.y--;
                i++;
            }
            cellDimensions.y = i;
            while(sourceImage[position.x, position.y].R < threshold) {
                position.y--;
                outlineSize++;
            }
            return cellDimensions;
        }
        public Vector2 GetTableDimensions(Image<Rgba32> sourceImage, Vector2 cellDimensions) {
            Vector2 dimensions = new Vector2(sourceImage.Width / cellDimensions.x, sourceImage.Height / cellDimensions.y);
            return dimensions;
        }

        public Rectangle GetCellRect(Image<Rgba32> sourceImage, Vector2 cellDimensions, Vector2 cellPosition, int outlineSize, out Vector2 cellOccupation) {
            Vector2 rectPosition = new Vector2(0, 0);
            int threshold = Config.threshold;
            rectPosition.x = sourceImage.Width - cellPosition.x * (cellDimensions.x + outlineSize * 2);
            rectPosition.y = sourceImage.Height - cellPosition.y * (cellDimensions.y + outlineSize * 2);
            Vector2 position = rectPosition;
            //find cell height
            int i = 0;
            while(sourceImage[position.x, position.y].R > threshold) {
                i++;
                position.y++;
            }
            int height = Math.Clamp((int)Math.Round((float)i / (float)cellDimensions.y), 1, 3);
            //find cell width
            i = 0;
            position = rectPosition;
            while(sourceImage[position.x, position.y].R > threshold) {
                i++;
                position.x++;
            }
            int width = Math.Clamp((int)Math.Round((float)i / (float)cellDimensions.x), 1, 3);
            Rectangle cellRect = new Rectangle(rectPosition.x, rectPosition.y, cellDimensions.x * width, cellDimensions.y * height);
            cellOccupation = new Vector2(width, height);
            return cellRect;
        }
        public CellType GetCellType(Vector2 occupation, string textData) {
            if(occupation.x > 1) return CellType.Lecture;
            else if(textData.Length == 0) return CellType.Empty;
            return CellType.Practice;
        }
        public bool GetCellFrequency(string textData) {
            if(textData.ToLower().Contains("kalen")) return false;
            return true;
        }
        public string GetCellCode(string textData, CellType type) {
            if(type == CellType.Practice) return textData.Split()[0];
            if(type == CellType.Lecture) return textData.Substring(textData.IndexOf("(") + 1, textData.IndexOf(")") - textData.IndexOf("(") - 1);
            return "";
        }
    }
}
