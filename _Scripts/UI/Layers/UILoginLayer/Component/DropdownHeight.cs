using UnityEngine;
using UnityEngine.UI;

public class DropdownHeightFix : MonoBehaviour
{
    
    [SerializeField] public float maxHeight = 400f;

    public void FixHeight()
    {
        RectTransform template = GetComponent<RectTransform>();
        template.sizeDelta = new Vector2(template.sizeDelta.x, maxHeight);
    }

    void Start()
    {
        FixHeight();
    }
}