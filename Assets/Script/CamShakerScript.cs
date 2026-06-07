using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class CamShakerScript : MonoBehaviour
{
    public float amplitude, frequency, time;

    public bool singleton = false;

    private CinemachineCamera cam;
    private CinemachineBasicMultiChannelPerlin cbmcp;
    //private CinemachinePositionComposer cbpc;

    public static CamShakerScript instance;

    private void Awake()
    {
        
        if(singleton)instance = this;
    }


    public IEnumerator shake()
    {
        if (PlayerMovement.instance.dead) yield break; 
        cam = ShakeCamScript.instance.gameObject.GetComponent<CinemachineCamera>();
        cbmcp = ShakeCamScript.instance.gameObject.GetComponent<CinemachineBasicMultiChannelPerlin>();

        cbmcp.AmplitudeGain = amplitude;
        cbmcp.FrequencyGain = frequency;

        cam.enabled = true;

        if (time > 0)
        {
            yield return new WaitForSeconds(time);
            Stop();
            
        }

    }

    public void StartShake(float amp, float freq, float time)
    {
        amplitude = amp;
        frequency = freq;
        this.time = time;



        StartCoroutine(shake());
    }

    public void StartShake(Vector3 shakeSettings)
    {
        amplitude = shakeSettings.x;
        frequency = shakeSettings.y;
        this.time = shakeSettings.z;

        StartCoroutine(shake());
    }

    public void Stop()
    {
        StopAllCoroutines();
        cam.enabled = false;
    }

}
