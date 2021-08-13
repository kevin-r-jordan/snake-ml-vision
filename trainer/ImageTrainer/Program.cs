using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.ML;
using Microsoft.ML.Vision;
using static Microsoft.ML.DataOperationsCatalog;

namespace ImageTrainer
{
    partial class Program
    {
        static void Main(string[] args)
        {
            // Set the path for input images.
            var projectDirectory = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "../../../"));
            var assetsRelativePath = Path.Combine(projectDirectory, "assets");

            MLContext mlContext = new MLContext(seed: 1);

            // Load all the original images info.
            IEnumerable<ImageData> images = LoadImagesFromDirectory(folder: assetsRelativePath, useFolderNameAsLabel: true);

            // Shuffle images.
            IDataView shuffledFullImagesDataset = mlContext.Data.ShuffleRows(mlContext.Data.LoadFromEnumerable(images));

            // Apply transforms to the input dataset:
            // MapValueToKey : map 'string' type labels to keys
            // LoadImages : load raw images to "Image" column
            shuffledFullImagesDataset = mlContext.Transforms.Conversion
                .MapValueToKey("Label", keyOrdinality: Microsoft.ML.Transforms.ValueToKeyMappingEstimator.KeyOrdinality.ByValue)
                .Append(mlContext.Transforms.LoadRawImageBytes("Image", assetsRelativePath, "ImagePath"))
                .Fit(shuffledFullImagesDataset)
                .Transform(shuffledFullImagesDataset);

            // Split the data 90:10 into train and test sets.
            TrainTestData trainTestData = mlContext.Data.TrainTestSplit(shuffledFullImagesDataset, testFraction: 0.1, seed: 1);
            IDataView trainDataset = trainTestData.TrainSet;
            IDataView testDataset = trainTestData.TestSet;

            // Set the options for ImageClassification.
            var options = new ImageClassificationTrainer.Options()
            {
                FeatureColumnName = "Image",
                LabelColumnName = "Label",
                Arch = ImageClassificationTrainer.Architecture.ResnetV2101,
                Epoch = 50,
                BatchSize = 10,
                LearningRate = 0.01f,
                MetricsCallback = (metrics) => Console.WriteLine(metrics),
                ValidationSet = testDataset,
                EarlyStoppingCriteria = null
            };

            // Create the ImageClassification pipeline.
            var pipeline = mlContext.MulticlassClassification.Trainers.ImageClassification(options)
                .Append(mlContext.Transforms.Conversion.MapKeyToValue(outputColumnName: "PredictedLabel", inputColumnName: "PredictedLabel"));

            Console.WriteLine("*** Training the image classification model with DNN Transfer Learning on top of the selected pre-trained model/architecture ***");

            // Train the model
            // This involves calculating the bottleneck values, and then
            // training the final layer. Sample output is: 
            // Phase: Bottleneck Computation, Dataset used: Train, Image Index:   1
            // Phase: Bottleneck Computation, Dataset used: Train, Image Index:   2
            // ...
            // Phase: Training, Dataset used:      Train, Batch Processed Count:  18, Learning Rate:       0.01 Epoch:   0, Accuracy:  0.9166667, Cross-Entropy:  0.4787029
            // ...
            // Phase: Training, Dataset used:      Train, Batch Processed Count:  18, Learning Rate: 0.002265001 Epoch:  49, Accuracy:          1, Cross-Entropy: 0.03529362
            // Phase: Training, Dataset used: Validation, Batch Processed Count:   3, Epoch:  49, Accuracy:  0.8238096
            var trainedModel = pipeline.Fit(trainDataset);

            Console.WriteLine("Training with transfer learning finished.");

            // Save the trained model.
            mlContext.Model.Save(trainedModel, shuffledFullImagesDataset.Schema, "model.zip");

            Console.WriteLine("Done!");
        }

        static IEnumerable<ImageData> LoadImagesFromDirectory(string folder, bool useFolderNameAsLabel = true)
        {
            var files = Directory.GetFiles(folder, "*", searchOption: SearchOption.AllDirectories);

            foreach (var file in files)
            {
                if ((Path.GetExtension(file) != ".jpg") && (Path.GetExtension(file) != ".png"))
                    continue;

                var label = Path.GetFileName(file);

                if (useFolderNameAsLabel)
                    label = Directory.GetParent(file).Name;
                else
                {
                    for (int index = 0; index < label.Length; index++)
                    {
                        if (!char.IsLetter(label[index]))
                        {
                            label = label.Substring(0, index);
                            break;
                        }
                    }
                }

                yield return new ImageData()
                {
                    ImagePath = file,
                    Label = label
                };
            }
        }
    }
}