using UnityEngine;

public class JoystickPlayerController : MonoBehaviour
{
    private float speed = 5f; // Скорость передвижения
    public FixedJoystick joystick; // Ссылка на Fixed Joystick
    public Rigidbody rb; // Ссылка на Rigidbody
    public Animator animator; // Ссылка на компонент Animator
    public float rotationSpeed = 10f; // Скорость вращения персонажа
    public GameObject startMenu; // UI-объект меню
    public GameObject joystickUI; // UI-объект джойстика

    private bool isInMenu = true; // Флаг для меню
    private bool isGrounded = true; // Флаг, указывающий на касание земли

    void Start()
    {
        // Начинаем с анимации танца и показываем меню
        animator.SetBool("isDancing", true);
        joystickUI.SetActive(false); // Джойстик выключен в начале
    }

    void Update()
    {
        if (!isInMenu)
        {
            // Проверяем, касается ли персонаж земли
            if (isGrounded)
            {
                animator.SetBool("isJumping", false); // Если на земле, выключаем анимацию прыжка
            }
            else
            {
                animator.SetBool("isJumping", true); // Если не на земле, включаем анимацию прыжка
            }
        }
    }

    public void SetGroundedState(bool grounded)
    {
        isGrounded = grounded;
        animator.SetBool("isJumping", !grounded); // Устанавливаем анимацию прыжка
    }


    public void StartGame()
    {
        // Этот метод вызывается при нажатии кнопки "Start" из меню
        isInMenu = false;
        animator.SetBool("isDancing", false); // Останавливаем анимацию танца
        animator.SetBool("isWalking", false); // Переход в idle

        // Поворачиваем персонажа на 180 градусов по оси Y
        transform.Rotate(0, 180, 0);

        // Активируем джойстик и убираем меню
        joystickUI.SetActive(true);
        startMenu.SetActive(false);
    }

    void FixedUpdate()
    {
        if (isInMenu) return; // Не выполнять движение, если персонаж в меню

        // Получаем направление на основе ввода джойстика
        Vector3 direction = new Vector3(joystick.Horizontal, 0, joystick.Vertical);

        // Если джойстик используется (есть движение)
        if (direction.magnitude >= 0.1f)
        {
            // Включаем анимацию ходьбы
            animator.SetBool("isWalking", true);

            // Перемещаем персонажа
            Vector3 move = direction.normalized * speed * Time.fixedDeltaTime;
            rb.MovePosition(rb.position + move);

            // Вращаем персонажа в направлении движения
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            rb.rotation = Quaternion.Slerp(rb.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
        }
        else
        {
            // Останавливаем анимацию ходьбы (включаем Idle)
            animator.SetBool("isWalking", false);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        // Если персонаж касается объекта с тегом "Floor", он на земле
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
