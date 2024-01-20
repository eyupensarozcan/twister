using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    public GameObject sceneTransitionObject;
    public GameObject mainMenuLayout;
    public GameObject entryMenuLayout;
    public GameObject loadingScreen;
    public Slider loadingSlider;
    public AudioMixer audioMixer;
    public GameObject menuMusic;
    public GameObject menuSFX;
    
    private bool isStarted = false;
    
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isStarted && Input.GetKeyDown(KeyCode.Return))
        {
            isStarted = true;
            StartCoroutine(EntryMenu());
        }
    }

    public void StartGame(int gameId)
    {
        mainMenuLayout.GetComponent<Animator>().SetTrigger("Start");
        GetComponent<Animator>().SetTrigger("Start");
        Cursor.lockState = CursorLockMode.Locked;
        switch (gameId)
        {
            case 1:
                GetComponent<Animator>().SetTrigger("StartRollABall");
                break;
            case 2:
                GetComponent<Animator>().SetTrigger("StartGoingBalls");
                break;
        }
    }

    public void LoadGame()
    {
        sceneTransitionObject.GetComponent<Animator>().SetTrigger("Start");
    }

    public void LoadNextScene()
    {
        SceneManager.LoadScene(1);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public IEnumerator EntryMenu()
    {
        entryMenuLayout.GetComponent<Animator>().SetTrigger("Start");
        yield return new WaitForSeconds(0.45f);
        Cursor.lockState = CursorLockMode.None;
        mainMenuLayout.GetComponent<Animator>().SetTrigger("Load");
        entryMenuLayout.GetComponent<CanvasGroup>().blocksRaycasts = false;
        yield break;
    }

    public void LoadScene(int sceneId)
    {
        StartCoroutine(LoadSceneAsync(sceneId));
    }

    public void QualitySettings(int quality)
    {
        switch (quality)
        {
            case 0:
                UnityEngine.QualitySettings.SetQualityLevel(0);
                break;
            case 1:
                UnityEngine.QualitySettings.SetQualityLevel(1);
                break;
            case 2:
                UnityEngine.QualitySettings.SetQualityLevel(2);
                break;
            case 3:
                UnityEngine.QualitySettings.SetQualityLevel(3);
                break;
        }
    }

    public void SetVolume(float volume)
    {
        audioMixer.SetFloat("MasterVolume", volume);
    }

    public void MenuMusicOut()
    {
        menuMusic.GetComponent<Animator>().SetTrigger("Out");
    }

    public void MenuSFXOn()
    {
        menuSFX.SetActive(true);
        menuSFX.GetComponent<Animator>().SetTrigger("Out");
    }

    public IEnumerator LoadSceneAsync(int sceneId)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneId);
        loadingScreen.SetActive(true);

        while (!operation.isDone)
        {
            float progressValue = Mathf.Clamp01(operation.progress / 0.9f);
            loadingSlider.value = progressValue;
            yield return null;
        }
    }
    
}
