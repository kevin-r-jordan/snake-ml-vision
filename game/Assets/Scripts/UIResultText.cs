using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UIResultText : MonoBehaviour
{
    private Text _resultText;

    private void Awake()
    {
        _resultText = GetComponent<Text>();
    }

    private void Start()
    {
        GameManager.Instance.OnPredictionChanged += HandleOnPredictionChanged;
    }

    private void HandleOnPredictionChanged(ImagePrediction imagePrediction)
    {
        var predictionLabel = imagePrediction.Score.Max() > .5 ? imagePrediction.PredictedLabel : "UNKNOWN";
        _resultText.text = predictionLabel.ToUpper();
    }
}
