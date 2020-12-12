using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class Gif : MonoBehaviour
{
    public Sprite[] allImages;

    Image ImageComponent;

    float timer = 0;
    public int xFramePerSeconds = 1;
    float speedCalculated = 0;

    int nowFrames = 0;

    void Start()
    {
        ImageComponent = GetComponent<Image>();
        speedCalculated = 1 / (float)xFramePerSeconds;
        timer = speedCalculated;
    }


    void FixedUpdate()
    {



        if (timer <= 0)
        {
            if (nowFrames < allImages.Length - 1)
            {
                nowFrames += 1;
            }
            else
            {
                nowFrames = 0;
            }

            ImageComponent.sprite = allImages[nowFrames];
            timer = speedCalculated;


        }
        else
        {
            timer -= Time.deltaTime;
        }

        

    }
}
