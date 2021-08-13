using Microsoft.ML.Data;

namespace ImageTrainer
{
    partial class Program
    {
        // ImageData class holding the image path and label.
        public class ImageData
        {
            [LoadColumn(0)]
            public string ImagePath;

            [LoadColumn(1)]
            public string Label;
        }
    }
}