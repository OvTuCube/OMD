using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

//Type�� ���� ����
//0 �׳� �ֿ���ִ� ��ġ�� ������Ʈ
//1 ���� �Ӽ��� ���� ������ 2�� ������
//2 �� �Ӽ��� ���� ��̿� 2�� ������
//3 �Ϲ� �Ӽ� �߰� ������ ����

[Serializable]
public enum StandingType
{
    Stuff = 0,
    Wood,
    Stone,
    Normal
}

public class Status : MonoBehaviour
{
    [SerializeField]
    private int _hp = 1;
    public int HP { get { return _hp; } set { _hp = value; } }
    [SerializeField]
    private StandingType _standingType = StandingType.Normal;
    public StandingType StandingType {get { return _standingType; } set { _standingType = value; } }

    private bool _isSpawn = false;
    private bool _isAlive = false;
    public bool IsAlive { get { return _isAlive; } set { _isAlive = value; } }

    //������Ʈ ��ȯ�� ���� ������Ʈ��Ʈ�ѷ� ����
    private ObjectController _objectController;

    //===============================================================
    //�׿����� ȹ�������/��������� ����
    private Dictionary<int, int> _dropItems;
    public Dictionary<int, int> DropItems { get { return _dropItems; } set { _dropItems = value; } }
    //===============================================================

    public void SpawnStatus(Transform spawnPoint, int hp = 1, StandingType standingType = StandingType.Normal)
    {
        if (_isSpawn) return;
        if (_isAlive) return;
        _isSpawn = true;

        _hp = hp;
        _standingType = standingType;
        _isAlive = true;

        this.gameObject.transform.position = spawnPoint.position;
        this.gameObject.transform.rotation = spawnPoint.rotation;
    }

    private void Dead(GameObject Killer)
    {
        _isAlive = false;
        //������ ��ĳ�Ұ���

        if (_standingType == StandingType.Stuff)
        {
            foreach (KeyValuePair<int, int> pair in _dropItems)
            {
                Killer.GetComponent<PlayerInventory>().AddItem_IndexCount(pair.Key, pair.Value); ;
            }
        }
        else
        {
            foreach (KeyValuePair<int, int> pair in _dropItems)
            {
                Vector3 pos;
                pos = this.gameObject.transform.position;

                _objectController.CreateItemObject(pair.Key, pair.Value, pos);
            }
        }

        Destroy(this.gameObject);
    }

    void Check(GameObject Killer)
    {
        //����
        if(_hp < 1)
        {
            Dead(Killer);
        }
    }

    public void Killed(GameObject Killer)
    {
        //������ �ޱ�

        //������Ʈ ����
        Dead(Killer);
    }

    public bool ChangeHP(GameObject Killer,int hpValue)
    {
        if (_hp < 1) return true;

        //���
        _hp += hpValue;

        if(hpValue > 0)//ȸ��
        {
        }
        else//�ǰ�
        {
        }

        Check(Killer);

        return !_isAlive;
    }

    private void Awake()
    {
        _objectController = GameObject.Find("InGameManager").GetComponent<ObjectController>();
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
