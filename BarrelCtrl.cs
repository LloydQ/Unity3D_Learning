using UnityEngine;
using System.Collections;

public class BarrelCtrl : MonoBehaviour {

    //表示爆炸效果的变量
    public GameObject ExplosureEffect;
    private Transform tr;

    //要随机选择的纹理数组
    public Texture[] textures;

    //保存被子弹击中次数的变量
    private int hitCount = 0;

	// Use this for initialization
	void Start () {
        tr = GetComponent<Transform>();
        int idx = Random.Range(0, textures.Length);
        GetComponentInChildren<MeshRenderer>().material.mainTexture = textures[idx];
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    //碰撞发生时触发的回调函数
    private void OnCollisionEnter(Collision collision) {
        if(collision.collider.tag == "BULLET")
        {
            //删除子弹
            Destroy(collision.gameObject);
            //累加油桶被子弹击中的次数，3次以上则触发爆炸
            if (++hitCount >= 3)
            {
                ExplosureBarrel();
            }
        }
    }

    //实现油桶爆炸的函数
    void ExplosureBarrel() {
        //生成爆炸效果的粒子
        Instantiate(ExplosureEffect, tr.position, Quaternion.identity);
        //以指定原点为中心，获取半径10.0f内的Collider对象
        Collider[] colls = Physics.OverlapSphere(tr.position, 10.0f);
        //对获取的Collider对象施加爆炸力
        foreach (Collider coll in colls)
        {
            Rigidbody rbody = coll.GetComponent<Rigidbody>();
            if(rbody != null)
            {
                rbody.mass = 1.0f;
                rbody.AddExplosionForce(1000.0f, tr.position, 10.0f, 300.0f);
            }
        }
        //5秒后删除油桶模型
        Destroy(gameObject, 5.0f);
    }
}
