using Microsoft.ML;
using System;
using System.IO;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public event Action<ImagePrediction> OnPredictionChanged;

    private PredictionEngine<InMemoryImageData, ImagePrediction> _predictionEngine;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;

            Debug.Log("Creating Context Object");
            MLContext mlContext = new MLContext();
            ITransformer trainedModel;
            DataViewSchema schema;
            
            string modelsPath = Application.dataPath + "\\Models\\model.zip";
            using (var stream = new FileStream(modelsPath, FileMode.Open, FileAccess.Read, FileShare.Read))
                trainedModel = mlContext.Model.Load(stream, out schema);
            Debug.Log("Model Loaded");

            _predictionEngine = mlContext.Model.CreatePredictionEngine<InMemoryImageData, ImagePrediction>(trainedModel);
            Debug.Log("Prediction Engine Generated");

            DontDestroyOnLoad(gameObject);
        }
    }

    public void PredictDirection(byte[] imageBytes)
    {
        InMemoryImageData imageToPredict = new InMemoryImageData { Image = imageBytes };

        ImagePrediction prediction = _predictionEngine.Predict(imageToPredict);

        if (OnPredictionChanged != null)
            OnPredictionChanged(prediction);
    }
}
