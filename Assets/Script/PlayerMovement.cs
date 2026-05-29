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

    [SerializeField] private float dashForce = 7f;
    [SerializeField] private float dashTime = 1.35f;
    [SerializeField] private float dashCooldown = 1.35f;

    private bool isDashing = false;
    private bool dashAvailable = true;

    private bool movementLocked = false;

    private Rigidbody2D rb;


    [Header("Combat")]
    [SerializeField] private float attackCooldown;
    private bool isAttacking, attackAvailable;

    [SerializeField] private GameObject puckPrefab;
    [SerializeField] private Transform puckTransform;


    [Header("Interaction")]
    [SerializeField] private float interactRadius = 2f;
    [SerializeField] private LayerMask interactLayer;
    private ContactFilter2D interactFilter;

    public bool isReading = false;

    [Header("Controls")]

    [SerializeField] private InputSystem_Actions pl_controls;
    [HideInInspector] public InputAction move, jump, attack, heavyAttack ,dash, downslam, block, interact;

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
    [SerializeField]
    private GameObject puckHover;

    public static PlayerMovement instance;

    private void Awake()
    {
        pl_controls = new InputSystem_Actions();
        shakeCamScript = GetComponent<CamShakerScript>();
        instance = this;
        //sFlash = GetComponent<spriteFlashScript>();

        interactFilter = new ContactFilter2D();
        interactFilter.SetLayerMask(interactLayer);
        interactFilter.useTriggers = true;

        isDashing = false;
        attackAvailable = true;
        isAttacking = false;
    }

    private void OnEnable()
    {
        move = pl_controls.Player.Move;
        jump = pl_controls.Player.Jump;
        attack = pl_controls.Player.Attack;
        //dash = pl_controls.Player.Dash;
        interact = pl_controls.Player.Interact;

        InputAction[] inputActions = { move, jump, attack, dash, interact};

        foreach (InputAction action in inputActions)
        {
            action.Enable();
        }


        attack.performed += Attack_RegAction;
        heavyAttack.started += Attack_HeavyStart;
        heavyAttack.canceled += Attack_HeavyCancel;
        heavyAttack.performed += Attack_HeavyAction;
        dash.performed += DashAction;
        downslam.performed += DownSlamAction;
        //interact.performed += InteractAction;
    }


    private void OnDisable()
    {
        InputAction[] inputActions = { move, jump, attack, heavyAttack ,dash, downslam, block, interact};

        foreach (InputAction action in inputActions)
        {
            action.Disable();
        }

    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }


    void Update()
    {
        if (!movementLocked) handleMovement();
        handleAim();
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
        return !isDashing && attackAvailable && !isAttacking;
    }


    private void lockMovement(bool resetSpeed)
    {
        movementLocked = true;
        if(resetSpeed)rb.linearVelocity = Vector3.zero;
    }

    private void unlockMovement()
    {
        movementLocked = false; 
    }

    private void resetMovement()
    {
        rb.linearVelocity = Vector3.zero;
    }

    private IEnumerator attack_Regular()
    {
        print("a");
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
        //puckHover.GetComponent<Animator>().StartPlayback();

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


    //      //     //   //    CONTROL METHODS     //      //    //     //

    private void DashAction(InputAction.CallbackContext callbackContext)
    {
        //startAction(Dash());
    }

    private void ExitInteraction(InputAction.CallbackContext callbackContext)
    {

    }

    private void Attack_RegAction(InputAction.CallbackContext callbackContext)
    {
        startAction(attack_Regular());
    }

    private void Attack_HeavyStart(InputAction.CallbackContext callbackContext)
    {
        //if(!CanAttack()) return;

       // heavyTEMP.SetActive(true);
       // lockMovement(true);
    }

    private void Attack_HeavyAction(InputAction.CallbackContext callbackContext)
    {
        //heavyTEMP.SetActive(false);
        //unlockMovement();

        //startAction(attack_heavy());
    }

    private void Attack_HeavyCancel(InputAction.CallbackContext callbackContext)
    {
        //heavyTEMP.SetActive(false);
        //unlockMovement();
    }

    private void DownSlamAction(InputAction.CallbackContext callbackContext)
    {
        //startAction(startDownSlam());
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
