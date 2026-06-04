using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;
 
public class S_MovementController : MonoBehaviour
{
    //Déplacement au click gauche.
    public static event System.Action<Vector3> OnGroundTouch;
    private NavMeshAgent agent;
    [Header("Movement Settings")]
    public float moveSpeed = 6f;
    [Header("Input Settings")]
    [SerializeField] float sampleDistance = 0.5f;
    [SerializeField] LayerMask groundLayer;
    //===============
    Rigidbody rigidBody;
    Camera cam;
    Vector3 velocity;

    void Start()
    {
        //Déplacement au click gauche.
        agent = GetComponent<NavMeshAgent>();  
        rigidBody = GetComponent<Rigidbody>();
        agent.speed = moveSpeed;
        agent.updateRotation = false;
 
        //===============
 
        cam = Camera.main;
    }
 
 
    void Update()
    {
        Vector3 mousePos = cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, cam.transform.position.y));
        velocity = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized * moveSpeed;
 
        /* transform.LookAt(mousePos + Vector3.up * transform.position.y);*/
 
        // Rotation avec clic droit maintenu
        if (Input.GetMouseButton(1))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
 
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, groundLayer))
            {
                Vector3 direction = hit.point - transform.position;
                direction.y = 0f;
 
                if (direction.sqrMagnitude > 0.01f)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(direction);
                    transform.rotation = Quaternion.Slerp(
                        transform.rotation,
                        targetRotation,
                        10f * Time.deltaTime
                    );
                }
            }
        }
 
        //Déplacement au click gauche.
        if(Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if(Physics.Raycast(ray, out RaycastHit hit, groundLayer))
            {
                if(NavMesh.SamplePosition(hit.point, out NavMeshHit navMeshHit, sampleDistance, NavMesh.AllAreas))
                {
                    agent.SetDestination(navMeshHit.position);
                    OnGroundTouch?.Invoke(navMeshHit.position);
                }
                else
                    Debug.Log("Clicked point is not availble in this area");
            }    
        } 
        //===============
    }
 
    void FixedUpdate()
    {
        rigidBody.MovePosition(rigidBody.position + velocity * Time.fixedDeltaTime);
    }
}
