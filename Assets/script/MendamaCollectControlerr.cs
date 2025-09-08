using UnityEngine;

public class MendamaCollectController : MonoBehaviour
{
    [Header("�����]�w")]
    public float collectTime = 2f;       // �`���ݭn���
    public float collectRadius = 0.5f;   // �`���d��b�|

    [Header("UI �����]�w")]
    public Vector3 UIOffset = new Vector3(-10f, 0f, 0f);
    public Vector3 TopRowOffset = new Vector3(-10f, 0f, 0f);

    public bool isTopRow = false;

    private float collectProgress = 0f;
    private bool wasInside = false;

    private PlayerController player;
    private CollectMDUI collectUI;
    private MendamaCollectUI MdCollectUI;
    private MazeGenerator mazeGen;

    private Vector3 offsetToUse;

    private void Start()
    {
        if (player == null)
            player = FindObjectOfType<PlayerController>();

        if (collectUI == null)
        {
            collectUI = FindObjectOfType<CollectMDUI>();
        }

        if (MdCollectUI == null)
            MdCollectUI = FindObjectOfType<MendamaCollectUI>();
        
        if(mazeGen == null)
            mazeGen = FindObjectOfType<MazeGenerator>();


    }

    private void Update()
    {
        if (player == null || collectUI == null) return;

        // �p�⪱�a�Z��
        float distance = Vector3.Distance(player.transform.position, transform.position);
        bool isInside = distance <= collectRadius;   

        // ---- �i�X�d��P�_ ----
        if (isInside && !wasInside)
        {
            Debug.Log($"{name} ���a�i�J���ɽd��");
            collectProgress = 0f;
            ShowCollectUI(); // �i�D UI ���H�ۤv
        }
        else if (!isInside && wasInside)
        {
            Debug.Log($"{name} ���a���}���ɽd��A�i�׭��m");
            collectProgress = 0f;
            collectUI.Hide();
        }

        wasInside = isInside;

        // ---- �����i�� ----
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
        Debug.Log($"{name} ���ɦ��������I");
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