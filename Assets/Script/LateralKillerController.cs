using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LateralKillerController : MonoBehaviour
{
    [SerializeField] private Transform m_PouTransform;

    private void Update()
    {
        transform.position = new Vector2(transform.position.x, m_PouTransform.position.y);
    }
}
