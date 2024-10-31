using UnityEngine;
using UnityEngine.UI;

public class JoystickPlayerController : MonoBehaviour
{
    private float speed = 5f;
    public FixedJoystick joystick;
    public Rigidbody rb;
    public Animator animator;
    public float rotationSpeed = 10f;
    public GameObject startMenu;
    public GameObject joystickUI;
    public Button _homeBtn;
    private SceneTransition sceneTransition;
    [SerializeField] private GameObject _walkSound;
    [SerializeField] private GameObject _jumpSound;

    private bool isInMenu = true;
    private bool isGrounded = true;

    void Start()
    {
        sceneTransition = GetComponent<SceneTransition>();
        animator.SetBool("isDancing", true);
        joystickUI.SetActive(false);
        _homeBtn.onClick.AddListener(ReloadMenuScene);
    }

    private void ReloadMenuScene()
    {
        sceneTransition.LoadSceneWithFade("MainMenu");
    }

    void Update()
    {
        if (!isInMenu)
        {
            if (isGrounded)
            {
                animator.SetBool("isJumping", false);
                _jumpSound.SetActive(false);
            }
            else
            {
                animator.SetBool("isJumping", true);
                _jumpSound.SetActive(true);
            }
        }
    }

    public void SetGroundedState(bool grounded)
    {
        isGrounded = grounded;
        animator.SetBool("isJumping", !grounded);
    }


    public void StartGame()
    {
        isInMenu = false;
        animator.SetBool("isDancing", false);
        animator.SetBool("isWalking", false);

        transform.Rotate(0, 180, 0);

        joystickUI.SetActive(true);
        startMenu.SetActive(false);
        _homeBtn.gameObject.SetActive(true);
    }

    void FixedUpdate()
    {
        if (isInMenu) return;

        Vector3 direction = new Vector3(joystick.Horizontal, 0, joystick.Vertical);

        if (direction.magnitude >= 0.1f)
        {
            animator.SetBool("isWalking", true);
            _walkSound.SetActive(true);
            Vector3 move = direction.normalized * speed * Time.fixedDeltaTime;
            rb.MovePosition(rb.position + move);

            Quaternion targetRotation = Quaternion.LookRotation(direction);
            rb.rotation = Quaternion.Slerp(rb.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
        }
        else
        {
            animator.SetBool("isWalking", false);
            _walkSound.SetActive(false);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Floor"))
        {
            isGrounded = true;
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Floor"))
        {
            isGrounded = false;
        }
    }
}