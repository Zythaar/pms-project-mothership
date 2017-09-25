using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PMS.AI
{

    public class AStarAgent : MonoBehaviour
    {
        [Header("Steering")]
        /*Maximum movement speed when following a path.

        An agent will typically need to speed up and slow down as it follows a path (eg, it will slow down to make a tight turn). The speed is often limited by the length of a path segment and the time taken to accelerate and brake, but the speed will not exceed the value set by this property even on a long, straight path.

        See Also: desiredVelocity.*/
        [SerializeField]
        float speed;
        public float Speed { get { return speed; } set { speed = value; } }




        /*Maximum turning speed in (deg/s) while following a path.
         * 
        This is the maximum rate at which the agent can turn as it rounds the "corner" defined by a waypoint. 
        The actual turning circle is also influenced by the speed of the agent on approach and also the maximum acceleration. 
        See Also: acceleration, velocity.*/
        [SerializeField]
        float angularSpeed;
        public float AngularSpeed { get { return angularSpeed; } set { angularSpeed = value; } }

        /*The maximum acceleration of an agent as it follows a path, given in units / sec^2.
        * 
        An agent does not follow precisely the line segments of the path calculated by the 
        navigation system but rather uses the waypoints along the path as intermediate destinations. 
        This value is the maximum amount by which the agent can accelerate while moving towards the next waypoint. */
        [SerializeField]
        float acceleration;
        public float Acceleration { get { return angularSpeed; } set { angularSpeed = value; } }

        /*Stop within this distance from the target position.

        It is seldom possible to land exactly at the target point, so this property can be used to set an acceptable radius
        within which the agent should stop. A larger stopping distance will give the agent more room for manoeuvre at the end of the path 
        and might avoid sudden braking, turning or other unconvincing AI behaviour. */
        [SerializeField]
        float stoppingDistance;
        public float StoppingDistance { get { return stoppingDistance; } set { stoppingDistance = value; } }

        public bool AutoBraking { get; set; }

        //[Header("Obstacle Avoidance")]
        //[SerializeField]
        float radius;
        public float Radius { get { return radius; } set { radius = value; } }
        public bool AutoRepath { get; set; }

        //The desired velocity of the agent including any potential contribution from avoidance. (Read Only)
        public float DesiredVelocity { get; private set; }


        /*Gets or attempts to set the destination of the agent in world-space units.

        Getting:

        Returns the destination set for this agent.

        • If a destination is set but the path is not yet processed the position returned will be valid navmesh 
        position that's closest to the previously set position.
        • If the agent has no path or requested path - returns the agents position on the navmesh.
        • If the agent is not mapped to the navmesh (e.g. scene has no navmesh) - returns a position at infinity.

        Setting:

        Requests the agent to move to the valid navmesh position that's closest to the requested destination.

        • The path result may not become available until after a few frames. Use pathPending to query for outstanding results.
        • If it's not possible to find a valid nearby navmesh position (e.g. scene has no navmesh) no path is requested. 
        Use SetDestination and check return value if you need to handle this case explicitly.*/
        public Vector3 Destination { get; set; }

        public bool HasPath { get; private set; }

        public bool IsOnGrid { get; private set; }
        public bool IsOnOffGridLink { get; private set; }

        public bool IsPathStale { get; private set; }

        /*This property holds the stop or resume condition of the NavMesh agent.

        If set to True, the NavMesh agent's movement will be stopped along its current path. 
        If set to False after the NavMesh agent has stopped, it will resume moving along its current path.         */
        public bool IsStopped { get; private set; }

        public Vector3 NextPosition { get; set; }

        //ObstacleAvoidanceType

        /*Property to get and set the current path.

        This property can be useful for GUI, debugging and other purposes to get the points of the path 
        calculated by the navigation system. Additionally, a path created from user code can be set for 
        the agent to follow in the usual way. An example of this might be a patrol route designed for coverage 
        rather than optimal distance between two points.*/
        public AStarPath Path { get; set; }

        //Is a path in the process of being computed but not yet ready? (Read Only)
        public bool PathPending { get; private set; }

        //The status of the current path (complete, partial or invalid).
        public AStarPathStatus PathStatus { get; set; }



        /*The distance between the agent's position and the destination on the current path. (Read Only)

        If the remaining distance is unknown then this will have a value of infinity.         */
        public float RemainingDistance { get; private set; }



        /*Get the current steering target along the path. (Read Only)

        This is typically the next corner along the path or the end point of the path.

        Unless the agent is moving on an OffMeshLink, there is a straight path between the agent and the steeringTarget.

        When approaching an OffMeshLink for traversal - the value is the position where the agent will enter the link.
        While agent is traversing an OffMeshLink the value is the position where the agent will leave the link.        */
        public Vector3 SteeringTarget { get; private set; }



        /*Gets or sets whether the transform position is synchronized with the simulated agent position. The default value is true.

        When true: changing the transform position will affect the simulated position and vice-versa.

        When false: the simulated position will not be applied to the transform position and vice-versa.

        Setting updatePosition to false can be used to enable explicit control of the transform position via script. 
        This allows you to use the agent's simulated position to drive another component, which in turn sets the transform position 
        (eg. animation with root motion or physics).

        When enabling the updatePosition (from previously being disabled), the transform will be moved to the simulated position. 
        This way the agent stays constrained to the navmesh surface.*/
        public bool UpdatePosition { get; set; }

        //Should the agent update the transform orientation?
        public bool UpdateRotation { get; set; }

        /*Access the current velocity of the NavMeshAgent component, or set a velocity to control the agent manually.

        Reading the variable will return the current velocity of the agent based on the crowd simulation.

        Setting the variable will override the simulation (including: moving towards destination, collision avoidance, and acceleration control) 
        and command the NavMesh Agent to move using the specific velocity directly. When the agent is controlled using a velocity, its movement is 
        still constrained on the NavMesh.

        Setting the velocity directly, can be used for implementing player characters, which are moving on NavMesh and affecting the rest of the 
        simulated crowd. In addition, setting priority to high (a small value is higher priority), will make other simulated agents to avoid the player 
        controlled agent even more eagerly.

        It is recommended to set the velocity each frame when controlling the agent manually, and if releasing the control to the simulation, set the 
        velocity to zero. If agent’s velocity is set to some value and then stopped updating it, the simulation will pick up from there and the agent 
        will slowly decelerate (assuming no destination is set).

        Note that reading the velocity will always return value from the simulation. If you set the value, the effect will show up in the next update. 
        Since the returned velocity comes from the simulation (including avoidance and collision handling), it can be different than the one you set.

        The velocity is specified in distance units per second (same as physics), and represented in global coordinate system.*/
        public Vector3 Velocity { get; set; }

        /*Calculate a path to a specified point and store the resulting path.

       This function can be used to plan a path ahead of time to avoid a delay in gameplay when the path is needed. 
       Another use is to check if a target position is reachable before moving the agent.
       */
        public bool CalculatePath(Vector3 targetPosition, AStarPath path)
        {
            // returns true if a path is found
            return false;
        }

        // Apply relative movement to current position

        // if the agent has a path it will be adjusted
        public void Move (Vector3 offset)
        {
            // offset: The relative movement vector.
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, offset, dL);
        }

        float CalcTime()
        {
            if (acceleration != 0)
                return speed / acceleration;
            return 1;
        }
        bool isMoving = false;
        private void FixedUpdate()
        {
            //if (isMoving)
            //{
            //    MoveTowardsTarget
            //}
        }

        public bool RayCast(Vector3 targetPosition, out AStarHit hit)
        {
            // True if there is an obstacle between the agent and the target position, otherwise false. 

            AStarHit outHit = new AStarHit();
            hit = outHit;
            return false;
        }

        public void ResetPath()
        {
            // Clears the current path

            // the agent will not start looking for a new path until SetDestination is called
        }

        public bool SetDestination (Vector3 target)
        {

            currentWayPoint = target;

            if (!isShipDriving)
            {
                //if (OnBusyShip != null)
                //    OnBusyShip();
                isShipDriving = true;
            }

            return isShipDriving;
            //  True if the destination was requested successfully, otherwise false. 

            /*
             * Sets or updates the destination thus triggering the calculation for a new path.

            Note that the path may not become available until after a few frames later. While the path is being computed, pathPending will be true. 
            If a valid path becomes available then the agent will resume movement.

             * */
        }

        public bool SetPath(AStarPath path)
        {
            // True if the path is succesfully assigned. 
            return false;

            /*
            Assign a new path to this agent.

            If the path is succesfully assigned the agent will resume movement toward the new target. 
            If the path cannot be assigned the path will be cleared (see ResetPath).

             * */
        }

        public bool Warp(Vector3 newPosition)
        {
            return false;//True if agent is successfully warped, otherwise false. 
        }

        public struct AStarHit
        {
            public float distance;
            public bool hit;
            public int mask;
            public Vector3 normal;
            public Vector3 position;
        }
        #region PMS Old

        Vector3 currentWayPoint;
        // Engine
        [Header("Movement")]
        public bool isShipDriving;
        protected float threshold = 0.05f;
        [Header("speed")]
        public float maxSpeed = 1;
        public float rotSpeed = 5;

        // Use this for initialization
        private void Start()
        {

        }

        // Update is called once per frame
        private void Update()
        {
            if (isShipDriving)
            {
                RotateToTarget();

                MoveTowardsTarget(currentWayPoint);
            }
        }

        public virtual void SetCurrentWaypoint(Vector2 waypoint)
        {
            currentWayPoint = waypoint;

            if (!isShipDriving)
            {
                //if (OnBusyShip != null)
                //    OnBusyShip();
                isShipDriving = true;
            }
        }

        protected void RotateToTarget(Vector3 target)
        {
            Vector3 direction = target - transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            Quaternion targetRot = Quaternion.AngleAxis(angle, Vector3.forward);    // setze Rotation zum ziel
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotSpeed * Time.deltaTime);    // rotiere mit rotspeed zum ziel
        }

        protected void RotateToTarget()
        {
            RotateToTarget(currentWayPoint);
        }

        protected virtual void MoveTowardsTarget(Vector2 targetPosition)
        {
            // Movement
            if (Vector3.Distance(transform.position, targetPosition) > (threshold + stoppingDistance))
            {
                //shipState = ShipState.move;
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, maxSpeed * Time.deltaTime);
            }
            else
            {
                isShipDriving = false;
                //shipState = ShipState.stop;

                //if (wayPointQueue.Count == 0 && OnIdleShip != null)
                //    OnIdleShip();
            }
        }

        #endregion

        #region PMS Acceleration
        public float MAX_TURN_SPD = 2;
        public float MAX_ACCELERATION;
        public float MAX_VELOCITY = 2.5f;
        public float TIME_MAX_VEL = 1;
        public float MIN_ROTATION_ANGLE = 2;

        public float locationDst, velocity, targetDst;

        float d;

        float dN;

        float dV;

        float vAcc;

        float breakDst;

        float dL;


        //bool isMoving;
        float absMaxVel;

        Vector3 relativeTargetPos;

        void CalcAcceleration()
        {
            MAX_ACCELERATION = MAX_VELOCITY / TIME_MAX_VEL;
        }

        void UpdateMovement(float dT)
        {
            d = targetDst - locationDst;
            dN = Mathf.Sign(d);
            dV = MAX_ACCELERATION * dN * dT;
            vAcc = Mathf.Clamp(velocity + dV, -MAX_VELOCITY, MAX_VELOCITY);
            breakDst = 0.5f * (vAcc * vAcc) / MAX_ACCELERATION;
            if (Mathf.Abs(d) <= breakDst)
                dV = -dV;
            velocity = Mathf.Clamp(velocity + dV, -MAX_VELOCITY, MAX_VELOCITY);



            dL = velocity * dT;
            if (Mathf.Abs(dL) > Mathf.Abs(d))
            {
                dL = d;
                velocity = 0;
                isMoving = false;
                //OnEndMovement();
            }
            locationDst += dL;

            #region Debug
            //if (drawDebug)
            //    print("target= " + targetDst + ", location= " + locationDst + ", velocity= " + velocity);

            if (Mathf.Abs(velocity) > absMaxVel)    //DEBUG
                absMaxVel = Mathf.Abs(velocity);
            #endregion
        }

        void ApplyMovement()
        {
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, relativeTargetPos, dL);
        }

        Vector3 dirToTargetNormalized;

        void LookAtTarget()
        {
            //dirNormalized = (lookTarget - transform.position).normalized;
            if (dirToTargetNormalized == Vector3.zero)
                return;

            float targetAngle = Mathf.Atan2(dirToTargetNormalized.y, dirToTargetNormalized.x) * Mathf.Rad2Deg;
            //float angle = Vector3.Angle(transform.right, dirNormalized);

            Quaternion rotation = Quaternion.AngleAxis(targetAngle, Vector3.forward);

            float absAndleDiff = Mathf.Abs(Mathf.DeltaAngle(targetAngle, transform.localEulerAngles.z));
            if (absAndleDiff > MIN_ROTATION_ANGLE)
                transform.localRotation = Quaternion.Slerp(transform.localRotation, rotation, MAX_TURN_SPD * Time.deltaTime);

            //float targetAngle = Mathf.Atan2(dirToTargetNormalized.y, dirToTargetNormalized.x) * Mathf.Rad2Deg;
            //transform.localRotation = Quaternion.AngleAxis(targetAngle, Vector3.forward);
        }

        #endregion

    }

    public enum AStarPathStatus
    {
        PathComplete, /*The path terminates at the destination.*/
        PathPartial,  /*The path cannot reach the destination.*/
        PathInvalid   /*The path is invalid.*/
    };

    public class AStarPath
    {
        /*
         * A path as calculated by the navigation system.

        The path is represented as a list of waypoints stored in the corners array. 
        These points are not set directly from user scripts but a NavMeshPath with points 
        correctly assigned is returned by the NavMesh.CalculatePath function and the NavMeshAgent.path property.

         * */


        public Vector3[] Corners { get; private set; }
        public AStarPathStatus Status { get; private set; }

        //Constructor
        public AStarPath()
        {

        }

        public void ClearCorners()
        {

        }

        //public int GetCornersNonAlloc(out Vector3[] results)
        //{


        //}
    }

}
