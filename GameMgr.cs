using UnityEngine;
using System.Collections;
//使用list数据类型前需要添加的命名空间
using System.Collections.Generic;

public class GameMgr : MonoBehaviour {

    //保存怪兽出现的所有位置的数组
    public Transform[] points;
    //要分配怪兽预设的变量
    public GameObject monsterPrefab;
    //保存事先生成的怪兽的List数据类型
    public List<GameObject> monsterPool = new List<GameObject>();

    //生成怪兽的周期
    public float createTime = 2.0f;
    //生成怪兽的最大数量
    public int maxMonster = 10;
    //控制是否终止游戏的变量
    public bool isGameOver = false;
    //声明单例模式的实例变量
    public static GameMgr instance = null;

    //声明表示声音的变量
    public float sfxVolumn = 1.0f;
    //静音功能
    public bool isSfxMute = false;

    private void Awake() {
        //将GameMgr类带入实例
        instance = this;
    }

    // Use this for initialization
    void Start () {
        //获取层次视图SpawnPoint下的所有Transform组件
        points = GameObject.Find("SpawnPoint").GetComponentsInChildren<Transform>();

        //生成怪兽并保存到对象池
        for(int i = 0; i < maxMonster; i++)
        {
            //生成怪兽预设
            GameObject monster = (GameObject)Instantiate(monsterPrefab);
            //设置生成怪兽名
            monster.name = "Monster_" + i.ToString();
            //禁用生成的怪兽
            monster.SetActive(false);
            //将生成的怪兽添加到对象池
            monsterPool.Add(monster);
        }

        if (points .Length > 0)
        {
            //调用生成怪兽的协程函数
            StartCoroutine(this.CreateMonster());
        }

	}
	
	// Update is called once per frame
	void Update () {
	
	}

    //生成怪兽的协程函数
    IEnumerator CreateMonster() {
        //无限循环直到游戏结束
        while(!isGameOver){
            //程序挂起一段时间（怪兽生成周期）
            yield return new WaitForSeconds(createTime);

            //玩家死亡时跳出当前协程
            if (isGameOver)
                yield break;

            //循环处理对象池中的每个对象
            foreach(GameObject monster in monsterPool)
            {
                //通过是否禁用判断可以使用的怪兽
                if (!monster.activeSelf)
                {
                    //计算随机位置
                    int idx = Random.Range(1, points.Length);
                    //设置怪兽出现位置
                    monster.transform.position = points[idx].position;
                    //激活怪兽
                    monster.SetActive(true);
                    //激活最后一个对象池中的怪兽预设并跳出for循环
                    break;
                }
            }
            //当前已生成对的怪兽的数量
            int monsterCount = (int)GameObject.FindGameObjectsWithTag("MONSTER").Length;

            //只有比怪兽最大的数量小时才继续生成怪兽
            if (monsterCount<maxMonster)
            {
                //程序挂起一段时间（怪兽生成周期）
                yield return new WaitForSeconds(createTime);

                //计算随机位置
                int idx = Random.Range(1, points.Length);

                //动态生成怪兽
                Instantiate(monsterPrefab, points[idx].position, points[idx].rotation);
            }
            else
            {
                yield return null;
            }
        }
    }

    //声音共享函数
    public void PlaySfx(Vector3 pos,AudioClip sfx) {
        //如果静音选项为true则立即停止声音
        if (isSfxMute)
            return;

        //动态生成游戏对象
        GameObject soundObj = new GameObject("Sfx");
        //指定声音发出的位置
        soundObj.transform.position = pos;

        //向生成的游戏对象添加AudioSource组件
        AudioSource audioSource = soundObj.AddComponent<AudioSource>();
        //设置AudioSource属性
        audioSource.clip = sfx;
        audioSource.minDistance = 10.0f;
        audioSource.maxDistance = 30.0f;
        //可以用sfxVolumn变量控制游戏音量
        audioSource.volume = sfxVolumn;
        //播放声音
        audioSource.Play();

        //声音播放结束之后，删除之前动态生成的游戏对象
        Destroy(soundObj, sfx.length);
    }
}
