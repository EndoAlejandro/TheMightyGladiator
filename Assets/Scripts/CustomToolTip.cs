using TMPro;
using UnityEngine;

public class CustomToolTip : MonoBehaviour
{
    // private TMP_Text _text;
    // private void Awake() => _text = GetComponent<TMP_Text>();
    private void Start()
    {
        transform.rotation = Quaternion.Euler(45, 0, 0);
        SetVisibility(false);
    }
    public void SetVisibility(bool isVisible) => gameObject.SetActive(isVisible);
}