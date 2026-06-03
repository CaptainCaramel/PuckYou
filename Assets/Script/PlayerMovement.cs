using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{

    [Header("Movement")]

    [SerializeField] protected float baseSpeed = 10f, nonBaseSpeed, fallBackSpeed;
    protected bool movementLocked = false;
    protected Rigidbody2D rb;


    [Header("Combat")]
    [SerializeField] private float attackCooldown;
    protected bool isAttacking, attackAvailable;
    protected bool aimLocked = false;

    public bool isQing, isEing, isUlting;
    protected bool q_onCooldown, e_onCooldown, ult_onCooldown;

    [SerializeField] private GameObject puckPrefab;
    [SerializeField] protected Transform puckTransform;
    [SerializeField] protected Animator spinAnimator;


    [Header("Controls")]

    [SerializeField] protected InputSystem_Actions pl_controls;
    [HideInInspector] public InputAction move, jump, attack, ability1, ability2, ulti;

    public Coroutine currentAction;

    [Header("HP")]

    [SerializeField] private int hp = 100;
    float hpCurVel = 0f;
    [SerializeField] private float invTime = 1f;
    private bool invincible = false;
    //private spriteFlashScript sFlash;
    

    [Header("VFX")]
    [SerializeField]
    private CamShakerScript shakeCamScript;

    [SerializeField] protected ShakeSelfScript shakeSelfScript;

    //Amp, Freq, Time
    private Vector3 damageShake = new Vector3( 3f, 0.2f, 0.1f);

    [Header("Dev")]
    [SerializeField]
    private bool godMode = false;

    [Header("Sprite")]
    [SerializeField] protected GameObject puckHover;

    public static PlayerMovement instance;

    public UnityEngine.UI.Slider hpslider;

    public Animator yinulisGamgisAnimator;

    private void Awake()
    {
        hp = 100;
        gameObject.name = "Player";
        pl_controls = new InputSystem_Actions();
        shakeCamScript = GetComponent<CamShakerScript>();
        instance = this;
        //sFlash = GetComponent<spriteFlashScript>();
        attackAvailable = true;
        isAttacking = false;

        isQing = false;
        isEing = false;
        isUlting = false;

        q_onCooldown = false;
        e_onCooldown = false;
        ult_onCooldown = false;

        gameObject.name = "Player";

        hpslider = GameObject.Find("HP").GetComponent<UnityEngine.UI.Slider>();
        nonBaseSpeed = baseSpeed / 2;
        fallBackSpeed = baseSpeed;

        yinulisGamgisAnimator = GameObject.Find("YinulisGamge").GetComponent<Animator>();
    }

    protected virtual void OnEnable()
    {
        move = pl_controls.Player.Move;
        attack = pl_controls.Player.Attack;
        InputAction[] inputActions = { move, attack };

        foreach (InputAction action in inputActions)
        {
            action.Enable();
        }


        attack.performed += Attack_RegAction;
    }

    protected virtual void enableAttacksInputs()
    {
        InputAction[] inputActions = { ability1, ability2, ulti };

        foreach (InputAction action in inputActions)
        {
            action.Enable();
        }
    }


    private void OnDisable()
    {
        InputAction[] inputActions = { move, attack, ability1, ability2, ulti };

        foreach (InputAction action in inputActions)
        {
            action.Disable();
        }

    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }


    protected virtual void Update()
    { 
        
    }

    private void FixedUpdate()
    {
        if (!movementLocked) handleMovement();
        if (!aimLocked) handleAim();
        handleHP();
    }

    void handleHP()
    {
        hp = Mathf.Clamp(hp, 0, 100);
        float sliderTarget = hp * 0.79f;

        float currHP = Mathf.SmoothDamp(hpslider.value, sliderTarget, ref hpCurVel, 0.2f);
        hpslider.value = currHP;
    }

    void handleAim()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        Vector3 aimDirection = (mousePos - transform.position).normalized;
        float angle = (Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg);
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    private void handleMovement()
    {
        Vector2 moveXY = move.ReadValue<Vector2>();

        rb.linearVelocity = moveXY * baseSpeed;

    }

    //private bool CanDash()
    //{
       // return !isDashing && dashAvailable && !isAttacking;
    //}

    private bool CanAttack()
    {
        return attackAvailable && !isAttacking;
    }


    protected void lockMovement(bool resetSpeed)
    {
        movementLocked = true;
        if(resetSpeed)rb.linearVelocity = Vector3.zero;
    }

    protected void unlockMovement()
    {
        movementLocked = false; 
    }

    private void resetMovement()
    {
        rb.linearVelocity = Vector3.zero;
    }

    private IEnumerator attack_Regular()
    {
        if(!CanAttack()) yield break;


        attackAvailable = false;
        isAttacking = true;

        //if (onPlayerAttack != null) onPlayerAttack();

        //Attack Code
        GameObject puck = Instantiate(puckPrefab, puckTransform.position , puckTransform.rotation);
        //puck.GetComponent<PuckScript>().returnObj = gameObject;
        puckHover.SetActive(false);


        currentAction = null;
    }

    public IEnumerator puckReturn()
    {
        puckHover.SetActive(true);

        isAttacking = false;
        attackAvailable = true;

        yield return null;
    }

    private IEnumerator damage(int damage)
    {
        invincible = true;
        hp -= damage;
        //sFlash.callFlash();
        //shakeCamScript.StartShake(damageShake);
        StartCoroutine(frameStop(0.12f));

        yield return new WaitForSeconds(invTime);
        invincible = false;

        currentAction = null;
    }

    private void setHP(int hp)
    {
        //set hp code here
    }


    private IEnumerator frameStop(float time)
    {
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(time);
        Time.timeScale = 1f;
    }

    private void startAction(IEnumerator action)
    {
        if (currentAction != null) return;
        currentAction = StartCoroutine(action);
    
    }

    private void handleAttacked()
    {
        //if (isBlocking) {
        //    StartCoroutine(attackBlocked());
        //}
        //else damage(4);

    }


    protected virtual bool CanQ()
    {
        return !q_onCooldown && !isQing; 
    }

    protected virtual bool CanE()
    {
        return !e_onCooldown && !isEing;
    }

    protected virtual bool CanR()
    {
        return !ult_onCooldown && !isUlting;
    }


    //      //     //   //    CONTROL METHODS     //      //    //     //

    private void Attack_RegAction(InputAction.CallbackContext callbackContext)
    {
        startAction(attack_Regular());
    }

    protected virtual void Attack_QAction(InputAction.CallbackContext callbackContext) 
    {
        if(!CanQ()) return;
    }

    protected virtual void Attack_EAction(InputAction.CallbackContext callbackContext)
    {
        if (!CanE()) return;
    }

    protected virtual void Attack_UltAction(InputAction.CallbackContext callbackContext)
    {
        if (!CanR()) return;
    }

    //      //     //   //    COLLISION METHODS     //      //    //     //

    private void OnTriggerEnter2D(Collider2D collision)
    {
        String tag = collision.gameObject.tag;
        /*
        if (collision.gameObject.CompareTag("attackHitbox"))
        {
            if (!invincible && !godMode) handleAttacked();
        }

        if(collision.gameObject.layer == 7)
        {
            startAction(damage(1));
        }
        */
        print("collision with " + tag);
        print("collision with name: " + collision.gameObject.name);
        if (tag.Contains("Hit")) 
        {
            int damageDeal = 0;
            if (tag.Equals("oneHit")) damageDeal = 4;
            else if (tag.Equals("twoHit")) damageDeal = 12;
            else if (tag.Equals("threeHit")) damageDeal = 15;
            else if (tag.Equals("fourHit")) damageDeal = 18;
            StartCoroutine(damage(damageDeal));
            if(collision.gameObject.name != "bite" && collision.gameObject.name != "Punch") Destroy(collision.gameObject);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Slower")
        {
            yinulisGamgisAnimator.Play("IceFadeIn");
            baseSpeed = nonBaseSpeed; 
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Slower")
        {
            yinulisGamgisAnimator.Play("IceFadeOut");
            baseSpeed = fallBackSpeed;
        }
    }
}
