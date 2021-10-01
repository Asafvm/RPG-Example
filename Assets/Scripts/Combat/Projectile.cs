using System;
using System.Collections;
using System.Collections.Generic;

using RPG.Attributes;

using UnityEngine;
using UnityEngine.Events;

public class Projectile : MonoBehaviour
{
    [SerializeField] float speed = 1f;
    [SerializeField] bool isHomingProjectile = false;
    [SerializeField] ParticleSystem hitEffect = null;
    [SerializeField] GameObject[] destroyOnHit;
    GameObject instigator = null;

    float damage = 0;

    [SerializeField] UnityEvent projectileHit;

    Health target = null;
    [SerializeField] private float projectileLifetime = 5f;
    private float destroyDelay = 2f;

    private void Start()
    {
        if (target == null) return;

        transform.LookAt(GetAimLocation());

        Invoke(nameof(DestroySelf), projectileLifetime);
    }

    private void DestroySelf()
    {
        Destroy(gameObject);
    }

    private void Update()
    {
        if (target == null ) return;
        if(isHomingProjectile && target.IsAlive)
            transform.LookAt(GetAimLocation());
        transform.Translate(Vector3.forward  * speed * Time.deltaTime);
    }

    public void SetTarget(Health target, GameObject instigator, float damage)
    {
        this.target = target;
        this.damage = damage;
        this.instigator = instigator;
    }

    private Vector3 GetAimLocation()
    {
        if(target.TryGetComponent(out CapsuleCollider targetCollider))
            return target.transform.position + (Vector3.up * targetCollider.height / 2);
        return target.transform.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        projectileHit?.Invoke();
        if (other.GetComponent<Health>() != target || !target.IsAlive) return;
        if(hitEffect)
            Instantiate(hitEffect, transform.position, target.transform.rotation.normalized);
        target.TakeDamage(instigator, damage);
        speed = 0;

        foreach (GameObject toDestroy in destroyOnHit)
        {
            Destroy(toDestroy);
        }

        Destroy(gameObject, destroyDelay);

    }
}
