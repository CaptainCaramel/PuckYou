using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject flash;

    private void Start()
    {
        AudioListener.volume = 1;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void play()
    {
        StartCoroutine(startGame());
    }

    IEnumerator startGame()
    {
        flash.SetActive(true);
        yield return new WaitForSeconds(0.4f);
        SceneManager.LoadScene(2);
    }

    public void quit()
    {
        Application.Quit();
    }
}
