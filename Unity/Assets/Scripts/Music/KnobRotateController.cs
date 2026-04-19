using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class KnobRotateController : MonoBehaviour, IDragHandler, IPointerDownHandler, IEndDragHandler
{
    [SerializeField]
    GameObject arm, fakeArm;
    [SerializeField]
    float maxArmReset;

    public Action<float> OnValueChanged;

    Quaternion dragStartRotation, dragStartInverseRotation;
    float lastRotationArm;
    Quaternion armTargetRotation;

    void ChangeAngle(Quaternion rotation)
    {
        transform.localRotation = rotation;
        OnValueChanged?.Invoke(rotation.eulerAngles.z);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        dragStartRotation = transform.localRotation;
        lastRotationArm = transform.localEulerAngles.z;

        Vector3 worldPoint;
        if (DragWorldPoint(eventData, out worldPoint))
        {
            dragStartInverseRotation = Quaternion.Inverse(Quaternion.LookRotation(worldPoint - transform.position, Vector3.forward));
            arm.transform.rotation = Quaternion.identity;
            fakeArm.transform.rotation = arm.transform.rotation;
        }
        else
        {
            Debug.LogWarning("Couldn't get drag start world point");
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector3 worldPoint;
        if (DragWorldPoint(eventData, out worldPoint))
        {
            Quaternion currentDragAngle = Quaternion.LookRotation(worldPoint - transform.position, Vector3.forward);
            ChangeAngle(currentDragAngle * dragStartInverseRotation * dragStartRotation);

            float currentAngle = transform.localEulerAngles.z;
            float delta = Mathf.Abs(Mathf.DeltaAngle(lastRotationArm, currentAngle));

            if (delta >= maxArmReset)
            {
                lastRotationArm = currentAngle;
                fakeArm.transform.rotation = Quaternion.identity;
            }

            arm.SetActive(true);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        arm.SetActive(false);
    }

    bool DragWorldPoint(PointerEventData eventData, out Vector3 worldPoint)
    {
        return RectTransformUtility.ScreenPointToWorldPointInRectangle(
            GetComponent<RectTransform>(),
            eventData.position,
            eventData.pressEventCamera,
            out worldPoint);
    }

    void Update()
    {
        arm.transform.rotation = Quaternion.Lerp(arm.transform.rotation, fakeArm.transform.rotation, Time.deltaTime * 15);
    }
}
