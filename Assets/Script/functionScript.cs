using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class functionScript : MonoBehaviour
{
    //VICI ROM YLEOBA KODIA PRSOTA ZUSTAD 5:00 ARI DA IMENA MEZAREBA DAWERA

    public GameObject red, blue, yellow;
    public Animator roller;
    public Text Choose;
    public GameObject fadeout;
    public static int klasi; // 0 bersk, 1 blue, 2 yel
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        klasi = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void rollOutYellow()
    {
        yellow.SetActive(true);
        red.SetActive(false); 
        blue.SetActive(false);
        klasi = 2;
        rollout();
    }

    public void rollOutRed()
    {
        print("roll");
        red.SetActive(true);
        blue.SetActive(false);
        yellow.SetActive(false);
        klasi = 0;
        rollout();
    }

    public void rollOutBlue()
    {
        blue.SetActive(true);
        yellow.SetActive(false);
        red.SetActive(false);
        klasi = 1;
        rollout();
    }

    public void rollout()
    {
        roller.Play("rollout");
    }

    public void start()
    {
        StartCoroutine(starter());
    }

    IEnumerator starter()
    {
        fadeout.SetActive(true);
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(4);
    }
}
