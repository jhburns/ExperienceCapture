using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using Capture;

public class UIController : MonoBehaviour, ICapturable
{
    public Button again;
    public Button done;
    public Text getReady;
    public Text timeDisplay;
    public Image spinner;
    public string initialTimeDisplay;

    public Image[] prompts;
    private string[] keyMappings;
    private int promptIndex;

    private float countdown;
    private float responceTime;

    private bool waiting;
    private bool responding;

    void Start()
    {
        initMapping();
        SetupRound();
    }

    private void initMapping()
    {
        keyMappings = new string[]
        {
            "space",
            "b",
            "n",
            "v"
        };
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

                prompts[promptIndex].gameObject.SetActive(true);
                getReady.gameObject.SetActive(false);
                spinner.gameObject.SetActive(false);
            }
        }
        else if (responding)
        {
            responceTime += Time.deltaTime;

            if (Input.GetKeyDown(keyMappings[promptIndex]))
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
        countdown = Random.Range(2f, 7f);
        responceTime = 0f;
        waiting = true;
        responding = false;

        UpdateText(0f);

        again.gameObject.SetActive(false);
        done.gameObject.SetActive(false);
        getReady.gameObject.SetActive(true);
        timeDisplay.gameObject.SetActive(false);
        spinner.gameObject.SetActive(true);

        foreach (Image p in prompts)
        {
            p.gameObject.SetActive(false);
        }

        promptIndex = Random.Range(0, prompts.Length);
    }

    public void NextScene()
    {
        SceneManager.LoadScene("Cleanup");
    }

    public object getCapture()
    {
        return new
        {
            againButtonIsActive = again.IsActive(),
            doneButtonIsActive = done.IsActive(),
            promptIndex = promptIndex,

            // Wanted to see if you can do this, not that you should do it
            promptIsActive = (prompts[0].IsActive() 
                              || prompts[1].IsActive() 
                              || prompts[2].IsActive() 
                              || prompts[3].IsActive()),

            getReadyTextIsActive = getReady.IsActive(),
            timeTextIsActive = timeDisplay.IsActive(),

            countdown = countdown,
            responceTime = responceTime,
            isWaiting = waiting,
            isResponding = responding
        };
    }
}