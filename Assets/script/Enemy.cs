using UnityEngine;
using System.Collections;

public class EnemyPatrolFree : MonoBehaviour
{
    [Header("玩家")]
    public Transform player;

    [Header("巡邏範圍")] 
    public Vector2 areaMin;
    public Vector2 areaMax;

    [Header("距離設定")] 
    public float orbitRange = 2f;          // 繞圈半徑
    public float chaseRange = 5f;          // 追擊半徑
    [Range(1f, 5f)] public float leaveRangeFactor = 1.5f;

    [Header("時間設定")] 
    public float orbitTimeBeforeAttack = 2f;
    public float extraOrbitTime = 3f;
    public float wiggleHoverTime = 0.3f;
    public float wiggleDriftTime = 1f;
    public float wiggleEndPauseTime = 0.2f;
    public float vanishFadeTime = 1f;
    public float vanishWaitTime = 5f;

    [Header("速度設定")] 
    [Range(0f, 2f)] public float patrolSpeedFactor = 1f;
    [Range(0f, 3f)] public float patrolDelay = 3f;
    [Range(0f, 2f)] public float chaseSpeedFactor = 1f;
    [Range(0f, 2f)] public float orbitSpeedFactor = 1f;
    [Range(0f, 2f)] public float leaveSpeedFactor = 0.5f;


    [Header("透明度設定")]
    [Range(0f, 1f)] public float minAlpha = 0.3f;
    [Range(0f, 1f)] public float maxAlpha = 1f;
    public float flickerSpeed = 1f;
    public float holdTime = 0.5f;

    [Header("來回狀態")]
    public float wiggleRoundTrips = 1.5f;
    private bool wiggleInit = false;
    private int wigglePhase = 0;
    private float wigglePhaseTimer = 0f;
    private int wiggleHalfTripsDone = 0;
    private float startAngle, targetAngle;
    private Vector3 center;

    [Header("重新生成")]
    public MazeRenderer mazeRenderer;
    public PlayerSpawn playerSpawn;
    public MendamaSpawner mdManager;
    // ====================== 私有變數 ======================
    private float patrolMinSpeed = 1f;
    private float patrolMaxSpeed = 1.3f;
    private float chaseSpeed = 3f;
    private float orbitSpeed = 120f;
    private Vector2 targetPos;
    private float moveSpeed;
    private float orbitTimer = 0f;
    private float spawnDelay = 10f;
    private PlayerController pc;
    private SpriteRenderer sr;

