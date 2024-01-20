using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class LevelController : MonoBehaviour
{
    public GameObject player;
    private bool isGamePaused = false;
    public AudioMixer audioMixer;
    public TextMeshProUGUI warningText;
    public TextMeshProUGUI questText;
    [HideInInspector] public int cleanedRuinCount;
    public GameObject sceneTransition;
    private bool isGameStarted = false;

    private void Start()
    {
        player.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        Cursor.lockState = CursorLockMode.None;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftAlt) && isGameStarted)
        {
            Cursor.lockState = CursorLockMode.None;
        }
        
        if (Input.GetKeyUp(KeyCode.LeftAlt) && !isGamePaused && isGameStarted)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }

        if(questText != null) questText.text = "Ignite all 5 of ruins: " + cleanedRuinCount + "/5";

        if (cleanedRuinCount >= 5)
        {
            StartCoroutine(LevelWin());
        }
    }

    private IEnumerator LevelWin()
    {
        if(warningText != null) warningText.gameObject.SetActive(true);
        if(warningText != null) warningText.text = "Completed mission: Communicate with gods";
        yield return new WaitForSeconds(1);
        sceneTransition.GetComponent<Animator>().SetTrigger("Start");
        yield return new WaitForSeconds(1.25f);
        SceneManager.LoadScene(1);
    }

    public void StartLevel()
    {
        player.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        Cursor.lockState = CursorLockMode.Locked;
        isGameStarted = true;
    }
    
    public void OnPause()
    {
        player.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        isGamePaused = true;
    }

    public void OnPlay()
    {
        player.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        Cursor.lockState = CursorLockMode.Locked;
        isGamePaused = false;
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

    public void MainMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
    
    public void LoadScene(int sceneId)
    {
        SceneManager.LoadScene(sceneId);
    }
}
