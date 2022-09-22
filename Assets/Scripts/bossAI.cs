using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum Boss_State
{
    IDLE,

    WAITING,

    STOMP,

    Dead
}

public class bossAI : MonoBehaviour, IDamageable
{
    [Header("-----Boss Components-----")]
    [SerializeField] NavMeshAgent bossAgent;
    [SerializeField] Renderer mesh;
    [SerializeField] Animator bossAnim;
    [SerializeField] ParticleSystem shockWavePs;
    float distanceToPlayer;

    [Header("-----Boss Stats-----")]
    [SerializeField] int HP;
    [SerializeField] float turnSpeed;
    [Range(1, 180)][SerializeField] int FOV;
    [SerializeField] float distanceForStomp;
    [SerializeField] float timeBetweenStomps;
    [SerializeField] int shockWaveAnimationTime;
    [SerializeField] int ShockWaveDamage;

    [Header("-----Boss Weapon Stats-----")]
    [SerializeField] GameObject bullet;
    [SerializeField] float shootRate;
    [SerializeField] Transform shootPos;

    public Boss_State state = Boss_State.IDLE;
    Vector3 PlayerDir;
    float countDownTimer;
    public bool isShooting;

    
    
    // Start is called before the first frame update
    void Start()
    {

        gameManager.instance.enemyIncrement();
        bossAnim = GetComponent<Animator>();
        /*shockWavePs = transform.Find("Shockwave").GetChild(0).GetComponent<ParticleSystem>();*/
    }

    // Update is called once per frame
    void Update()
    {

        bossAgent.SetDestination(gameManager.instance.player.transform.position);

        canSeePlayer();

        PlayerDir = gameManager.instance.player.transform.position - transform.position;
        bossAnim.SetFloat("Speed", Mathf.Lerp(bossAnim.GetFloat("Speed"), bossAgent.velocity.normalized.magnitude, Time.deltaTime * 4));
        updateState();
    }

    void facePlayer()
    {
        PlayerDir.y = 0;

        Quaternion rotation = Quaternion.LookRotation(PlayerDir);

        transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * turnSpeed);
    }
    public void takeDamage(int dmg)
    {
        HP -= dmg;
        StartCoroutine(flashDamage());

        if (HP <= 0)
        {
            gameManager.instance.enemyDecrement();
            Destroy(gameObject);
        }
    }
    IEnumerator flashDamage()
    {
        mesh.material.color = Color.red;

        yield return new WaitForSeconds(0.1f);

        mesh.material.color = Color.white;
    }
    IEnumerator shoot()
    {
        isShooting = true;

        Instantiate(bullet, shootPos.position, transform.rotation);
        yield return new WaitForSeconds(shootRate);

        isShooting = false;
    }

    void canSeePlayer()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position + transform.up, PlayerDir, out hit))
        {
            Debug.DrawRay(transform.position + transform.up, PlayerDir);
            if (hit.collider.CompareTag("Player"))
            {
                facePlayer();

                if (!isShooting)
                {
                    StartCoroutine(shoot());
                }
            }
        }
    }
    IEnumerator stompShockWave()
    {
        bossAnim.SetTrigger("Shock Wave");

        yield return new WaitForSeconds(shockWaveAnimationTime);

        shockWavePs.Play();
        damageFromshockWave();

    }
    void updateState()
    {
        distanceToPlayer = Vector3.Distance(gameManager.instance.player.transform.position, transform.position);
        Debug.Log("Distance to player " + distanceToPlayer + "State " + state);

        switch (state)
        {
            case Boss_State.IDLE:
                {
                    if (distanceToPlayer < distanceForStomp)
                    {
                        state = Boss_State.STOMP;
                    }
                }
                break;
            case Boss_State.STOMP:
                {
                    countDownTimer = timeBetweenStomps;
                    StartCoroutine(stompShockWave());
                    state = Boss_State.WAITING;
                }
                break;
            case Boss_State.WAITING:
                {
                    countDownTimer -= Time.deltaTime;
                    if (countDownTimer <= 0)
                    {
                        state = Boss_State.IDLE;
                    }
                }
                break;
        }

    }
    void damageFromshockWave()
    {
        if (gameManager.instance.playerScript.touchGround() == true)
        {
            gameManager.instance.playerScript.takeDamage(ShockWaveDamage);
        }
    }
}
