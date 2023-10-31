using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WFAISchedule
{
    public class ImageProcessor
    {
        public Image CropWhitespace(Image<Rgba32> sourceImage)
        {
            //find whitespace
            int threshold = 120;
            int scanHeight = sourceImage.Height / 2;
            int scanWidth = sourceImage.Width / 2;
            int xLhs = 0;
            while (xLhs < sourceImage.Width)
            {
                if (sourceImage[xLhs, scanHeight].R < threshold) break;
                xLhs++;
            }
            int xRhs = sourceImage.Width;
            while (xRhs > 0)
            {
                if (sourceImage[xRhs - 1, scanHeight].R < threshold) break;
                xRhs--;
            }
            int yLhs = 0;
            while (yLhs < sourceImage.Height)
            {
                if (sourceImage[scanHeight, yLhs].R < threshold) break;
                yLhs++;
            }
            int yRhs = sourceImage.Height;
            while (yRhs > 0)
            {
                if (sourceImage[scanHeight - 1, yRhs - 1].R < threshold) break;
                yRhs--;
            }
            //prepare crop dimensions
            Rectangle cropRectangle = new Rectangle(xLhs, yLhs, xRhs - xLhs, yRhs - yLhs);
            sourceImage.Mutate(x => x.Crop(cropRectangle));
            return sourceImage;
        }
        public Vector2 FindCellDimensions(Image<Rgba32> sourceImage, out int outlineSize)
        {
            int threshold = 120;
            Vector2 dimensions = new Vector2(0, 0);
            Vector2 startPosition = new Vector2(sourceImage.Width - 4, sourceImage.Height / 2);
            int y = startPosition.y;
            int dir = 1;
            int i = 0;
            int bottomEdge = 0;
            outlineSize = 0;
            while(y < sourceImage.Height)
            {
                if(sourceImage[startPosition.x, y].R < threshold)
                {
                    if (dir > 0)
                    {
                        while(outlineSize < sourceImage.Height)
                        {
                            outlineSize++;
                            if (sourceImage[startPosition.x, y + outlineSize].R > threshold) break;
                        }
                        dir = -1;
                        y = startPosition.y - 1;
                    }
                    else
                    {
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
            while (x < sourceImage.Width)
            {
                if (sourceImage[x, bottomEdge + 1].R < threshold)
                {
                    if (dir > 0)
                    {
                        dir = -1;
                        x = startPosition.x - 1;
                    }
                    else break;
                }
                x += dir;
                i++;
            }
            dimensions.x = i;
            return dimensions;
        }
        public Vector2 FindPivotCell(Image<Rgba32> sourceImage, Vector2 cellSize, int outlineSize = 2)
        {
            int threshold = 120;
            Vector2 pivotCell = new Vector2(outlineSize, 0);
            int y = sourceImage.Height / 2;
            bool foundEdge = false;
            while (y < sourceImage.Height)
            {   
                if(sourceImage[outlineSize + 1, y].R < threshold)
                {
                    foundEdge = true;
                    for(int i = 0; i < cellSize.x / 2; i++)
                    {
                        //check for straight horizontal line to make sure it didn't scan some random text
                        if (sourceImage[i * 2 + 2, y].R > threshold) foundEdge = false;
                    }
                }
                if (foundEdge)
                {
                    pivotCell.y = y + outlineSize;
                    break;
                }
                y++;
            }
            return pivotCell;
        }
        public Vector2 DetermineCellPosition(Image<Rgba32> sourceImage, Vector2 pivotPoint, Vector2 cellDimensions)
        {
            Vector2 position = new Vector2(0, 0);
            position.y = (sourceImage.Height - pivotPoint.y) / cellDimensions.y;
            position.x = (sourceImage.Width - pivotPoint.x) / cellDimensions.x;
            return position;
        }
        public Vector2 GetTableDimensions(Image<Rgba32> sourceImage, Vector2 cellDimensions)
        {
            Vector2 dimensions = new Vector2(0, 0);
            dimensions.x = sourceImage.Width / cellDimensions.x;
            dimensions.y = sourceImage.Height / cellDimensions.y;
            return dimensions;
        }
        public Rectangle GetCellRect(Image<Rgba32> sourceImage, Vector2 cellPosition, Vector2 pivotPoint, Vector2 pivotCellPosition, Vector2 cellDimensions, int outlineSize, Vector2 tableDimensions)
        {
            Vector2 localPosition = pivotCellPosition - cellPosition;
            Console.WriteLine(localPosition.x + " " + localPosition.y);
            Vector2 rectPosition = new Vector2(localPosition.x * (cellDimensions.x + outlineSize) + pivotPoint.x, localPosition.y * (cellDimensions.y + outlineSize) + pivotPoint.y);
            Rectangle rect = new Rectangle(rectPosition.x, rectPosition.y, cellDimensions.x - 1, cellDimensions.y - 1);
            return rect;
        }
    }
}
