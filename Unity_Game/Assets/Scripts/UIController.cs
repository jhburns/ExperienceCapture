﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using Capture;

public class UIController : MonoBehaviour, ICapturable
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
        countdown = Random.Range(2f, 8.0f);
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

    public object getCapture()
    {
        return new
        {
            againButtonIsActive = again.IsActive(),
            doneButtonIsActive = done.IsActive(),
            promptTextIsActive = prompt.IsActive(),
            getReadyTextIsActive = getReady.IsActive(),
            timeTextIsActive = timeDisplay.IsActive(),

            countdown = countdown,
            responceTime = responceTime,
            isWaiting = waiting,
            isResponding = responding
        };
    }
}