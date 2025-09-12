using UnityEngine;

public class MendamaCollectController : MonoBehaviour
{
    [Header("收集設定")]
    public float collectTime = 2f;       // 蒐集需要秒數
    public float collectRadius = 0.5f;   // 蒐集範圍半徑

    [Header("UI 偏移設定")]
    public Vector3 UIOffset = new Vector3(-10f, 0f, 0f);
    public Vector3 TopRowOffset = new Vector3(-10f, 0f, 0f);

    public bool isTopRow = false;

    private float collectProgress = 0f;
    private bool wasInside = false;

    private PlayerController player;
    private CollectMDUI collectUI;
    private MendamaCollectUI MdCollectUI;
    private MazeGenerator mazeGen;
    private PlayerInputHandler playerInputHandler;
    private SpriteRenderer sr;
    private Vector3 offsetToUse;

    private void Start()
    {      
        player = FindObjectOfType<PlayerController>();
        collectUI = FindObjectOfType<CollectMDUI>();               
        MdCollectUI = FindObjectOfType<MendamaCollectUI>();        
        mazeGen = FindObjectOfType<MazeGenerator>();            
        playerInputHandler = FindObjectOfType<PlayerInputHandler>();        
        sr = GetComponent<SpriteRenderer>();
        
        if (sr != null)
        {
            sr.sortingOrder = -Mathf.RoundToInt(transform.position.y * 100f);
        }

    }

    private void Update()
    {
        if (player == null || collectUI == null) return;

        // 計算玩家距離
        float distance = Vector3.Distance(player.transform.position, transform.position);
        bool isInside = distance <= collectRadius;

        // ---- 進出範圍判斷 ----
        if (isInside && !wasInside)
        {
            Debug.Log($"{name} 玩家進入面玉範圍");
            collectProgress = 0f;
            ShowCollectUI(); // 告訴 UI 跟隨自己
        }
        else if (!isInside && wasInside)
        {
            Debug.Log($"{name} 玩家離開面玉範圍，進度重置");
            collectProgress = 0f;
            collectUI.Hide();
        }
        else if (playerInputHandler.IsHoldingBreath == true)
        {
            collectProgress = 0f;
        }

            wasInside = isInside;

        // ---- 收集進度 ----
        if (isInside && player.IsCollecting)
        {
            collectProgress += Time.deltaTime;
            collectUI.SetProgress(collectProgress / collectTime);

            if (collectProgress >= collectTime)
            {
                CollectDone();
            }
        }
        else if (!player.IsCollecting)
        {
            collectProgress = 0f;
            if (wasInside)
                collectUI.SetProgress(0f);
        }
    }
    private void ShowCollectUI()
    {
        if (isTopRow)
        {
            offsetToUse = TopRowOffset;
        }
        else
        {
            offsetToUse = UIOffset;
        }
        collectUI.Show(transform, offsetToUse);
    }
    private void CollectDone()
    {
        Debug.Log($"{name} 面玉收集完成！");
        MdCollectUI.UpdateCollected();
        collectUI.Hide();
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, collectRadius);
    }
}