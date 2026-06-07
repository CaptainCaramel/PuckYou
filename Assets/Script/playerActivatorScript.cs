using Unity.VisualScripting;
using UnityEngine;

public class playerActivatorScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject red, ult;
    public GameObject blue;
    public GameObject yellow;

    [SerializeField] private int overrideClass = -1;

    void Awake()
    {
        int klasi = functionScript.klasi;

        if(overrideClass != -1) klasi = overrideClass;
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
}
