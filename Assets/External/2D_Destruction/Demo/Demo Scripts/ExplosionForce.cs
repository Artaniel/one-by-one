using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Explodable))]
public class ExplosionForce : MonoBehaviour {
	public float force = 350;
	public float radius = 5;
	public float upliftModifer = 5;

    private Rigidbody2D[] rigidbodies;

    private void Start()
    {
        rigidbodies = GetComponentsInChildren<Rigidbody2D>(true);
    }
	
    /// <summary>
    /// create an explosion force
    /// </summary>
    /// <param name="position">location of the explosion</param>
	public void DoExplosion(Vector3 position, float additionalPower){
        GetComponent<Explodable>().explode();
        StartCoroutine(WaitAndExplode(position, additionalPower));
	}

    /// <summary>
    /// exerts an explosion force on all rigidbodies within the given radius
    /// </summary>
    /// <returns></returns>
	private IEnumerator WaitAndExplode(Vector3 position, float additionalPower){
		yield return new WaitForFixedUpdate();

        Vector3 explosionPosition = (transform.position + position) / 2;
		foreach(Rigidbody2D coll in rigidbodies)
        {
            float newForce = force * Random.Range(0.5f + additionalPower, 1.5f + additionalPower);
            AddExplosionForce(coll, newForce, explosionPosition, radius, upliftModifer);
		}
	}

    /// <summary>
    /// adds explosion force to given rigidbody
    /// </summary>
    /// <param name="body">rigidbody to add force to</param>
    /// <param name="explosionForce">base force of explosion</param>
    /// <param name="explosionPosition">location of the explosion source</param>
    /// <param name="explosionRadius">radius of explosion effect</param>
    /// <param name="upliftModifier">factor of additional upward force</param>
    private void AddExplosionForce(Rigidbody2D body, float explosionForce, Vector3 explosionPosition, float explosionRadius, float upliftModifier = 0)
	{
		var dir = (body.transform.position - explosionPosition);	
		float wearoff = 1 - (dir.magnitude / explosionRadius);
        Vector3 baseForce = dir.normalized * explosionForce * wearoff;
        baseForce.z = 0;
		body.AddForce(baseForce);
        body.AddTorque(Random.Range(-720, 720));

        if (upliftModifer != 0)
        {
            float upliftWearoff = 1 - upliftModifier / explosionRadius;
            Vector3 upliftForce = Vector2.up * explosionForce * upliftWearoff;
            upliftForce.z = 0;
            body.AddForce(upliftForce);
        }
		
	}
}
