using UnityEngine;
using UnityEngine.UI;
using System.Collections;

//挂载动画片段
[System.Serializable]

public class Anim {
    public AnimationClip idle;
    public AnimationClip runForward;
    public AnimationClip runBackward;
    public AnimationClip runRight;
    public AnimationClip runLeft;
}

public class PlayerCtrl : MonoBehaviour {

    private float h = 0.0f;
    private float v = 0.0f;
    private Transform tr;
    public float moveSpeed = 8.0f;
    public float rotSpeed = 100.0f;

    public Anim anim;
    public Animation _animation;
    public int hp = 100;
    private int initHp;
    public Image imgHpBar;

    //声明委派和事件
    public delegate void PlayerDieHandler();
    public static event PlayerDieHandler OnPlayerDie;

    //访问游戏管理器的变量
    private GameMgr gameMgr;

    // Use this for initialization
    void Start () {
        initHp = hp;
        tr = GetComponent<Transform>();

        //获取GameMgr脚本
        gameMgr = GameObject.Find("GameManager").GetComponent<GameMgr>();

        _animation = GetComponentInChildren<Animation>();
        _animation.clip = anim.idle;
        _animation.Play();
	}
	
	// Update is called once per frame
	void Update () {
        h = Input.GetAxis("Horizontal");
        v = Input.GetAxis("Vertical");

        //Debug.Log("H=" + h.ToString());
        //Debug.Log("V=" + v.ToString());

        //计算前后左右移动方向向量
        Vector3 moveDir = (Vector3.forward * v) + (Vector3.right * h);
        //移动
        tr.Translate(moveDir.normalized * moveSpeed * Time.deltaTime, Space.Self);

        //镜头跟随目标移动
        tr.Rotate(Vector3.up * Time.deltaTime * rotSpeed * Input.GetAxis("Mouse X"));

        //平滑动画
        if(v >= 0.1f)
        {
            //前进动画
            _animation.CrossFade(anim.runForward.name, 0.3f);
        }
        else if (v <= -0.1f)
        {
            //后退动画
            _animation.CrossFade(anim.runBackward.name, 0.3f);
        }
        else if (h <= -0.1f)
        {
            //左移动画
            _animation.CrossFade(anim.runLeft.name, 0.3f);
        }
        else if (h >= 0.1f)
        {
            //右移动画
            _animation.CrossFade(anim.runRight.name, 0.3f);
        }
        else
        {
            //待机动画
            _animation.CrossFade(anim.idle.name, 0.3f);
        }
    }

    //被怪物攻击到使触发的函数
    private void OnTriggerEnter(Collider coll) {
        //如果发生碰撞的是带有PUNCH标签的怪物的手，扣血
        if(coll.gameObject.tag == "PUNCH")
        {
            hp -= 10;

            //调整Image UI元素的fillAmount属性。以调整生命条长度
            imgHpBar.fillAmount = (float)hp / (float)initHp;

            //Debug.Log("Player's hp = " + hp.ToString());
            //生命值为0时，人物死亡
            if (hp <= 0)
            {
                PlayerDie();
            }
        }
    }

    //角色死亡函数
    void PlayerDie() {
        Debug.Log("Game Over");

        /*
         * //获取所有拥有Monster Tag的游戏对象
        GameObject[] monsters = GameObject.FindGameObjectsWithTag("MONSTER");

        //依次调用所有怪兽的OnPlayerDie函数
        foreach(GameObject monster in monsters)
        {
            monster.SendMessage("OnPlayerDie", SendMessageOptions.DontRequireReceiver);
        }
        */
        //触发事件
        OnPlayerDie();
        //更新游戏管理器的isGameOver变量值以停止生成怪兽
        gameMgr.isGameOver = true;

        //访问GameMgr的单例并更改其isGameOver变量值
        GameMgr.instance.isGameOver = true;
    }
}
