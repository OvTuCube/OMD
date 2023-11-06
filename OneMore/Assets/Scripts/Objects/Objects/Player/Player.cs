using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Player : MonoBehaviour
{
    //�÷��̾� �ִϸ��̼� ����
    private PlayerController _playerController;
    private NavMeshAgent _playerNavMeshAgent;

    //==============================================
    private Animator _animator;
    [SerializeField]
    private float _axTime = 1.0f;
    private WaitForSeconds _waitAxTime;

    [SerializeField]
    private float _pickTime = 1.0f;
    private WaitForSeconds _waitPickTime;

    //�ൿ�� ���� �̵� ����=========================
    public bool _isCanMove = true;


    //�ൿ�� ���� �̵� ����=========================

    //private Item _hundledItem;

    //==============================================
    //�÷��̾� ��Ʈ��

    //������Ʈ Ÿ��(��,����)�� ����/����ִ� �����ۿ� ���� ��ȣ�ۿ� �޶���
    public void Act(GameObject target)
    {
        Status targetStatus = target.GetComponent<Status>();
        StandingType standingType = targetStatus.StandingType;
        if (targetStatus != null) 
        {
            switch (standingType) 
            {
                case StandingType.Stuff:
                    {
                        Picking(targetStatus);
                    }
                    break;
                case StandingType.Wood:
                    {
                        Axing(targetStatus);
                    }
                    break;
                case StandingType.Stone:
                    {
                        Axing(targetStatus);
                    }
                    break;
                case StandingType.Normal:
                    {
        
                    }
                    break;
            }
        }
    }

    //==============================================
    //Animation ����

    void Axing(Status targetStatus)
    {
        _animator.SetTrigger("Ax");

        Vector3 tempDestPos = targetStatus.gameObject.transform.position;
        tempDestPos.y = 0;
        LookAt(tempDestPos);

        _playerNavMeshAgent.isStopped = true;

        StartCoroutine(AxingCounting(targetStatus));
    }

    IEnumerator AxingCounting(Status targetStatus)
    {
        Debug.Log("AxingCounting Start");

        bool isAxingFinish = false;
        while (true)
        {
            if (isAxingFinish)
            {
                if(targetStatus.ChangeHP(-200))//���߿� ���� �÷��̾ ����ִ� �������� �ִ� ��ȣ�ۿ밪�� �־����
                    _playerController.ReBakeNMSLate();

                _playerNavMeshAgent.isStopped = false;

                break;
            }

            isAxingFinish = true;

            yield return _waitAxTime;
        }
    }

    void Picking(Status targetStatus)
    {
        _animator.SetTrigger("Pick");

        Vector3 tempDestPos = targetStatus.gameObject.transform.position;
        tempDestPos.y = 0;
        LookAt(tempDestPos);

        _playerNavMeshAgent.isStopped = true;

        StartCoroutine(PickingCounting(targetStatus));
    }

    IEnumerator PickingCounting(Status targetStatus)
    {
        Debug.Log("PickingCounting Start");

        bool isPickingFinish = false;
        while (true)
        {
            if (isPickingFinish)
            {
                targetStatus.Killed(this.gameObject);
                _playerController.ReBakeNMSLate();

                _playerNavMeshAgent.isStopped = false;

                break;
            }

            isPickingFinish = true;

            yield return _waitAxTime;
        }
    }

    void LookAt(Vector3 destPos)
    {
        destPos.y = this.gameObject.transform.position.y;

        this.gameObject.transform.LookAt(destPos);
    }

    //==============================================

    //�ִϸ����ͳ� �ʽ����� ������ ���� ����
    private void Awake()
    {
        _animator = this.gameObject.GetComponent<Animator>();

        //�ൿ ��� Ÿ��
        _waitAxTime = new WaitForSeconds(_axTime);
        _waitPickTime = new WaitForSeconds(_pickTime);

        _playerController = GameObject.Find("InGameManager").GetComponent<PlayerController>();
        _playerNavMeshAgent = this.gameObject.GetComponent<NavMeshAgent>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
