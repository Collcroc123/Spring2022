using UnityEngine;
using UnityEngine.EventSystems;

public class OnHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Animator anim;
    void Start()
    {
        anim = GetComponent<Animator>();
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("HOVERING");
        anim.SetBool("Hovering", true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("STOPPED HOVERING");
        anim.SetBool("Hovering", false);
    }
}
