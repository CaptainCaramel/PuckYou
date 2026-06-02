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

    [SerializeField] private float baseSpeed = 10f;
    protected bool movementLocked = false;
    protected Rigidbody2D rb;


    [Header("Combat")]
    [SerializeField] private float attackCooldown;
    protected bool isAttacking, attackAvailable;
    protected bool aimLocked = false;

    protected bool isQing, isEing, isUlting;
    protected bool q_onCooldown, e_onCooldown, ult_onCooldown;

    [SerializeField] private GameObject puckPrefab;
    [SerializeField] private Transform puckTransform;
    [SerializeField] protected Animator spinAnimator;


    [Header("Controls")]

    [SerializeField] private InputSystem_Actions pl_controls;
    [HideInInspector] public InputAction move, jump, attack, ability1, ability2, ulti;

    public Coroutine currentAction;

    [Header("HP")]

    [SerializeField] private int hp = 5;
    [SerializeField] private float invTime = 1f;
    private bool invincible = false;
    //private spriteFlashScript sFlash;
    

    [Header("VFX")]
    [SerializeField]
    private CamShakerScript shakeCamScript;

    //Amp, Freq, Time
    private Vector3 damageShake = new Vector3( 3f, 0.2f, 0.1f);

    [Header("Dev")]
    [SerializeField]
    private bool godMode = false;

    [Header("Sprite")]
    [SerializeField] protected GameObject puckHover;

    public static PlayerMovement instance;

    private void Awake()
    {
        pl_controls = new InputSystem_Actions();
        shakeCamScript = GetComponent<CamShakerScript>();
        instance = this;
        //sFlash = GetComponent<spriteFlashScript>();
        attackAvailable = true;
        isAttacking = false;
    }

    protected virtual void OnEnable()
    {
        move = pl_controls.Player.Move;
        attack = pl_controls.Player.Attack;
        ability1 = pl_controls.Player.Ability1;
        ability2 = pl_controls.Player.Ability2;
        ulti = pl_controls.Player.Ulti;
        

        InputAction[] inputActions = { move, jump, attack, ability1, ability2, ulti};

        foreach (InputAction action in inputActions)
        {
            action.Enable();
        }


        attack.performed += Attack_RegAction;
        ability1.performed += Attack_QAction;
        ability2.performed += Attack_EAction;
        ulti.performed += Attack_UltAction;
    }


    private void OnDisable()
    {
        InputAction[] inputActions = { move, jump, attack, ability1, ability2, ulti };

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
        if (!movementLocked) handleMovement();
    }

    private void FixedUpdate()
    {
        if (!aimLocked) handleAim();
    }

    void handleAim()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        Vector3 aimDirection = (mousePos - transform.position).normalized;
        float angle = (Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg);
        transform.eulerAngles = new Vector3(0, 0, angle);
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
        puck.GetComponent<PuckScript>().returnObj = gameObject;
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


    public void exitInteraction()
    {
        pl_controls.Player.Enable();
        currentAction = null;
    }



    private bool CanQ()
    {
        return !q_onCooldown && isQing && CanAttack(); 
    }

    private bool CanE()
    {
        return true;
    }

    private bool CanR()
    {
        return true;
    }


    //      //     //   //    CONTROL METHODS     //      //    //     //

    private void Attack_RegAction(InputAction.CallbackContext callbackContext)
    {
        startAction(attack_Regular());
    }

    private void test(InputAction.CallbackContext callbackContext)
    {
        print("asd");
    }

    protected virtual void Attack_QAction(InputAction.CallbackContext callbackContext) 
    {
        if(!CanQ()) return;
        print("baseQ");
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
        if (collision.gameObject.CompareTag("attackHitbox"))
        {
            if (!invincible && !godMode) handleAttacked();
        }

        if(collision.gameObject.layer == 7)
        {
            startAction(damage(1));
        }
    }

}
