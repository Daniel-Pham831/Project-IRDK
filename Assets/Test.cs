using System.Collections;
using System.Collections.Generic;
using Maniac.Utils.Extension;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Test : MonoBehaviour
{

    [SerializeField] private Rigidbody2D rb;

    private void FixedUpdate()
    {
        UpdateMovement();
    }

    private void UpdateMovement()
    {
        var horizontal = Input.GetAxisRaw("Horizontal");
        var vertical = Input.GetAxisRaw("Vertical");

        rb.velocity = new Vector2(horizontal, vertical).normalized * 5f;
    }
}
