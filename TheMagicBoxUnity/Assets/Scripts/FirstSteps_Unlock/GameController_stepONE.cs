using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController_stepONE : MonoBehaviour
{
    public GameObject OuterRing, InnerRing;

    DoubleSides_FillAmount OuterRingFill, InnerRingFill;

    ControllerController controllerController;

    // 記錄轉過的最大值
    float maxInnerCL, maxInnerCCL = 0;
    float maxOutterCL, maxOutterCCL = 0;

    void Start()
    {
        OuterRingFill = OuterRing.GetComponent<DoubleSides_FillAmount>();
        InnerRingFill = InnerRing.GetComponent<DoubleSides_FillAmount>();

        controllerController = GameObject.FindGameObjectWithTag("controllerController").GetComponent<ControllerController>();

        /*OuterRingFill.SetFill(0.5f, 0.0f);*/
        InnerRingFill.SetFill(0.6f, 0.0f);
    }

    
    void Update()
    {
        /*float nowOuterRingValue = ((float)ControllerController.outterRing_data - (float)ControllerController.initialoutterRing_data) / 1024;
        if (nowOuterRingValue > 0 && nowOuterRingValue >= maxOutterCL)
        {
            maxOutterCL = nowOuterRingValue;
        }
        OuterRingFill.SetFill(maxOutterCCL, maxOutterCL);*/
      

    }
}
