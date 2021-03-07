﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 一定時間経過したら削除されるオブジェクト
/// </summary>
public class Effect : MonoBehaviour
{
    [SerializeField] float lifeTime=0f;
    public UnityEvent OnDestroy { get; } = new UnityEvent();
    float m_time=0f;

    // Update is called once per frame
    void Update()
    {
        if (m_time >= lifeTime)
        {
            Destroy(gameObject);
            OnDestroy.Invoke();
        }
        m_time += Time.deltaTime;

    }
}
