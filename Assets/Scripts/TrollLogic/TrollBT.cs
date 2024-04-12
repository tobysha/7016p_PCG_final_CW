using NPBehave;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TrollBT : MonoBehaviour
{
    private TrollMovement m_Movement;    // Reference to tank's movement script, used by the AI to control movement.
    private List<GameObject> Thief_Targets; // List of troll targets for this Thief
    private Root tree;
    private Blackboard blackboard;

    // Initialisation
    private const int IDLE = 0;
    private const int Chasing = 1;
    private const int WallHitting = 2;
    private int currentAction;
    private List<int> utilityScores;

    private float[] thief_distance;
    private float items_detect_distance = 50f;
    //RayAngle
    private float raycastAngle = 45f;
    private float ScanmaxDistance = 5f;
    private float directDistance = 40f;

    public float viewRadius = 40f;//distance
    public int viewAngleStep = 20;//step
    [Range(0, 360)]
    public float viewAngle = 360f;//angle
    private void Awake()
    {
        Thief_Targets = new List<GameObject>();
    }

    // Start behaviour tree
    private void Start()
    {
        m_Movement = GetComponent<TrollMovement>();

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
        DrawFieldOfView();
        MapScan();
    }
    void DrawFieldOfView()
    {
        Vector3 forward_left = Quaternion.Euler(0, 0, -viewAngle / 2f) * transform.right * viewRadius;

        for (int i = 0; i <= viewAngleStep; i++)
        {
            Vector3 v = Quaternion.Euler(0, 0, viewAngle / viewAngleStep * i) * forward_left;
            Vector3 pos = transform.position + v;
            //Scene
            Debug.DrawLine(transform.position, pos, Color.red);
            RaycastHit2D hitInfo = Physics2D.Raycast(transform.position+ transform.right * 1f, v, viewRadius);
            if (hitInfo.collider != null)
            {
                if (hitInfo.collider.CompareTag("Thief"))
                {
                    if (!Thief_Targets.Contains(hitInfo.collider.gameObject))
                    {
                        Thief_Targets.Add(hitInfo.collider.gameObject);
                    }
                }
            }
        }
    }
    private void ChangeObjectColor(Color color)
    {
        
        if (GetComponent<Renderer>() != null)
        {
            
            Material newMaterial = new Material(GetComponent<Renderer>().material);
            
            newMaterial.color = color;
            
            GetComponent<Renderer>().material = newMaterial;
        }
    }

    private void Objectsdistance()
    {
        thief_distance = new float[Thief_Targets.Count];
        if (Thief_Targets.Count != 0)
        {
            for (int i = 0; i < Thief_Targets.Count; i++)
            {
                thief_distance[i] = Distance_calculate(Thief_Targets[i]);
            }
        }
    }
    private float Distance_calculate(GameObject gameObject)
    {
        float t_distance = Mathf.Sqrt(Mathf.Pow((gameObject.transform.position.x - this.gameObject.transform.position.x), 2) + Mathf.Pow((gameObject.transform.position.y - this.gameObject.transform.position.y), 2)); ;
        return t_distance;
    }
    private void ObjectsUpdate()
    {
        utilityScores[Chasing] = 0;
        Objectsdistance();
        if (Thief_Targets.Count != 0)
        {
            int Close_Thief = System.Array.IndexOf(thief_distance, Mathf.Min(thief_distance));
            if (thief_distance[Close_Thief] <= items_detect_distance)
            {
                //Debug.Log("shenmedongxi");
                utilityScores[Chasing] = 50 - (int)thief_distance[Close_Thief];
            }
        }
        //Debug.Log("Collecting:"+utilityScores[Collecting]);
        //Debug.Log("Running:" + utilityScores[Running]);
    }
    private void WallHittingAvoid()
    {
        RaycastHit2D FrontHit = new RaycastHit2D();
        Vector2 rayOrigin = transform.position + transform.right * 1f;
        FrontHit = Physics2D.Raycast(rayOrigin, transform.right, 0.8f);
        if (FrontHit.collider != null && (FrontHit.collider.CompareTag("Wall")|| FrontHit.collider.CompareTag("Troll")))
        {
            utilityScores[WallHitting] = 100;
        }
        else
        {
            utilityScores[WallHitting] = 0;
        }
    }
    private void ObjectsStateUpdate()
    {
        List<GameObject> toRemove = new List<GameObject>();

        foreach (GameObject thief in Thief_Targets)
        {
            if (thief != null && !thief.activeSelf)
            {
                toRemove.Add(thief);
            }
        }

        foreach (GameObject obj in toRemove)
        {
            Thief_Targets.Remove(obj);
        }

    }
    private void updateScores()
    {

        utilityScores[IDLE] = 10;
        ObjectsStateUpdate();
        ObjectsUpdate();
        WallHittingAvoid();
        //Debug.Log("youzaipao");
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

            case Chasing:
                return TrackingTarget();

            case WallHitting:
                return TurnAround();
            default:
                return new Root(StopTurning());
        }
    }

    // -1: fast reverse
    // 0: no change
    // 1: fast forward
    private void Move(float velocity)
    {
        m_Movement.AIMove(velocity);
    }
    // -1: fast turn left
    // 0: no change
    // 1: fast turn right
    private void Turn(float velocity)
    {
        m_Movement.AITurn(velocity);
    }


    private void MapScan()
    {
        RaycastHit2D FrontHit = new RaycastHit2D();
        Vector2 rayOrigin = transform.position + transform.right * 1f;
        Vector2 LeftrayDirection = Quaternion.Euler(0, 0, -raycastAngle) * transform.right;
        Vector2 RightrayDirection = Quaternion.Euler(0, 0, raycastAngle) * transform.right;

        RaycastHit2D leftHit = Physics2D.Raycast(rayOrigin, LeftrayDirection, ScanmaxDistance);

        if (leftHit.collider != null && leftHit.collider.CompareTag("Wall"))
        {
            blackboard["WallonLeft"] = true;

        }
        else
        {
            blackboard["WallonLeft"] = false;
        }
        RaycastHit2D rightHit = Physics2D.Raycast(rayOrigin, RightrayDirection, ScanmaxDistance);
        //Debug.Log("�����н���");
        if (rightHit.collider != null && rightHit.collider.CompareTag("Wall"))
        {
            blackboard["WallonRight"] = true;
        }
        else
        {
            blackboard["WallonRight"] = false;
        }
        //FrontHit = Physics2D.Raycast(rayOrigin, transform.right, directDistance);
        //if (FrontHit.collider != null && FrontHit.collider.CompareTag("Thief") && !Thief_Targets.Contains(FrontHit.collider.gameObject))
        //{
        //    Thief_Targets.Add(FrontHit.collider.gameObject);
        //}
        //else
        //{

        //}
        //rightHit = Physics2D.Raycast(rayOrigin, RightrayDirection, directDistance);
        //leftHit = Physics2D.Raycast(rayOrigin, LeftrayDirection, directDistance);
        //if (rightHit.collider != null && rightHit.collider.CompareTag("Thief") && !Thief_Targets.Contains(rightHit.collider.gameObject))
        //{
        //    Thief_Targets.Add(rightHit.collider.gameObject);
        //}
        //if (leftHit.collider != null && leftHit.collider.CompareTag("Thief") && !Thief_Targets.Contains(leftHit.collider.gameObject))
        //{
        //    Thief_Targets.Add(leftHit.collider.gameObject);
        //}

    }
    private void ThiefLocation()
    {
        if (Thief_Targets.Count != 0)
        {
            int Close_Thief = System.Array.IndexOf(thief_distance, Mathf.Min(thief_distance));
            if (Close_Thief < Thief_Targets.Count)
            {
                Vector2 targetPos = Thief_Targets[Close_Thief].gameObject.transform.position;
                Vector2 localPos = this.transform.InverseTransformPoint(targetPos);
                Vector2 heading = localPos.normalized;
                blackboard["targetDistance"] = localPos.magnitude;
                blackboard["targetInFront"] = heading.x > 0;
                blackboard["targetOnRight"] = heading.y > 0;
                blackboard["targetOffCentre"] = Mathf.Abs(heading.x);
            }
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
                new Action(() => Move(-0.5f)),
                new Action(() => Turn(UnityEngine.Random.Range(0, 2) < 1 ? -1f : 1f)),
                new Wait(0.5f)
            )
        );
    }
    // Just turn slowly
    private Root Scaning()
    {
        return new Root(new Service(0.1f, () => { MapScan(); },
                    new Sequence(
                        new Action(() => ChangeObjectColor(Color.gray)),
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
        return new Root(new Service(0.1f, () => { ThiefLocation(); },
            new Sequence(
                new Action(() => ChangeObjectColor(Color.blue)),
                    new Selector(
                        new BlackboardCondition("targetInFront", Operator.IS_EQUAL, true, Stops.LOWER_PRIORITY_IMMEDIATE_RESTART,
                            new Sequence(
                                new Action(() => Move(0.5f)),
                                new Selector(
                                    new BlackboardCondition("targetOnRight", Operator.IS_EQUAL, false, Stops.SELF,
                                    new Sequence(RandomturnLeft())
                                    ),
                                    new BlackboardCondition("targetOnRight", Operator.IS_EQUAL, true, Stops.SELF,
                                    new Sequence(RandomturnRight())
                                    )
                                )
                            )
                        ),
                        new BlackboardCondition("targetInFront", Operator.IS_EQUAL, false, Stops.LOWER_PRIORITY_IMMEDIATE_RESTART,
                            new Sequence(
                                new Action(() => TurnAround())
                            )
                        ),

                    DirectionSeleter()
                    )
                    
                )
            )
        );
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if ( collision.collider.gameObject.CompareTag("Thief") && Thief_Targets.Contains(collision.gameObject) )
        {
            //Debug.Log("collision");
            Thief_Targets.Remove(collision.gameObject);
        }
    }
}
