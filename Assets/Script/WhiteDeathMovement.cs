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
    [SerializeField] private float fullQCooldown = 5;
    [SerializeField] GameObject halfChargedPuck, fullChargedPuck;

    [SerializeField] GameObject bomb;
    [SerializeField] private float eCooldown = 7;

    [SerializeField] private int ultShotAmount = 5;
    [SerializeField] private float UltCooldown = 20f;
    [SerializeField] private float UltShootDelta = 4f;

    [SerializeField] private GameObject ultShot;

    [SerializeField] private float ultSpeed;
 
    [SerializeField] private float zoomOutCamSize = 9;
    [SerializeField] private float zoomOutCamSpeed = 1.5f;

    [SerializeField] private float knockbacktime = 0.25f;
    [SerializeField] private float knockbackForce = 200f;

    [Header("Audio")]
    [SerializeField] private AudioClip qSound;
    [SerializeField] private AudioClip chargedQSound, eSound ,ultStartSound, ultSound, ultShotSound;

    protected override void Awake()
    {
        playerClass = PlayerClass.theWhiteDeath;
        base.Awake();
    }
    protected override void OnEnable()
    {
        base.OnEnable();

        ability1 = pl_controls.WhiteDeath.Ability1;
        ability2 = pl_controls.WhiteDeath.Ability2;
        ulti = pl_controls.WhiteDeath.Ulti;

        ability1.performed += Attack_FullQ;
        ability1.canceled += Attack_ReleaseQ;
        ability1.started += Attack_StartQ;

        ability2.performed += Attack_EAction;

        ulti.performed += Attack_UltAction;

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

        audioMgr.playAudio(chargedQSound, 0.45f, 1, transform, audioMgr.sfx);

        shakeSelfScript.stopShake();
        GameObject puck = Instantiate(fullChargedPuck, puckTransform.position, puckTransform.rotation);
        puck.GetComponent<PuckScript>().returnObj = gameObject;
        WhiteDeathPuckMovement t = puck.GetComponent<WhiteDeathPuckMovement>();
        t.StartCoroutine(t.returnCRT());
        puckHover.SetActive(false);

        AbilitySliderController.instance.resetSlider(0);

        lockMovement(true);
        rb.AddForce(-transform.right * knockbackForce * 2.5f);
        rb.linearDamping = 1.75f;
        yield return new WaitForSeconds(knockbacktime + 0.25f);
        rb.linearDamping = 0;
        unlockMovement();

        currentAction = null;


        

        q_onCooldown = true;
        yield return new WaitForSeconds(fullQCooldown - (knockbacktime+0.25f));
        q_onCooldown = false;   
    }

    private IEnumerator normalQ()
    {
        attackAvailable = false;
        isAttacking = true;

        audioMgr.playAudio(qSound, 0.45f, 1, transform, audioMgr.sfx);

        GameObject puck = Instantiate(halfChargedPuck, puckTransform.position, puckTransform.rotation);
        puck.GetComponent<PuckScript>().returnObj = gameObject;
        WhiteDeathPuckMovement t = puck.GetComponent<WhiteDeathPuckMovement>();
        t.StartCoroutine(t.returnCRT());
        puckHover.SetActive(false);

        AbilitySliderController.instance.resetSlider(0);

        lockMovement(true);
        rb.AddForce(-transform.right * knockbackForce);
        rb.linearDamping = 1.75f;
        yield return new WaitForSeconds(knockbacktime);
        rb.linearDamping = 0;
        unlockMovement();

        currentAction = null;

        

        q_onCooldown = true;
        yield return new WaitForSeconds(qCooldown-knockbacktime);
        q_onCooldown = false;
    }

    private IEnumerator EAttack()
    {
        Transform t;
        if (!isAttacking) t = puckTransform;
        else t = PuckScript.instance.transform;

        audioMgr.playAudio(eSound, 0.45f, 1, transform, audioMgr.sfx);

        Instantiate(bomb, t.position, t.rotation);

        AbilitySliderController.instance.resetSlider(1);

        e_onCooldown = true;
        yield return new WaitForSeconds(eCooldown);
        e_onCooldown = false;
    }

    private IEnumerator UltLife()
    {
        isUlting = true;

        CameraManager.instance.changeZoom(zoomOutCamSize, zoomOutCamSpeed, true);

        audioMgr.playAudio(ultSound, 0.45f, 1, transform, audioMgr.sfx);

        for (int i = 0; i < ultShotAmount; i++)
        {
            yield return new WaitForSeconds(UltShootDelta);
            Vector2 mousePos = Mouse.current.position.ReadValue();
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
            worldPos.z = 0;

            audioMgr.playAudio(ultShotSound, 0.45f, 1, transform, audioMgr.sfx);
            Instantiate(ultShot, worldPos, Quaternion.identity);     
        }

        CameraManager.instance.resetZoom(zoomOutCamSpeed/2, true);

        isUlting = false;

        AbilitySliderController.instance.resetSlider(2);

        ult_onCooldown = true;
        yield return new WaitForSeconds(UltCooldown);
        ult_onCooldown = false;

    }

    protected override bool CanAttack()
    {
        return base.CanAttack() && !isChargingQ;
    }


    protected override bool CanQ()
    {
        return base.CanQ() && !isAttacking && attackAvailable;
    }

    protected override bool CanE()
    {
        return base.CanE();
    }

    private void Attack_StartQ(InputAction.CallbackContext callbackContext)
    {
        print(CanQ());
        if (!CanQ()) { return; }

        shootSlider.gameObject.SetActive(true);
        isChargingQ = true;
    }
    protected void Attack_FullQ(InputAction.CallbackContext callbackContext)
    {
        if (!CanQ() ||  !isChargingQ) return;

        CameraManager.instance.changeZoom(zoomOutCamSize, zoomOutCamSpeed, true);

        isQMaxxed = true;
        shakeSelfScript.Begin();
    }

    protected void Attack_ReleaseQ(InputAction.CallbackContext callbackContext)
    {
        if (!CanQ() || !isChargingQ) return;

        CameraManager.instance.resetZoom(zoomOutCamSpeed / 2, true);

        if (isQMaxxed) StartCoroutine(superQ());
        else StartCoroutine(normalQ());

        shootSlider.gameObject.SetActive(false);

        isQMaxxed = false;
        isChargingQ = false;
        shootSlider.value = 0;
    }

    protected override void Attack_EAction(InputAction.CallbackContext callbackContext)
    {
        if (!CanE() && !isQing) return;

        StartCoroutine(EAttack());

    }

    protected override void Attack_UltAction(InputAction.CallbackContext callbackContext)
    {
        if(!CanR()) return;

        StartCoroutine(UltLife());
    }
}
