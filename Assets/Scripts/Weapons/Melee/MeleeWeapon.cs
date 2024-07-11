using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class MeleeWeapon : Weapon
{
    [SerializeField] private SwingAnimation swingAnimation = new();


    private Vector2 startPosition;
    private Quaternion startRotation;

    private Vector2 lastPosition;
    
    private float SignedDirection => parentSpriteRenderer ? parentSpriteRenderer.flipX ? -1 : 1 : Mathf.Sign(transform.position.x - lastPosition.x);

    private SpriteRenderer parentSpriteRenderer;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        parentSpriteRenderer = GetComponentInParent<SpriteRenderer>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        spriteRenderer.enabled = false;
    }


    protected override void Update()
    {
        base.Update();
    }

    private void LateUpdate()
    {
        lastPosition = transform.position;
    }

    protected override void Fire()
    {
        base.Fire();

        swingAnimation.StartTime = Time.time;
        startPosition = transform.localPosition;
        startRotation = transform.localRotation;

        StartCoroutine(Swing());
    }

    protected virtual IEnumerator Swing()
    {
        spriteRenderer.enabled = true;
        float t = 0;
        Transform swordTransform = transform;
        float direction = SignedDirection; 

        while (t < 1)
        {
            t = (Time.time - swingAnimation.StartTime) / swingAnimation.SwingTime;
            t = Mathf.Clamp01(t);

            if (swingAnimation.Curve != null && swingAnimation.Curve.keys.Length > 1) t = swingAnimation.Curve.Evaluate(t);

            float lerpAngle = Mathf.LerpAngle(0, direction * swingAnimation.Angle, t);

            Vector2 targetPosition = swingAnimation.Position;
            targetPosition.x *= direction;
            
            Vector2 lerpPosition = Vector2.Lerp(startPosition, startPosition + targetPosition, t);

            Quaternion rotFromAngle = Quaternion.AngleAxis(lerpAngle, Vector3.back);

            rotFromAngle *= startRotation;

            swordTransform.localRotation = rotFromAngle;
            swordTransform.localPosition = lerpPosition;
        
            yield return new WaitForEndOfFrame();            
        }
        
        swordTransform.localRotation = startRotation;
        swordTransform.localPosition = startPosition;
        spriteRenderer.enabled = false;
        StopCoroutine(Swing());
    }
}

[Serializable]
public struct SwingAnimation
{
    [SerializeField] public float Angle;
    [SerializeField] public Vector2 Position;
    [SerializeField] public float SwingTime;
    [SerializeField] public AnimationCurve Curve;
    [HideInInspector] public float StartTime;
}
