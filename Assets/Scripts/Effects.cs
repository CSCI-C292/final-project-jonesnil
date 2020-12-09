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
    [SerializeField] float fadeSpeed;
    bool shotEffect;
    bool gameOver;
    Text gameOverText;
    Button titleButton;
    Text titleButtonText;

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
            BloodScreenFade();
        }

        if (shotEffect || gameOver)
        {
            BloodScreenUp();
        }
    }

    void SetGameOverUI(bool setting)
    {
        crosshair.enabled = !setting;
        gameOver = setting;
        gameOverText.enabled = setting;
        titleButton.image.enabled = setting;
        titleButton.enabled = setting;
        titleButton.enabled = setting;
        titleButtonText.enabled = setting;
    }

    void OnGameOver(object sender, EventArgs args)
    {
        SetGameOverUI(true);


        GameEvents.PlayerShot -= OnPlayerShot;
        GameEvents.GameOver -= OnGameOver;
    }

    void BloodScreenFade()
    {
        Color newScreenColor = bloodScreen.color;
        newScreenColor.a = Mathf.Lerp(newScreenColor.a, 0, fadeSpeed);
        bloodScreen.color = newScreenColor;
    }

    void BloodScreenUp()
    {
        Color newScreenColor = bloodScreen.color;
        newScreenColor.a = Mathf.Lerp(newScreenColor.a, 1, fadeSpeed);
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
}
