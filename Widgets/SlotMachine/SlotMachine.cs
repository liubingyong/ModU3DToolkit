using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using System.Linq;
using PathologicalGames;
using DG.Tweening;
using UnityEngine.UI;

public class SlotMachine : MonoBehaviourEx
{
    public float delayAfterSpin = 0f;

    public bool isDespined = false;
    public bool isAutoSpin = false;

    public Reel[] reels;

    public AudioClip start;
    public AudioClip loop;
    public AudioClip end;

    [HideInInspector]
    public bool isRunning = false;

    private AudioSource audioSource;

    private float spinTime;
    private int finishedReelCount;

    private Coroutine autoSpinCoroutine;

    [SerializeField]
    private UnityEvent _onFinished = new UnityEvent();

    public UnityEvent OnFinished
    {
        get { return _onFinished; }
        set { _onFinished = value; }
    }

    void Start()
    {
        audioSource = this.GetComponent<AudioSource>();

        for (int i = 0; i < reels.Length; i++)
        {
            reels[i].OnFinished.AddListener(OnReelFinished);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            Spin();
        }

        if (Input.GetKeyDown(KeyCode.F2))
        {
            DespinAll();
        }
    }

    public void Spin()
    {
        if (!isRunning)
        {
            spinTime = Time.time;
            isDespined = false;

            isRunning = true;
            finishedReelCount = 0;

            if (audioSource != null)
            {
                audioSource.clip = start;
                audioSource.loop = false;
                audioSource.Play();

                Invoke(() =>
                {
                    audioSource.clip = loop;
                    audioSource.loop = true;
                    audioSource.Play();
                }, start.length);
            }

            StartCoroutine(SpinCore());
        }
        else
        {
            Debug.Log("SlotMachine is runnning.");
        }
    }

    public void DespinAll()
    {
        if (!isDespined)
        {
            StartCoroutine(DespinAllCore());
            isDespined = true;
        }
    }

    public void Reset()
    {
        for (int i = 0; i < reels.Length; i++)
        {
            reels[i].Reset();
        }
    }

    private void Despin(int reelIndex, bool reminderwhenCall)
    {
        if (isRunning)
        {
            StartCoroutine(DespinCore(reels[reelIndex]));
        }
    }

    private IEnumerator DespinCore(Reel reel)
    {
        reel.Despin();

        yield return new WaitForSeconds(0.5f);

        if (audioSource != null)
        {
            audioSource.clip = end;
            audioSource.loop = false;
            audioSource.Play();
        }
    }

    private IEnumerator DespinAllCore()
    {
        yield return new WaitForEndOfFrame();

        for (int i = 0; i < reels.Length; i++)
        {
            Despin(i, isDespined);
            yield return new WaitForSeconds(0.5f);
        }
    }

    private IEnumerator SpinCore()
    {
        yield return new WaitForEndOfFrame();

        if (delayAfterSpin > 0)
        {
            foreach (var reel in reels)
            {
                reel.Spin();
                yield return delayAfterSpin > 0 ? new WaitForSeconds(delayAfterSpin) : null;
            }
        }
        else
        {
            foreach (var reel in reels)
            {
                reel.Spin();
            }
        }

        yield return new WaitForSeconds(3f);

        if (!isDespined)
        {
            DespinAll();
        }
    }

    private void OnReelFinished()
    {
        StartCoroutine(OnReelFinishedCore());
    }

    private IEnumerator OnReelFinishedCore()
    {
        yield return null;

        finishedReelCount++;

        if (finishedReelCount == reels.Length)
        {
            StopCoroutine(SpinCore());

            isRunning = false;
            OnFinished.Invoke();

            if (isAutoSpin)
            {
                autoSpinCoroutine = StartCoroutine(AutoSpinCore());
            }
        }
    }

    private IEnumerator AutoSpinCore()
    {
        yield return new WaitForSeconds(1f);
        Spin();

        yield return new WaitForSeconds(Random.Range(3f, 4f));
        DespinAll();
    }
}
