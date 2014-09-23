using UnityEngine;
using System.Collections;

public class tryCutter : MonoBehaviour {

	float speed = 3f;
	public bool boxcollider = true;
	public bool splitonce = true;
	public GameObject particle;
	
	private Vector3 daxra;
	private Vector3 hitpoint1;
	private Vector3 hitpoint2;
	private bool primitive = true;
	private Vector3 closestpoint2;
	private Mesh hitedmesh;
	private Vector3[] hitedvertices;
	

	void OnTriggerEnter(Collider other2){
		hitpoint1 =  other2.collider.ClosestPointOnBounds(transform.position); 
		hitedmesh = other2.GetComponent<MeshFilter>().mesh;
		hitedvertices = hitedmesh.vertices;
	}
	
	void OnTriggerExit(Collider other){
		hitpoint2 = other.collider.ClosestPointOnBounds(transform.position);
		
		if(other.gameObject.tag !="splitted"){
			if(other.gameObject.tag=="primitive"){
				primitive=true;
			}else{
				primitive=false;
			}

			Debug.Log("Primtive: "+primitive);

			Vector3 closestpoint = other.ClosestPointOnBounds(transform.position);
			Instantiate(particle, closestpoint, Quaternion.identity);
			
			GameObject clone = other.transform.gameObject;
			GameObject newobject = Instantiate (clone, other.transform.position, other.transform.rotation) as GameObject;
			
			Mesh newmesh = newobject.GetComponent<MeshFilter>().mesh; 
			Vector3[] newvertices = newmesh.vertices;
			
			daxra = Vector3.Cross((hitpoint1 - hitpoint2).normalized, new Vector3(0,0,1));  
			Plane splitter = new Plane(daxra,(hitpoint1 + hitpoint2)/2);
			
			if(primitive){
				for (int i=0; i < newvertices.Length; i++)
				{
					if(splitter.GetSide(newobject.gameObject.transform.TransformPoint(newvertices[i]))){
						newvertices[i] =  newobject.transform.InverseTransformPoint(newobject.transform.TransformPoint(newvertices[i]) - splitter.GetDistanceToPoint(newobject.transform.TransformPoint(newvertices[i])) * splitter.normal);
					}else{
						hitedvertices[i] =  other.transform.InverseTransformPoint(other.transform.TransformPoint(hitedvertices[i]) - splitter.GetDistanceToPoint(other.transform.TransformPoint(hitedvertices[i])) * splitter.normal);
					}
				}
			}else{
				for (int i=0; i < newvertices.Length; i++)
				{
					if(splitter.GetSide(newobject.gameObject.transform.TransformPoint(newvertices[i]))){
						newvertices[i] = newobject.transform.InverseTransformPoint((hitpoint1 + hitpoint2)/2);
					}else{
						hitedvertices[i] = other.transform.InverseTransformPoint((hitpoint1 + hitpoint2)/2);
					}
				}
			}	
			newmesh.vertices = newvertices;
			newmesh.RecalculateBounds();
			hitedmesh.vertices = hitedvertices;
			hitedmesh.RecalculateBounds();
			
			if(newobject.GetComponent<MeshCollider>() && !boxcollider){
				newobject.GetComponent<MeshCollider>().sharedMesh=newmesh;
				newobject.GetComponent<MeshCollider>().convex=true;
				other.GetComponent<MeshCollider>().sharedMesh = hitedmesh;
				other.GetComponent<MeshCollider>().convex=true;
			}
			
			if(newobject.GetComponent<MeshCollider>() && boxcollider){
				Destroy(newobject.GetComponent<MeshCollider>());
				newobject.AddComponent<BoxCollider>();
				Destroy(other.collider.GetComponent<MeshCollider>());
				other.collider.gameObject.AddComponent<BoxCollider>();	
			}
			
			if(newobject.GetComponent<BoxCollider>() && boxcollider){
				newobject.GetComponent<BoxCollider>().size=newmesh.bounds.size;
				newobject.GetComponent<BoxCollider>().center=newmesh.bounds.center;
				if(newobject.GetComponent<BoxCollider>().size.y <= 0.01){
					Destroy(newobject);
				}
				other.collider.GetComponent<BoxCollider>().size= hitedmesh.bounds.size;
				other.collider.GetComponent<BoxCollider>().center=hitedmesh.bounds.center;
				if(other.collider.GetComponent<BoxCollider>().size.y <= 0.01){
					Destroy(other.collider.gameObject);
				}
			}
			
			if(newobject.GetComponent<BoxCollider>() && !boxcollider){
				Destroy(newobject.GetComponent<BoxCollider>());
				newobject.AddComponent<MeshCollider>();
				newobject.collider.GetComponent<MeshCollider>().sharedMesh = newmesh;
				newobject.collider.GetComponent<MeshCollider>().convex=true;
				Destroy(other.collider.GetComponent<BoxCollider>());
				other.collider.gameObject.AddComponent<MeshCollider>();
				other.collider.GetComponent<MeshCollider>().sharedMesh = hitedmesh;
				other.collider.GetComponent<MeshCollider>().convex=true;
			}
			if(newobject.GetComponent<SphereCollider>() && !boxcollider){
				Destroy(newobject.GetComponent<SphereCollider>());
				newobject.AddComponent<MeshCollider>();
				newobject.collider.GetComponent<MeshCollider>().sharedMesh = newmesh;
				newobject.collider.GetComponent<MeshCollider>().convex=true;
				Destroy(other.collider.GetComponent<SphereCollider>());
				other.collider.gameObject.AddComponent<MeshCollider>();
				other.collider.GetComponent<MeshCollider>().sharedMesh = hitedmesh;
				other.collider.GetComponent<MeshCollider>().convex=true;
			}
			
			if(newobject.GetComponent<SphereCollider>() && boxcollider){
				Destroy(newobject.GetComponent<SphereCollider>());
				newobject.AddComponent<BoxCollider>();
				newobject.GetComponent<BoxCollider>().size=newmesh.bounds.size;
				newobject.GetComponent<BoxCollider>().center=newmesh.bounds.center;
				if(newobject.GetComponent<BoxCollider>().size.y <= 0.01){
					Destroy(newobject);
				}
				Destroy(other.collider.GetComponent<SphereCollider>());
				other.collider.gameObject.AddComponent<BoxCollider>();
				other.collider.GetComponent<BoxCollider>().size= hitedmesh.bounds.size;
				other.collider.GetComponent<BoxCollider>().center=hitedmesh.bounds.center;
				if(other.collider.GetComponent<BoxCollider>().size.y <= 0.01){
					Destroy(other.collider.gameObject);
				}
			}
			
			if(newobject.GetComponent<CapsuleCollider>() && boxcollider){
				Destroy(newobject.GetComponent<CapsuleCollider>());
				newobject.AddComponent<BoxCollider>();
				newobject.GetComponent<BoxCollider>().size= hitedmesh.bounds.size;
				newobject.GetComponent<BoxCollider>().center=hitedmesh.bounds.center;
				if(newobject.GetComponent<BoxCollider>().size.y <= 0.01){
					Destroy(newobject);
				}
				Destroy(other.collider.GetComponent<CapsuleCollider>());
				other.collider.gameObject.AddComponent<BoxCollider>();
				other.collider.GetComponent<BoxCollider>().size= hitedmesh.bounds.size;
				other.collider.GetComponent<BoxCollider>().center=hitedmesh.bounds.center;
				if(other.collider.GetComponent<BoxCollider>().size.y <= 0.01){
					Destroy(other.collider.gameObject);
				}
			}
			
			if(newobject.GetComponent<CapsuleCollider>()&& !boxcollider){
				Destroy(newobject.GetComponent<CapsuleCollider>());
				newobject.AddComponent<MeshCollider>();
				newobject.collider.GetComponent<MeshCollider>().sharedMesh = newmesh;
				newobject.collider.GetComponent<MeshCollider>().convex=true;
				Destroy(other.collider.GetComponent<CapsuleCollider>());
				other.collider.gameObject.AddComponent<MeshCollider>();
				other.collider.GetComponent<MeshCollider>().sharedMesh = hitedmesh;
				other.collider.GetComponent<MeshCollider>().convex=true;
			}
			
			if(splitonce){
				newobject.gameObject.tag="splitted";
				other.collider.gameObject.tag="splitted";
			}
			
			if(!newobject.rigidbody){
				newobject.AddComponent<Rigidbody>();
				other.collider.gameObject.AddComponent<Rigidbody>();
			}
		}
	}
}