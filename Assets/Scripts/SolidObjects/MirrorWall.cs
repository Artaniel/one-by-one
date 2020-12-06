using UnityEngine;

public class MirrorWall : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.gameObject.TryGetComponent(out BulletLife bulletLife))
        {
            print("bullet trigger");
            Transform bulletTransform = coll.transform;

            Vector2 closestPoint = coll.ClosestPoint(bulletLife.transform.position);
            var normal = (bulletLife.transform.position - new Vector3(closestPoint.x, closestPoint.y)).normalized;

            Vector2 reflectDir = Vector2.Reflect(bulletLife.transform.right, normal);
            float rot = Mathf.Atan2(reflectDir.y, reflectDir.x) * Mathf.Rad2Deg;
            bulletLife.transform.eulerAngles = new Vector3(0, 0, rot);
        }
    }
}
