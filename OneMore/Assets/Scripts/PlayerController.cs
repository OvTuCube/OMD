using System.Collections;
using System.Collections.Generic;
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

    private Vector3 _mainCamaeraStartPosision;//카메라 세팅 저장
    private int _layermaskTerrain;

    //플레이어 관련 변수
    private Vector3 _playerDestPos;
    private NavMeshAgent _playerNavMeshAgent;
    private Vector3 _lookrotation;
    private float _extraRotationSpeed = 3.0f;

    //플레이어 애니메이션 관련
    private Animator _animator;

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
    }

    private void ExtraDestRot()
    {
        _lookrotation = _playerNavMeshAgent.destination - _playerObject.transform.position;
        _playerObject.transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(_lookrotation), _extraRotationSpeed * Time.deltaTime);
    }

    private void InputKey()
    {
        //플레이어가 있으면
        if(_playerObject != null)
        {
            if(Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                RaycastHit hit;
                if(Physics.Raycast(ray,out hit,1000.0f, _layermaskTerrain))
                {
                    _playerDestPos = hit.point;
                    _playerNavMeshAgent.destination = _playerDestPos;
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
