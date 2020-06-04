using UnityEngine;

public class Face : Align
{
    public override float GetRotation(float ampRotation = 0)
    {
        float desiredOrientation = 0;
        Vector2 direction = target.transform.position - transform.position;
        if (direction.magnitude > 0.0f)
        {
            desiredOrientation = Mathf.Atan2(direction.x, direction.y);
            desiredOrientation *= Mathf.Rad2Deg;
            desiredOrientation += ampRotation;
        }

        return base.GetRotation(desiredOrientation);
    }
}
