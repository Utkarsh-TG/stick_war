using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AttackText : MonoBehaviour
{
    public float lifeTime;
    private TMP_Text attackText;
    private bool moveUp;
    private Rigidbody2D rb;
    public float speed = 5f;
    private float aVal = 255;
    public float fadingSpeed = 15f;
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        attackText = GetComponent<TMP_Text>();
        moveUp = true;
        StartCoroutine("LifeTick");
    }

    private void FixedUpdate()
    {
        if (moveUp)
        {
            rb.velocity = Vector2.up* speed* Time.fixedDeltaTime;
        }
        else
        {
            Destroy(gameObject);
        }

        attackText.faceColor = new Color(255, 255, 255, aVal);

        aVal -= fadingSpeed;
    }
    
    IEnumerator LifeTick()
    {
        yield return new WaitForSecondsRealtime(lifeTime);
        moveUp = false;
    }
}
