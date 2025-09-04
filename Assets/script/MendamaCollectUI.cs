
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
            Debug.Log("�������������I");
            // �o�̤���i�H�I�s PlayerAura.OnAllCollected() �� Exit.Open()
        }
    }

    private void UpdateCollectUI() 
    {
        collectText.text = $"{collectCount}/{totalCount}";
    }
}
