using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DropBlocks : MonoBehaviour {

	const float drop_interval = 2f;		//ブロックの落ちるスピード
	float timer = 0f;		//タイマー
	const int STAGE_SIZE_X = 8;		//stageのサイズ(8,7,8)
	const int STAGE_SIZE_Y = 7;
	const int STAGE_SIZE_Z = 8;
	Vector3 down_amount = new Vector3(0f, 0.08f, 0f);
	private GameObject test;
	private Text t;



	public static bool confirmed = true;		//stageが確定したらtrue。それによって新しくブロックが生成されたらfalse

	void Start()
	{
		test = GameObject.Find ("abletodrop");
		t = test.GetComponent<Text> ();
	}

	// Update is called once per frame
	void Update () 
	{
		if (!confirmed) {
			timer += Time.deltaTime;
			if (timer > drop_interval) {		//drop_intervalごとにdrop_down
				drop_down ();
				timer = 0f;
			}
		}
	}


	//drop_down()に関数が入らないのでUpdateが読み込まれていない？
	//ステージを全探索して、現在落下中のブロックを見つけたら一つ下に落とす
	void drop_down()
	{
		int[,,] tmp_stage = new int[STAGE_SIZE_X,STAGE_SIZE_Y,STAGE_SIZE_Z];
		tmp_stage = StageState.stage;

		bool able_to_drop = drop_or_not();


		//何かに引っかかったら、2を全て1にして確定。落とせたら、座標移動させる
		if (able_to_drop) {
			GameController.nowBlock.transform.position -= down_amount;
		} else {
			confirm_stage (tmp_stage);
			confirmed = true;
		} 
		t.text = able_to_drop.ToString();
			

		//落とせたら、次のdrop_intervalまで暫定の2のままにさせておく
	}


	//2を1に書き換えて、ステージ情報を確定させる
	void confirm_stage(int[,,] stage)
	{
		for (int i = 0; i < STAGE_SIZE_X; i++) {
			for (int j = 0; j < STAGE_SIZE_Y; j++) {
				for (int k = 0; k < STAGE_SIZE_Z; k++) {
					if (StageState.stage [i, j, k] == 2) {
						StageState.stage [i, j, k] = 1;
					}
				}
			}
		}
	}


	//全探索させて、2をdropさせる。何かに引っかかってる場合はfalse。うまく落とせたらtrue
	bool drop_or_not()
	{
		//全探索
		for (int i = 0; i < STAGE_SIZE_X; i++) {
			for (int j = STAGE_SIZE_Y-1; j >= 0; j--) {
				for (int k = 0; k < STAGE_SIZE_Z; k++) {
					if (StageState.stage [i, j, k] == 2) {
						if (StageState.stage [i, j + 1, k] == 1) {		//動かしてるブロックの下にすでにobjectがあった場合false
							return false;
						} else {
							StageState.stage [i, j + 1, k] = 2;			//2をずらす
							StageState.stage [i, j, k] = 0;
						}
					}
				}
			}
		}
		return true;
	}
		
}
