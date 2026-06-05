using Unity.VisualScripting;
using UnityEngine;

public class playerActivatorScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject red, ult;
    public GameObject blue;
    public GameObject yellow;
    void Awake()
    {
        int klasi = functionScript.klasi;
        switch(klasi)
        {
            case 0:
                red.SetActive(true);
                ult.SetActive(true);
                break;
            case 1:
                blue.SetActive(true);
                break;
            case 2:
                yellow.SetActive(true);
                break;
        }
            
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
