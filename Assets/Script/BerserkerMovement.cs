using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class BerserkerMovement : PlayerMovement
{

    [SerializeField] private float dashForce = 7f;
    [SerializeField] private float dashTime = 1.35f;
    [SerializeField] private float dashCooldown = 1.35f;

    [SerializeField] private float dashChargeIncrement = 0.075f;
    private float dashCharge;
    private bool isChargingDash = false;
    [SerializeField] private Slider dashSlider;

    [SerializeField] private GameObject dashHitBox;

    public bool isDashing = false;

    [SerializeField] private float eCooldown = 2.5f;


    [SerializeField] private float ultTime = 20f;
    [SerializeField] private float ultCooldown = 35f;
    [SerializeField] private float healAmount = 4f;

    [SerializeField] private float ultCamSize = 5f;

    [SerializeField] private GameObject ultPucks;
    private bool berserk;

    [SerializeField] private float spinTime = 1.4f;
    [SerializeField] private float spinCooldown = 2f;

    [SerializeField] private GameObject spinPuck;

    [Header("Audio")]
    [SerializeField] private AudioClip qSound;
    [SerializeField] private AudioClip eSound, ultStartSound, ultSound;

    protected override void Awake()
    {
        playerClass = PlayerClass.berserker;
        base.Awake();
    }
    protected override void OnEnable()
    {
        base.OnEnable();
        ability1 = pl_controls.Berserker.Ability1;
        ability2 = pl_controls.Berserker.Ability2;
        ulti = pl_controls.Berserker.Ulti;

        enableAttacksInputs();

        ability1.performed += Attack_QAction;
        ability2.started += Attack_EStarted;
        ability2.canceled += Attack_EAction;
        ulti.performed += Attack_UltAction;

    }

    protected override void Update()
    {
        base.Update();
        if (isChargingDash) handleDashCharge();
    }

    private void handleDashCharge()
    {
        dashCharge += dashChargeIncrement * Time.deltaTime;
        if(dashCharge > 1) dashCharge = 1;

        dashSlider.value = dashCharge;
    }



    private IEnumerator dash()
    {
        isEing = true;
        isDashing = true;
        dashSlider.gameObject.SetActive(false);
        lockMovement(true);

        dashHitBox.SetActive(true);

        spriteAnimator.Play("red_slide");
        audioMgr.playAudio(eSound, 0.5f, 1, transform, audioMgr.sfx);

        rb.AddForce(transform.right *  dashForce * dashCharge * 250);
        dashCharge = 0;
        dashSlider.value = 0;

        yield return new WaitForSeconds(dashTime);
        dashHitBox.SetActive(false);

        unlockMovement();
        isDashing = false;
        isEing = false;

        AbilitySliderController.instance.resetSlider(1);

        e_onCooldown = true;
        yield return new WaitForSeconds(dashCooldown);
        e_onCooldown = false;
    }

    private IEnumerator qAttack()
    {
        yield return null;
        aimLocked = true;
        isAttacking = true;
        isQing = true;
        
        spinPuck.SetActive(true);
        puckHover.SetActive(false);

        audioMgr.playAudio(qSound, 0.5f, 1, transform, audioMgr.sfx);

        spinAnimator.Play("Spin");
        yield return new WaitForSeconds(spinTime);


        spinPuck.SetActive(false);
        puckHover.SetActive(true);

        aimLocked = false;
        isAttacking = false;
        isQing = false;

        AbilitySliderController.instance.resetSlider(0);

        q_onCooldown = true;
        yield return new WaitForSeconds(spinCooldown);
        q_onCooldown = false;

    }

    private IEnumerator ultStart()
    {
        isUlting = true;
        Time.timeScale = 0.2f;

        CameraManager.instance.changeZoom(ultCamSize, 1, true);
        VignetteControllerScript.instance.changeIntensity(0.4f, 1, true);

        audioMgr.playAudio(ultStartSound, 0.6f, 1, transform, audioMgr.sfx);

        yield return new WaitForSecondsRealtime(2.5f);

        Time.timeScale = 1;
        CameraFlashScript.instance.callFlash();
        VignetteControllerScript.instance.changeColor(Color.red);
        CameraManager.instance.resetZoom(0, true);

        StartCoroutine(berserkMode());
    }

    private IEnumerator berserkMode()
    {
        audioMgr.playAudio(ultSound, 0.4f, 1, transform, audioMgr.sfx);

        ultPucks.SetActive(true);
        berserk = true;
        yield return new WaitForSeconds(ultTime);
        ultPucks.SetActive(false);
        berserk = false;

        CameraFlashScript.instance.callFlash();
        VignetteControllerScript.instance.resetIntensity();
        VignetteControllerScript.instance.changeColor(Color.black);

        AbilitySliderController.instance.resetSlider(2);

        ult_onCooldown = true;
        yield return new WaitForSeconds(ultCooldown);
        isUlting = false;
        ult_onCooldown= false;
    }

    public void heal(int amount)
    {
        hp += amount;
    }


    protected override bool CanQ()
    {
        return base.CanQ() && !isAttacking;
    }

    protected override void Attack_QAction(InputAction.CallbackContext callbackContext)
    {
        if (!CanQ()) return;
        StartCoroutine(qAttack());
    }


    protected virtual void Attack_EStarted(InputAction.CallbackContext callbackContext)
    {
        if (!CanE()) return;

        isChargingDash = true;
        dashSlider.gameObject.SetActive(true);
    }

    protected override void Attack_EAction(InputAction.CallbackContext callbackContext)
    {
        if (!CanE() || !isChargingDash) return;

        isChargingDash = false;
        StartCoroutine(dash());

    }

    protected override void Attack_UltAction(InputAction.CallbackContext callbackContext)
    {
        print("isUlting: " + isUlting);
        if (!CanR() || isUlting) return;
        StartCoroutine(ultStart());
    }

}
