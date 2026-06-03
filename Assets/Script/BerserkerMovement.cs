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

        rb.AddForce(transform.right *  dashForce * dashCharge * 250);
        dashCharge = 0;
        dashSlider.value = 0;

        yield return new WaitForSeconds(dashTime);
        dashHitBox.SetActive(false);

        unlockMovement();
        isDashing = false;
        isEing = false;

        yield return new WaitForSeconds(dashCooldown);
    }

    private IEnumerator qAttack()
    {
        yield return null;
        aimLocked = true;
        isAttacking = true;
        isQing = true;
        
        spinPuck.SetActive(true);
        puckHover.SetActive(false);
        spinAnimator.Play("Spin");
        yield return new WaitForSeconds(spinTime);


        spinPuck.SetActive(false);
        puckHover.SetActive(true);

        aimLocked = false;
        isAttacking = false;
        isQing = false;

        q_onCooldown = true;
        yield return new WaitForSeconds(spinCooldown);
        q_onCooldown = false;

    }

    private IEnumerator ultStart()
    {
        Time.timeScale = 0.2f;

        CameraManager.instance.changeZoom(ultCamSize, 1, true);
        VignetteControllerScript.instance.changeIntensity(0.5f, 1, true);
        yield return new WaitForSecondsRealtime(2.5f);

        Time.timeScale = 1;
        CameraFlashScript.instance.callFlash();
        VignetteControllerScript.instance.changeColor(Color.red);
        CameraManager.instance.resetZoom(0, true);

        StartCoroutine(berserkMode());
    }

    private IEnumerator berserkMode()
    {
        ultPucks.SetActive(true);
        berserk = true;
        yield return new WaitForSeconds(ultTime);
        ultPucks.SetActive(false);
        berserk = false;

        CameraFlashScript.instance.callFlash();
        VignetteControllerScript.instance.resetIntensity();
        VignetteControllerScript.instance.changeColor(Color.black);

        ult_onCooldown = true;
        yield return new WaitForSeconds(ultCooldown);
        ult_onCooldown= false;
    }

    public void heal(float amount)
    {
        //
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
        isChargingDash = false;
        StartCoroutine(dash());

    }

    protected override void Attack_UltAction(InputAction.CallbackContext callbackContext)
    {
        if (!CanR()) return;
        StartCoroutine(ultStart());
    }

}
