using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class WhiteDeathMovement : PlayerMovement 
{

    [SerializeField] private Slider shootSlider;
    [SerializeField] private float shootChargeIncrement = 0.7f;

    private bool isChargingQ = false;
    private bool isQMaxxed = false;
    [SerializeField] private float qCooldown = 5;
    [SerializeField] private float fullQCooldown = 8;
    [SerializeField] GameObject halfChargedPuck, fullChargedPuck;

    [SerializeField] private float zoomOutCamSize = 9;
    [SerializeField] private float zoomOutCamSpeed = 1.5f;

    [SerializeField] private float knockbacktime = 0.25f;
    [SerializeField] private float knockbackForce = 200f;
    protected override void OnEnable()
    {
        base.OnEnable();

        ability1 = pl_controls.WhiteDeath.Ability1;
        ability2 = pl_controls.WhiteDeath.Ability2;
        ulti = pl_controls.WhiteDeath.Ulti;

        ability1.performed += Attack_FullQ;
        ability1.canceled += Attack_ReleaseQ;
        ability1.started += Attack_StartQ;

        enableAttacksInputs();
    }

    protected override void Update()
    {
        base.Update();
        if (isChargingQ) handleQCharge();
    }

    private void handleQCharge()
    {
        shootSlider.value += shootChargeIncrement * Time.deltaTime;
    }

    private IEnumerator superQ()
    {
        attackAvailable = false;
        isAttacking = true;

        CameraManager.instance.resetZoom(zoomOutCamSpeed / 2);

        shakeSelfScript.stopShake();

        GameObject puck = Instantiate(fullChargedPuck, puckTransform.position, puckTransform.rotation);
        puck.GetComponent<PuckScript>().returnObj = gameObject;
        WhiteDeathPuckMovement t = puck.GetComponent<WhiteDeathPuckMovement>();
        t.StartCoroutine(t.returnCRT());
        puckHover.SetActive(false);

        lockMovement(true);
        rb.AddForce(-transform.right * knockbackForce * 2.5f);
        rb.linearDamping = 1.75f;
        yield return new WaitForSeconds(knockbacktime + 0.25f);
        rb.linearDamping = 0;
        unlockMovement();

        currentAction = null;


        q_onCooldown = true;
        yield return new WaitForSeconds(fullQCooldown);
        q_onCooldown = false;   
    }

    private IEnumerator normalQ()
    {
        attackAvailable = false;
        isAttacking = true;

        GameObject puck = Instantiate(halfChargedPuck, puckTransform.position, puckTransform.rotation);
        puck.GetComponent<PuckScript>().returnObj = gameObject;
        WhiteDeathPuckMovement t = puck.GetComponent<WhiteDeathPuckMovement>();
        t.StartCoroutine(t.returnCRT());
        puckHover.SetActive(false);

        lockMovement(true);
        rb.AddForce(-transform.right * knockbackForce);
        rb.linearDamping = 1.75f;
        yield return new WaitForSeconds(knockbacktime);
        rb.linearDamping = 0;
        unlockMovement();

        currentAction = null;


        q_onCooldown = true;
        yield return new WaitForSeconds(qCooldown);
        q_onCooldown = false;
    }

    protected override bool CanQ()
    {
        return base.CanQ() && !isAttacking;
    }

    private void Attack_StartQ(InputAction.CallbackContext callbackContext)
    {
        if (!CanQ()) return;

        shootSlider.gameObject.SetActive(true);
        isChargingQ = true;
    }
    protected void Attack_FullQ(InputAction.CallbackContext callbackContext)
    {
        if (!CanQ()) return;

        CameraManager.instance.changeZoom(zoomOutCamSize, zoomOutCamSpeed);

        isQMaxxed = true;
        shakeSelfScript.Begin();
    }

    protected void Attack_ReleaseQ(InputAction.CallbackContext callbackContext)
    {
        if (!CanQ()) return;

        if (isQMaxxed) StartCoroutine(superQ());
        else StartCoroutine(normalQ());

        isQMaxxed = false;
        isChargingQ = false;
        shootSlider.value = 0;
    }
}
