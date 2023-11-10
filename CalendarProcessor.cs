namespace WFAISchedule {
    public class CalendarProcessor : ImageProcessor {
        //Dictionary<string, int> MonthIndex = 
        public Vector2 FindCellDimensions(Image<Rgba32> sourceImage, out int outlineSize) {
            int threshold = Config.threshold;
            Vector2 dimensions = new Vector2(0, 0);
            Vector2 startPosition = new Vector2(sourceImage.Width - 4, sourceImage.Height / 2);
            int y = startPosition.y;
            int dir = 1;
            int i = 0;
            int bottomEdge = 0;
            outlineSize = 0;
            while(y < sourceImage.Height) {
                if(sourceImage[startPosition.x, y].R < threshold) {
                    if(dir > 0) {
                        while(outlineSize < sourceImage.Height) {
                            outlineSize++;
                            if(sourceImage[startPosition.x, y + outlineSize].R > threshold) break;
                        }
                        dir = -1;
                        y = startPosition.y - 1;
                    } else {
                        bottomEdge = y;
                        break;
                    }
                }
                y += dir;
                i++;
            }
            dimensions.y = i;
            dir = 1;
            int x = startPosition.x;
            i = 0;
            while(x < sourceImage.Width) {
                if(sourceImage[x, bottomEdge + 1].R < threshold) {
                    if(dir > 0) {
                        dir = -1;
                        x = startPosition.x - 1;
                    } else break;
                }
                x += dir;
                i++;
            }
            dimensions.x = i;
            return dimensions;
        }
        public Vector2 FindPivotCell(Image<Rgba32> sourceImage, Vector2 cellSize, int outlineSize = 2) {
            int threshold = Config.threshold;
            Vector2 pivotCell = new Vector2(outlineSize, 0);
            int y = sourceImage.Height / 2;
            bool foundEdge = false;
            while(y < sourceImage.Height) {
                if(sourceImage[outlineSize + 1, y].R < threshold) {
                    foundEdge = true;
                    for(int i = 0; i < cellSize.x / 2; i++) {
                        //check for straight horizontal line to make sure it didn't scan some random text
                        if(sourceImage[i * 2 + 2, y].R > threshold) foundEdge = false;
                    }
                }
                if(foundEdge) {
                    pivotCell.y = y + outlineSize;
                    break;
                }
                y++;
            }
            return pivotCell;
        }
        public Vector2 DetermineCellPosition(Image<Rgba32> sourceImage, Vector2 pivotPoint, Vector2 cellDimensions, int outlineSize) {
            Vector2 position = new Vector2(0, 0);
            position.y = (sourceImage.Height - pivotPoint.y) / (cellDimensions.y - outlineSize);
            position.x = (sourceImage.Width - pivotPoint.x) / (cellDimensions.x - outlineSize);
            return position;
        }
        public Vector2 GetTableDimensions(Image<Rgba32> sourceImage, Vector2 cellDimensions) {
            Vector2 dimensions = new Vector2(0, 0);
            dimensions.x = Math.Clamp(sourceImage.Width / cellDimensions.x, 0, 5);
            dimensions.y = Math.Clamp(sourceImage.Height / cellDimensions.y, 0, 5);
            return dimensions;
        }
        public Rectangle GetCellRect(Image<Rgba32> sourceImage, Vector2 cellPosition, Vector2 pivotPoint, Vector2 pivotCellPosition, Vector2 cellDimensions, int outlineSize, Vector2 tableDimensions) {
            Vector2 localPosition = pivotCellPosition - cellPosition;
            Console.WriteLine(pivotCellPosition.x + " " + pivotCellPosition.y);
            Console.WriteLine(cellPosition.x + " " + cellPosition.y);
            Console.WriteLine(localPosition.x + " " + localPosition.y);
            Vector2 rectPosition = new Vector2(localPosition.x * (cellDimensions.x + outlineSize) + pivotPoint.x, localPosition.y * (cellDimensions.y + outlineSize) + pivotPoint.y);
            Vector2 crop = new Vector2(0, 0);
            if(rectPosition.x + cellDimensions.x - 1 > sourceImage.Width) crop.x = (rectPosition.x + cellDimensions.x - 1) - sourceImage.Width;
            if(rectPosition.y + cellDimensions.y - 1 > sourceImage.Height) crop.y = (rectPosition.y + cellDimensions.y - 1) - sourceImage.Height;
            Console.WriteLine($"crop: {crop.x} {crop.y}");
            Rectangle rect = new Rectangle(rectPosition.x, rectPosition.y, cellDimensions.x - 1 - crop.x, cellDimensions.y - 1 - crop.y);
            return rect;
        }
        public Rectangle GetMonthRect(Image<Rgba32> sourceImage) {
            Vector2 position = new Vector2(sourceImage.Width / 6, 0);
            while(sourceImage[position.x, position.y].R < Config.threshold) {
                position.y++;
            }
            int count = 0;
            while(sourceImage[position.x, position.y].R >= Config.threshold) {
                count++;
                position.y++;
            }
            return new Rectangle(0, 0, sourceImage.Width, count);
        }
        public List<string> GetCellSubjects(string textData, out int day) {
            List<string> entries;
            entries = textData.Split().ToList();
            int i = -1;
            day = 0;
            foreach(string entry in entries) {
                try {
                    i++;
                    day = int.Parse(entry);
                    break;
                } catch {
                }
            }
            entries.RemoveAt(i);
            entries.RemoveAll(s => string.IsNullOrWhiteSpace(s));
            return entries;
        }
    }
    
}
