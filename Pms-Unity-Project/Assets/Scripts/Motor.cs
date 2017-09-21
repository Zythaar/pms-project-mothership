using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PMS.AI;

[RequireComponent(typeof(AStarAgent))]
public class Motor : MonoBehaviour 
{
    Transform target;
    AStarAgent agent;

    // Use this for initialization
    void Start()
    {
        agent = GetComponent<AStarAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        if (target != null)
        {
            agent.SetDestination(target.position);
            //FaceTarget();
        }
    }

    public void MoveToPoint(Vector3 point)
    {
        agent.SetDestination(point);
    }

    public void FollowTarget(Interactable newTarget)
    {
        agent.StoppingDistance = newTarget.radius * .8f;
        //agent.UpdateRotation = false;
        target = newTarget.interactionTransform;
    }

    public void StopFollowingTarget()
    {
        agent.StoppingDistance = 0;
        agent.UpdateRotation = true;
        target = null;
    }

    void FaceTarget()
    {
        Vector3 direction = (target.position - transform.position).normalized;
        //Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, direction.y, 0f));
        //transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }
}
