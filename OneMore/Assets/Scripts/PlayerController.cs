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

    private GameObject _playerPrefab = null;//�÷��̾� ���ҽ�
    private GameObject _playerObject = null;//�÷��̾�� ����� �ΰ��ӿ�����Ʈ

    private UnityEngine.Vector3 _mainCamaeraStartPosision;//ī�޶� ���� ����
    private int _layermaskTerrain;
    private int _layermaskObject;

    //�÷��̾� ���� ����
    private UnityEngine.Vector3 _playerDestPos;
    private NavMeshAgent _playerNavMeshAgent;
    private UnityEngine.Vector3 _lookrotation;
    private float _extraRotationSpeed = 3.0f;

    [SerializeField]
    float _interactionRange = 1.0f;
    GameObject _hittedTarget = null;

    //�÷��̾� �ִϸ��̼� ����
    private Animator _animator;

    //��Ʈ���ϴ� �÷��̾��� �ൿ component
    private Player _player;

    //����Ž���Ʈ��
    private NavMeshSurfaceControl _navMeshSurfaceControl;

    //UIController�ϱ� ���⼭ input�����ϴϱ�
    UIManager _uiManager;

    //==========================================================

    //�ͷ����� �ͷ��θ޴����� ����Ǹ� 
    public void SetPlayerStart(Transform StartPos)
    {
        PlayerStart = StartPos;
    }

    public void SpawnPlayer()
    {
        //�ӽ÷� �÷��̾� ��ġ ����

        //�÷��̾� �ٲ� ȣ��
        //�÷��̾� ���ҽ��� ����
        _playerPrefab = Resources.Load<GameObject>("Prefabs/Bamon");
        _playerObject = Instantiate(_playerPrefab, PlayerStart.position, PlayerStart.rotation);

        //ī�޶� ��������
        _mainCamaeraStartPosision = Camera.main.transform.position;

        //ī�޶� ��ġ
        Camera.main.transform.position = _mainCamaeraStartPosision + _playerObject.transform.position;

        //�÷��̾� �׺�޽� ���� �� �÷��̾� ���� ����
        _playerNavMeshAgent = _playerObject.GetComponent<NavMeshAgent>();
        _animator = _playerObject.GetComponent<Animator>();
        _player = _playerObject.GetComponent<Player>();

        //����Ž��ʱ�ȭ
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
        //�÷��̾ ������
        if(_playerObject != null)
        {
            //��Ŭ�� �̵�
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
            //��Ŭ�� ��ȣ�ۿ�
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
            //�����̽��� ������ �ݱ�
            if(Input.GetKeyDown(KeyCode.Space))
            {
                Debug.Log("Input SpaceBar");
                _player.TakeDropItem();
            }

            //�� ������ inven������ ����
            if(Input.GetKeyDown(KeyCode.Tab))
            {
                _uiManager.ItemSlotUI();
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

    public void SetPlayerPos()
    {
        UnityEngine.Vector3 tempPos = new UnityEngine.Vector3();
        tempPos.x = 32;
        tempPos.y = 2;
        tempPos.z = 32;

        _playerObject.transform.position = tempPos;
    }

    private void Awake()
    {
        _layermaskTerrain = LayerMask.GetMask("Terrain");
        _layermaskObject = LayerMask.GetMask("Object");

        _uiManager = GameObject.Find("InGameManager").GetComponent<UIManager>();
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        //�Է�
        InputKey();
        //ĳ���� �� Ȯ���ϰ� ������
        //ExtraDestRot();
        //ī�޶� ����ٴϰ�
        //�ִϸ��̼� �� �ʱ�ȭ
        AnimatorValueUpdate();
        SetCameraPos();
    }
}
