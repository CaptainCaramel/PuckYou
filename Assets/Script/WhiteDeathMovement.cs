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
    [SerializeField] GameObject halfChargedPuck, fullChargedPuck;

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
        yield return null;
        print("fullQ");
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

    private void Attack_StartQ(InputAction.CallbackContext callbackContext)
    {
        shootSlider.gameObject.SetActive(true);
        isChargingQ = true;
    }
    protected void Attack_FullQ(InputAction.CallbackContext callbackContext)
    {
        isQMaxxed = true;
    }

    protected void Attack_ReleaseQ(InputAction.CallbackContext callbackContext)
    {
        if (isQMaxxed) StartCoroutine(superQ());
        else StartCoroutine(normalQ());

        isQMaxxed = false;
        isChargingQ = false;
        shootSlider.value = 0;
    }
}
