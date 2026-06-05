using System;
using System.Collections;
using UnityEngine;

public class MeleeEnemyScript : EnemyScript
{
    public GameObject attHitbox;
    public float cooldown;
    public float attackDur;
    public float chargeUp;
    public bool isWhite, isGolem;
    public GameObject sparks;
    public SpriteRenderer slowerRenderer, selfRenderer;


    [SerializeField] private AudioClip slash, smash;
    private AudioClip attackSound;

    [SerializeField] private AudioClip[] wolfDeathSounds;
    [SerializeField] private AudioClip[] golemDeathSounds;
    protected override void Start()
    {
        base.Start();
        float speed = isGolem ? UnityEngine.Random.Range(3.5f, 4) : (isWhite ? 4f : 3f);
        attackSound = isGolem ? smash : slash;
        deathSound = isGolem? golemDeathSounds : wolfDeathSounds;

        float range = 0.8f;
        setSpeedAndRange(speed, 0.8f);
        if (!isGolem) fov = 45f;
        else
        {
            slowerRenderer.sortingOrder = UnityEngine.Random.Range(10, 19);
            range = UnityEngine.Random.Range(0.6f, 1);
            fov = 180f;
        }
        setSpeedAndRange(speed, range);
    }

    protected override IEnumerator AttackCrt()
    {
        halt();
        shouldRotate = false;
        if (isGolem)
        {
            animator.SetBool("isAttacking", true);
            sparks.SetActive(false);
        }
        yield return new WaitForSeconds(chargeUp);
        audioManager.instance.playAudio(attackSound, 0.5f, 1, transform, audioManager.instance.sfx);
        attHitbox.SetActive(true);
        yield return new WaitForSeconds(attackDur);
        //Time.timeScale = 0;
        attHitbox.SetActive(false);
        isAttacking = false;
        shouldRotate = true;
        if (isGolem)
        {
            animator.SetBool("isAttacking", false);
            sparks.SetActive(true);
        }
        yield return new WaitForSeconds(cooldown);
        canAttack = true;
    }
}
















