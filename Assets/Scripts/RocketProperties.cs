using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketProperties : MonoBehaviour
{

    public float damage = 80f;
    public float rocketSpeed = 10f;
    public float lifeSpan = 3f;
    public GameObject impactEffect;
    public float blastRadius = 5f;
    public float explosionForce = 700f;

    private float countDown;

    private void Start()
    {
        countDown = lifeSpan;
        
    }

    private void Update()
    {
        countDown -= Time.deltaTime;

        if (countDown <= 0f)
        {
            Explode(transform.position, transform.rotation);
        }
        transform.Translate(Vector3.up * Time.deltaTime * rocketSpeed);
        //Destroy(gameObject, lifeSpan);
    }

    private void OnCollisionEnter(Collision collision)
    {

        ContactPoint contact = collision.contacts[0];
        Quaternion rot = Quaternion.FromToRotation(Vector3.forward, contact.normal);
        Vector3 pos = contact.point;
        Explode(pos, rot);

        if (collision.gameObject.tag == "Enemy")
        {
            collision.gameObject.GetComponent<EnemyProperties>().health -= damage;
        }
    }

    private void OnDisable()
    {
        List<Transform> children = new List<Transform>();
        for (int i = 0; i < transform.childCount; i++)
        {
            children.Add(transform.GetChild(i));
        }

        transform.DetachChildren();

        foreach (Transform child in children)
        {
            ParticleSystem ps = child.gameObject.GetComponent<ParticleSystem>();
            if (ps != null)
            {
                ps.Stop();
                Destroy(child.gameObject, 1.5f);
            }
            else
            {
                Destroy(child.gameObject);
            }
            
        }
    }

    void Explode(Vector3 pos, Quaternion rot)
    {
        

        GameObject impactGO = Instantiate(impactEffect, pos, rot);
        Destroy(impactGO, 4f);

        Collider[] colliders = Physics.OverlapSphere(pos, blastRadius);
        foreach (Collider nearbyObject in colliders)
        {
            Rigidbody rb = nearbyObject.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddExplosionForce(explosionForce, pos, blastRadius);
            }
        }

        Destroy(gameObject);

        
    }

}
