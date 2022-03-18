using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using System;

public class Auido_MicReact : MonoBehaviour
{
    [SerializeField, Range(0f, 10f)] float m_gain = 1f; // 音量に掛ける倍率
    public float m_volumeRate; // 音量(0-1)
    public Vector2 BigBubleCreateRange;
    public GameObject BigBubleGO;
    public GameObject CreatedBuble;

    public VisualEffect vfx;
    public float micVolume = 50.0f;


    private bool keyIsBlock = false; //キー入力ブロックフラグ
    private DateTime pressedKeyTime; //前回キー入力された時間
    private TimeSpan elapsedTime; //キー入力されてからの経過時間

    private TimeSpan blockTime = new TimeSpan(0, 0, 5); //ブロックする時間　1s

    //true :big false many
    public bool Mode = false;


    // Use this for initialization
    void Start()
    {
        AudioSource aud = GetComponent<AudioSource>();
        if ((aud != null) && (Microphone.devices.Length > 0)) // オーディオソースとマイクがある
        {
            string devName = Microphone.devices[1]; // 複数見つかってもとりあえず0番目のマイクを使用
            int minFreq, maxFreq;
            Microphone.GetDeviceCaps(devName, out minFreq, out maxFreq); // 最大最小サンプリング数を得る
            aud.clip = Microphone.Start(devName, true, 2, minFreq); // 音の大きさを取るだけなので最小サンプリングで十分
            aud.Play(); //マイクをオーディオソースとして実行(Play)開始
        }
    }

    // Update is called once per frame
    void Update()
    {
        
        if (keyIsBlock)
        {
            elapsedTime = DateTime.Now - pressedKeyTime;
            if (elapsedTime > blockTime)
            {
                Destroy(CreatedBuble);
                CreatedBuble = null;
                keyIsBlock = false;

            }
            else
            {
                //return;
            }
        }

        if(Input.GetKey(KeyCode.Space))
        {
            Mode = !Mode;
        }

        //big
        if (Mode)
        {
            vfx.SetFloat("SpawnRate", 0);

            if (m_volumeRate < BigBubleCreateRange.x)
            {
                if (CreatedBuble != null)
                {
                    if (!CreatedBuble.GetComponent<Animator>().GetBool("MoveFlag"))
                    {
                        keyIsBlock = true;
                        pressedKeyTime = DateTime.Now;

                        //CreatedBuble.GetComponent<Animator>().SetTrigger("Tr_Move");
                        CreatedBuble.GetComponent<Animator>().SetBool("MoveFlag", true);
                        Debug.Log(m_volumeRate);
                    }
                }
            }
            else if (BigBubleCreateRange.x < m_volumeRate && m_volumeRate < BigBubleCreateRange.y)
            {
                
                if (CreatedBuble == null)
                {

                    CreatedBuble = Instantiate(BigBubleGO);
                    CreatedBuble.SetActive(true);
                    CreatedBuble.GetComponent<Animator>().SetBool("AirInFlag", true);
                }
            }
            else
            {
                if (CreatedBuble != null)
                {
                    if (!CreatedBuble.GetComponent<Animator>().GetBool("MoveFlag"))
                    {
                        keyIsBlock = true;
                        pressedKeyTime = DateTime.Now;

                        CreatedBuble.GetComponent<Animator>().SetBool("MoveFlag", true);
                        Debug.Log(m_volumeRate);
                    }
                }
            }
        }
        //large
        else
        {
            if (m_volumeRate < 0.5f)
                {
                    vfx.SetFloat("SpawnRate", 0);
            }
            else
            {

                vfx.SetFloat("SpawnRate", m_volumeRate);
            }
        }
    }

    // オーディオが読まれるたびに実行される
    private void OnAudioFilterRead(float[] data, int channels)
    {
        float sum = 0f;
        for (int i = 0; i < data.Length; ++i)
        {
            sum += Mathf.Abs(data[i]); // データ（波形）の絶対値を足す
        }
        // データ数で割ったものに倍率をかけて音量とする
        m_volumeRate = Mathf.Clamp01(sum * m_gain / (float)data.Length) * micVolume;

    }
}