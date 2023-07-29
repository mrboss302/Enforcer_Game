using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public int playerID = 1;
    public CharacterController characterController { get; set; }
    [Header("+++ SETUP +++")]
    public float slowMotion = 0.3f;
    public Transform characterHolder;
    public Animator anim;
    public GameObject bloodFX;

    [Header("+++ MOVE +++")]
    public float moveSpeed = 5;
    public float gravity = -35f;
    public float jumpHeightMax = 2;

    public LayerMask layerAsGround;
    [ReadOnly] public Vector2 velocity;
    [ReadOnly] public bool isGrounded = false;

    bool isPlaying = false;
    [HideInInspector] public bool isDead = false;

    [Header("+++ CAMERA +++")]
    public GameObject vcam1;
    public GameObject vcam2LookRight, vcam2LookLeft, vcam2JumpLeft, vcam2JumpRight, vcam2SlideLeft, vcam2SlideRight, vcam3, vcamFinish;

    [Header("---AUDIO---")]
    public AudioClip soundJump;
    public AudioClip soundSlide;
    public AudioClip[] soundDie;

    [HideInInspector] public float horizontalInput = 1;
    float velocityXSmoothing;
    [HideInInspector] public PlayerGun playerGun;

    private void Awake()
    {
        transform.forward = Vector3.forward;     //look along Z Axis
        bloodFX.SetActive(false);
        playerGun = GetComponent<PlayerGun>();
        characterController = GetComponent<CharacterController>();
        if (anim == null)        //if null then try to find the own animator
            anim = GetComponent<Animator>();

        vcam1.SetActive(true);
        vcam2JumpLeft.SetActive(false);
        vcam2JumpRight.SetActive(false);
        vcam2LookRight.SetActive(false);
        vcam2LookLeft.SetActive(false);
        vcam2SlideLeft.SetActive(false);
        vcam2SlideRight.SetActive(false);
        vcam3.SetActive(false);
        vcamFinish.SetActive(false);
    }

    public void Shoot()
    {
        if (playerGun.Shot())
            anim.SetTrigger("shoot");
    }

    public void Play()
    {
        isPlaying = true;
    }

    [HideInInspector] public ActionTrigger currentTrigger;
    public void MotionAction(ActionTrigger action)
    {
        currentTrigger = action;
        Time.timeScale = slowMotion;
        Time.fixedDeltaTime = 0.02F * Time.timeScale;
        
        SetVCam(2);
        UIBulletHolder.Instance.Setup();
        playerGun.Init(currentTrigger.allowBullet);
        Invoke("AllowShooting", 0.25f);
    }

    void AllowShooting()
    {
        playerGun.AllowShooting(true);
    }

    int camNo = 1;

    void SetVCam(int no)
    {
        camNo = no;
        if (camNo == 2)
        {
            switch (currentTrigger.motionAction)
            {
                case ShootingActions.LookRight:
                    Look();
                    break;
                case ShootingActions.LookLeft:
                    Look();
                    break;
                case ShootingActions.JumpRight:
                    //characterHolder.localRotation = Quaternion.Euler(0, 75, 30);
                    Jump();
                    break;
                case ShootingActions.JumpLeft:
                    //characterHolder.localRotation = Quaternion.Euler(0, -75, -30);
                    Jump();
                    break;
                case ShootingActions.SlidingRight:
                    Slide();
                    break;
                case ShootingActions.SlidingLeft:
                    Slide();
                    break;
            }
        }
        else if (camNo == 1)
        {
            currentTrigger = null;
            playerGun.AllowShooting(false);
            characterHolder.localRotation = Quaternion.Euler(0, 0, 0);
        }

        if (camNo == 3)
        {
            currentTrigger = null;
            SwitchCam();
        }
        else
        {
            Invoke("SwitchCam", camNo == 2 ? 0.1f : 0.05f);
            Invoke("SetCursor", camNo == 2 ? 0.2f : 0f);
        }
    }

    void SwitchCam()
    {
        vcam1.SetActive(camNo == 1);

        if (camNo == 2)
        {
            switch (currentTrigger.motionAction)
            {
                case ShootingActions.LookRight:
                    vcam2LookRight.SetActive(true);
                    break;
                case ShootingActions.LookLeft:
                    vcam2LookLeft.SetActive(true);
                    break;
                case ShootingActions.JumpRight:
                    vcam2JumpRight.SetActive(true);
                    break;
                case ShootingActions.JumpLeft:
                    vcam2JumpLeft.SetActive(true);
                    break;
                case ShootingActions.SlidingRight:
                    vcam2SlideRight.SetActive(true);
                    break;
                case ShootingActions.SlidingLeft:
                    vcam2SlideLeft.SetActive(true);
                    break;
            }
        }
        else
        {
            vcam2LookRight.SetActive(false);
            vcam2LookLeft.SetActive(false);
            vcam2JumpLeft.SetActive(false);
            vcam2JumpRight.SetActive(false);
            vcam2SlideLeft.SetActive(false);
            vcam2SlideRight.SetActive(false);
        }

        vcam3.SetActive(camNo == 3);
    }

    void Update()
    {
        Move();

        HandleAnimation();
    }

    private void Move()
    {
        transform.forward = Vector3.right;

        float targetVelocityX = moveSpeed * horizontalInput;

        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, 0.1f);

        CheckGround();

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = 0;
        }
        else
            velocity.y += gravity * Time.deltaTime;     //add gravity

        if (!isPlaying || isDead)
            velocity.x = 0;

        Vector3 finalVelocity = velocity;
        if (isGrounded && groundHit.normal != Vector3.up)        //calulating new speed on slope
            GetSlopeVelocity(ref finalVelocity);

        //CheckLimitPos(ref finalVelocity);
        characterController.Move(finalVelocity * Time.deltaTime);
    }

    void GetSlopeVelocity(ref Vector3 vel)
    {
        var crossSlope = Vector3.Cross(groundHit.normal, Vector3.forward);
        vel = vel.x * crossSlope;
        Debug.DrawRay(transform.position, crossSlope * 10);
    }

    void HandleAnimation()
    {
        anim.SetFloat("speed", Mathf.Abs(velocity.x));
        anim.SetBool("isGrounded", isGrounded);
        anim.SetFloat("height speed", velocity.y);
        anim.SetBool("isSliding", isSliding);
        anim.SetBool("isDead", isDead);
    }

    public void Die()
    {
        if (isDead)
            return;

        bloodFX.SetActive(true);
        StopAllCoroutines();
        SetVCam(3);
        SoundManager.PlaySfx(soundDie[Random.Range(0,soundDie.Length)]);
        isDead = true;
        anim.SetTrigger("die");

        GameManager.Instance.GameOver();
    }

    public void Gameover()
    {
        StopAllCoroutines();

        playerGun.AllowShooting(false);
        anim.SetBool("looking", false);
        anim.applyRootMotion = true;
        isSliding = false;
        isLookingSide = false;
        velocity.x = 0;
        isPlaying = false;
    }

    [ReadOnly] public bool isSliding = false;

    public void Slide()
    {
        StartCoroutine(SlidingCo());
    }

    IEnumerator SlidingCo()
    {
        isSliding = true;

        if(currentTrigger.motionAction == ShootingActions.SlidingRight)
            characterHolder.transform.localScale = new Vector3(-1, 1, 1);

        //anim.SetBool("isFacingRight", currentTrigger.motionAction == ShootingActions.SlidingRight);

        while (Vector3.Distance(currentTrigger.transform.position, transform.position) < currentTrigger.activeDistance)
        {
            yield return null;
        }

        if (prepareToDie)
            yield break;

        isSliding = false;
        if (currentTrigger.motionAction == ShootingActions.SlidingRight)
            characterHolder.transform.localScale = new Vector3(1, 1, 1);

        if (!isDead)
        {
            Time.timeScale = 1;
            Time.fixedDeltaTime = 0.02F;
            SetVCam(1);
        }
    }

    bool prepareToDie = false;
    public void PrepareToDie()
    {
        prepareToDie = true;
        velocity = Vector3.zero;
    }

    RaycastHit groundHit;
    void CheckGround()
    {
        if (prepareToDie)
            return;

        isGrounded = false;
        if (velocity.y > 0.1f)
            return;

        if (Physics.SphereCast(transform.position + Vector3.up * 1, characterController.radius * 0.9f, Vector3.down, out groundHit, 1f, layerAsGround))
        {
            float distance = transform.position.y - groundHit.point.y;
            if (distance <= (characterController.skinWidth + 0.1f))
                isGrounded = true;
        }
    }

    public void Jump()
    {
        if (GameManager.Instance.gameState != GameManager.GameState.Playing)
            return;
        if (isGrounded)
        {
            StartCoroutine(JumpCo());
        }
    }

    IEnumerator JumpCo()
    {
        SoundManager.PlaySfx(soundJump);

        isGrounded = false;
        var _height = jumpHeightMax;
        velocity.y = Mathf.Sqrt(_height * -2 * gravity);
        velocity.x = characterController.velocity.x;
        characterController.Move(velocity * Time.deltaTime);

        Vector3 targetRotation;
        if(currentTrigger.motionAction == ShootingActions.JumpRight)
            targetRotation = new Vector3(0, 75, 30);
        else
            targetRotation = new Vector3(0, -75, -30);

        float a = 0;
        float speed = 2;
        while (a < 1)
        {
            a += (Time.deltaTime / Time.timeScale)* speed;
            a = Mathf.Clamp01(a);
            var rot = Vector3.Lerp(Vector3.zero, targetRotation, a);
            characterHolder.localRotation = Quaternion.Euler(rot);
            yield return null;
        }

        yield return new WaitForSeconds(1);

        while (!isGrounded) { yield return null; }

        Time.timeScale = 1;
        //Time.fixedDeltaTime = 0.02F;
        SetVCam(1);
    }

    [ReadOnly] public bool isLookingSide = false;
    void Look()
    {
        StartCoroutine(LookCo());
    }

    IEnumerator LookCo()
    {
        isLookingSide = true;
        anim.SetBool("looking", true);

        if(currentTrigger.motionAction == ShootingActions.LookRight)
            characterHolder.transform.localScale = new Vector3(-1, 1, 1);

        while (Vector3.Distance(currentTrigger.transform.position, transform.position) < currentTrigger.activeDistance)
        {
            yield return null;
        }

        if (prepareToDie)
            yield break;

        anim.SetBool("looking", false);
        isLookingSide = false;
        if (currentTrigger.motionAction == ShootingActions.LookRight)
            characterHolder.transform.localScale = new Vector3(1, 1, 1);
        if (!isDead)
        {
            Time.timeScale = 1;
            Time.fixedDeltaTime = 0.02F;
            SetVCam(1);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Finish"))
        {
            GameManager.Instance.FinishGame();
            vcamFinish.SetActive(true);
            isPlaying = false;
        }
    }
}
