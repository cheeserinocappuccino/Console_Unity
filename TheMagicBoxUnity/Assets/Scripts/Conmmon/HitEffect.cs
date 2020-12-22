using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class HitEffect : MonoBehaviour
{

    public Sprite[] allImages;

    Image ImageComponent;

    public float speed = 0.1f;
    float timer = 0;

    int nowFrames = 0;

    public bool called = false;
    // Start is called before the first frame update
    void Start()
    {
        ImageComponent = GetComponent<Image>();
        
    }

    // Update is called once per frame
    void Update()
    {
        if (called)
        {
            if (timer > 0)
            {
                timer -= Time.deltaTime;
            }
            else if (timer <= 0)
            {
                timer = speed;

                if (nowFrames < allImages.Length - 1)
                {
                    nowFrames += 1;
                }
                else
                {
                    nowFrames = 0;
                    ImageComponent.color = new Color(0, 0, 0, 0);
                    called = false;
                }
                ImageComponent.sprite = allImages[nowFrames];

            }

        }
        



    }

    public void CallHit(Color _color)
    {
        ImageComponent.color = _color;
        timer = speed;
        called = true;
    }
}
