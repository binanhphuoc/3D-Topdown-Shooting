using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class JoystickController : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler {

	private Image bgImg;
	private Image joystickImg;
	private Vector2 inputVector;
	private bool onActive;
	private bool isInverse = false;

	private void Start()
	{
		bgImg = GetComponent<Image> ();
		joystickImg = transform.GetChild (0).GetComponent<Image> ();
		onActive = false;
	}

	public virtual void OnDrag(PointerEventData ped)
	{
		Vector2 pos;
		if (RectTransformUtility.ScreenPointToLocalPointInRectangle (bgImg.rectTransform, ped.position, ped.pressEventCamera, out pos)) {

			pos.x = (pos.x / bgImg.rectTransform.sizeDelta.x);
			pos.y = (pos.y / bgImg.rectTransform.sizeDelta.y);

			if (bgImg.rectTransform.anchoredPosition.x < 0)
				pos.x++;

			//Debug.Log (pos);
			inputVector = new Vector2 (pos.x * 2 - 1, pos.y * 2 - 1);
			//Debug.Log (inputVector);
			inputVector = (inputVector.magnitude > 1.0f) ? inputVector.normalized : inputVector;

			joystickImg.rectTransform.anchoredPosition = new Vector3 (
				inputVector.x * bgImg.rectTransform.sizeDelta.x / 3,
				inputVector.y * bgImg.rectTransform.sizeDelta.y / 3);
			
		}

	}

	public virtual void OnPointerDown(PointerEventData ped)
	{
		onActive = true;
		OnDrag (ped);
	}

	public virtual void OnPointerUp(PointerEventData ped)
	{
		inputVector = Vector2.zero;
		joystickImg.rectTransform.anchoredPosition = Vector3.zero;
		onActive = false;
	}

	public float Horizontal()
	{
		if (inputVector.x != 0)
			return inputVector.x  * (isInverse ? -1 : 1);
		else
			return Input.GetAxis ("Horizontal")  * (isInverse ? -1 : 1);
	}

	public float Vertical()
	{
		if (inputVector.y != 0)
			return inputVector.y * (isInverse ? -1 : 1);
		else
			return Input.GetAxis ("Vertical") * (isInverse ? -1 : 1);
	}

	public bool isActive ()
	{
		return onActive;
	}

	public void setInverse(bool b)
	{
		isInverse = b;
	}
		
}
