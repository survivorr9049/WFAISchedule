using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WFAISchedule {
    public class ImageProcessor {
        public void CropWhitespace(Image<Rgba32> sourceImage) {
            //find whitespace
            int threshold = 120;
            int scanHeight = sourceImage.Height / 2;
            int scanWidth = sourceImage.Width / 2;
            int xLhs = 0;
            while(xLhs < sourceImage.Width) {
                if(sourceImage[xLhs, scanHeight].R < threshold) break;
                xLhs++;
            }
            int xRhs = sourceImage.Width;
            while(xRhs > 0) {
                if(sourceImage[xRhs - 1, scanHeight].R < threshold) break;
                xRhs--;
            }
            int yLhs = 0;
            while(yLhs < sourceImage.Height) {
                if(sourceImage[scanHeight, yLhs].R < threshold) break;
                yLhs++;
            }
            int yRhs = sourceImage.Height;
            while(yRhs > 0) {
                if(sourceImage[scanHeight - 1, yRhs - 1].R < threshold) break;
                yRhs--;
            }
            //prepare crop dimensions
            Rectangle cropRectangle = new Rectangle(xLhs, yLhs, xRhs - xLhs, yRhs - yLhs);
            sourceImage.Mutate(x => x.Crop(cropRectangle));
        }
        public bool IsGrayscale(Rgba32 pixel, int maxDelta = 2) {
            int deltaRB = Math.Abs(pixel.R - pixel.B);
            int deltaBG = Math.Abs(pixel.B - pixel.G);
            int delta = Math.Max(deltaRB, deltaBG);
            return delta <= maxDelta;
        }
        
    }
}
