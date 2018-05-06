using UnityEngine;
//为了访问UI组件，需要声明使用UI命名空间
using UnityEngine.UI;
using System.Collections;

public class GameUI : MonoBehaviour {

    //声明文本变量
    public Text txtScore;
    //声明分数变量
    private int totScore;

	// Use this for initialization
	void Start () {
        //初次运行时加载之前保存的分数值
        totScore = PlayerPrefs.GetInt("TOT_SCORE", 0);
        DispScore(0);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    //显示分数函数
    public void DispScore(int score) {
        totScore += score;
        txtScore.text = "score <color=#ff0000>" + totScore.ToString() + "</color>";

        //保存分数
        PlayerPrefs.SetInt("TOT_SCORE", totScore);
    }
}
