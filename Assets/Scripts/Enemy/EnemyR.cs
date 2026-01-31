using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
public class EnemyR : Enemy
{
    [SerializeField] protected Animator animator;
    [SerializeField] protected Rigidbody2D rb;
    [SerializeField] public float speed;
    private bool isAnimating = false;
    protected override void Start()
    {
        base.Start();
        mask = "red";

        enemyObject.rotation = Quaternion.Euler(0, 0, 0);
        direction = 1;
    }
    async void Update()
    {
        if (!isAnimating)
        {
            enemyObject.position += Vector3.right * direction * Time.deltaTime * speed;
        }
        if (enemyObject.position.x > EndPoint2.position.x && !isAnimating)
        {
            animator.SetBool("isStopping", true);
            isAnimating = true;
            direction *= -1;
            await Task.Delay(2683);
            animator.SetBool("isStopping", false);
            enemyObject.rotation = Quaternion.Euler(new Vector3(0, 180 * (direction == -1 ? 1 : 0), 0));
            isAnimating = false;
        } else if(enemyObject.position.x < EndPoint1.position.x && !isAnimating)
        {
            animator.SetBool("isStopping", true);
            isAnimating = true;
            direction *= -1;
            await Task.Delay(2683);
            animator.SetBool("isStopping", false);
            enemyObject.rotation = Quaternion.Euler(new Vector3(0, 180 * (direction == -1 ? 1 : 0), 0));
            isAnimating = false;
        }
    }
}