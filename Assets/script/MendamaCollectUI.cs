
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MendamaCollectUI : MonoBehaviour
{
    public int collectCount = 0;
    public Image MdCollect;
    public TextMeshProUGUI collectText;
    public BreathingLight Lt;


    private int totalCount = 0;
    private MazeRenderer MR;
    private EnemyPatrolFree Ep;

    // Start is called before the first frame update
    void Start()
    {
        MR = FindObjectOfType<MazeRenderer>();
        Lt = FindObjectOfType<BreathingLight>();
        Ep = FindObjectOfType<EnemyPatrolFree>();
        UpdateCollectUI();
    }

    public void SetTotalCount(int total)
    { 
        totalCount = total;
        collectCount = 0;
        UpdateCollectUI();
    }

    public void UpdateCollected()
    {
        collectCount++;
        UpdateCollectUI();
        if (collectCount >= totalCount)
        {
            Debug.Log("全部收集完成！");
            MR.OpenExit();
            Lt.OnAllCollected();
            Ep.Disable();
        }
        
    }
    public void UpdateUsed()
    {
            collectCount--;
            UpdateCollectUI();
    }

    private void UpdateCollectUI() 
    {
        collectText.text = $"{collectCount}/{totalCount}";
    }
}
