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

    private GameObject _playerPrefab = null;//�÷��̾� ���ҽ�
    private GameObject _playerObject = null;//�÷��̾�� ����� �ΰ��ӿ�����Ʈ

    private Vector3 _mainCamaeraStartPosision;//ī�޶� ���� ����
    private int _layermaskTerrain;

    //�÷��̾� ���� ����
    private Vector3 _playerDestPos;
    private NavMeshAgent _playerNavMeshAgent;
    private Vector3 _lookrotation;
    private float _extraRotationSpeed = 3.0f;

    //�÷��̾� �ִϸ��̼� ����
    private Animator _animator;

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
    }

    private void ExtraDestRot()
    {
        _lookrotation = _playerNavMeshAgent.destination - _playerObject.transform.position;
        _playerObject.transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(_lookrotation), _extraRotationSpeed * Time.deltaTime);
    }

    private void InputKey()
    {
        //�÷��̾ ������
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
