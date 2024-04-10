using NPBehave;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ThiefBT : MonoBehaviour
{
    public int m_PlayerNumber = 1;      // Used to identify which tank belongs to which player.  This is set by this tank's manager.
    public int m_Behaviour = 0;         // Used to select an AI behaviour in the Unity Inspector
    public int troll_Number = 2;
    public int diamond_Number = 4;

    private ThiefMovement m_Movement;    // Reference to tank's movement script, used by the AI to control movement.
    private List<GameObject> troll_Targets; // List of troll targets for this Thief
    private List<GameObject> diamond_Targets;
    private Root tree;                  
    private Blackboard blackboard;      

    // Initialisation
    private const int IDLE = 0;
    private const int Searching = 1;
    private const int Running = 2;
    private const int WallHitting = 3;
    private int currentAction;
    private List<int> utilityScores;

    private float[] troll_distance;
    private float[] diamond_distance;
    private float current_distance;
    //RayAngle
    private float raycastAngle = 45f;
    private float ScanmaxDistance = 5f;
    private float directDistance = 40f;
    private void Awake()
    {
        troll_Targets = new List<GameObject>();
        diamond_Targets = new List<GameObject>();
    }

    // Start behaviour tree
    private void Start()
    {

        Debug.Log("Initialising AI player " + m_PlayerNumber);
        // Scripts for tank control
        m_Movement = GetComponent<ThiefMovement>();

        // Set initial action
        currentAction = IDLE;
        SwitchTree(SelectBehaviourTree(currentAction));

        // Set utility scores to zero
        utilityScores = new List<int>();
        utilityScores.Add(0); 
        utilityScores.Add(0); 
        utilityScores.Add(0);
        utilityScores.Add(0);
        //ini_distance();
    }
    private void Update()
    {
        updateScores();
        int maxValue = utilityScores.Max(t => t);
        int maxIndex = utilityScores.IndexOf(maxValue);

        if (currentAction != maxIndex)
        {
            currentAction = maxIndex;
            SwitchTree(SelectBehaviourTree(currentAction));
        }
    }
    private void ini_distance()
    {
        if(troll_distance!=null)
        {
            for (int i = 0; i < troll_Number; i++)
            {
                troll_distance[i] = Distance_calculate(troll_Targets[i]);
            }
        }
        if (diamond_distance != null)
        {
            for (int i = 0; i < diamond_Number; i++)
            {
                diamond_distance[i] = Distance_calculate(diamond_Targets[i]);
            }
        }    
    }
    private int get_close_target()
    {
        return 0;
    }
    private float Distance_calculate(GameObject gameObject)
    {
        float t_distance = Mathf.Sqrt(Mathf.Pow((gameObject.transform.position.x - this.gameObject.transform.position.x), 2) + Mathf.Pow((gameObject.transform.position.x - this.gameObject.transform.position.x), 2)); ;
        return t_distance;
    }
    private void updateScores()
    {
        //current_distance = Distance_calculate(troll_Targets[0]); 
        //float rate = current_distance / troll_distance[0];
        //if (rate < 0.8)
        //{
        //    utilityScores[Searching] = 15;
        //}
        //else
        //{
        //    utilityScores[Searching] = 5;
        //}
        utilityScores[IDLE] = 10;
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, 3f))
        {
            if (hit.collider.CompareTag("Wall"))
            {
                utilityScores[WallHitting] = 100;
            }
        }
        else
        {
            if (utilityScores[WallHitting] != 0)
            {
                utilityScores[WallHitting] = 0;
            }
        }
    }

    private void SwitchTree(Root t)
    {
        if (tree != null) tree.Stop();

        tree = t;
        blackboard = tree.Blackboard;
#if UNITY_EDITOR
        Debugger debugger = (Debugger)this.gameObject.AddComponent(typeof(Debugger));
        debugger.BehaviorTree = tree;
#endif

        tree.Start();
    }

    private Root SelectBehaviourTree(int action)
    {
        switch (action)
        {
            case IDLE:
                return Scaning();

            case Searching:
                return TrackingTarget();

            case Running:
                return Scaning();
            case WallHitting:
                return TurnAround();
            default:
                return new Root(StopTurning());
        }
    }

    /**************************************
     * 
     * TANK ACTIONS
     * 
     * Move, turn and fire
     */

    // ACTION: move the tank with a velocity between -1 and 1.
    // -1: fast reverse
    // 0: no change
    // 1: fast forward
    private void Move(float velocity)
    {
        m_Movement.AIMove(velocity);
    }

    // ACTION: turn the tank with angular velocity between -1 and 1.
    // -1: fast turn left
    // 0: no change
    // 1: fast turn right
    private void Turn(float velocity)
    {
        m_Movement.AITurn(velocity);
    }


    private void WallhitScan()
    {
        Vector3 LeftrayDirection = Quaternion.Euler(0, -raycastAngle, 0) * transform.forward;
        Vector3 RightrayDirection = Quaternion.Euler(0, raycastAngle, 0) * transform.forward;
        // ·¢ÉäÉäÏß
        RaycastHit hit;
        if (Physics.Raycast(transform.position, LeftrayDirection, out hit, ScanmaxDistance))
        {
            if(hit.collider.CompareTag("Wall"))
            {
                blackboard["WallonLeft"] = true;
            }
        }
        else
        {
            blackboard["WallonLeft"] = false;
        }
        if (Physics.Raycast(transform.position, RightrayDirection, out hit, ScanmaxDistance))
        {
            if (hit.collider.CompareTag("Wall"))
            {
                blackboard["WallonRight"] = true;
            }
        }
        else
        {
            blackboard["WallonRight"] = false;
        }


        //change
        //Vector3 targetPos = TargetTransform().position;
        //Vector3 localPos = this.transform.InverseTransformPoint(targetPos);
        //Vector3 heading = localPos.normalized;
        //blackboard["targetDistance"] = localPos.magnitude;
        //blackboard["targetInFront"] = heading.z > 0;
        //blackboard["targetOnRight"] = heading.x > 0;
        //blackboard["targetOffCentre"] = Mathf.Abs(heading.x);
    }
    private void UpdateWallPositioin()
    {

    }

    // Register an enemy target 
    public void AddTroll(GameObject target)
    {
        troll_Targets.Add(target);
    }
    public void AddDiamond(GameObject target)
    {
        diamond_Targets.Add(target);
    }

    // Get the transform for the first target
    private Transform TargetTransform(GameObject gameObject)
    {
        if (gameObject!=null)
        {
            return gameObject.transform;
        }
        else
        {
            return null;
        }
    }

    /**************************************
     * 
     * BEHAVIOUR TREES
     * 
     */
    private Root TurnAround()
    {
        return new Root(new Sequence(
                new Action(() => Turn(1f)),
                new Wait(0.5f)
            )
        );
    }
    // Just turn slowly
    private Root Scaning()
    {
        return new Root(new Service(0.1f, () => { WallhitScan(); },
                    new Sequence(
                        //new Action(() => Move(0f)),
                        DirectionSeleter(),
                        new Wait(0.2f),
                        new Action(() => Turn(0f)),
                        new Action(() => Move(0.3f))
                        //new Wait(2.0f)
                    )
                    )
                );
    }
    private Selector DirectionSeleter()
    {
        return new Selector(new BlackboardCondition("WallonLeft", Operator.IS_EQUAL, true, Stops.SELF,
                            new Sequence(RandomturnRight())
                        ),
                        new BlackboardCondition("WallonRight", Operator.IS_EQUAL, true, Stops.SELF,
                            new Sequence(RandomturnLeft())
                            ),
                        new Sequence(                  
                        new Action(() => RandomTurn())
                    )
                 );
                        
    }
    private Node StopTurning()
    {
        return new Action(() => Turn(0));
    }

    private Node RandomTurn()
    {
        return new Action(() => Turn(UnityEngine.Random.Range(-0.2f, 0.2f)));
    }
    private Node RandomturnRight()
    {
        return new Action(() => Turn(UnityEngine.Random.Range(0.4f, 0.6f)));

    }

    private Node RandomturnLeft()
    {
        return new Action(() => Turn(UnityEngine.Random.Range(-0.4f, -0.6f)));
    }

    private Root TrackingTarget()
    {
        return new Root(
            new Sequence(
                    new Selector(
                        new BlackboardCondition("targetOnRight", Operator.IS_EQUAL, false, Stops.SELF,
                            new Sequence(
                                RandomturnLeft()
                                //Fire_judge()
                                )
                        ),
                        new BlackboardCondition("targetOnRight", Operator.IS_EQUAL, true, Stops.SELF,
                            new Sequence(
                                RandomturnRight()
                                //Fire_judge()
                                )
                    ),

                    new Selector(
                        new BlackboardCondition("targetInFront", Operator.IS_EQUAL, true, Stops.LOWER_PRIORITY_IMMEDIATE_RESTART,
                            new Sequence(
                                new Action(() => Move(0.2f))
                            )
                        ),
                        new BlackboardCondition("targetInFront", Operator.IS_EQUAL, false, Stops.LOWER_PRIORITY_IMMEDIATE_RESTART,
                            new Sequence(
                                new Action(() => Move(0.0f))
                            )
                        )
                    )
                    //new Action(() => Move(0.1f))
                    )
                )
            
        );
    }
}
