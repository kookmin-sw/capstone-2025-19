using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(UIInfoData))]
public class UIInfoInteract : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] GameObject blackBoardPrefab;
    RectTransform blackBoard;
    TextMeshProUGUI content;
    UIInfoData uiInfoData;
    bool pointerEnter = false;
    public void OnPointerEnter(PointerEventData eventData)
    {
        pointerEnter = true;
        blackBoard = Instantiate(blackBoardPrefab, InventoryController.Instance.popupParent).GetComponent<RectTransform>();
        //blackBoard.position = Input.mousePosition;
        content = blackBoard.GetComponentInChildren<TextMeshProUGUI>();
        uiInfoData = GetComponent<UIInfoData>();
        content.text = uiInfoData.GetContent();
        content.raycastTarget = false;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        pointerEnter = false;
        Destroy(blackBoard.gameObject);
        blackBoard = null;
        content = null;
        uiInfoData = null;
    }

    void FixedUpdate()
    {
        if (pointerEnter)
        {
            Vector2 m_position = Input.mousePosition;
            bool shouldBeRightSide = m_position.x > Screen.width * 0.8f;
            if(shouldBeRightSide)
            {
                SetPivotAndAnchor(new Vector2(1, 1));
            }
            else
            {
                SetPivotAndAnchor(new Vector2(0, 1));
            }
            blackBoard.position = m_position;
        }
    }
    void SetPivotAndAnchor(Vector2 newPivot)
    {
        // 앵커와 피벗을 동시에 조정
        blackBoard.pivot = newPivot;
        blackBoard.anchorMin = newPivot;
        blackBoard.anchorMax = newPivot;
    }


}
