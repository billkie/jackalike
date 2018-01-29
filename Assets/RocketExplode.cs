using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RocketExplode : MonoBehaviour
{
    public ParticleSystem part;
    public GameObject explosionEffect;
    public List<ParticleCollisionEvent> collisionEvents;
    public float blastRadius = 5f;
    public float explosionForce = 700f;
    private float countDown;
    private bool exploded = false;

    void Start()
    {
        
        part = GetComponent<ParticleSystem>();
        collisionEvents = new List<ParticleCollisionEvent>();
        float lifeSpan = part.main.startLifetime.constant;
        Debug.Log(part.main.startLifetime.constant);
        Debug.Log(collisionEvents);
        countDown = lifeSpan;
    }

    private void Update()
    {
        countDown -= Time.deltaTime;

        if (countDown <= 0f && exploded == false)
        {
            Explode();
            exploded = true;
        }
    }

    void OnParticleCollision(GameObject other)
    {
        int numCollisionEvents = part.GetCollisionEvents(other, collisionEvents);

        Explode();
        exploded = true;
    }

    void Explode()
    {
        Quaternion rot;
        Vector3 pos;

        if(collisionEvents.Count > 0)
        {
            rot = Quaternion.FromToRotation(Vector3.forward, collisionEvents[0].normal);
            pos = collisionEvents[0].intersection;
        }
        else
        {

            rot = part.transform.rotation;
            pos = part.transform.position;
            
        }
        
        GameObject impactGO = Instantiate(explosionEffect, pos, rot);
        Destroy(impactGO, 3f);

        Collider[] colliders = Physics.OverlapSphere(collisionEvents[0].intersection, blastRadius);
        foreach(Collider nearbyObject in colliders)
        {
            Rigidbody rb = nearbyObject.GetComponent<Rigidbody>();
            if(rb != null)
            {
                rb.AddExplosionForce(explosionForce, collisionEvents[0].intersection, blastRadius);
            }
        }
        List<Transform> children = new List<Transform>();
        for (int i = 0; i < transform.childCount; i++)
        {
            children.Add(transform.GetChild(i));
        }
        
        transform.DetachChildren();
        Destroy(gameObject);

        foreach(Transform child in children)
        {
            Destroy(child.gameObject, 3f);
        }
    }
}
