using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Type에 관련 설명
//0 그냥 주울수있는 배치된 오브젝트
//1 나무 속성을 가져 도끼에 2배 데미지
//2 돌 속숭을 가져 곡갱이에 2배 데미지
//3 일반 속성 추가 데미지 없음

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

    private void Dead()
    {
        _isAlive = false;
        //죽음때 어캐할건지

        Destroy(this.gameObject);
    }

    void Check()
    {
        //죽음
        if(_hp < 1)
        {
            Dead();
        }
    }

    public void Killed(GameObject Killer)
    {
        //아이템 받기

        //오브젝트 삭제
        Dead();
    }

    public bool ChangeHP(int hpValue)
    {
        if (_hp < 1) return true;

        //계산
        _hp += hpValue;

        if(hpValue > 0)//회복
        {
        }
        else//피격
        {
        }

        Check();

        return !_isAlive;
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
