using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class ResultManage : MonoBehaviour
{
    /*public int perfectNum;
    public int goodNum;
    public int missNum;*/

    private int[] nums = new int[4];
    public Text perfectText;
    public Text goodText;
    public Text missText;
    public Text ComboText;
    //public Slider ultimateSlider;
    //public GameObject bossImage;
    //public GameObject bossRotateDummyTarget;
    //public GameObject StuffGotoDummyTarget;
    //public GameObject resultGotoDummyTarget;
    //public Image winLoseImage;
    //public Sprite loseSprite;
    NumParseGuy parseGuy;

    public AnimationCurve addingSpeedCurve;

    //public GameObject firstPage;
    public bool doneCounting = false;
    public bool startUltimateCount = false;
    public bool showBoss = false;
    public bool showWinLose = false;
    //public float firstPageExitposX = -2000;

    Image fadingImage;
    public NumParseGuy numParseGuy;
    void Start()
    {

        parseGuy = GameObject.FindGameObjectWithTag("parseGuy").GetComponent<NumParseGuy>();

        StartCoroutine(ICountTimer());

        try
        {
            fadingImage = GameObject.FindGameObjectWithTag("Fade").GetComponent<Image>();
            // Fade效果
            fadingImage.gameObject.GetComponent<Animator>().SetTrigger("FadeIn");
        }
        catch
        {
            Debug.Log("沒有fadingimage");
        }
        StartCoroutine(SetFade());
    }


    void Update()
    {


        //float eachPerfectNoteTimePos = 1.0f / parseGuy.perfectNum;
        ///nowPerfectNoteTimePos = eachPerfectNoteTimePos * (nums[0]);



        perfectText.text = nums[0].ToString();
        goodText.text = nums[1].ToString();
        missText.text = nums[2].ToString();
        ComboText.text = nums[3].ToString();
        if (doneCounting)
        {
            
        }

        if (startUltimateCount)
        {
            
        }

        if (showBoss)
        {
            
        }

        if (showWinLose)
        {
           
        }
    }

    IEnumerator ICountTimer()
    {
        yield return new WaitForSeconds(3.0f);
        // perfect
        while (nums[0] < parseGuy.perfectNum)
        {

            float nowTimePos = (1.0f / parseGuy.perfectNum) * (nums[0]);

            nums[0] += 1;
            yield return new WaitForSeconds(0.5f - addingSpeedCurve.Evaluate(nowTimePos));
        }
        Debug.Log("add perfect done");
        yield return new WaitForSeconds(1.0f);

        // good
        while (nums[1] < parseGuy.goodNum)
        {
            float nowTimePos = (1.0f / parseGuy.goodNum) * (nums[1]);

            nums[1] += 1;
            yield return new WaitForSeconds(0.5f - addingSpeedCurve.Evaluate(nowTimePos));
        }
        Debug.Log("add good done");
        yield return new WaitForSeconds(1.0f);

        // miss
        while (nums[2] < parseGuy.missNum)
        {
            float nowTimePos = (1.0f / parseGuy.missNum) * (nums[2]);

            nums[2] += 1;
            yield return new WaitForSeconds(0.5f - addingSpeedCurve.Evaluate(nowTimePos));
        }
        Debug.Log("add miss done");

        // combo
        while (nums[3] < parseGuy.combo)
        {
            float nowTimePos = (1.0f / parseGuy.missNum) * (nums[3]);

            nums[3] += 1;
            yield return new WaitForSeconds(0.5f - addingSpeedCurve.Evaluate(nowTimePos));
        }
        Debug.Log("add combo done");

        yield return new WaitForSeconds(3.0f);
        doneCounting = true;

        Debug.Log("dont all Counting");

        yield return new WaitForSeconds(1.0f);
        startUltimateCount = true;
        yield return new WaitForSeconds(1.0f);
        showBoss = true;
        yield return new WaitForSeconds(1.0f);
        showWinLose = true;
        yield return new WaitForSeconds(1.0f);
        try
        {
            fadingImage.gameObject.GetComponent<Animator>().SetTrigger("FadeOut");
        }
        catch
        {
            Debug.Log("沒有fade圖");
        }
        yield return new WaitForSeconds(1.5f);

        SceneManager.LoadScene("Entry_Step1");

        //Destroy(fadingImage.gameObject);

        StopAllCoroutines();
    }

    IEnumerator SetFade()
    {
        yield return new WaitForSeconds(0.2f);
        try
        {
            fadingImage = GameObject.FindGameObjectWithTag("Fade").GetComponent<Image>();
            // Fade效果
            fadingImage.gameObject.GetComponent<Animator>().SetTrigger("FadeIn");

        }
        catch
        {
            Debug.Log("沒有fadingimage");
        }
    }



}
