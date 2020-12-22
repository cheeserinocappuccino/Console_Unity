using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;
public class GameController_stepONE : MonoBehaviour
{
    public GameObject OuterRing, InnerRing;

    DoubleSides_FillAmount OuterRingFill, InnerRingFill;

    ControllerController controllerController;

    // 記錄轉過的最大值
    float maxInnerCL, maxInnerCCL = 0;
    float maxOutterCL, maxOutterCCL = 0;

    // -----流程控制-----
    // 進到選歌
    public static bool GetInSelectsong; // 轉完內外圈後進到選歌的控制
    public GameObject Aerobat_frame;
    public GameObject chooseSong_background;
    GameObject[] FadeOutWhileGetInSelectsong_group;
    float opacity_of_this_object_in_fadeout_group = 1.0f;
    public float fadeoutSpeed = 1.0f;
    public float aerobat_inflation_speed = 0.5f;
    float inflatx = 1;
    float inflaty = 1;

    // 正在選歌
    
    public bool isSelectingSong = false;
    public bool canFloatOutSelectingSongUis = false;
    public static GameObject planetRotationGroup;
    GameObject[] FadeInWhileGetInSelectsong_group;
    float opacity_of_this_object_in_fadeINNNN_group = 0.0f;
    public float fadeINNNSpeed = 0.5f;
    float now_rotationg_group_targetAngle = 0;
    float planetRotationGroup_angle_lerping = 0;
    public int nowsongIndex = 0;
    public string[] allSongNames;
    public int[] allSongDifficulty; // 像這種名子跟難度要寫成一個class比較好 有時間的話，之後看懂之前寫的東西的話應該可以複製過來
    public Sprite[] difficultySprite; // [0] = 1, [1] = 2, [2] = 3 星的圖片
    public Image showDifficultyImageObject;
    public Image showAnimationImageObject;
    public string selected_songName;
    bool RotatingActions = false;
    bool starsAnimationIsDone = true;
    public Sprite[] eachStarsAnimationSprite;
    float starChangeFrameFlag = 0;
    bool starAnimationDirection = true;
    public float starChangeFramesPerSecond = 12; //Frame per seconds
    float starChangeCalculatedSpeed = 0;
    int starsNowframes;
    // Song info

    public Image songInfoImageComponent;
    public Sprite[] songInfos;

    // preview song
    public AudioSource previewAudioSource;
    public AudioClip[] previewClips;

    // 指令控制
    bool commandSended_ShowSelectSongText = false;

    // 接收按鈕
    static bool innerRingPressed = false;
    static bool OuterRingPressed = false;

    // 接收轉盤Tick
    static bool leftTick = false;
    static bool rightTick = false;
    float angleAsATick = 45.0f;
    float Out_lastTickAngle = 0;
    float In_lastTickAngle = 0;

    // 計算轉過的路徑
    int Out_maxCR, Out_maxCCR = 0;
    int IN_maxCR, IN_maxCCR = 0;
    

    // 接收確認選歌的訊息
    static bool TBChooseSongOk = false;

    void Start()
    {
        OuterRingFill = OuterRing.GetComponent<DoubleSides_FillAmount>();
        InnerRingFill = InnerRing.GetComponent<DoubleSides_FillAmount>();

        controllerController = GameObject.FindGameObjectWithTag("controllerController").GetComponent<ControllerController>();
        planetRotationGroup = GameObject.FindGameObjectWithTag("planetRotationGroup");
        FadeOutWhileGetInSelectsong_group = GameObject.FindGameObjectsWithTag("FadeOutWhileGetInSelectsong");
        FadeInWhileGetInSelectsong_group = GameObject.FindGameObjectsWithTag("FadeInWhileGetInSelectsong");

        starChangeCalculatedSpeed = 1 / starChangeFramesPerSecond;
        InnerRingFill.SetFill(0.6f, 0.0f);
    }


