using System.Threading;
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using System.Collections;

public class McConnorScript : PlayerMovement
{
    private float defaultSpeed;

    [SerializeField] private float dashForce = 7f;
    [SerializeField] private float dashTime = 1.35f;
    [SerializeField] private float dashCooldown = 1.35f;

    [SerializeField] private float dashChargeIncrement = 0.075f;
    private float dashCharge;
    private bool isChargingDash = false;

    [SerializeField] private GameObject dashHitBox;

    public bool isDashing = false;

    [SerializeField] private float eCooldown = 2.5f;


    [SerializeField] private float ultTime = 10f;
    [SerializeField] private float ultCooldown = 35f;

    [SerializeField] private GameObject ultPucks;

    private AfterimageGenerator afterimageGenerator;


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


        defaultSpeed = baseSpeed;
    }



    private IEnumerator dash()
    {
        isEing = true;
        isDashing = true;
        lockMovement(true);

        dashHitBox.SetActive(true);

        rb.AddForce(transform.right * dashForce * 250);

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

    }

    private IEnumerator ultStart()
    {
        yield return null;
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


    protected virtual void Attack_EStarted(InputAction.CallbackContext callbackContext)
    {
        if (!CanE()) return;

        isChargingDash = true;
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
