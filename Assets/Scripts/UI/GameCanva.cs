using UnityEngine;
using TMPro; // Asegúrate de usar TextMeshPro
using DG.Tweening;

public class GameCanva : MonoBehaviour
{
    public static GameCanva instance;
    public TextMeshProUGUI previewText; 
    public TextMeshProUGUI activeMaskText; 
    public TextMeshProUGUI scoreValue;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    void Start()
    {
        previewText.text = "Pre-selección";
        activeMaskText.text = "MÁSCARA ACTIVA";
        
    }
    public void UpdatePreview(int index)
    {
        previewText.text = "Pre-selección: " + index;
        previewText.transform.DOPunchScale(Vector3.one * 0.1f, 0.1f);
    }

    public void UpdateFinalMask(int index)
    {
        activeMaskText.text = "MÁSCARA ACTIVA: " + index;
        activeMaskText.transform.DOShakePosition(0.2f, 10f);
    }

    public void UpdateScore(int value)
    {
        scoreValue.text = value.ToString();
        scoreValue.transform.DOPunchScale(Vector3.one * 0.1f, 0.1f);
    }

    public void SetInitialValue(int value)
    {
        scoreValue.text = value.ToString();
    }
}
