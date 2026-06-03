using DG.Tweening;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class VignetteControllerScript : MonoBehaviour
{
    public static VignetteControllerScript instance;

    public Volume sceneVolume;
    private Vignette vignette;

    private float _larpAmount;

    private float _duration;

    [SerializeField] private float defaultVignetteSize;

    private void Awake()
    {
        instance = this;
        sceneVolume.profile.TryGet(out vignette);
        print(vignette);
        defaultVignetteSize = (float)vignette.intensity;
        
        vignette.color.overrideState = true;
    }


    public void changeColor(Color color)
    {
        vignette.color.value = color;
    }


    public void changeIntensity(float intensity, float speed, bool unscaledTime)
    {
        _duration = speed;

        _larpAmount = (float)vignette.intensity;
        DOTween.To(GetLerpValue, SetLerpValue, intensity, _duration).SetEase(Ease.InOutCubic).OnUpdate(OnLerpUpdate).OnComplete(OnLerpComplete).SetUpdate(unscaledTime);
    }

    public void resetIntensity(float speed, bool unscaledTime)
    {

        _duration = speed;

        _larpAmount = (float)vignette.intensity;
        DOTween.To(GetLerpValue, SetLerpValue, defaultVignetteSize, _duration).SetEase(Ease.InOutCubic).OnUpdate(OnLerpUpdate).OnComplete(OnLerpComplete).SetUpdate(unscaledTime);
    }

    public void resetIntensity()
    {
        vignette.intensity.Override(defaultVignetteSize);
    }

    public void OnLerpUpdate()
    {
        vignette.intensity.overrideState = true;
        vignette.intensity.value = GetLerpValue();
        
    }

    private void OnLerpComplete()
    {
        //DOTween.To(GetLerpValue, SetLerpValue, 0f, _duration).SetEase(Ease.InCubic).OnUpdate(OnLerpUpdate);
    }

    private float GetLerpValue()
    {
        return _larpAmount;
    }

    private void SetLerpValue(float f)
    {
        _larpAmount = f;
    }


    
}
