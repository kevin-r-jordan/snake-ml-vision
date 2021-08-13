using Microsoft.ML.Data;

/// <summary>
/// mageData class holding the image path and label.
/// </summary>
public class InMemoryImageData
{
    [LoadColumn(0)]
    public byte[] Image;

    [LoadColumn(1)]
    public string Label;
}

/// <summary>
/// ImagePrediction class holding the score and predicted label metrics. 
/// </summary>
public class ImagePrediction
{
    [ColumnName("Score")]
    public float[] Score;

    [ColumnName("PredictedLabel")]
    public string PredictedLabel;
}