using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockGenerator : MonoBehaviour
{

	const float interval = 1f;
	private float timer = 0f;
	public GameObject T, O, S, I, L;
	public Material red, green, blue, yellow;
	private GameObject stage;
	private Vector3 generateVec;
	//生成されるブロックの座標(transform.position)
	private Vector3 generatePos = new Vector3 (4, 5, 4);
	//生成されるブロックの位置

	Vector3[] blockpos;
	private bool setup = false;

	public static string nowBlockName = "";


	// Use this for initialization
	void Start ()
	{
		
	}



	// Update is called once per frame
	void Update ()
	{
		if (GameController.isGameStarted && !setup) {
			stage = GameObject.FindGameObjectWithTag ("stage");
			generateVec = stage.transform.Find ("generatePos").gameObject.transform.position;
			setup = true;
		}
		if (GameController.isGameStarted && DropBlocks.confirmed) {		//ステージが確定したらinterval秒後に生成
			timer += Time.deltaTime;
			if (interval < timer) {		//時間になったら新たなブロック生成
				generate_block (randomG ());
				timer = 0;
				DropBlocks.confirmed = false;
				DropBlocks.finish_put = false;
			}
		}
	}

	GameObject randomG ()
	{
		int r = Random.Range (1, 6);
		switch (r) {
		case 1:
			return T;
		case 2:
			return O;
		case 3:
			return S;
		case 4:
			return I;
		case 5:
			return L;
		default:
			return T;
		}
	}


	//blockがgeneratePosに生成される
	void generate_block (GameObject block)
	{
		GameController.nowBlock = Instantiate (block, generateVec, Quaternion.identity) as GameObject;
		colorChoice (GameController.nowBlock);
		nowBlockName = block.name;
		data_maker (nowBlockName);
	}

	void colorChoice (GameObject block)
	{
		Material mat = randomM ();
		foreach (Transform child in block.transform) {
			if(child.tag == "SmallBlock")
				child.GetComponent<Renderer> ().material = mat;
		}
	}

	Material randomM ()
	{
		int r = Random.Range (0, 4);
		switch (r) {
		case 0:
			return red;
		case 1:
			return blue;
		case 2:
			return green;
		case 3:
			return yellow;
		default:
			return red;
		}
	}


	//それぞれのブロックに応じた形のベクトルを作成する
	//回転中心はこの作成した配列の[0]になる
	//配列の順番は関係ない
	void data_maker (string name)
	{
		switch (name) {
		case "Block-T":
			blockpos = new Vector3[] {
				generatePos, 
				generatePos + create_vec (0, 1, 0),
				generatePos + create_vec (-1, 0, 0),
				generatePos + create_vec (1, 0, 0)
			};
			break;

		case "Block-O":
			blockpos = new Vector3[] {
				generatePos,
				generatePos + create_vec (1, 0, 0),
				generatePos + create_vec (0, 1, 0),
				generatePos + create_vec (1, 1, 0)
			};
			break;

		case "Block-S":
			blockpos = new Vector3[] {
				generatePos,
				generatePos + create_vec (0, 1, 0),
				generatePos + create_vec (-1, 0, 0),
				generatePos + create_vec (-1, -1, 0)
			};
			break;
		case "Block-I":
			blockpos = new Vector3[] {
				generatePos,
				generatePos + create_vec (0, 1, 0),
				generatePos + create_vec (0, -1, 0),
				generatePos + create_vec (0, -2, 0)
			};
			break;
		case "Block-L":
			blockpos = new Vector3[] {
				generatePos,
				generatePos + create_vec (0, 1, 0),
				generatePos + create_vec (0, -1, 0),
				generatePos + create_vec (1, -1, 0)
			};
			break;
		}

		//static変数に代入して参照できるように
		GameController.nowBlockPos = blockpos;
	}


	//ベクトルを作成
	Vector3 create_vec (int x, int y, int z)
	{
		return new Vector3 (x, y, z);
	}
}
