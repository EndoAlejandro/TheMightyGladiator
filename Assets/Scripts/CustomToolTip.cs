using UnityEngine;

public class CustomToolTip : MonoBehaviour
{
    private void Start()
    {
        transform.rotation = Quaternion.Euler(45, 0, 0);
        SetVisibility(false);
    }
    public void SetVisibility(bool isVisible) => gameObject.SetActive(isVisible);
}