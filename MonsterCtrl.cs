using UnityEngine;
using System.Collections;

public class MonsterCtrl : MonoBehaviour {

    //声明表示怪兽状态信息的Enumerable变量
    public enum MonsterState {  idle, trace, attack, die };
    //保存怪兽当前状态的Enumeration变量,默认为idle状态
    public MonsterState monsterState = MonsterState.idle;

    //为提高速度而向变量分配各种组件
    private Transform monsterTr;
    private Transform playerTr;
    private NavMeshAgent nvAgent;
    private Animator animator;
    //血迹效果预设
    public GameObject bloodEffect;
    //血迹贴图效果预设
    public GameObject bloodDecal;

    //怪兽血量
    private int hp = 100;

    //追击范围
    public float traceDist = 100.0f;
    //攻击范围
    public float attackDist = 2.0f;

    //怪兽是否死亡
    private bool isDie = false;

    //声明GameUI对象
    private GameUI gameUI;

	// Use this for initialization
	void Awake () {
        //获取怪兽的transform组件
        monsterTr = this.gameObject.GetComponent<Transform>();
        //获取怪兽要追击的对象——玩家的transform组件
        playerTr = GameObject.FindWithTag("Player").GetComponent<Transform>();
        //获取导航组件
        nvAgent = this.gameObject.GetComponent<NavMeshAgent>();
        //获取Animator组件
        animator = this.gameObject.GetComponent<Animator>();
        //获取GameUI游戏对象的GameUI脚本
        gameUI = GameObject.Find("GameUI").GetComponent<GameUI>();
	}

    //脚本运行时注册事件
    private void OnEnable() {
        PlayerCtrl.OnPlayerDie += this.OnPlayerDie;

        /*
         * 这里检查怪物状态的逻辑使用了协程函数，可以使代码更加高效，简洁，不必像
         * 放在updata函数里每一帧都去检测，当然，把这个逻辑放在Updata函数中也可以
         */

        //运行定期检查怪兽当前状态的协程函数
        StartCoroutine(this.CheckMonsterStates());
        //运行根据怪兽当前状态执行相应例程的协程函数
        StartCoroutine(this.MonsterAction());
    }

    //脚本结束运行时解除事件
    private void OnDisable() {
        PlayerCtrl.OnPlayerDie -= this.OnPlayerDie;
    }

    // Update is called once per frame
    void Update () {
        nvAgent.destination = playerTr.position;
    }

    //定期检查怪兽当前状态并更新monsterState的值
    IEnumerator CheckMonsterStates() {
        while (!isDie)
        {
            //暂时使该段逻辑挂起，等待0.2s后再执行后续代码
            yield return new WaitForSeconds(0.2f);

            //测量怪兽与玩家之间的距离
            float dist = Vector3.Distance(playerTr.position, monsterTr.position);

            if (dist <= attackDist)
                monsterState = MonsterState.attack;
            else if (dist <= traceDist)
                monsterState = MonsterState.trace;
            else
                monsterState = MonsterState.idle;
        }
    }

    //根据怪兽当前状态执行适当操作
    IEnumerator MonsterAction() {
        while (!isDie)
        {
            switch (monsterState)
            {
                case MonsterState.idle:
                    nvAgent.Stop();    //停止追击
                    animator.SetBool("isTrace", false);    //将Animator的IsTrace变量设置为false
                    break;
                case MonsterState.trace:
                    nvAgent.destination = playerTr.position;     //获取玩家位置
                    nvAgent.Resume();   //重新追击
                    animator.SetBool("isTrace", true);    //将Animator的IsTrace变量设置为true
                    break;
                case MonsterState.attack:
                    //停止追击
                    nvAgent.Stop();
                    //将isAttack设置为true后，转换为attack状态
                    animator.SetBool("isAttack", true);
                    break;
            }
            yield return null;
        }
    }

    //检查怪兽是否与子弹碰撞
    private void OnCollisionEnter(Collision collision) {
        if(collision.gameObject.tag == "BULLET")
        {
            //调用血迹效果函数
            CreateBloodEffect(collision.transform.position);

            //受到子弹伤害并减少怪兽hp
            hp -= collision.gameObject.GetComponent<BulletCtrl>().damage;
            if (hp <= 0)
            {
                MonsterDie();
            }
            //删除子弹对象Bullet
            Destroy(collision.gameObject);
            //触发isHit Trigger，使怪兽从Any State转换为gothit状态
            animator.SetTrigger("isHit");
        }
    }

    //血迹效果函数
    void  CreateBloodEffect(Vector3 pos) {
        //生成血迹效果
        GameObject blood1 = (GameObject)Instantiate(bloodEffect, pos, Quaternion.identity);
        Destroy(blood1, 2.0f);

        //贴图生成位置：计算在地面以上的位置
        Vector3 decalPos = monsterTr.position + (Vector3.up * 0.05f);
        //随机设置贴图旋转值
        Quaternion decalRot = Quaternion.Euler(90, 0, Random.Range(0, 360));

        //生成贴图预设
        GameObject blood2 = (GameObject)Instantiate(bloodDecal, decalPos, decalRot);
        //调整贴图大小，使其每次生成的尺寸都不同
        float scale = Random.Range(1.5f, 3.5f);
        blood2.transform.localScale = Vector3.one * scale;

        //5s后删除血迹预设
        Destroy(blood2, 5.0f);
    }

    //处理玩家死亡时的函数
    void OnPlayerDie() {
        //停止所有检测怪兽状态的协程函数
        StopAllCoroutines();
        //停止追击并播放动画
        nvAgent.Stop();
        animator.SetTrigger("isPlayerDie");
    }

    //怪兽死亡处理例程
    void MonsterDie() {

        //将死亡的怪兽Tag更改为Untagged
        gameObject.tag = "Untagged";

        //停止所有例程
        StopAllCoroutines();

        isDie = true;
        monsterState = MonsterState.die;
        nvAgent.Stop();
        animator.SetTrigger("isDie");

        //禁用怪兽的Collider
        gameObject.GetComponentInChildren<CapsuleCollider>().enabled = false;

        foreach (Collider coll in gameObject.GetComponentsInChildren<SphereCollider>())
        {
            coll.enabled = false;
        }

        //调用GameUI脚本处理分数累加与显示的函数
        gameUI.DispScore(50);

        //调用将怪兽放回对象池的协程函数
        StartCoroutine(this.PushObjectPool());
    }

    IEnumerator PushObjectPool() {
        yield return new WaitForSeconds(3.0f);

        //初始化各种变量
        isDie = false;
        hp = 100;
        gameObject.tag = "MONSTER";
        monsterState = MonsterState.idle;

        //重新激活怪兽的Collider
        gameObject.GetComponentInChildren<CapsuleCollider>().enabled = true;

        foreach (Collider coll in gameObject.GetComponentsInChildren<SphereCollider>())
        {
            coll.enabled = true;
        }

        //禁用怪兽
        gameObject.SetActive(false);
    }
}
