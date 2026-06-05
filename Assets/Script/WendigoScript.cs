using System.Collections;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using static UnityEngine.Rendering.DebugUI;

public class WendigoScript : EnemyScript
{
    public float chargeDistance = 25f, attackDistance = 2f;
    public bool canCharge = true, canReCheck = true;
    public GameObject hexaHitbox, clawHitbox;
    public float chargeUpS, chargeUpC, attackDurC, cooldownC, attackDurS, cooldownS, chargeSpeed;
    public bool isCharging, isBegin = true;
    public Animator clawAnimator;
    public CamShakerScript cum;
    public Vector3 wallShake = new Vector3(2f,2f,1f);
    public bool alrHit;
    public bool start;
    public UnityEngine.UI.Slider hpslider;
    new private int hp = 100;
    float hpCurVel = 0f;
    public Transform wendysFuneralLocation;
    public GameObject ender;
    public GameObject wall;
    new public spriteFlashScript spriteFlash;
    public Animator endingAnimator;
    public Animator chucker;
    bool first;

    [SerializeField] private AudioClip slashDouble, slashLong, wallHit;
    [SerializeField] private AudioClip[] sikdili;

    private AudioSource currAttackSource;

    [SerializeField] private GameObject hitbox, chargeHitbox;

    protected override void Start()
    {
        deathSound = sikdili;

        base.Start();
        setSpeedAndRange(8, 0);
        rotationSpeed = 360;
        alrHit = false;
        StartCoroutine(starter());
        snapAtTarget(wall.transform);
        spriteFlash = GetComponent<spriteFlashScript>();
        hp = 100;
        first = true;
    }

    public override void Movement()
    {
        if (!start) return;

        handleHP();
        print("aq var");
        bool isInRange = distance <= attackDistance;
        if (shouldRotate || isAttacking && isInRange && !isCharging) lookAt(angleToTarget);

        if (isAttacking) return;

        if(distance <= chargeDistance && canReCheck)
        {
            if(checkForCharge())
            {
                StartCoroutine(charge(true));
                return;
            }
        }
        
        if(isInRange)
        {
            hitbox.SetActive(false);

            //if(currAttackSource != null && currAttackSource.isPlaying) currAttackSource.Stop();
            Attack();
        }
        else
        {
            animator.SetBool("isWalking", true);
            clawAnimator.SetBool("isAttacking", true);
            hitbox.SetActive(true);

            //if (currAttackSource == null || !currAttackSource.isPlaying) currAttackSource = audioManager.instance.playAudio(slashDouble, 0.5f, 1, transform, audioManager.instance.sfx);
            rb.linearVelocity = direction * (speed);
            alrHit = true;
        }
    }


    void handleHP()
    {
        hp = Mathf.Clamp(hp, 0, 100);
        float currHP = Mathf.SmoothDamp(hpslider.value, hp, ref hpCurVel, 0.2f);
        hpslider.value = currHP;
    }

    IEnumerator charge(bool isFirst)
    {
        if (!canAttack && isFirst) yield break;
        hitbox.SetActive(false);

        isAttacking = true;
        isCharging = true;
        animator.SetBool("isHit", false);
        if (isFirst)
        {
            halt();
            clawAnimator.SetBool("isCrashingOut", false);
            clawAnimator.SetBool("isAttacking", false);
            animator.SetBool("isCharging", true);
            yield return new WaitForSeconds(0.5f);
        }
        print("Charging");
        isAttacking = true;
        canAttack = false;
        if(isFirst)yield return new WaitForSeconds(chargeUpC);
        shouldRotate = false;
        Vector2 dir = direction;
        //animator.Play("Charge");

        chargeHitbox.SetActive(true);
        rb.linearVelocity = dir * (chargeSpeed);
    }

    bool checkForCharge()
    {
        StartCoroutine(disableChecking());
        if (first)
        {
            first = false;
            return true;
        } 
        if (canCharge)
        {
            return (Random.Range(0, 6) == 0) ? true : false;
        }
        else return false;
    }

    IEnumerator disableChecking()
    {
        canReCheck = false;
        yield return new WaitForSeconds(4f);
        canReCheck = true;
    }

    protected override void Attack()
    {
        if (!canAttack) return;

        halt();

        animator.SetBool("isWalking", false);
        clawAnimator.SetBool("isCrashingOut", true);

        isAttacking = true;
        canAttack = false;


        int random = Random.Range(0, 1);

        if(random == 0)
        {
            //animator.Play("Swinging");
            StartCoroutine(swing());
        }
    }
    IEnumerator swing()
    {
        halt();
        shouldRotate = false;
        GameObject attHitbox = UnityEngine.Random.Range(0, 2) == 0 ? clawHitbox : hexaHitbox;
        yield return new WaitForSeconds(chargeUpS);
        currAttackSource = audioManager.instance.playAudio(slashLong, 0.5f, 1, transform, audioManager.instance.sfx);

        //attHitbox.SetActive(true);
        yield return new WaitForSeconds(attackDurS);
        //Time.timeScale = 0;
        //attHitbox.SetActive(false);
        clawAnimator.SetBool("isCrashingOut", false);
        //animator.SetBool("isIdling", true);
        yield return new WaitForSeconds(cooldownS);
        //Time.timeScale = 0;
        isAttacking = false;
        shouldRotate = true;
        canAttack = true;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        print("collision gameobject: " + collision.gameObject.tag);
        if(isCharging && collision.gameObject.tag.Equals("border"))
        {
            print("dashing again");
            audioManager.instance.playAudio(wallHit, 0.65f, 1, transform, audioManager.instance.sfx);
            chargeHitbox.SetActive(false);


            cum.StartShake(wallShake);
            StartCoroutine(backCharge());
        }
    }

    IEnumerator backCharge()
    {
        rb.angularVelocity = 0;
        rb.linearVelocity = -transform.forward * 50;
        halt();
        if (isBegin)
        {
            isBegin = false;
            animator.SetBool("isHit", true);
            transform.Rotate(new Vector3(0, 0, 180));
            yield return new WaitForSeconds(1.5f);
            snapAtTarget(player);
            animator.SetBool("isHit", false);
            StartCoroutine(charge(false));
        }
        else
        {
            isBegin = true;
            isCharging = false;
            animator.SetBool("isStopped", true);
            yield return new WaitForSeconds(0.3f);
            animator.SetBool("isCharging", false);
            animator.SetBool("isStopped", false);
            canAttack = true;
            isAttacking = false;
            shouldRotate = true;
            yield break;
        }
    }


    public IEnumerator starter()
    {
        start = false;
        yield return null;
        start = true;
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "puck")
        {
            damage(2);
        }
    }

    public override void damage(int damage)
    {
        audioManager.instance.playAudio(damageSound, 0.5f, 1, transform, audioManager.instance.sfx);

        spriteFlash.callFlash();
        hp-=damage;
        if(hp <= 0)
        {
            start = false;
            Instantiate(deathParticles, wendysFuneralLocation.position, Quaternion.identity);
            endingAnimator.SetBool("end", true);
            chucker.SetBool("chuck", true);
            Destroy(hpslider);
            Destroy(gameObject);
        }
    }
}
