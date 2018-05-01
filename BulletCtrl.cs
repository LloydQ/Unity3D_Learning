using UnityEngine;
using System.Collections;

public class BulletCtrl : MonoBehaviour {

    //子弹威力
    public int damage = 20;
    //子弹发射速度
    public float speed = 1000.0f;

	// Use this for initialization
	void Start () {
        GetComponent<Rigidbody>().AddForce(transform.forward * speed);  //AddForce函数：运行游戏时就可以为Rigidbody组件添加初始推动力
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
