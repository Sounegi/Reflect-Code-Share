using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class PlayerControlScript : MonoBehaviour
{
    private static PlayerControlScript instance;

    #region Player Components
    [Header("Player Components")]
    [HideInInspector][SerializeField] private Rigidbody2D rb;
    [HideInInspector][SerializeField] private SpriteRenderer playerSprite;
    [HideInInspector][SerializeField] private Animator animator;
    #endregion
    
    #region Movement Variables
    [Header("Movement Variables")]
    [SerializeField] public bool canMove;
    [SerializeField] public float movementspeed = 3f;
    [HideInInspector][SerializeField] private Vector2 movementInput;
    [HideInInspector][SerializeField] private Vector2 dashDirection;
    #endregion
    
    #region Dash Variables
    [Header("Dash Variables")]
    [SerializeField] DashManager dashManager;
    [SerializeField] public float dashSpeed = 5f;
    [SerializeField] public int dashID = 1;
    [SerializeField] public float dashDuration = 0.5f;
    [SerializeField] public float dashCastTime = 1.5f;
    [SerializeField] public float dashCooldown = 10f;
    [SerializeField] public int dashCounter;
    [SerializeField] public int dashMaxCharge;
    [SerializeField] public float dashRefresh;
    [HideInInspector][SerializeField] public bool canDash = true;
    [HideInInspector][SerializeField] public bool currentlyDashing;
    [HideInInspector][SerializeField] public ParticleSystem BlinkParticle;
    [SerializeField] public float blinkRange = 5f;
    public UnityEvent playerDashEvent;
    #endregion

    #region Reflect Variables
    [Header("Reflect Variables")]
    [SerializeField] public Transform spawnPoint;
    [SerializeField] public GameObject reflector;
    [SerializeField] public bool mirrorRotate;
    [HideInInspector][SerializeField] GameObject shield;
    [HideInInspector][SerializeField] bool isReflecting = false;
    [HideInInspector][SerializeField] float orbitRadius = 3f;
    [HideInInspector][SerializeField] public int numShields = 4;
    [HideInInspector][SerializeField] float rotationSpeed = 100f;
    [HideInInspector][SerializeField] public float attackRange = 0f;
    [HideInInspector] [SerializeField] private List<float> angle_pos = new List<float>();
    [HideInInspector] [SerializeField] private List<GameObject> rotateshields = new List<GameObject>();


    #endregion

    #region Heal/Vial Action Variables
    [Header("Vial Variables")]
    public HealActionManager healManager;
    public int HealID =1;
    public float aoeHealRadius = 3;
    public float aoeHealTime = 100f;
    public float aoeHealTotal = 10f;
    public float normalHeal = 5f;

    public int ReflectShieldHP = 0;
    public float DisintegrationDuration = 10f;
    #endregion

    private bool isSprinting;
    public float sprintspeed = 4.25f;

    GameObject bullet_holder;

    private float reflectionTimer = 0f;
    private float reflectionInterval = 0.1f; // 1 second interval
    public float stamina_regen;
    public float stamina_decay;
    private bool reflect_shield_freeze;
    private bool reflect_force_stop;
    public UnityEvent<Vector3> playerHealEvent;
    [SerializeField] public AudioClip normal_healing_audio_clip;
    [SerializeField] public AudioClip aoe_healing_audio_clip;
    [SerializeField] public AudioClip special_healing_audio_clip;
    [SerializeField] public AudioClip dashing_audio_clip;
    [SerializeField] public AudioClip blinking_audio_clip;
    [SerializeField] public AudioClip walking_audio_clip;
    [SerializeField] public AudioClip shield_summon_audio_clip;
    [SerializeField] public AudioClip shield_reflect_audio_clip;
    [SerializeField] public AudioClip shield_break_audio_clip;
    [SerializeField] public AudioClip bulletCapturedSFX;
    [SerializeField] public AudioClip player_hurt_audio_clip;
    [SerializeField] public AudioClip player_die_audio_clip;
    [SerializeField] public AudioClip stamina_out_audio_clip;
    // 0 = normal heal
    // 1 = aoe heal
    // 2 = special heal
    // 3 = dashing effect
    // 4 = walking

    public AudioSource shields_up;
    public AudioSource NormalPitchSource;
    public AudioSource WalkSpecificSource;

    private void OnEnable()
    {
        DontDestroyOnLoad(gameObject);
    }

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public static PlayerControlScript GetInstance()
    {
        return instance;
    }

    public Rigidbody2D GetRigidBody()
    {
        return rb;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerSprite = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        dashManager = GetComponent<DashManager>();
        BlinkParticle = GetComponent<ParticleSystem>();
        BlinkParticle.Stop();

        rb.gravityScale = 0f;
        isReflecting = false;
        HealID = 1;
        dashID = 1;
        // shield_time = max_shield_time;
        // shield_cooldown = 0f;
        mirrorRotate = false;
        numShields = 0;
        canDash = true;
        reflect_shield_freeze = false;
        reflect_force_stop = false;

        dashMaxCharge = 1;
        dashCounter = 1;
        dashRefresh = 0;
        dashCastTime = 1.5f;

        aoeHealRadius = 3;
        aoeHealTime = 10f;
        aoeHealTotal = 10f;

        stamina_decay = -1f;
        stamina_regen = 1f;
        ReflectShieldHP = 1;
        //CreateOrbitingShields();
    }

    void Update()
    {
        if (animator.GetBool("isDead"))
            return;
        ManageSprite();
        if(!reflect_shield_freeze)
            ManageMovement();
        ManageShieldAction();
    }

    void ManageMovement()
    {
        if (currentlyDashing)
        {
            return;
        }
        shields_up.pitch = 1.4f;
        // if (isSprinting)
        // {
        //     rb.velocity = (movementInput) * sprintspeed;
        // }
        // else
        // {
        //     rb.velocity = (movementInput) * movementspeed;
        // }
        dashRefresh += Time.deltaTime;

        if(dashRefresh >= dashCooldown)
        {
            dashRefresh = 0f;
            if (dashCounter < dashMaxCharge)
            {
                dashCounter++;
                Debug.Log("dash Restored : " + dashCounter);
            }
        }

        rb.velocity = (movementInput) * movementspeed;

    }

    void ManageSprite()
    {
        if (rb.velocity == Vector2.zero)
        {
            animator.SetBool("isWalking", false);
        }
        else
        {
            animator.SetBool("isWalking", true);
        }

        if (movementInput.x > 0f)
        {
            playerSprite.flipX = false;
        }
        else if (movementInput.x < 0f)
        {
            playerSprite.flipX = true;
        }

        if (currentlyDashing)
        {
            animator.SetBool("isDashing", true);
            return;
        }
        animator.SetBool("isDashing", false);
        //if (DashAbility.Ability_Status == AbilityStatus.ACTIVE) return;
        // if (isSprinting)
        // {
        //     rb.velocity = (movementInput) * sprintspeed;
        // }
        // else
        // {
        //     rb.velocity = (movementInput) * movementspeed;
        // }

    }

    void ManageShieldAction()
    {
        //Debug.Log("current stamina is: " + PlayerManager.GetInstance().GetStamina());
        RotateShields();

        reflectionTimer -= Time.deltaTime;

        if (reflectionTimer <= 0f)
        {
            reflectionTimer = reflectionInterval;
            if (isReflecting)
                PlayerManager.GetInstance().AdjustStaminaPoint(stamina_decay);
            else
                PlayerManager.GetInstance().AdjustStaminaPoint(stamina_regen);
        }

        if (isReflecting)
        {
            Reflect();
            if (PlayerManager.GetInstance().currentStamina <= 0f)
            {
                isReflecting = false;
                Destroy(shield);
            }
        }
    }

    void Reflect()
    {
        Vector2 clickdirection = CameraInstance.GetInstance().GetCamera().ScreenToWorldPoint(Mouse.current.position.ReadValue()) - rb.transform.position;
        clickdirection = clickdirection.normalized;

        float angle = Mathf.Atan2(clickdirection.y, clickdirection.x);
        angle = (angle + 2 * Mathf.PI) % (2 * Mathf.PI);
        angle = angle * Mathf.Rad2Deg;

        Vector2 spawnposition = (Vector2)spawnPoint.position + clickdirection * attackRange;
    
        if (shield == null)
        {
            shield = Instantiate(reflector, spawnposition, Quaternion.Euler(0, 0, angle - 90), gameObject.transform);
            Physics2D.IgnoreCollision(GetComponent<Collider2D>(), shield.GetComponent<Collider2D>(), true);
        }
        else
        {
            shield.transform.position = spawnposition;
            shield.transform.rotation = Quaternion.Euler(0, 0, angle - 90);
        }
    }
    public void CreateOrbitingShields()
    {
        if (rotateshields.Count == numShields) return;

        if (isReflecting) return;

        angle_pos.Clear();
        DestroyRotating();

        for (int i = 0; i < numShields; i++)
        {
            float angle = i * (360f / numShields);
            Vector2 orbitPosition = rb.transform.position + new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad)) * orbitRadius;
            GameObject generated = Instantiate(reflector, orbitPosition, Quaternion.identity, gameObject.transform);
            generated.tag = "RotatingReflect";
            angle_pos.Add(angle);
            rotateshields.Add(generated);
        }
    }
    void RotateShields()
    {
        foreach (GameObject singular_shield in rotateshields)
        {
            GameObject kaca = singular_shield;
            if (kaca != null)
            {
                Vector2 shieldPosition = (Vector2)kaca.transform.position - (Vector2)rb.transform.position;
                float angle = Mathf.Atan2(shieldPosition.y, shieldPosition.x) * Mathf.Rad2Deg;

                angle += rotationSpeed * Time.deltaTime;
                kaca.transform.position = (Vector2)rb.transform.position + new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad)) * orbitRadius;
                kaca.transform.rotation = Quaternion.Euler(0, 0, angle - 90);
            }
        }
    }

    
    void DestroyRotating()
    {
        foreach (GameObject singular_shield in rotateshields)
        {
            if (singular_shield != null)
                Destroy(singular_shield);
        }
        rotateshields.Clear();
    }
    
    public void OnMove(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (!WalkSpecificSource.isPlaying)
            {
                WalkSpecificSource.Play();
            }
            movementInput = context.ReadValue<Vector2>();
            movementInput.Normalize();
        }
        if (context.canceled)
        {
            WalkSpecificSource.Stop();
            movementInput = context.ReadValue<Vector2>();
            movementInput.Normalize();
        }

    }

    private IEnumerator PlayAnimationAndWait(InputAction.CallbackContext context)
    {
        Debug.Log("animator called");
        reflect_shield_freeze = true;
        shields_up.PlayOneShot(shield_summon_audio_clip);
        animator.Play("skill1", -1, 0f);
        yield return null;
        // Wait until the current animation is finished
        while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
        {
            yield return null;
        }
        reflect_shield_freeze = false;
        // Animation has finished playing, proceed with the remaining code
        if (reflect_force_stop)
        {
            reflect_force_stop = false;
            yield break;
        }
        PlayerManager.GetInstance().AdjustStaminaPoint(-5);
        isReflecting = true;
        float remaining_time = shields_up.clip.length - shields_up.time;
    }


    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.performed && PlayerManager.GetInstance().currentStamina > 0)// && Sword.GetInstance() == null)
        {
            isReflecting = false;
            if (!animator.GetBool("isDead") && !animator.GetCurrentAnimatorStateInfo(0).IsName("skill1"))
            {
                StartCoroutine(PlayAnimationAndWait(context));
            }
            //PlayerManager.GetInstance().AdjustStaminaPoint(-5);
            //isReflecting = true;
        }
        else if (context.canceled)
        {
            if(reflect_shield_freeze)
                reflect_force_stop = true;
            if (shield != null)
            {
                Destroy(shield);
                shield = null;
            }
            isReflecting = false;
        }
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        if (context.performed && canDash)
        {
            // Debug.Log("Dashing");
            dashManager.StartDash(this);
            playerDashEvent.Invoke();
        }
    }

    public void OnHeal(InputAction.CallbackContext context)
    {
        if(context.performed && PlayerManager.GetInstance().m_canHeal)
        {
            // Debug.Log("Healing");
            healManager.StartHeal(this);
            playerHealEvent.Invoke(transform.position);
        }
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if(context.performed)
        {
            Exit.GetInstance().ChangeScene();
        }
    }

    /*
    public void FindBullet()
    {
        float steal_distance = 5f;
        float thickness = 5f;
        Vector2 boxSize = new Vector2(thickness, steal_distance);

        Collider2D[] hitColliders = Physics2D.OverlapBoxAll(transform.position, boxSize, 0f);//, Vector2.SignedAngle(Vector2.right, clickdirection));
        //Debug.DrawRay(rb.transform.position, clickdirection.normalized * steal_distance, Color.red, 1f);
        foreach (Collider2D collider in hitColliders)
        {
            float distanceToPlayer = Vector2.Distance(collider.transform.position, rb.transform.position);
            if (distanceToPlayer > steal_distance) continue;

            // Check the tag of the object
            if (collider.CompareTag("Bullet"))
            {
                // Call a method to steal the projectile
                StealProjectile(collider.gameObject);
                break; // Break the loop after stealing the first projectile (adjust if needed)
            }
        }
    }

    public void StealProjectile(GameObject bullet)
    {
        bullet_holder = Instantiate(bullet, transform.position, Quaternion.identity);
        bullet_holder.SetActive(false);

        BaseBulletBehavior bulletbehav = bullet_holder.GetComponent<BaseBulletBehavior>();
        bulletbehav.PlayerForceOwnership();

        Destroy(bullet);
    }
    */
    public void ShootStolenProjectile()
    {
        if (PlayerManager.GetInstance().stolen_bullet_holder != null)
        {
            Debug.Log("peluru lagi dibuang");
            float bulletSpeed = 15f;
            //BaseBulletBehavior bulletbehav = PlayerManager.GetInstance().stolen_bullet_holder.GetComponent<BaseBulletBehavior>();
            //string get_bullet_type = bulletbehav.GetBulletType();
            PlayerManager.GetInstance().stolen_bullet_holder.SetActive(true);
            PlayerManager.GetInstance().stolen_bullet_holder.transform.position = transform.position;
            // Make the projectile reappear in the game view
            GameObject newProjectile = PlayerManager.GetInstance().stolen_bullet_holder;
            newProjectile.GetComponent<BaseBulletBehavior>().Activate();
            //ActivateBullet(newProjectile, get_bullet_type);

            Vector2 direction = (CameraInstance.GetInstance().GetCamera().ScreenToWorldPoint(Mouse.current.position.ReadValue()) - rb.transform.position).normalized;
            newProjectile.GetComponent<Rigidbody2D>().velocity = direction * bulletSpeed;
            newProjectile.GetComponent<Transform>().right = direction; //work now but don't know why
            PlayerManager.GetInstance().stolen_bullet_holder = null;
        }
    }

    /*
    public void ActivateBullet(GameObject newProjectile, string bullet_type)
    {
        switch (bullet_type)
        {
            case "ExplosiveBullet":
                ExplosiveBullet explode = newProjectile.GetComponent<ExplosiveBullet>();
                CircleCollider2D explode_collid = newProjectile.GetComponent<CircleCollider2D>();
                explode.PlayerForceOwnership();
                explode.enabled = true;
                explode_collid.enabled = true;
                break;

            case "BouncingBullet":
                BouncingBullet bounce = newProjectile.GetComponent<BouncingBullet>();
                CircleCollider2D collid = newProjectile.GetComponent<CircleCollider2D>();
                bounce.PlayerForceOwnership();
                bounce.enabled = true;
                collid.enabled = true;
                break;

            case "NormalBullet":
                NormalBullet normal = newProjectile.GetComponent<NormalBullet>();
                CircleCollider2D normal_collid = newProjectile.GetComponent<CircleCollider2D>();
                normal.PlayerForceOwnership();
                normal.enabled = true;
                normal_collid.enabled = true;
                break;

            case "PiercingBullet":
                PiercingBullet pierce = newProjectile.GetComponent<PiercingBullet>();
                EdgeCollider2D pierce_collid = newProjectile.GetComponent<EdgeCollider2D>();
                pierce.PlayerForceOwnership();
                pierce.enabled = true;
                pierce_collid.enabled = true;
                break;

            case "SpreadBullet":
                SpreadBullet spread = newProjectile.GetComponent<SpreadBullet>();
                CircleCollider2D spread_collid = newProjectile.GetComponent<CircleCollider2D>();
                spread.PlayerForceOwnership();
                spread.enabled = true;
                spread_collid.enabled = true;
                break;
        }
        return;
    } */

    public void OnSteal(InputAction.CallbackContext context)
    {
        if (context.performed && PlayerManager.GetInstance().stolen_bullet_holder != null)
        {
            ShootStolenProjectile();
            if (bullet_holder == null)
                Debug.Log("bullet shot");
        }
    }
}
