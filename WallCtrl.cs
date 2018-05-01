using UnityEngine;
using System.Collections;

public class WallCtrl : MonoBehaviour {
    //表示火花粒子对象的变量
    public GameObject sparkEffect;

    //碰撞开始时触发的事件
    private void OnCollisionEnter(Collision collision) {
        //比较发生碰撞的游戏对象的TAG值
        if (collision.collider.tag == "BULLET")
        {
            //动态生成火花粒子并将其保存为变量
            GameObject spark = (GameObject)Instantiate(sparkEffect, collision.transform.position, Quaternion.identity);
            //经过ParticleSystem组件的duration时间后删除
            Destroy(spark, spark.GetComponent<ParticleSystem>().duration + 0.2f);
            //删除发生碰撞的游戏对象
            Destroy(collision.gameObject);
        }
    }

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
