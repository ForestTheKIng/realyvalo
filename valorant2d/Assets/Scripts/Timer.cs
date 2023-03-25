using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System;


public class Timer : MonoBehaviour
{
    public TMP_Text timerText;
    const float maxTime = 180f;
    private float currentTime = maxTime;

    const float startGameTime = 20f;
    private float startCurrentTime = startGameTime;

    public bool gameStarted = false;
    public GameObject barriers;

    // Update is called once per frame


    void Update()
    {
        if (gameStarted == true){
            currentTime -= 1 * Time.deltaTime;
            int currentTimeInt = (int) Math.Round(currentTime);
            timerText.text = currentTimeInt.ToString();
            if (currentTime <= 0){
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        } else {
            startCurrentTime -= 1 * Time.deltaTime;
            int startTimeInt = (int) Math.Round(startCurrentTime);
            timerText.text = startTimeInt.ToString();
            if (startCurrentTime <= 0){
                gameStarted = true;
                barriers.SetActive(false);
            }
        }
    }
}
