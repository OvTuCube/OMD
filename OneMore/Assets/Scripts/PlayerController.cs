using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private Transform _playerStart = null;

    public Transform PlayerStart
    { get { return _playerStart; } set { _playerStart = value; } }

    private GameObject _playerPrefab = null;//플레이어 리소스
    private GameObject _playerObject = null;//플레이어로 사용할 인게임오브젝트

    private UnityEngine.Vector3 _mainCamaeraStartPosision;//카메라 세팅 저장
    private int _layermaskTerrain;
    private int _layermaskObject;

    //플레이어 관련 변수
    private UnityEngine.Vector3 _playerDestPos;
    private NavMeshAgent _playerNavMeshAgent;
    private UnityEngine.Vector3 _lookrotation;
    private float _extraRotationSpeed = 3.0f;

    [SerializeField]
    float _interactionRange = 1.0f;
    GameObject _hittedTarget = null;

    //플레이어 애니메이션 관련
    private Animator _animator;

    //컨트롤하는 플레이어의 행동 component
    private Player _player;

    //내비매쉬컨트롤
    private NavMeshSurfaceControl _navMeshSurfaceControl;

    //==========================================================

    //터레인의 터레인메니져가 실행되면 
    public void SetPlayerStart(Transform StartPos)
    {
        PlayerStart = StartPos;
    }

    public void SpawnPlayer()
    {
        //임시로 플레이어 위치 지정

        //플레이어 바뀔때 호출
        //플레이어 리소스를 스폰
        _playerPrefab = Resources.Load<GameObject>("Prefabs/Bamon");
        _playerObject = Instantiate(_playerPrefab, PlayerStart.position, PlayerStart.rotation);

        //카메라 설정저장
        _mainCamaeraStartPosision = Camera.main.transform.position;

        //카메라 배치
        Camera.main.transform.position = _mainCamaeraStartPosision + _playerObject.transform.position;

        //플레이어 네비메쉬 저장 및 플레이어 저장 관련
        _playerNavMeshAgent = _playerObject.GetComponent<NavMeshAgent>();
        _animator = _playerObject.GetComponent<Animator>();
        _player = _playerObject.GetComponent<Player>();

        //내비매쉬초기화
        _navMeshSurfaceControl = GameObject.Find("NavMeshSurface").GetComponent<NavMeshSurfaceControl>();
        _navMeshSurfaceControl.SetPlayerPos(_playerObject.transform);
    }

    private void ExtraDestRot()
    {
        _lookrotation = _playerNavMeshAgent.destination - _playerObject.transform.position;
        _playerObject.transform.rotation = UnityEngine.Quaternion.Slerp(transform.rotation, UnityEngine.Quaternion.LookRotation(_lookrotation), _extraRotationSpeed * Time.deltaTime);
    }

    public void ReBakeNMS()
    {
        _navMeshSurfaceControl._isObjectDestory = true;
    }

    public void ReBakeNMSLate()
    {
        StartCoroutine(_navMeshSurfaceControl.LateUpdateNMS());
    }

    private void InputKey()
    {
        //플레이어가 있으면
        if(_playerObject != null)
        {
            //우클릭 이동
            if(Input.GetMouseButtonDown(1))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                RaycastHit hit;
                if(Physics.Raycast(ray,out hit,1000.0f, _layermaskTerrain))
                {
                    _playerDestPos = hit.point;
                    _playerNavMeshAgent.destination = _playerDestPos;
                }
            }
            //좌클릭 상호작용
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, 1000.0f, _layermaskObject))
                {
                    UnityEngine.Vector3 playerPos = _playerObject.transform.position;
                    playerPos.y = 0.0f;
                    UnityEngine.Vector3 objectPos = hit.point;
                    objectPos.y = 0.0f;

                    float objectDistance = UnityEngine.Vector3.Distance(playerPos, objectPos);

                    if (objectDistance <= _interactionRange)
                    {
                        _hittedTarget = hit.collider.gameObject;
                        if(_hittedTarget)
                        {
                            if(_player)
                            {
                                _player.Act(_hittedTarget);
                            }
                        }
                    }
                }
            }
        }
    }

    private void SetCameraPos()
    {
        Camera.main.transform.position = _mainCamaeraStartPosision + _playerObject.transform.position;
    }

    private void AnimatorValueUpdate()
    {
        if(_playerObject != null) 
        {
            _animator.SetFloat("Speed", _playerNavMeshAgent.velocity.magnitude);
        }
    }

    private void Awake()
    {
        _layermaskTerrain = LayerMask.GetMask("Terrain");
        _layermaskObject = LayerMask.GetMask("Object");
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //입력
        InputKey();
        //캐릭터 더 확실하게 돌리기
        //ExtraDestRot();
        //카메라 따라다니게
        //애니메이션 값 초기화
        AnimatorValueUpdate();
        SetCameraPos();
    }
}
