using UnityEngine;
using System.Collections;


public class myGUI : MonoBehaviour {
	public Texture btnTexture;
	private GameObject input;

	private string target = "";
	private string percentage = "";
	private float per = 0f;
	private Rect positionTarget = new Rect (290, 430, 100, 50);

	private GameObject originalOne;
	private GameObject oldOne;
	private GameObject newOne;
	private Plane splitter;
	private Mesh oldmesh;
	
	private Vector3[] hitedvertices;


	// Use this for initialization
	void Start () {
		//originalOne = input;
		//oldmesh = originalOne.GetComponent<MeshFilter>().mesh;
		//hitedvertices = oldmesh.vertices;

	}
	RaycastHit hit;


	private void OnGUI(){
		percentage = GUI.TextField (new Rect (180, 430, 70, 30), percentage, 25);


		if (GUI.Button (new Rect (10, 10, 50, 50), btnTexture)) {
			
			per = float.Parse(percentage);
			Debug.Log("Per: "+per);
			cut(per);
		}
		
		
		GUI.Label(positionTarget, "Cut target: "+target +"   "+percentage);	
	}

	// Update is called once per frame
	void Update () {

		Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
		if (Physics.Raycast (ray, out hit, 1000)) {


			if (Input.GetMouseButtonDown(1)){
				Debug.Log(hit.collider.name);
				target = hit.collider.name;
				input = hit.collider.gameObject;
				originalOne = input;
				oldmesh = originalOne.GetComponent<MeshFilter>().mesh;
				hitedvertices = oldmesh.vertices;
			}

			/*
			if (Input.GetMouseButtonDown(0)){


				Debug.Log("==========");
				//Debug.Log(input.GetComponent<MeshCollider>().convex);



				originalOne = input;
				oldmesh = originalOne.GetComponent<MeshFilter>().mesh;
				hitedvertices = oldmesh.vertices;
				Debug.Log("Click one name: "+input.name);
				Debug.Log("Oldmesh: "+oldmesh.vertexCount);
				Debug.Log("hiteVertices: "+hitedvertices.Length);
			} */


		}

		Debug.DrawRay (ray.origin, ray.direction*1000, Color.yellow);
	}



	private void cut(float percen){
		
		Vector3 cutPoLocal = new Vector3 (0.0f, 0.0f, percen);
		Vector3 cutPo2Local = new Vector3 (0.0f, 1f, percen);
		Vector3 cutPo3Local = new Vector3 (1f, 0.0f,percen);
		
		Vector3 cutPoWorld = originalOne.transform.TransformPoint(cutPoLocal);
		Vector3 cutPo2World = originalOne.transform.TransformPoint(cutPo2Local);
		Vector3 cutPo3World = originalOne.transform.TransformPoint(cutPo3Local);
		
		
		GameObject sphere1 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
		sphere1.transform.position = cutPoWorld;
		GameObject sphere2 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
		sphere2.transform.position = cutPo2World;
		GameObject sphere3 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
		sphere3.transform.position = cutPo3World;
		
		Vector3 vect1 = cutPo2World - cutPoWorld;
		Vector3 vect2 = cutPo3World - cutPoWorld;
		
//		Debug.Log ("V1: "+vect1);
//		Debug.Log ("V2: "+vect2);
		Vector3 daxra = Vector3.Cross(vect1.normalized, vect2.normalized);
		Debug.Log (daxra);
		splitter = new Plane(daxra,cutPoWorld);
		
		GameObject clone = originalOne;
		//clone.name = "duoduo";
		newOne = Instantiate (clone, originalOne.transform.position, originalOne.transform.rotation) as GameObject;
		newOne.name = "111";
		
		
		Mesh newmesh = newOne.GetComponent<MeshFilter>().mesh; 
		Vector3[] newvertices = newmesh.vertices;
		
		for (int i=0; i < newvertices.Length; i++)
		{
			if(splitter.GetSide(newOne.gameObject.transform.TransformPoint(newvertices[i]))){
				newvertices[i] =  newOne.transform.InverseTransformPoint(newOne.transform.TransformPoint(newvertices[i]) - splitter.GetDistanceToPoint(newOne.transform.TransformPoint(newvertices[i])) * splitter.normal);
			}else{
				hitedvertices[i] =  originalOne.transform.InverseTransformPoint(originalOne.transform.TransformPoint(hitedvertices[i]) - splitter.GetDistanceToPoint(originalOne.transform.TransformPoint(hitedvertices[i])) * splitter.normal);
			}
		}
		newmesh.vertices = newvertices;
		newmesh.RecalculateBounds();
		oldmesh.vertices = hitedvertices;
		oldmesh.RecalculateBounds();

	

		if(newOne.GetComponent<MeshCollider>()  ){
			newOne.GetComponent<MeshCollider>().sharedMesh=newmesh;
			newOne.GetComponent<MeshCollider>().convex=true;
			originalOne.GetComponent<MeshCollider>().sharedMesh = oldmesh;
			originalOne.GetComponent<MeshCollider>().convex=true;
		}

		
		
		if(newOne.GetComponent<BoxCollider>()  ){
			Destroy(newOne.GetComponent<BoxCollider>());
			newOne.AddComponent<MeshCollider>();
			newOne.collider.GetComponent<MeshCollider>().sharedMesh = newmesh;
			newOne.collider.GetComponent<MeshCollider>().convex=true;
			Destroy(originalOne.collider.GetComponent<BoxCollider>());
			originalOne.collider.gameObject.AddComponent<MeshCollider>();
			originalOne.collider.GetComponent<MeshCollider>().sharedMesh = oldmesh;
			originalOne.collider.GetComponent<MeshCollider>().convex=true;
		}

		if(newOne.GetComponent<SphereCollider>()  ){
			Destroy(newOne.GetComponent<SphereCollider>());
			newOne.AddComponent<MeshCollider>();
			newOne.collider.GetComponent<MeshCollider>().sharedMesh = newmesh;
			newOne.collider.GetComponent<MeshCollider>().convex=true;
			Destroy(originalOne.collider.GetComponent<SphereCollider>());
			originalOne.collider.gameObject.AddComponent<MeshCollider>();
			originalOne.collider.GetComponent<MeshCollider>().sharedMesh = oldmesh;
			originalOne.collider.GetComponent<MeshCollider>().convex=true;
		}
		
		
		
		if(newOne.GetComponent<CapsuleCollider>() ){
			Destroy(newOne.GetComponent<CapsuleCollider>());
			newOne.AddComponent<MeshCollider>();
			newOne.collider.GetComponent<MeshCollider>().sharedMesh = newmesh;
			newOne.collider.GetComponent<MeshCollider>().convex=true;
			Destroy(originalOne.collider.GetComponent<CapsuleCollider>());
			originalOne.collider.gameObject.AddComponent<MeshCollider>();
			originalOne.collider.GetComponent<MeshCollider>().sharedMesh = oldmesh;
			originalOne.collider.GetComponent<MeshCollider>().convex=true;
		}
		
		
		
	}
}
