using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{ 
    //覚える画面のGameObject
    public GameObject ImageBackRem;

    //覚えたボタン
    public GameObject RemButton;

    //次へボタン
    public GameObject NextButton;

    //テキスト
    public GameObject TextBox;

    //ImageのGameObject 0～15
    public GameObject[] Images = new GameObject[16];

    //覚える画面のImage
    public GameObject[] RemImages = new GameObject[16];

    //爆弾Image
    public GameObject[] BombImage = new GameObject[3];

    //旗Image
    public GameObject[] FlagImage = new GameObject[2];

    //星Image
    public GameObject StarImage;

    //爆弾の位置
    public int[] bomb = new int[3];

    //スタート＆ゴールの位置
    public int[] flag = new int[2];

    //星の位置
    public int star = -1;

    //ドラッグしているか
    public bool doDrag;

    //もう通ったruteか 0～15
    public bool[] didPass = new bool[16];

    //ルートの保存 1～16 (n+1)
    public int[] rute = new int[16];

    //何番目か
    public int nowrute;

    //星を取ったか
    public bool havestar;

    //時間制限
    public float TimeLimit = 30.0f;

    public static int Success = 0;

    public static int Fail = 0;

    public static int Star = 0;

    // Start is called before the first frame update
    void Start()
    {
        ResetTag();

        SymbolPlace();

        TextBox.SetActive(false);

        NextButton.SetActive(false);

        RemButton.SetActive(true);

        //覚えた画面
        ImageBackRem.SetActive(true);

        //ドラッグしていない
        doDrag = false;

        havestar = false;

        //リセット
        ResetRute();
    }

    // Update is called once per frame
    void Update()
    {
        TimeLimit -= Time.deltaTime;

        if(TimeLimit <= 0)
        {
            SceneManager.LoadScene("ResultScene");
        }
    }

    //覚えたボタン押下時
    public void PushRemButton()
    {
        ImageBackRem.SetActive(false);
        RemButton.SetActive(false);
        NextButton.SetActive(true);
        TextBox.SetActive(true);
    }

    //次へボタン押下時
    public void PushNextButton()
    {
        Start();
    }

    //Imageドラッグ終了時
    public void EndDragImage()
    {
        int i;
        for (i = 0; i < 16; i++)
        {
            if (didPass[i])
            {
                Images[i].GetComponent<Image>().color = new Color(0.75f, 0.75f, 0.75f,1);
            }
        }

        if(doDrag && Images[rute[nowrute - 1] - 1].tag == "start & goal")
        {
            Text Newtext = TextBox.GetComponent<Text>();
            if (havestar)
            {
                Newtext.text = "Great!!";
                Star++;
                Success++;
            }
            else
            {
                Newtext.text = "Good!";
                Success++;
            }
            ImageBackRem.SetActive(true);
        }

        doDrag = false;

        ResetRute();
    }

    //Imageドラッグ開始時
    public void BeginDragImage()
    {
        //マウスの座標を取得
        int placenumber = Determine(Input.mousePosition.x, Input.mousePosition.y);

        if (Images[placenumber].tag == "start & goal")
        {
            //マウスの座標下のImageの色を変える
            Images[placenumber].GetComponent<Image>().color = new Color(0.5f, 0.5f, 0.5f, 1);

            //ruteを保存
            rute[0] = placenumber + 1;

            //通った
            didPass[placenumber] = true;

            //ドラッグしている
            doDrag = true;
        }
    }

    //マウスがImageに乗ったとき
    public void PointerEnterImage()
    {
        //マウスの座標を取得
        int placenumber = Determine(Input.mousePosition.x, Input.mousePosition.y);

        //ドラッグしているなら
        if (doDrag && Cango(rute[nowrute - 1] - 1,placenumber) && didPass[placenumber] == false)
        {
            //マウスの座標下のImageの色を変える
            Images[placenumber].GetComponent<Image>().color = new Color(0.5f, 0.5f, 0.5f, 1);

            if(Images[placenumber].tag == "bomb")
            {
                Text Newtext = TextBox.GetComponent<Text>();
                Newtext.text = "Miss";
                Fail++;
                ImageBackRem.SetActive(true);
            }

            if(Images[placenumber].tag == "star")
            {
                havestar = true;
            }

            //通った
            didPass[placenumber] = true;

            //ruteを保存
            rute[nowrute] = placenumber + 1;

            //次へ
            nowrute++;
        }
    }

    //行と列からImageを取得
    public int Determine(float X, float Y)
    {
        int i, j;
        int k = 0;
        int x, y;

        x = Row(X);
        y = Col(Y);

        for(j = 1; j < 5; j++)
        {
            for(i = 1; i < 5; i++)
            {
                if (x == i && y == j)
                {
                    return k;
                }
                k++;
            }
        }
        return -1;
    }
    
    //横列座標
    public int Row(float x)
    {
        if(20 < x && x <= 210)
        {
            return 1;
        }
        if (210 < x && x <= 400)
        {
            return 2;
        }
        if (400 < x && x <= 590)
        {
            return 3;
        }
        if (590 < x && x <= 780)
        {
            return 4;
        }
        return 0;
    }

    //縦列座標
    public int Col(float y)
    {
        if (830 < y && y <= 1020)
        {
            return 1;
        }
        if (640 < y && y <= 830)
        {
            return 2;
        }
        if (450 < y && y <= 640)
        {
            return 3;
        }
        if (260 < y && y <= 450)
        {
            return 4;
        }
        return 0;
    }

    //データをリセット
    public void ResetRute()
    {
        int i;
        for(i = 0;i < 16;i++)
        {
            rute[i] = 0;
            didPass[i] = false;
        }
        nowrute = 1;
    }

    //進めるImageを判定
    public bool Cango(int placenumber,int beforeplacenumber)
    {
        int cango = beforeplacenumber - placenumber;

        switch (cango)
        {
            case -4:
            case -1:
            case 1:
            case 4:
                return true;
            default:
                return false;
        }
    }

    public void SymbolPlace()
    {
        int i, k;
        int[] provisional = new int[16];
        int x;

        for (i = 0; i < 16; i++)
        {
            provisional[i] = i;
        }

        for (i = 0; i < 6; i++)
        {
            x = Random.Range(0, 16 - i);

            if (i < 3)
            {
                bomb[i] = provisional[x];
                Images[bomb[i]].tag = "bomb";
                BombImage[i].transform.position = RemImages[bomb[i]].transform.position;
                //Destroy(RemImages[bomb[i]]);
            }
            if (i == 3 || i == 4)
            {
                flag[i - 3] = provisional[x];
                Images[flag[i - 3]].tag = "start & goal";
                FlagImage[i - 3].transform.position = Images[flag[i - 3]].transform.position;
            }
            if (i == 5)
            {
                star = provisional[x];
                Images[star].tag = "star";
                StarImage.transform.position = Images[star].transform.position;
            }

            for (k = x; k < 15; k++)
            {
                provisional[k] = provisional[k + 1];
            }
        }
    }

    //Imageのtagをリセット
    public void ResetTag()
    {
        int i;
        for(i = 0;i<16;i++)
        {
            Images[i].tag = "Untagged";
        }
    }

    public static int GetSuccess()
    {
        int k = Success;
        Success = 0;
        return k;
    }

    public static int GetFail()
    {
        int k = Fail;
        Fail = 0;
        return k;
    }

    public static int GetStar()
    {
        int k = Star;
        Star = 0;
        return k;
    }
}
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                              