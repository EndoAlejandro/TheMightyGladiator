using TMPro;
using UnityEngine;

public class CustomToolTip : MonoBehaviour
{
    [SerializeField] private string displayText;
    private TMP_Text _text;

    private void Awake()
    {
        _text = GetComponent<TMP_Text>();
        _text.SetText(displayText);
    }

    private void Start()
    {
        transform.rotation = Quaternion.Euler(45, 0, 0);
        SetVisibility(false);
    }

    public void SetVisibility(bool isVisible) => _text.gameObject.SetActive(isVisible);
}