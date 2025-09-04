
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MendamaCollectUI : MonoBehaviour
{
    public Image MdCollect;
    public TextMeshProUGUI collectText;

    private int collectCount = 0;
    private int totalCount = 0;


    // Start is called before the first frame update
    void Start()
    {
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
            // 這裡之後可以呼叫 PlayerAura.OnAllCollected() 或 Exit.Open()
        }
    }

    private void UpdateCollectUI() 
    {
        collectText.text = $"{collectCount}/{totalCount}";
    }
}
