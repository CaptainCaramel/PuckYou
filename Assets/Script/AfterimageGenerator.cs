using System.Collections;
using UnityEngine;

public class AfterimageGenerator : MonoBehaviour
{
    [SerializeField] private GameObject AfterimageSprite;
    [SerializeField] private float imageLiveTime;
    [SerializeField] private float imageSpawnDelta;

    public void startGeneration()
    {
        StartCoroutine(genLoop());
    }

    public void stopGeneration()
    {
        StopAllCoroutines();
    }

    private IEnumerator genLoop()
    {
        spawnImage();
        yield return new WaitForSeconds(imageSpawnDelta);
        StartCoroutine(genLoop());
    }

    private void spawnImage()
    {
        GameObject g = Instantiate(AfterimageSprite, transform.position, transform.rotation);
        Destroy(g, imageLiveTime);
    }
}
