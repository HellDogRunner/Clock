using UnityEngine;
using UnityEngine.EventSystems;

public abstract class HandDragable : MonoBehaviour, IDragHandler
{
    protected Vector2 _clockCenter;
    protected float _lastHandAngle;

    protected float GetAngle(Vector2 position)
    {
        Vector2 direction = position - _clockCenter;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90;
        angle = (angle + 360) % 360; 
        angle = 360 - angle; 
        return angle;
    }

    public abstract void OnDrag(PointerEventData eventData);
}
