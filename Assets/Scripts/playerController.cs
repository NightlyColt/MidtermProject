using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerController : MonoBehaviour, IDamageable
{
    [SerializeField] CharacterController controller;
    [SerializeField] float gravityValue;

    [Header("----- Player Stats -----")]
    [SerializeField] float playerSpeed;
    [SerializeField] float jumpHeight;
    [SerializeField] int jumpsMax;
    [SerializeField] int HP;

    [Header("----- Gun Stats -----")]
    [SerializeField] float shootRate;
    [SerializeField] int shootDist;
    [SerializeField] int shootDamage;
    [SerializeField] int ammoCount;
    [SerializeField] int magSize;
    [SerializeField] GameObject gunModel;
    [SerializeField] string gunName;
    [SerializeField] List<gunStats> gunStats;

    int HPOrig;
    int timesJumped;
    private Vector3 playerVelocity;
    Vector3 move;
    bool isShooting;
    public bool canMove;
    public float playerCollider;
    public int selectedGun;
    public string currentGun;
    public string previousGun;



    private void Start()
    {
        playerCollider = GetComponent<CapsuleCollider>().bounds.extents.y;
        canMove = true;
        HPOrig = HP;
        respawn();
    }

    void Update()
    {
        if(!gameManager.instance.isPaused && canMove)
        {            
            movement();
            StartCoroutine(shoot());
            if (transform.position.y < 0)
            {
                transform.position = gameManager.instance.playerSpawnPos.transform.position;
            }
        }
        
    }
    public bool touchGround()
    {
        bool isGrounded = Physics.Raycast(transform.position, -Vector3.up, playerCollider + 0.1f);
        return isGrounded;
    }

    void movement()
    {
        canMove = true;
        if (controller.isGrounded && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
            timesJumped = 0;
        }

        // First person movement
        move = (transform.right * Input.GetAxis("Horizontal") + transform.forward * Input.GetAxis("Vertical"));
        controller.Move(move * Time.deltaTime * playerSpeed);


        if (Input.GetButtonDown("Jump") && timesJumped < jumpsMax)
        {
            playerVelocity.y = jumpHeight;
            timesJumped++;
        }

        playerVelocity.y -= gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);
    }

    IEnumerator shoot()
    {
        if(!isShooting && Input.GetButton("Shoot"))
        {
            isShooting = true;

            RaycastHit hit;
            if(Physics.Raycast(Camera.main.ViewportPointToRay(new Vector2(0.5f, 0.5f)), out hit, shootDist))
            {
                // For Micah: If you need to test this with your scene change IDamageable to MyIDamageable
                // For Micah: Change back to IDamgeable class before committing and pushing
                if (hit.collider.GetComponent<IDamageable>() != null)
                {
                    hit.collider.GetComponent<IDamageable>().takeDamage(shootDamage);
                }
            }

            yield return new WaitForSeconds(shootRate);
            isShooting = false;
        }

    }

    public void respawn()
    {
        controller.enabled = false;
        HP = HPOrig;
        updatePlayerHP();
        transform.position = gameManager.instance.playerSpawnPos.transform.position;
        gameManager.instance.cursorUnlockUnpause();
        gameManager.instance.isPaused = false;
        gameManager.instance.playerDead = false;
        controller.enabled = true;
    }

    public void updatePlayerHP()
    {
        gameManager.instance.HPBar.fillAmount = (float)HP / (float)HPOrig;
    }

    public void takeDamage(int dmg)
    {
        HP -= dmg;
        updatePlayerHP();

        StartCoroutine(damageFlash());

        if (HP <= 0)
        {
            gameManager.instance.playerIsDead();
        }
    }

    IEnumerator damageFlash()
    {
        gameManager.instance.playerDamage.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        gameManager.instance.playerDamage.SetActive(false);
    }

    public void gunPickup(gunStats stats)
    {
        shootRate = stats.shootRate;
        shootDist = stats.shootDist;
        shootDamage = stats.shootDamage;
        ammoCount = stats.ammoCount;
        magSize = stats.magSize;
        gunName = stats.gunName;

        gunStats.Add(stats);
    }
    void selectGun()
    {

    }
}