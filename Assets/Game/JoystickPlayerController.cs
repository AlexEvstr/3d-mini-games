using UnityEngine;

public class JoystickPlayerController : MonoBehaviour
{
    private float speed = 5f; // Скорость передвижения
    public FixedJoystick joystick; // Ссылка на Fixed Joystick
    public Rigidbody rb; // Ссылка на Rigidbody
    public Animator animator; // Ссылка на компонент Animator
    public float rotationSpeed = 10f; // Скорость вращения персонажа

    void FixedUpdate()
    {
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
}
