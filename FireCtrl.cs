using UnityEngine;
using System.Collections;

//声明脚本需要的组件，以防止该组件被删除
[RequireComponent(typeof(AudioSource))]
public class FireCtrl : MonoBehaviour {

    //子弹预设
    public GameObject bullet;
    //子弹发射坐标
    public Transform firePos;
    //子弹发射声音
    public AudioClip fireSfx;
    //保存AudioSource组件的变量
    private AudioSource source = null;
    //连接MuzzleFlash的MeshRenderer组件
    public MeshRenderer muzzleFlash;

	// Use this for initialization
	void Start () {
        //获取AudioSource组件后分配到变量
        source = GetComponent<AudioSource>();
        //禁用MuzzleFlash MeshRenderer
        muzzleFlash.enabled = false;
	}

    // Update is called once per frame
    void Update() {
        if (Input.GetMouseButtonDown(0))
            Fire();
    }

    //开火函数
    void Fire() {
        //动态生成子弹的函数
        CreateBullet();

        //开枪时发声的函数
        GameMgr.instance.PlaySfx(firePos.position, fireSfx);
        //播放声音
        //source.PlayOneShot(fireSfx, 0.9f);
        //使用例程调用处理MuzzleFlash效果的函数
        StartCoroutine(this.ShowMuzzleFlash());
    }

    //生成子弹的函数
    void CreateBullet() {
        //动态生成Bullet预设
        Instantiate(bullet, firePos.position, firePos.rotation);
    }

    //短时间内反复激活、禁用MuzzleFlash效果
    IEnumerator ShowMuzzleFlash() {
        //随机更改MuzzleFlash的大小
        float scale = Random.Range(1.0f, 2.0f);
        muzzleFlash.transform.localScale = Vector3.one * scale;

        //MuzzleFlash以z轴为基准随机旋转
        Quaternion rot = Quaternion.Euler(0, 0, Random.Range(0, 360));
        muzzleFlash.transform.localRotation = rot;

        //激活使其显示
        muzzleFlash.enabled = true;

        //等待随机时间后再禁用MeshRenderer组件
        yield return new WaitForSeconds(Random.Range(0.05f, 0.3f));

        //禁用MeshRenderer组件使其不显示
        muzzleFlash.enabled = false;
    }
}
