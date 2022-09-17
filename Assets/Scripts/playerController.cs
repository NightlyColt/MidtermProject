using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerController : MonoBehaviour, IDamageable
{
    [Header("----- Player Controller -----")]
    [SerializeField] CharacterController controller;

    [Header("----- Player Attributes -----")]
    [Range(1, 100)][SerializeField] int HP;
    [Range(1, 100)][SerializeField] float playerSpeed;
    [Range(1, 10)][SerializeField] float sprintMultiplier;
    [Range(1, 10)][SerializeField] float jumpHeight;
    [Range(1, 25)][SerializeField] float gravityValue;
    [Range(1, 10)][SerializeField] int jumpsMax;

    [Header("----- Gun Stats -----")]
    [SerializeField] float shootRate;
    [SerializeField] int shootDist;
    [SerializeField] int shootDamage;
    [SerializeField] List<gunStats> gunStat = new List<gunStats>();
    [SerializeField] GameObject gunModel;

    [Header("----- Audio -----")]
    [SerializeField] AudioSource aud;
    [SerializeField] AudioClip[] playerDamage;
    [Range(0, 1)][SerializeField] float playerDamageVol;
    [SerializeField] AudioClip[] playerJumpSound;
    [Range(0, 1)][SerializeField] float playerJumpSoundVol;
    [SerializeField] AudioClip[] playerFootStepSound;
    [Range(0, 1)][SerializeField] float playerFootStepSoundVol;
    [Range(0, 1)][SerializeField] float gunShotSoundVol;


    int HPOrig;
    int timesJumped;
    Vector3 playerVelocity;
    Vector3 move;
    bool isShooting;
    int selectedGun;
    float playerSpeedOrig;
    bool isSprinting;
    bool playingFootsteps;

    private void Start()
    {
        playerSpeedOrig = playerSpeed;
        HPOrig = HP;
        respawn();
    }

    void Update()
    {
        if(!gameManager.instance.isPaused)
        {
            movement();

            sprint();

            StartCoroutine(footSteps());

            gunSelect();

            StartCoroutine(shoot());
        }
        
    }

    void movement()
    {
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
            aud.PlayOneShot(playerJumpSound[Random.Range(0, playerJumpSound.Length)], playerJumpSoundVol);
        }

        playerVelocity.y -= gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);
    }

    void sprint()
    {
        if (Input.GetButtonDown("Sprint"))
        {
            isSprinting = true;

            playerSpeed *= sprintMultiplier;
        }
        else if (Input.GetButtonUp("Sprint"))
        {
            isSprinting = false;

            playerSpeed = playerSpeedOrig;
        }
    }

    IEnumerator footSteps()
    {
        if (!playingFootsteps && controller.isGrounded && move.normalized.magnitude > 0.3f) // move.normalized.magnitude checks for player speed in any direction, .3 is a good delay to start steps
        {
            playingFootsteps = true;
            aud.PlayOneShot(playerFootStepSound[Random.Range(0, playerFootStepSound.Length)], playerFootStepSoundVol);

            if (isSprinting)
            {
                yield return new WaitForSeconds(0.3f);
            }
            else
            {
                yield return new WaitForSeconds(0.4f);
            }

            playingFootsteps = false;
        }
    }

    void gunSelect()
    {
        if (gunStat.Count > 1)
        {
            // if greater than zero go up
            if (Input.GetAxis("Mouse ScrollWheel") > 0 && selectedGun < gunStat.Count - 1)
            {
                selectedGun++;
                shootRate = gunStat[selectedGun].shootRate;
                shootDamage = gunStat[selectedGun].shootDamage;
                shootDist = gunStat[selectedGun].shootDist;

                //swap
                gunModel.GetComponent<MeshFilter>().sharedMesh = gunStat[selectedGun].model.GetComponent<MeshFilter>().sharedMesh;
                gunModel.GetComponent<MeshRenderer>().sharedMaterial = gunStat[selectedGun].model.GetComponent<MeshRenderer>().sharedMaterial;
            }// if less than zero go down
            else if (Input.GetAxis("Mouse ScrollWheel") < 0 && selectedGun > 0)
            {
                selectedGun--;
                shootRate = gunStat[selectedGun].shootRate;
                shootDamage = gunStat[selectedGun].shootDamage;
                shootDist = gunStat[selectedGun].shootDist;

                //swap
                gunModel.GetComponent<MeshFilter>().sharedMesh = gunStat[selectedGun].model.GetComponent<MeshFilter>().sharedMesh;
                gunModel.GetComponent<MeshRenderer>().sharedMaterial = gunStat[selectedGun].model.GetComponent<MeshRenderer>().sharedMaterial;
            }
        }
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

    /// <summary>
    /// Sets stats for currently equipped weapon
    /// </summary>
    /// <param name="stats"></param>
    public void gunPickup(gunStats stats)
    {
        shootRate = stats.shootRate;
        shootDamage = stats.shootDamage;
        shootDist = stats.shootDist;

        gunModel.GetComponent<MeshFilter>().sharedMesh = stats.model.GetComponent<MeshFilter>().sharedMesh; //hookups for mesh of model
        gunModel.GetComponent<MeshRenderer>().sharedMaterial = stats.model.GetComponent<MeshRenderer>().sharedMaterial; //hookups for material of model

        gunStat.Add(stats);
        selectedGun = gunStat.Count - 1;
    }


}