using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    private StateMachine stateMachine;
    private NavMeshAgent agent;
    private GameObject player;
    public NavMeshAgent Agent { get => agent; }
    public GameObject Player {get => player;}

    public Path path;
    [Header("Sight Values")]

    
    public float sightDistance = 20f;
    public float fieldOfView = 85f;
    public float eyeHeight; 
    [Header("Weapon Values")]
    public Transform gunBarrel;
    [Range(0.1f, 10)]
    public float fireRate;

    [SerializeField]
    private string currentState;

 
    
    void Start()
    {
        stateMachine = GetComponent<StateMachine>();
        agent = GetComponent<NavMeshAgent>();
        stateMachine.Initialise();
        player = GameObject.FindGameObjectWithTag("Player");
    }
    void Update()
    {
        CanSeePlayer();
        currentState = stateMachine.activeState.ToString();
    }
    public bool CanSeePlayer()
    {
        if(player != null)
        {
            if(Vector3.Distance(transform.position, player.transform.position)<sightDistance)
            {
                Vector3 targetDirection = player.transform.position - transform.position - (Vector3.up * eyeHeight);
                float angleToPlayer = Vector3.Angle(targetDirection, transform.forward);
                if(angleToPlayer >= -fieldOfView && angleToPlayer <= fieldOfView)
                {
                    
                    Ray ray = new Ray(transform.position + Vector3.up * eyeHeight, targetDirection);
                    RaycastHit hitInfo = new RaycastHit();
                    if(Physics.Raycast(ray,out hitInfo, sightDistance))
                    {
                        if(hitInfo.transform.gameObject == player)
                        {
                            return true;    
                        }
                    }
                    Debug.DrawRay(ray.origin, ray.direction * sightDistance);
                }
            }
        }
        return false; 
    }
}
