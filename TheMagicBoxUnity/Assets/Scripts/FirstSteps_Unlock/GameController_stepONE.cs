using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController_stepONE : MonoBehaviour
{
    public GameObject OuterRing, InnerRing;

    DoubleSides_FillAmount OuterRingFill, InnerRingFill;

    void Start()
    {
        OuterRingFill = OuterRing.GetComponent<DoubleSides_FillAmount>();
        InnerRingFill = InnerRing.GetComponent<DoubleSides_FillAmount>();

        OuterRingFill.SetFill(0.5f, 0.0f);
        InnerRingFill.SetFill(0.0f, 0.0f);
    }

    
    void Update()
    {
        
    }
}
