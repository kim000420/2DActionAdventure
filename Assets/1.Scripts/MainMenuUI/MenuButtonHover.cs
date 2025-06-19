using UnityEngine;
using UnityEngine.EventSystems;

public class MenuButtonHover : MonoBehaviour, IPointerEnterHandler
{
    public int myIndex;
    public MainMenuSelector selector;

    public void OnPointerEnter(PointerEventData eventData)
    {
        selector.HoverSelect(myIndex);
    }
}
