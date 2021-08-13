using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Snake : MonoBehaviour
{
    public Transform SegmentPrefab;
    public int InitialSize = 4;

    private Vector2 _direction = Vector2.right;
    private List<Transform> _segments = new List<Transform>();

    private void Start()
    {
        ResetState();

        GameManager.Instance.OnPredictionChanged += HandleOnPredictionChanged;
    }

    private void HandleOnPredictionChanged(ImagePrediction imagePrediction)
    {
        var predictionLabel = (imagePrediction.Score.Max() > .5 ? imagePrediction.PredictedLabel : "UNKNOWN").ToUpper();
        if (predictionLabel == "UP" && _direction != Vector2.down)
            _direction = Vector2.up;
        else if (predictionLabel == "DOWN" && _direction != Vector2.up)
            _direction = Vector2.down;
        else if (predictionLabel == "LEFT" && _direction != Vector2.right)
            _direction = Vector2.left;
        else if (predictionLabel == "RIGHT" && _direction != Vector2.left)
            _direction = Vector2.right;
    }
    
    private void FixedUpdate()
    {
        for (int i = _segments.Count - 1; i > 0; i--)
        {
            _segments[i].position = _segments[i - 1].position;
        }

        this.transform.position = new Vector3(
            Mathf.Round(this.transform.position.x + _direction.x),
            Mathf.Round(this.transform.position.y + _direction.y),
            0.0f);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Food")
            Grow();
        else if (other.tag == "Player" || other.tag == "Obstacle")
            ResetState();
    }

    private void Grow()
    {
        Transform segment = Instantiate(this.SegmentPrefab);
        segment.position = _segments[_segments.Count - 1].position;
        _segments.Add(segment);
    }

    private void ResetState()
    {
        for (int i = 1; i < _segments.Count; i++)
            Destroy(_segments[i].gameObject);

        _segments.Clear();
        _segments.Add(this.transform);
        this.transform.position = Vector3.zero;

        for (int i = 1; i < InitialSize; i++)
            Grow();
    }

}
