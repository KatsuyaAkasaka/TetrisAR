using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.iOS;
using System.Linq;


public class CreateStage : MonoBehaviour {

	public GameObject stage;
	private float countdown = 1f;

	private bool floorfoundflag = false;
	//private bool stagecreatedflag = false;

	public GameObject planePrefab;

	private GameObject go;

	private Dictionary<string, ARPlaneAnchorGameObject> planeAnchorMap;

	void Start ()
	{
		planeAnchorMap = new Dictionary<string,ARPlaneAnchorGameObject> ();
		// 各イベントを受け取るメソッド設定
		UnityARSessionNativeInterface.ARAnchorAddedEvent += AddAnchor;
		UnityARSessionNativeInterface.ARAnchorUpdatedEvent += UpdateAnchor;
		UnityARSessionNativeInterface.ARAnchorRemovedEvent += RemoveAnchor;
	}

	
	// Update is called once per frame
	void Update () {
		//if unity find floor , create stage
		if (floorfoundflag && !GameController.readyToStart) {
			countdown -= Time.deltaTime;
			if (countdown < 0f) {
				Vector3 floorpos = go.transform.position;
				GameObject.Instantiate (stage, floorpos + new Vector3 (0f, 0.1f, 0f), Quaternion.identity);
				GameController.readyToStart = true;
			}
		}
	}

	// 新しい平面が検出された場合
	public void AddAnchor(ARPlaneAnchor arPlaneAnchor)
	{
		// Anchorに合わせて新しい平面オブジェクト生成
		go = CreatePlaneInScene(arPlaneAnchor);
		// 生成した平面オブジェクトを管理用Listに登録
		ARPlaneAnchorGameObject arpag = new ARPlaneAnchorGameObject();
		arpag.planeAnchor = arPlaneAnchor;
		arpag.gameObject = go;
		planeAnchorMap.Add(arPlaneAnchor.identifier, arpag);


		floorfoundflag = true;


	}

	// 平面がなくなった場合
	public void RemoveAnchor(ARPlaneAnchor arPlaneAnchor)
	{
		if (planeAnchorMap.ContainsKey(arPlaneAnchor.identifier))
		{
			ARPlaneAnchorGameObject arpag = planeAnchorMap[arPlaneAnchor.identifier];
			Destroy(arpag.gameObject);
			planeAnchorMap.Remove(arPlaneAnchor.identifier);
		}
	}

	// 平面が更新された場合
	public void UpdateAnchor(ARPlaneAnchor arPlaneAnchor)
	{
		if (planeAnchorMap.ContainsKey(arPlaneAnchor.identifier))
		{
			ARPlaneAnchorGameObject arpag = planeAnchorMap[arPlaneAnchor.identifier];
			UpdatePlaneWithAnchorTransform(arpag.gameObject, arPlaneAnchor);
			arpag.planeAnchor = arPlaneAnchor;
			planeAnchorMap[arPlaneAnchor.identifier] = arpag;
		}
	}

	private GameObject CreatePlaneInScene(ARPlaneAnchor arPlaneAnchor)
	{
		// 新しい平面オブジェクトを生成
		GameObject newPlane;
		if (planePrefab != null)
		{
			newPlane = Instantiate(planePrefab);
		}
		else
		{
			newPlane = new GameObject();
		}

		newPlane.name = arPlaneAnchor.identifier;
		// 生成した平面オブジェクトをAnchorに合わせる
		return UpdatePlaneWithAnchorTransform(newPlane, arPlaneAnchor);
	}

	private GameObject UpdatePlaneWithAnchorTransform(GameObject plane, ARPlaneAnchor arPlaneAnchor)
	{
		// ARKit座標をUnity座標に変換
		plane.transform.position = UnityARMatrixOps.GetPosition(arPlaneAnchor.transform);
		plane.transform.rotation = UnityARMatrixOps.GetRotation(arPlaneAnchor.transform);

		MeshFilter mf = plane.GetComponentInChildren<MeshFilter>();

		if (mf != null)
		{
			//since our plane mesh is actually 10mx10m in the world, we scale it here by 0.1f
			mf.gameObject.transform.localScale = new Vector3(arPlaneAnchor.extent.x * 0.1f, arPlaneAnchor.extent.y * 0.1f, arPlaneAnchor.extent.z * 0.1f);
			//convert our center position to unity coords
			mf.gameObject.transform.localPosition = new Vector3(arPlaneAnchor.center.x, arPlaneAnchor.center.y, -arPlaneAnchor.center.z);
		}

		return plane;
	}


}
