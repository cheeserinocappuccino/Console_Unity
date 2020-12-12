using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class DoubleSides_FillAmount : MonoBehaviour
{
    Image theCircle;


    void Awake()
    {
        theCircle = this.GetComponent<Image>();

    }



    public void SetFill(float _CL_Amount, float _CCL_Amount)
    {
        float CCL_rotate_amount = 360 * _CCL_Amount;

        this.gameObject.transform.rotation = Quaternion.Euler(0, 0, CCL_rotate_amount);

        theCircle.fillAmount = _CL_Amount + _CCL_Amount;
    }

    
}
