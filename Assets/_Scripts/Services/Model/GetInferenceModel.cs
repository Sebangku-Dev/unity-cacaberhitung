using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Unity.Barracuda;
using System;
using UnityEngine.Rendering;

public class GetInferenceModel : MonoBehaviour
{
    public Texture2D texture;
    public NNModel modelAsset;
    private Model runtimeModel;
    private IWorker engine;

    [Serializable]
    public struct Prediction
    {
        public int predictedValue;
        public float[] predicted;
        public void SetPrediction(Tensor t)
        {
            predicted = t.AsFloats();
            predictedValue = Array.IndexOf(predicted, predicted.Max());

            Debug.Log($"Predicted : {predictedValue}");
        }
    }

    public Prediction prediction;
    // Start is called before the first frame update
    void Start()
    {
        runtimeModel = ModelLoader.Load(modelAsset);
        engine = WorkerFactory.CreateWorker(runtimeModel, WorkerFactory.Device.GPU);

        prediction = new Prediction();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) //Input.touchCount > 0 || Input.GetMouseButton(0)
        {
            var channelCount = 1; // grayscale, 3 = color, 4 = color + alpha
            var inputX = new Tensor(texture, channelCount);

            Tensor outputY = engine.Execute(inputX).PeekOutput();
            inputX.Dispose();

            prediction.SetPrediction(outputY);
        }
    }

    /// <summary>
    /// This function is called when the MonoBehaviour will be destroyed.
    /// </summary>
    void OnDestroy()
    {
        engine?.Dispose();
    }
}
