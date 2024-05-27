using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DrawAstexture : MonoBehaviour
{
    public Texture2D baseTexture, defaultTexture;
    [SerializeField] TextMeshProUGUI textHint;
    [SerializeField] GetInferenceModel model;

    // Update is called once per frame
    void Update()
    {
        DoMouseDrawing();
    }

    /// <summary>
    /// Allows drawing to the texture with a mouse
    /// </summary>
    /// <exception cref="Exception"></exception>
    private void DoMouseDrawing()
    {
        // foreach(Camera camera in Camera.allCameras) Debug.Log(camera.name);
        // Don't bother trying to run if we can't find the main camera.
        Camera camera = Camera.allCameras[1] ?? throw new Exception("Cannot find main camera");

        // Is the mouse being pressed?
        if (!Input.GetMouseButton(0) && !Input.GetMouseButton(1)) return;
        // Cast a ray into the scene from screenspace where the mouse is.
        Ray mouseRay = camera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // Do nothing if we aren't hitting anything.
        if (!Physics.Raycast(mouseRay, out hit)) return;
        // Do nothing if we didn't get hit.
        if (hit.collider.transform != transform) return;

        // Get the UV coordinate that the mouseRay hit
        Vector2 pixelUV = hit.textureCoord;
        pixelUV.x *= baseTexture.width;
        pixelUV.y *= baseTexture.height;

        // Set the color as white if the lmb is being pressed, black if rmb.
        Color colorToSet = Input.GetMouseButton(0) ? Color.white : Color.black;

        int x = (int)pixelUV.x;
        int y = (int)pixelUV.y;

        // for (int i = -6; i <= 6; i++)
        // {
        //     for (int j = -6; j <= 6; j++)
        //     {
        //         int newX = x + i;
        //         int newY = y + j;

        //         // Pastikan koordinat baru berada dalam batas tekstur
        //         if (newX >= 0 && newX < baseTexture.width && newY >= 0 && newY < baseTexture.height)
        //         {
        //             baseTexture.SetPixel(newX, newY, colorToSet);
        //         }
        //     }
        // }

        int radius = 10;

        for (int i = 0; i <= 2 * radius; i++)
        {
            // Loop untuk setiap kolom
            for (int j = 0; j <= 2 * radius; j++)
            {
                // Hitung jarak dari pusat lingkaran
                double distance = Math.Sqrt((i - radius) * (i - radius) + (j - radius) * (j - radius));

                // Jika jarak kurang dari atau sama dengan radius, cetak '*'
                if (distance <= radius)
                {
                    int newX = x + i-radius;
                    int newY = y + j-radius;

                    // Pastikan koordinat baru berada dalam batas tekstur
                    if (newX >= 0 && newX < baseTexture.width && newY >= 0 && newY < baseTexture.height)
                    {
                        baseTexture.SetPixel(newX, newY, colorToSet);
                    }
                }
            }
        }

        textHint.text = model.GetTextHint();
        baseTexture.Apply();

    }

    public void ResetTexture()
    {
        baseTexture.SetPixels(defaultTexture.GetPixels());
        baseTexture.Apply();
    }
}
