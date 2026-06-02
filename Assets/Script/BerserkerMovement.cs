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

    protected override void Attack_QAction(InputAction.CallbackContext callbackContext)
    {
        base.Attack_QAction(callbackContext);
        StartCoroutine(qAttack());
    }


    protected virtual void Attack_EStarted(InputAction.CallbackContext callbackContext)
    {
        isChargingDash = true;
        dashSlider.gameObject.SetActive(true);
    }

    protected override void Attack_EAction(InputAction.CallbackContext callbackContext)
    {
        isChargingDash = false;
        StartCoroutine(dash());

    }

    protected virtual void Attack_UltAction(InputAction.CallbackContext callbackContext)
    {

    }

}
