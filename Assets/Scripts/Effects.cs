using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class Effects : MonoBehaviour
{
    Text crosshair;
    Image bloodScreen;
    [SerializeField] float fadeSpeedShot;
    [SerializeField] float fadeSpeedGameOver;
    bool shotEffect;
    bool gameOver;
    Text gameOverText;
    Button titleButton;
    Text titleButtonText;
    Button retryButton;
    Text retryButtonText;

    void Start()
    {
        gameOver = false;
        shotEffect = false;

        GameEvents.PlayerShot += OnPlayerShot;
        GameEvents.GameOver += OnGameOver;

        crosshair = transform.GetChild(0).GetComponent<Text>();
        bloodScreen = transform.GetChild(1).GetComponent<Image>();
        gameOverText = transform.GetChild(2).GetComponent<Text>();
        titleButton = transform.GetChild(3).GetComponent<Button>();
        titleButtonText = titleButton.transform.GetChild(0).GetComponent<Text>();
        retryButton = transform.GetChild(4).GetComponent<Button>();
        retryButtonText = retryButton.transform.GetChild(0).GetComponent<Text>();

        SetGameOverUI(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (bloodScreen.color.a >= .6)
        {
            shotEffect = false;
        }

        if (bloodScreen.color.a != 0 & !shotEffect && !gameOver)
        {
            BloodScreenFade(fadeSpeedShot);
        }

        if (shotEffect &&  !gameOver)
        {
            BloodScreenUp(fadeSpeedShot);
        }

        if (gameOver) 
        {
            BloodScreenUp(fadeSpeedGameOver);
        }
    }

    void SetGameOverUI(bool setting)
    {
        crosshair.enabled = !setting;
        gameOver = setting;
        gameOverText.enabled = setting;
        SetButton(titleButton, titleButtonText, setting);
        SetButton(retryButton, retryButtonText, setting);
    }

    void SetButton(Button button, Text text,  bool setting) 
    {
        button.image.enabled = setting;
        button.enabled = setting;
        text.enabled = setting;
    }

    void OnGameOver(object sender, BoolEventArgs args)
    {
        bool won = args.boolPayload;

        if (won) 
        {
            gameOverText.text = "I'M GIVING YOU A RAISE!";
            Color dummyColor = Color.blue;
            dummyColor.a = bloodScreen.color.a;
            bloodScreen.color = dummyColor;
        }

        else
            gameOverText.text = "THE BASE IS TOAST";

        SetGameOverUI(true);

        GameEvents.PlayerShot -= OnPlayerShot;
        GameEvents.GameOver -= OnGameOver;
    }

    void BloodScreenFade(float speed)
    {
        Color newScreenColor = bloodScreen.color;
        newScreenColor.a = Mathf.Lerp(newScreenColor.a, 0, speed);
        bloodScreen.color = newScreenColor;
    }

    void BloodScreenUp(float speed)
    {
        Color newScreenColor = bloodScreen.color;
        newScreenColor.a = Mathf.Lerp(newScreenColor.a, 1, speed);
        bloodScreen.color = newScreenColor;
    }

    void OnPlayerShot(object sender, EventArgs args)
    {
        shotEffect = true;
    }

    public void TitleScreenButtonPressed()
    {
        SceneManager.LoadScene("StartScreen");
    }

    public void RetryButtonPressed() 
    {
        SceneManager.LoadScene("Level1");
    }
}