    private enum State { Patrol, Chase, Orbit, Attack, Wiggle, Leave, Vanish }
    private State state = State.Patrol;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        if (player != null) pc = player.GetComponent<PlayerController>();
        PickSpawnOutsideMap();
    }

    void Update()
    {
        if (player == null) return;
        float distToPlayer = Vector2.Distance(transform.position, player.position);

        switch (state)
        {
            case State.Patrol:
                Patrol();
                if (distToPlayer <= chaseRange)
                {
                    if (pc != null && pc.IsHoldingBreath == false)
                        state = State.Chase;  // 玩家範圍內 → Chase
                }
                break;

            case State.Chase:
                sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 1f);
                Chase();
                if (distToPlayer <= orbitRange)
                {
                    orbitTimer = 0f;
                    state = State.Orbit; // 進 Orbit
                }
                else if (distToPlayer > chaseRange)
                    state = State.Patrol;
                break;

            case State.Orbit:
                Orbit();
                orbitTimer += Time.deltaTime;
                if (pc != null && pc.IsHoldingBreath)
                    state = State.Wiggle; // 玩家閉氣 → Wiggle
                else if (orbitTimer >= orbitTimeBeforeAttack)
                    state = State.Attack; // 過時間 → 攻擊
                if (distToPlayer > orbitRange)
                    state = State.Chase;
                break;

            case State.Wiggle:
                Wiggle();
                break;

            case State.Attack:
                Attack();
                break;

            case State.Leave:
                LeavePlayer();
                break;

            case State.Vanish:
                break;
        }

        // 巡邏透明度呼吸
        if (state == State.Patrol && sr != null)
        {
            float cycle = (2f + holdTime * 2f);
            float t = (Time.time * flickerSpeed) % cycle;
            float alpha01 = t < 1f ? t / 1f :
                             t < 1f + holdTime ? 1f :
                             t < 2f + holdTime ? 1f - ((t - (1f + holdTime)) / 1f) : 0f;
            Color c = sr.color;
            c.a = Mathf.Lerp(minAlpha, maxAlpha, alpha01);
            sr.color = c;
        }
    }

    // ====================== 行為方法 ======================
    void Patrol()
    {
        Vector2 dir = (targetPos - (Vector2)transform.position).normalized;
        transform.position += (Vector3)(dir * moveSpeed * patrolSpeedFactor * Time.deltaTime);
        if (Vector2.Distance(transform.position, targetPos) < 0.2f)
        {
            Invoke("PickRandomTarget",patrolDelay);
            
        }
        
    }

    void Chase()
    {
        Vector2 dir = (player.position - transform.position).normalized;
        transform.position += (Vector3)(dir * chaseSpeed * chaseSpeedFactor * Time.deltaTime);
    }

    void Orbit()
    {
        transform.RotateAround(player.position, Vector3.forward, orbitSpeed * orbitSpeedFactor * Time.deltaTime);
        transform.rotation = Quaternion.identity;
    }

    void Wiggle()
    {
        center = player.position;

        if (!wiggleInit)
        {
            startAngle = Mathf.Atan2(transform.position.y - center.y, transform.position.x - center.x);
            targetAngle = startAngle + Mathf.PI;
            wigglePhase = 0;
            wigglePhaseTimer = 0f;
            wiggleHalfTripsDone = 0;
            wiggleInit = true;
        }

        switch (wigglePhase)
        {
            case 0: // 起點停留
                transform.position = center + new Vector3(Mathf.Cos(startAngle), Mathf.Sin(startAngle), 0f) * orbitRange;
                wigglePhaseTimer += Time.deltaTime;
                if (wigglePhaseTimer >= wiggleHoverTime) { wigglePhase = 1; wigglePhaseTimer = 0f; }
                break;

            case 1: // 飄動
                float t = Mathf.Clamp01(wigglePhaseTimer / wiggleDriftTime);
                float eased = Mathf.SmoothStep(0f, 1f, t);
                float ang = Mathf.LerpAngle(startAngle, targetAngle, eased);
                transform.position = center + new Vector3(Mathf.Cos(ang), Mathf.Sin(ang), 0f) * orbitRange;
                wigglePhaseTimer += Time.deltaTime;
                if (t >= 1f) { wigglePhase = 2; wigglePhaseTimer = 0f; }
                break;

            case 2: // 端點停留
                transform.position = center + new Vector3(Mathf.Cos(targetAngle), Mathf.Sin(targetAngle), 0f) * orbitRange;
                wigglePhaseTimer += Time.deltaTime;
                if (wigglePhaseTimer >= wiggleEndPauseTime)
                {
                    wiggleHalfTripsDone++;
                    if (wiggleHalfTripsDone >= wiggleRoundTrips * 2)
                    {
                        wiggleInit = false;
                        state = State.Leave;
                    }
                    else
                    {
                        startAngle = Mathf.Atan2(transform.position.y - center.y, transform.position.x - center.x);
                        targetAngle = startAngle + Mathf.PI;
                        wigglePhase = 0;
                        wigglePhaseTimer = 0f;
                    }
                }
                break;
        }

        transform.rotation = Quaternion.identity;

        if (pc != null && (!pc.IsHoldingBreath || pc.Stamina <= 0f))
        {
            wiggleInit = false;
            state = State.Attack;
            Debug.Log("敵人發現玩家沒氣，直接攻擊！");
        }
    }

    void Attack()
    {
        Debug.Log("敵人攻擊玩家！");
        PickSpawnOutsideMap();

        mazeRenderer.RegenerateMaze();

        // 玩家重新放進迷宮
        playerSpawn.MoveToMaze(mazeRenderer);

        // 重新生成 mendama
        mdManager.RespawnMendamas();
        
        state = State.Patrol;

    }

    void LeavePlayer()
    {
        Vector2 awayDir = ((Vector2)transform.position - (Vector2)player.position).normalized;
        transform.position += (Vector3)(awayDir * chaseSpeed * leaveSpeedFactor * Time.deltaTime);

        if (Vector2.Distance(transform.position, player.position) >= orbitRange * leaveRangeFactor)
        {
            StartCoroutine(FadeAndVanish());
            state = State.Vanish;
        }
    }

    IEnumerator FadeAndVanish()
    {
        state = State.Vanish;
        float t = 0f;
        Color c = sr.color;
        while (t < vanishFadeTime)
        {
            t += Time.deltaTime;
            c.a = Mathf.Lerp(1f, 0f, t / vanishFadeTime);
            sr.color = c;
            yield return null;
        }
        yield return new WaitForSeconds(vanishWaitTime);
        PickSpawnOutsideMap();
        c.a = 1f;
        sr.color = c;
        state = State.Patrol;
    }

    void PickRandomTarget()
    {
        float x = Random.Range(areaMin.x, areaMax.x);
        float y = Random.Range(areaMin.y, areaMax.y);
        targetPos = new Vector2(x, y);
        moveSpeed = Random.Range(patrolMinSpeed, patrolMaxSpeed);
    }

    void PickSpawnOutsideMap()
    {
        float margin = 2f;
        int side = Random.Range(0, 4);
        float x = 0f, y = 0f;
        switch (side)
        {
            case 0: x = areaMin.x - margin; y = Random.Range(areaMin.y, areaMax.y); break;
            case 1: x = areaMax.x + margin; y = Random.Range(areaMin.y, areaMax.y); break;
            case 2: y = areaMin.y - margin; x = Random.Range(areaMin.x, areaMax.x); break;
            case 3: y = areaMax.y + margin; x = Random.Range(areaMin.x, areaMax.x); break;
        }
        transform.position = new Vector2(x, y);
        Invoke("PickRandomTarget", spawnDelay);
    }
}