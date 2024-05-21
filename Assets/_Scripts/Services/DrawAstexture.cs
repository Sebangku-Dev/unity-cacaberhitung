using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawAstexture : MonoBehaviour
{
    public Texture2D baseTexture, defaultTexture;
    
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

        // Update the texture and apply.
        baseTexture.SetPixel((int)pixelUV.x, (int)pixelUV.y, colorToSet);
        baseTexture.Apply();

    }

    public void ResetTexture(){
        baseTexture.SetPixels(defaultTexture.GetPixels());
        baseTexture.Apply();
    }
}
