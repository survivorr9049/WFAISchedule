using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WFAISchedule {
    public class ScheduleProcessor : ImageProcessor {
        public Vector2 FindCellDimensions(Image<Rgba32> sourceImage, out int outlineSize) {
            Vector2 cellDimensions = new Vector2(0, 0);
            Vector2 position = new Vector2(sourceImage.Width-1, sourceImage.Height-1);
            int threshold = 60;
            outlineSize = 1;
            int i = 0;
            while(sourceImage[position.x, position.y].R < threshold) {
                position.x--;
                position.y--;
            }
            //check if color is in grayscale (empty tile)
            while(!((sourceImage[position.x, position.y].R == sourceImage[position.x, position.y].G) && (sourceImage[position.x, position.y].R == sourceImage[position.x, position.y].B)) || sourceImage[position.x, position.y].R < threshold) {
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
        public Rectangle GetCellRect(Image<Rgba32> sourceImage, Vector2 cellDimensions, Vector2 cellPosition, Vector2 tableDimensions, int outlineSize) {
            Vector2 rectPosition = new Vector2(0, 0);
            rectPosition.x = sourceImage.Width - cellPosition.x * (cellDimensions.x + outlineSize);
            rectPosition.y = sourceImage.Height - cellPosition.y * (cellDimensions.y + outlineSize);
            Rectangle cellRect = new Rectangle(rectPosition.x, rectPosition.y, cellDimensions.x, cellDimensions.y);
            return cellRect;
        }
    }
}
