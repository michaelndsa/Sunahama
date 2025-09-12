
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MendamaCollectUI : MonoBehaviour
{
    public Image MdCollect;
    public TextMeshProUGUI collectText;
    public MazeRenderer MR;

    public int collectCount = 0;
    private int totalCount = 0;


    // Start is called before the first frame update
    void Start()
    {
        MR = FindObjectOfType<MazeRenderer>();
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
