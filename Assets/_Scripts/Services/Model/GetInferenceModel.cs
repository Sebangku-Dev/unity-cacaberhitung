using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Unity.Barracuda;
using System;
using UnityEngine.Rendering;
using Unity.VisualScripting;

public class GetInferenceModel : MonoBehaviour
{
    public Texture2D texture;
    Texture2D textureToRender;
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

            if (predicted.Max() > 5.0f)
            {
                predictedValue = Array.IndexOf(predicted, predicted.Max());
            }
            else predictedValue = -1;

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

    public void OnCheckPrediction()
    {
        textureToRender = ResizeTextureGPU(texture, 28, 28);

        var channelCount = 1; // grayscale, 3 = color, 4 = color + alpha
        var inputX = new Tensor(textureToRender, channelCount);

        Tensor outputY = engine.Execute(inputX).PeekOutput();
        inputX.Dispose();

        prediction.SetPrediction(outputY);
    }

    public static Texture2D ResizeTextureGPU(Texture2D source, int newWidth, int newHeight)
    {
        RenderTexture rt = RenderTexture.GetTemporary(newWidth, newHeight);
        Graphics.Blit(source, rt);

        RenderTexture.active = rt;
        Texture2D result = new Texture2D(newWidth, newHeight);
        result.ReadPixels(new Rect(0, 0, newWidth, newHeight), 0, 0);
        result.Apply();

        RenderTexture.active = null;
        RenderTexture.ReleaseTemporary(rt);

        return result;
    }

    /// <summary>
    /// This function is called when the MonoBehaviour will be destroyed.
    /// </summary>
    void OnDestroy()
    {
        engine?.Dispose();
    }
}
