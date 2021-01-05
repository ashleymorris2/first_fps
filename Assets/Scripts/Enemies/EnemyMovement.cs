using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField] float chaseProximity = 10f;
    [SerializeField] float loseDistance = 15f;
    [SerializeField] float stopDistance = 2f;
    [SerializeField] NavMeshAgent enemyNavigation;

    private bool chasing;
    private float chaseTime = 5f;
    private float chaseCounter;

    private Animator animator;

    private Vector3 targetPoint;
    private Vector3 startPoint;


    void Start()
    {
        startPoint = transform.position;
        animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        ChasePlayer();
    }

    private void ChasePlayer()
    {
        targetPoint = Player.instance.transform.position;
        targetPoint.y = transform.position.y;

        if (!chasing)
        {
            if (Vector3.Distance(transform.position, targetPoint) < chaseProximity)
            {
                chasing = true;
                GetComponent<EnemyAttack>().StartAttackCountDown();

            }

            DecrementChaseCounter();
        }

        else
        {
            animator.SetBool("isMoving", true);
            CheckIfWithinStopingDistance();
            GetComponent<EnemyAttack>().FireAtPlayer();
        }

    }

    private void CheckIfWithinStopingDistance()
    {
        //If distance from us to the target position is greater than the stop distance
        //Go to target point
        //Or Else
        //Stop at our current position - stops the enemy getting up in our junk
        if (Vector3.Distance(transform.position, targetPoint) > stopDistance)
        {
            enemyNavigation.destination = targetPoint;
            transform.LookAt(Player.instance.transform.position);
        }
        else
        {
            enemyNavigation.destination = transform.position;
        }

        //Stop chasing and reset the counter if we're further away from the player than the lose distance
        if (Vector3.Distance(transform.position, targetPoint) > loseDistance)
        {
            chasing = false;
            chaseCounter = chaseTime;
        }
    }

    private void DecrementChaseCounter()
    {
        if (chaseCounter > 0)
        {
            chaseCounter -= Time.deltaTime;

            if (chaseCounter <= 0)
            {
                enemyNavigation.destination = startPoint;
            }
        }

        if (enemyNavigation.remainingDistance < 1)
        {
            animator.SetBool("isMoving", false);
            animator.SetTrigger("fireShot");
        }
        else
        {
            animator.SetBool("isMoving", true);
        }
    }

}
