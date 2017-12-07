using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockGenerator : MonoBehaviour {

	const float interval = 10f;
	float timer = 8f;
	public GameObject T;
	private GameObject stage;
	private Vector3 generateVec;	//生成されるブロックの座標(transform.position)
	private Vector3 generatePos = new Vector3 (4, 1, 4);	//生成されるブロックの位置

	Vector3[] blockpos;

	// Use this for initialization
	void Start () 
	{
		stage = (GameObject)Resources.Load ("Prefabs/Stage");
		generateVec = stage.transform.Find ("generatePos").gameObject.transform.position;
	}



	// Update is called once per frame
	void Update () 
	{
		if (GameController.isGameStarted && DropBlocks.confirmed) {		//ブロックがintervalごとに生成される
			timer += Time.deltaTime;
			if (interval < timer) {		//時間になったら新たなブロック生成
				generate_block (T);
				timer = 0;
				DropBlocks.confirmed = false;
			}
		}
	}


	//blockがgeneratePosに生成される
	void generate_block(GameObject block)
	{
		GameController.nowBlock = Instantiate (block, generateVec, Quaternion.identity) as GameObject;
		data_maker (block.tag);
	}



	//それぞれのブロックに応じた形のベクトルを作成する
	void data_maker(string name)
	{
		switch (name) {
		case "T":
			blockpos = new Vector3[] {
				generatePos, 
				generatePos - create_vec (0, -1, 0),
				generatePos - create_vec (-1, -1, 0),
				generatePos - create_vec (1, -1, 0)
			};
			break;
		}
		stage_update (blockpos);
	}


	//ベクトルを作成
	Vector3 create_vec(int x, int y, int z)
	{
		return new Vector3 (x, y, z);
	}


	//渡された配列に入っているベクトルの位置を2に書き換える
	void stage_update(Vector3[] ary)
	{
		for (int i = 0; i < ary.Length; i++) {
			StageState.stage [(int)ary [i].x, (int)ary [i].y, (int)ary [i].z] = 2;
		}
	}
}
