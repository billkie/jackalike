using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletProperties : MonoBehaviour {

    public float damage = 10f;
    public float lifeSpan = 3f;
    public GameObject impactEffect;

    //private float countDown;

    private void Start()
    {
        //countDown = lifeSpan;
    }

    private void Update()
    {
        //countDown -= Time.deltaTime;

        //if (countDown <= 0f)
        //{
        //    Destroy(gameObject);
        //}

        Destroy(gameObject, lifeSpan);
    }

    private void OnCollisionEnter(Collision collision)
    {

        ContactPoint contact = collision.contacts[0];
        Quaternion rot = Quaternion.FromToRotation(Vector3.up, contact.normal);
        Vector3 pos = contact.point;

        GameObject impactGO = Instantiate(impactEffect, pos, rot);
        Destroy(impactGO, 1f);
        Destroy(gameObject);

        if(collision.gameObject.tag == "Enemy")
        {
            collision.gameObject.GetComponent<EnemyProperties>().health -= damage;
        }
    }
}
