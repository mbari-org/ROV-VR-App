using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class MenuAppearScript : MonoBehaviour, IPointerClickHandler {
  
    public GameObject menu; // Assign in inspector
    private bool isShowing;

    public void OnPointerClick(PointerEventData eventData)
    {
        isShowing = !isShowing;
        menu.SetActive(isShowing);
    }
}