    void Update()
    {
        if(GetInSelectsong == false)
        {

        }

        // 進選歌流程會撥的動畫。撥到inflatx >= 3.0f為止
        if (GetInSelectsong)
        {
            opacity_of_this_object_in_fadeout_group -= Time.deltaTime * fadeoutSpeed;


            
            inflatx += Time.deltaTime * aerobat_inflation_speed;
            inflaty += Time.deltaTime * aerobat_inflation_speed;


            Aerobat_frame.GetComponent<Image>().rectTransform.localScale = new Vector3(inflatx, inflaty, 1);
            chooseSong_background.GetComponent<Image>().color = new Color(255, 255, 255, inflatx - 1.0f);
            for (int i = 0; i < FadeOutWhileGetInSelectsong_group.Length; i++)
            {

                FadeOutWhileGetInSelectsong_group[i].GetComponent<Image>().color = new Color(255, 255, 255, opacity_of_this_object_in_fadeout_group);
            }

            // 單純硬關掉不要讓放大跟減opacity一直跑/
            // 然後立一個flag說可以開始選歌了
            if(inflatx > 1.2f)
            {
                canFloatOutSelectingSongUis = true;
            }
            if (inflatx >= 1.3f)
            {
                GetInSelectsong = false;
                isSelectingSong = true;

                previewAudioSource.Play();
            }
        }

        if (canFloatOutSelectingSongUis)
        {
            // ----- 正在選歌的操作 -----
            // 轉盤數值轉換成Tick
            
            float outerValue = ControllerController.OutterRing_CalData / (float)41.0f;
            // Quaternion outerEuler = Quaternion.Euler(0, 0, outerValue);
            //Quaternion last_outerEuler = Quaternion.Euler(0, 0, Out_lastTickAngle);

            float Out_diffAngle = outerValue - Out_lastTickAngle;



            if (outerValue + In_lastTickAngle > 360 + angleAsATick + 1)
            {
                Out_diffAngle = -Out_diffAngle;
            }

            //Debug.Log(Out_lastTickAngle - outerValue + "__DiffAngle");

            if (Mathf.Abs(Out_diffAngle) > angleAsATick)
            {
                

                if(Out_diffAngle > 0)
                {
                    leftTick = true;
                    Debug.Log("逆時針");
                    Out_lastTickAngle = outerValue;
                }
                else
                {
                    rightTick = true;
                    Debug.Log("順時針");
                    Out_lastTickAngle = outerValue;


                }
                // 讓上次的角度變成這次的角度
                
            }


            // 傳指令給觸控板顯示SelectSong字樣
            if (!commandSended_ShowSelectSongText)
            {
                ControllerController.TBSend_ShowSelectSongText();
                commandSended_ShowSelectSongText = true;
            }

            // 先把東西都浮出來
            if (opacity_of_this_object_in_fadeINNNN_group <= 1.0f)
            {
                opacity_of_this_object_in_fadeINNNN_group += Time.deltaTime * fadeINNNSpeed;

                for (int i = 0; i < FadeInWhileGetInSelectsong_group.Length; i++)
                {

                    FadeInWhileGetInSelectsong_group[i].GetComponent<Image>().color = new Color(
                        FadeInWhileGetInSelectsong_group[i].GetComponent<Image>().color.r
                        , FadeInWhileGetInSelectsong_group[i].GetComponent<Image>().color.g
                        , FadeInWhileGetInSelectsong_group[i].GetComponent<Image>().color.b
                        , opacity_of_this_object_in_fadeINNNN_group);
                }
            }


            // 如果往左Tick一次
            if (leftTick || Input.GetKeyDown(KeyCode.RightArrow))
            {
                SelectingSong_LeftTickEvent();
                leftTick = false;
            }
            // 往右Tick一次
            else if (rightTick || Input.GetKeyDown(KeyCode.LeftArrow))
            {
                SelectingSong_RightTickEvent();
                rightTick = false;
            }
            
            Rotating_planet(false, now_rotationg_group_targetAngle);
            StarsAnimation();

            // 確認選歌之後，傳指令給觸控板顯示滑動，在等觸控板傳ok回來
            if (innerRingPressed )
            {
                canFloatOutSelectingSongUis = false;
                // 請觸控板顯示滑動
                ControllerController.TBSend_EnterChooseSongSlide();

                Debug.Log("收到內圈按鈕");


            }

            
        }

        // ControllerControllerc會呼叫這裡的threadHandle讓TBChooseSongOk = true;
        if (TBChooseSongOk)
        {
            // 讓觸控板回去背景圖
            //ControllerController.TBSend_GoBackGround();


            Debug.Log("馬上進到遊戲");
            SceneManager.LoadScene("Game_Step2");
            TBChooseSongOk = false;
        }

        // 每一幀復位pressed變數，模擬input.getkeydown
        innerRingPressed = false;
        OuterRingPressed = false;
        rightTick = false;
        leftTick = false;
    }

