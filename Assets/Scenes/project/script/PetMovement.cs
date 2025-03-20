using UnityEngine;

public class PetMovement : MonoBehaviour
{
    public float speed = 2f;
    private Vector3 targetPosition;
    private bool isMoving = false;
    private Transform targetTransform;

    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
        targetPosition = transform.position;
    }

    void Update()
    {
        if (isMoving)
        {
            MoveToTarget();
        }
    }

    public void MoveToPoint(Vector3 point, Transform pointtransform)
    {

        targetPosition = point;
        isMoving = true;
        animator.SetBool("IsFlying", true); // Включаем анимацию полета
        transform.LookAt(pointtransform);
    }

    void MoveToTarget()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            isMoving = false;
            animator.SetBool("IsFlying", false); // Возвращаемся в Idle
        }
    }


}