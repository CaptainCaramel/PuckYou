using DG.Tweening;
using Unity.Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager instance;

    public CinemachineCamera cm_Main, cm_Shake;

    private float _larpAmount;

    private float _duration;

    [SerializeField] private float defaultCamSize;

    private void Awake()
    {
        instance = this;
        defaultCamSize = cm_Main.Lens.OrthographicSize;
    }

    public void changeZoom(float orthSize, float speed, bool unscaledTime)
    {
        _duration = speed;

        _larpAmount = cm_Main.Lens.OrthographicSize;
        DOTween.To(GetLerpValue, SetLerpValue, orthSize, _duration).SetEase(Ease.InOutCubic).OnUpdate(OnLerpUpdate).OnComplete(OnLerpComplete).SetUpdate(unscaledTime);
    }

    public void resetZoom(float speed, bool unscaledTime)
    {

        _duration = speed;

        _larpAmount = cm_Main.Lens.OrthographicSize;
        DOTween.To(GetLerpValue, SetLerpValue, defaultCamSize, _duration).SetEase(Ease.InOutCubic).OnUpdate(OnLerpUpdate).OnComplete(OnLerpComplete).SetUpdate(unscaledTime);
    }

    private void OnLerpUpdate()
    {
        cm_Main.Lens.OrthographicSize = GetLerpValue();
        cm_Shake.Lens.OrthographicSize = GetLerpValue();

        
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
