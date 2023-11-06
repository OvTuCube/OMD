using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshSurfaceControl : MonoBehaviour
{
    [SerializeField]
    private float _exTime = 0.1f;
    private WaitForSeconds _waitExTime;

    private NavMeshSurface _navMeshSurface = null;
    private Transform _playerPos = null;
    private Vector3 _nmsPos;

    public bool _isObjectDestory = false;

    private float _reBakeDistance = 10;

    public void SetPlayerPos(Transform playerPos)
    {
        _playerPos = playerPos;

        _nmsPos.y = 0.0f;
        _nmsPos.x = _playerPos.position.x;
        _nmsPos.z = _playerPos.position.z;
    }

    public void AfterObjectDestory()
    {
        CheckNMSBake();
        _isObjectDestory = false;
    }

    public void CheckNMSBake()
    {
        if (_navMeshSurface != null && _playerPos != null)
        {
            _nmsPos.y = 0.0f;
            _nmsPos.x = _playerPos.position.x;
            _nmsPos.z = _playerPos.position.z;

            this.gameObject.transform.position = _nmsPos;
            _navMeshSurface.BuildNavMesh();
        }
    }

    public void UpdateNMS()
    {
        float nmsDistance = Vector3.Distance(this.transform.position, _playerPos.position);

        if (nmsDistance > _reBakeDistance)
        {
            CheckNMSBake();
        }
    }

    private void Awake()
    {
        _navMeshSurface = this.gameObject.AddComponent<NavMeshSurface>();
        _navMeshSurface.collectObjects = CollectObjects.Volume;
        _navMeshSurface.useGeometry = NavMeshCollectGeometry.PhysicsColliders;
        int layerTerrain = _navMeshSurface.layerMask = LayerMask.GetMask("Terrain");
        int layerObject = _navMeshSurface.layerMask = LayerMask.GetMask("Object");
        int layermasks = layerTerrain | layerObject;
        _navMeshSurface.layerMask = layermasks;

        _reBakeDistance = _navMeshSurface.size.x * 0.4f;
    }

    public IEnumerator LateUpdateNMS()
    {
        bool isExTime = false;

        while (true) 
        {
            if(isExTime)
            {
                CheckNMSBake();
                break;
            }
            isExTime = true;

            yield return _waitExTime;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        UpdateNMS();
    }

    private void LateUpdate()
    {
        if(_isObjectDestory)
            AfterObjectDestory();
    }

}
