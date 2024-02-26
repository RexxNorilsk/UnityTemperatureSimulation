using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Slice : MonoBehaviour
{
    public MeshRenderer MeshRenderer;
    public Action<int> SliceChange;

    private int _divisions;
    private float _max;
    private bool _active = false;
    private float _step;
    private int _currentPos = 0;

    private void Start()
    {
        _max = Mathf.Abs(transform.localPosition.y);
        MeshRenderer = GetComponent<MeshRenderer>();
        SliceChange += SetTransform;
        SetDivisions(10);
    }

    public void SetDivisions(int size) {
        _divisions = size/2;
        _currentPos = -_divisions;
        _step = (_max * 2) / size;
        transform.localPosition = new Vector3(
            transform.localPosition.x,
            -_max,
            transform.localPosition.z
        );
    }

    void OnMouseOver()
    {
        _active = true;
    }

    void OnMouseExit()
    {
        _active = false;
    }

    public void SetTransform(int pos)
    {
        transform.localPosition = new Vector3(
            transform.localPosition.x,
            _step* pos,
            transform.localPosition.z
        );
    }

    public void SetPosition(int pos)
    {
        _currentPos = pos;
        SliceChange?.Invoke(_currentPos);
    }


    public void Update()
    {
        if (_active && Input.mouseScrollDelta.y != 0)
        {
            SetPosition((int)Mathf.Clamp(_currentPos + Input.mouseScrollDelta.y, -_divisions, _divisions));
        }
    }
}