    void SelectingSong_LeftTickEvent()
    {
        // 所以 allSongNames[]在邏輯上有重要位置，基本上是看它的長度來限制選歌的
        if (nowsongIndex < allSongNames.Length - 1 && !RotatingActions)
        {
            nowsongIndex += 1;
            selected_songName = allSongNames[nowsongIndex];
            now_rotationg_group_targetAngle = nowsongIndex * 60;

            // 歌曲資訊
            songInfoImageComponent.sprite = songInfos[nowsongIndex];
            previewAudioSource.Stop();
            previewAudioSource.clip = previewClips[nowsongIndex];
            previewAudioSource.Play();
        }


    }

    void SelectingSong_RightTickEvent()
    {
        // 所以 allSongNames[]在邏輯上有重要位置，基本上是看它的長度來限制選歌的
        if (nowsongIndex > 0 && !RotatingActions)
        {
            nowsongIndex -= 1;
            selected_songName = allSongNames[nowsongIndex];
            now_rotationg_group_targetAngle = nowsongIndex * 60;

            // 歌曲資訊
            songInfoImageComponent.sprite = songInfos[nowsongIndex];
            previewAudioSource.Stop();
            previewAudioSource.clip = previewClips[nowsongIndex];
            previewAudioSource.Play();
        }


    }

    void StarsAnimation()
    {
        showAnimationImageObject.color = new Color(255, 255, 255, 0.5f);
        if (starsAnimationIsDone)
        {
            showDifficultyImageObject.sprite = difficultySprite[nowsongIndex];
        }

        starChangeFrameFlag -= Time.deltaTime;
        // 如果轉得差不多了才能立flag告訴大家可以繼續轉
        if (starChangeFrameFlag <= 0 && RotatingActions)
        {
            if(starsNowframes < eachStarsAnimationSprite.Length -1 && starAnimationDirection == true)
            {
                starsAnimationIsDone = false;
                starsNowframes++;
                RotatingActions = true;
                showDifficultyImageObject.color = new Color(255, 255, 255, 0);
            }
            else if(starsNowframes == eachStarsAnimationSprite.Length - 1 && starAnimationDirection == true)
            {
                starsNowframes--;
                starAnimationDirection = false;
            }
            else if(starsNowframes > 0 && starAnimationDirection == false)
            {
                starsNowframes--;
                

                
            }else if(starsNowframes == 0 && starAnimationDirection == false)
            {
                RotatingActions = false;
                showDifficultyImageObject.color = new Color(255, 0, 0, 1);
                starAnimationDirection = true;
                starsAnimationIsDone = true;

            }
            showAnimationImageObject.sprite = eachStarsAnimationSprite[starsNowframes];
            
        }

    }

    void Rotating_planet(bool _left_right, float _target_angle_z)
    {

        
        planetRotationGroup_angle_lerping = Mathf.Lerp(planetRotationGroup_angle_lerping, _target_angle_z, Time.deltaTime * 4.0f);

        planetRotationGroup.GetComponent<RectTransform>().rotation = Quaternion.Euler(0, 0, planetRotationGroup_angle_lerping);

        // 如果轉得差不多了才能立flag告訴大家可以繼續轉
        if (Mathf.Abs(planetRotationGroup_angle_lerping - _target_angle_z) <= 10.0f)
        {
           // RotatingActions = false;
            
        }
        else
        {
            RotatingActions = true;
           
        }

    }

    // 因為接收指令的執行續跟unity不一樣，會有時差，所以讓接收指令那邊呼叫unity執行續的東西，讓innerRingPressed加入unity執行續就能做到類似input.getkeyDOWN只開一幀的效果
    public static void InnerRingPressed_ThreadHandle()
    {
        innerRingPressed = true;
        
    }
    public static void OutterRingPressed_ThreadHandle()
    {
        OuterRingPressed = true;
        
    }
    public static void CanGoInGame_ThreadHandle()
    {
        TBChooseSongOk = true;

    }

    public static void GetInSelectingSong()
    {
        GetInSelectsong = true;
    }
}
