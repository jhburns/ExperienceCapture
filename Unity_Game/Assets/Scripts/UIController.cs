using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using Saver;

[System.Serializable]
public class UIController : MonoBehaviour, ISaveable
{
    public Button again;
    public Button done;
    public Text prompt;
    public Text getReady;
    public Text timeDisplay;
    public string initialTimeDisplay;

    private float countdown;
    private float responceTime;

    private bool waiting;
    private bool responding;

    void Start()
    {
        SetupRound();
    }

    void Update()
    {
        if (waiting)
        {
            countdown -= Time.deltaTime;
            
            if (countdown < 0f)
            {
                waiting = false;
                responding = true;

                prompt.gameObject.SetActive(true);
                getReady.gameObject.SetActive(false);
            }
        }
        else if (responding)
        {
            responceTime += Time.deltaTime;

            if (Input.GetKeyDown("space"))
            {
                responding = false;

                UpdateText(responceTime);

                again.gameObject.SetActive(true);
                done.gameObject.SetActive(true);
                timeDisplay.gameObject.SetActive(true);

            }
        }
    }

    private void UpdateText(float newTime)
    {
        timeDisplay.text = initialTimeDisplay + newTime + " sec";
    }

    public void SetupRound()
    {
        countdown = Random.Range(0f, 10.0f);
        responceTime = 0f;
        waiting = true;
        responding = false;

        UpdateText(0f);

        again.gameObject.SetActive(false);
        done.gameObject.SetActive(false);
        prompt.gameObject.SetActive(false);
        getReady.gameObject.SetActive(true);
        timeDisplay.gameObject.SetActive(false);
    }

    public void NextScene()
    {
        SceneManager.LoadScene("Cleanup");
    }

    public ICustomStorable getSave()
    {
        return new SaveUI(
                            again.IsActive(),
                            done.IsActive(),
                            prompt.IsActive(),
                            getReady.IsActive(),
                            timeDisplay.IsActive(),
                            countdown,
                            responceTime,
                            waiting,
                            responding
                         );
    }
}

[System.Serializable]
public class SaveUI: ICustomStorable
{
    public bool isAgainActive { get; private set; }
    public bool isDoneActive { get; private set; }
    public bool isPrompting { get; private set; }
    public bool isReady { get; private set; }
    public bool timeDisplayText { get; private set; }

    public float countdown { get; private set; }
    public float responceTime { get; private set; }

    public bool waiting { get; private set; }
    public bool responding { get; private set; }

    public SaveUI(
                    bool again,
                    bool done,
                    bool prompt,
                    bool ready,
                    bool timeDis,
                    float down,
                    float responce,
                    bool wait,
                    bool resp
                 )
    {
        isAgainActive = again;
        isDoneActive = done;
        isPrompting = prompt;
        isReady = ready;
        timeDisplayText = timeDis;

        countdown = down;
        responceTime = responce;

        waiting = wait;
        responding = resp;
    }
}