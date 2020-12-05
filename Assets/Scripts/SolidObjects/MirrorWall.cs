using UnityEngine;

public class MirrorWall : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.gameObject.TryGetComponent(out BulletLife bulletLife))
        {
            print("bullet trigger");
            Transform bulletTransform = coll.transform;
            RaycastHit2D hit = Physics2D.Raycast(bulletTransform.position, bulletTransform.right,
                float.PositiveInfinity, LayerMask.GetMask("Default"));
            if (hit)
            {
                Vector2 reflectDir = Vector2.Reflect(bulletTransform.right, hit.normal);
                float rot = Mathf.Atan2(reflectDir.y, reflectDir.x) * Mathf.Rad2Deg;
                bulletTransform.eulerAngles = new Vector3(0, 0, rot);
            }
        }
    }
}
