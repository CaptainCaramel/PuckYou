using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using DG.Tweening;

public class McConnorScript : PlayerMovement
{
    private float defaultSpeed;

    [SerializeField] private float ultSpeed = 14;

    [SerializeField] private float dashForce = 7f;
    [SerializeField] private float dashTime = 1.35f;
    [SerializeField] private float dashCooldown = 1.35f;

    [SerializeField] private GameObject dashHitBox, ultHitbox;

    public bool isDashing = false;

    [SerializeField] private float qCooldown = 10;
    [SerializeField] private float qTime = 7.5f;

    [SerializeField] private float eCooldown = 2.5f;


    [SerializeField] private float ultTime = 10f;
    [SerializeField] private float ultCooldown = 35f;

    private AfterimageGenerator afterimageGenerator;

    [SerializeField] TrailRenderer trailRenderer;

    [SerializeField] private GameObject fireTrailPrefab;
    private Coroutine qCRT;
    [SerializeField] private float spawnDelta;
    private WaitForSeconds spawnerWait;

    private float _lerpAmount;
    [SerializeField] private float ultSpeedUpTime;

    [Header("Audio")]
    [SerializeField] private AudioClip qSound;
    [SerializeField] private AudioClip eSound, ultSound;

    protected override void Awake()
    {
        playerClass = PlayerClass.frostbite;

        base.Awake();
        spawnerWait = new WaitForSeconds(spawnDelta);

        afterimageGenerator = GetComponent<AfterimageGenerator>();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        ability1 = pl_controls.McConnor.Ability1;
        ability2 = pl_controls.McConnor.Ability2;
        ulti = pl_controls.McConnor.Ulti;

        enableAttacksInputs();

        ability1.performed += Attack_QAction;
        ability2.performed += Attack_EAction;
        ulti.performed += Attack_UltAction;


        defaultSpeed = baseSpeed;
    }


    private IEnumerator trailSpawner()
    {
        yield return spawnerWait;
        Instantiate(fireTrailPrefab, transform.position, Quaternion.identity);
        qCRT = StartCoroutine(trailSpawner());
    }

    private IEnumerator dash()
    {
        isEing = true;
        isDashing = true;
        invincible = true;
        lockMovement(true);

        dashHitBox.SetActive(true);
        audioMgr.playAudio(eSound, 0.45f, 1, transform, audioMgr.sfx);

        spriteAnimator.Play("david_slide");
        trailRenderer.emitting = true;

        rb.AddForce(transform.right * dashForce);

        yield return new WaitForSeconds(dashTime);
        dashHitBox.SetActive(false);
        trailRenderer.emitting = false;

        unlockMovement();
        invincible = false;
        isDashing = false;
        isEing = false;

        AbilitySliderController.instance.resetSlider(1);

        e_onCooldown = true;
        yield return new WaitForSeconds(dashCooldown);
        e_onCooldown = false;
    }

    protected override bool CanAttack()
    {
        return base.CanAttack() && !isDashing;
    }

    private IEnumerator qAttack()
    {
        isQing = true;
        qCRT = StartCoroutine(trailSpawner());

        audioMgr.playAudio(qSound, 0.45f, 1, transform, audioMgr.sfx);

        yield return new WaitForSeconds(qTime);

        StopCoroutine(qCRT);
        qCRT = null;
        isQing = false;

        AbilitySliderController.instance.resetSlider(0);

        q_onCooldown = true;
        yield return new WaitForSeconds(qCooldown);
        q_onCooldown = false;

    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        if (isUlting) ultMovement();
    }

    private void ultMovement()
    {
        rb.linearVelocity = transform.right * baseSpeed;
    }

    private IEnumerator ultStart()
    {
        isUlting = true;
        invincible = true;
        speedUp();
        lockMovement(true);

        audioMgr.playAudio(ultSound, 0.45f, 1, transform, audioMgr.sfx);

        afterimageGenerator.startGeneration();
        CameraManager.instance.changeZoom(8, 4, false);
        ultHitbox.SetActive(true);
        yield return new WaitForSeconds(ultTime);


        resetSpeed();
        ultHitbox.SetActive(false);
        afterimageGenerator.stopGeneration();
        CameraManager.instance.resetZoom(1, false);
        isUlting = false;
        invincible = false;
        unlockMovement();

        AbilitySliderController.instance.resetSlider(2);

        ult_onCooldown = true;
        yield return new WaitForSeconds(ultCooldown);
        ult_onCooldown = false;
    }



    protected override bool CanQ()
    {
        return base.CanQ();
    }

    protected override void Attack_QAction(InputAction.CallbackContext callbackContext)
    {
        if (!CanQ()) return;
        StartCoroutine(qAttack());
    }

    protected override void Attack_EAction(InputAction.CallbackContext callbackContext)
    {
        if (!CanE()) return;
        StartCoroutine(dash());

    }

    protected override void Attack_UltAction(InputAction.CallbackContext callbackContext)
    {
        if (!CanR()) return;
        StartCoroutine(ultStart());
    }


    //////// Ult Speed Easing

    private void OnLerpUpdate()
    {
        baseSpeed = GetLerpValue();
    }

    private void resetSpeed()
    {
        DOTween.To(GetLerpValue, SetLerpValue, defaultSpeed, ultSpeedUpTime*0.25f).SetEase(Ease.OutExpo).OnUpdate(OnLerpUpdate);
    }

    private float GetLerpValue()
    {
        return _lerpAmount;
    }

    private void SetLerpValue(float f)
    {
        _lerpAmount = f;
    }

    public void speedUp()
    {
        _lerpAmount = 4;
        DOTween.To(GetLerpValue, SetLerpValue, ultSpeed, ultSpeedUpTime).SetEase(Ease.InExpo).OnUpdate(OnLerpUpdate);
    }



}
