using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class ItemObjectIndexCount : MonoBehaviour
{
    [SerializeField]
    private int _itemIndex;
    public int ItemIndex { get { return _itemIndex; } set { _itemIndex = value; } }

    [SerializeField]
    private int _itemCount;
    public int ItemCount { get { return _itemCount; } set { _itemCount = value; } }

    [SerializeField]
    Vector3 positionOffset = Vector3.zero;
    public Vector3 Position { get { return positionOffset; } } 

    [SerializeField]
    Quaternion rotation = Quaternion.identity;
    public Quaternion Rotation { get { return rotation; } }

    [SerializeField]
    Vector3 scale = Vector3.one;
    public Vector3 Scale { get { return scale; } }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
