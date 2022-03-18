using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using System;

public class Auido_MicReact : MonoBehaviour
{
    [SerializeField, Range(0f, 10f)] float m_gain = 1f; // ���ʂɊ|����{��
    public float m_volumeRate; // ����(0-1)
    public Vector2 BigBubleCreateRange;
    public GameObject BigBubleGO;
    public GameObject CreatedBuble;

    public VisualEffect vfx;
    public float micVolume = 50.0f;


    private bool keyIsBlock = false; //�L�[���̓u���b�N�t���O
    private DateTime pressedKeyTime; //�O��L�[���͂��ꂽ����
    private TimeSpan elapsedTime; //�L�[���͂���Ă���̌o�ߎ���

    private TimeSpan blockTime = new TimeSpan(0, 0, 5); //�u���b�N���鎞�ԁ@1s

    //true :big false many
    public bool Mode = false;


    // Use this for initialization
    void Start()
    {
        AudioSource aud = GetComponent<AudioSource>();
        if ((aud != null) && (Microphone.devices.Length > 0)) // �I�[�f�B�I�\�[�X�ƃ}�C�N������
        {
            string devName = Microphone.devices[1]; // �����������Ă��Ƃ肠����0�Ԗڂ̃}�C�N���g�p
            int minFreq, maxFreq;
            Microphone.GetDeviceCaps(devName, out minFreq, out maxFreq); // �ő�ŏ��T���v�����O���𓾂�
            aud.clip = Microphone.Start(devName, true, 2, minFreq); // ���̑傫������邾���Ȃ̂ōŏ��T���v�����O�ŏ\��
            aud.Play(); //�}�C�N���I�[�f�B�I�\�[�X�Ƃ��Ď��s(Play)�J�n
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

    // �I�[�f�B�I���ǂ܂�邽�тɎ��s�����
    private void OnAudioFilterRead(float[] data, int channels)
    {
        float sum = 0f;
        for (int i = 0; i < data.Length; ++i)
        {
            sum += Mathf.Abs(data[i]); // �f�[�^�i�g�`�j�̐�Βl�𑫂�
        }
        // �f�[�^���Ŋ��������̂ɔ{���������ĉ��ʂƂ���
        m_volumeRate = Mathf.Clamp01(sum * m_gain / (float)data.Length) * micVolume;

    }
}