using UnityEngine;
using System.Collections;
using System.Xml;
using Component;
using Joint;
using System.Collections.Generic;
using System.Globalization;


public class LoadXML : MonoBehaviour {
	private float startTime;
	private bool resetTime = false;
	private bool othersCanMove = true;


	const string bone = "bone";
	const string bone_properties = "bone_properties";
	const string bone_name = "bone_name";
	const string bone_size = "bone_size";
	const string joint_pos_parent = "joint_pos_parent";
	const string joint_pos_child = "joint_pos_child";
	const string bone_type = "bone_type";
	const string rotation_about_xyz = "rotation_about_xyz";
	const string children = "children";
	const string x_ = "x";
	const string y_ = "y";
	const string z_ = "z";
	
	const string original_mesh_path_name = "original_mesh_path_name";
	const string skeleton_mesh_path_name = "skeleton_mesh_path_name";
	const string mesh_cut_part = "mesh_cut_part";
	const string polygon_name = "polygon_name";
	const string origin = "origin";
	const string bone_coord_respect_to_world = "bone_coord_respect_to_world";
	const string local_x_repect_to_wolrd = "local_x_repect_to_wolrd";
	const string local_y_repect_to_wolrd = "local_y_repect_to_wolrd";
	const string local_z_repect_to_wolrd = "local_z_repect_to_wolrd";

	private	string test = "test";


	private component root;  // store the component tree
	private List<joint> jointList = new List<joint>(); // store all the joints

	private Dictionary<string, component> componentMap = new Dictionary<string, component> (); //The component map: help to find component by name
	private Dictionary<string, Dictionary<string, joint>> jointMap = new Dictionary<string, Dictionary<string, joint>> ();// The joint map: help to find joint by name

	//private component currentbone = null;
	private joint currentJoint = null;

	private Stack mystack = new Stack();

	char[] spliter = {','};

	void Start()
	{
		// read skeleton.xml
		TextAsset textXML = (TextAsset)Resources.Load("skeleton", typeof(TextAsset));
		XmlDocument xml = new XmlDocument();
		xml.LoadXml(textXML.text);

		XmlNode rootXML = xml.FirstChild;
		root = process (rootXML);

		test = rootXML.Name.ToString ();

		// read infor.xml
		TextAsset textXML2 = (TextAsset)Resources.Load("info", typeof(TextAsset));
		XmlDocument xml2 = new XmlDocument();
		xml2.LoadXml(textXML2.text);

		uploadMesh (xml2);



		getJointSequence (root.getName(), jointMap);

		
		DebugTest ();
	}

	void OnGUI(){
		GUI.Label (new Rect(20, 20, 50, 50), test);
	}

	private void getJointSequence(string start, Dictionary<string, Dictionary<string, joint>> myMap){
		if (myMap.ContainsKey (start)) {
			Dictionary<string, joint> temp = myMap [start];
			foreach(KeyValuePair<string, joint> entry in temp) {
				jointList.Add(entry.Value);
				getJointSequence(entry.Key, myMap);
			}
		}
	}
	
	void uploadMesh(XmlDocument xml){
		string objName = "";
		string boneName = "";
		string fileName = "";

		XmlNode rootXML = xml.FirstChild;
		if (rootXML.ChildNodes[0].Name.ToString () == original_mesh_path_name) 
		{
			string inner = rootXML.ChildNodes[0].InnerText;
			string[] innerArr = inner.Split(spliter, System.StringSplitOptions.RemoveEmptyEntries);
			objName = innerArr[0];
		}


		XmlNodeList elemList = xml.GetElementsByTagName ("mesh_cut_part");
		Debug.Log (elemList.Count);

		foreach (XmlNode node in elemList) {
			if(node.ChildNodes[0].Name.ToString() == polygon_name){
				fileName = node.ChildNodes[0].InnerText;
				fileName = fileName.Substring(0, fileName.IndexOf("."));
				Debug.Log("File Name: "+fileName);
			}
			if(node.ChildNodes[1].Name.ToString() == bone_name){
				boneName = node.ChildNodes[1].InnerText;
				Debug.Log("Bone Name: "+boneName);
			}


			GameObject instance = (GameObject)Instantiate(Resources.Load(fileName, typeof(GameObject)));
		

			GameObject newInstance = meshProcess(instance);

			GameObject goodMesh = (GameObject)Instantiate(newInstance);

			Destroy(instance);

			goodMesh.name = "M_"+boneName;

			if (componentMap.ContainsKey(boneName)){
				Debug.Log("OOOOOOOOOO "+boneName);
				componentMap[boneName].setMesh(goodMesh);
			}
			
		}
		
		
	}

	GameObject meshProcess(GameObject meshContainer){

		GameObject obj;
		MeshFilter viewedModelFilter = meshContainer.GetComponentInChildren<MeshFilter> ();
		Mesh targetMesh = viewedModelFilter.mesh;

		obj = meshContainer.transform.Find("default").gameObject;
		Vector3 p;

		Debug.Log ("@@@@@" +obj.name);

		Bounds b = targetMesh.bounds;
		Vector3 offset = -1 * b.center;
		Vector3 last_p = new Vector3(offset.x / b.extents.x, offset.y / b.extents.y, offset.z / b.extents.z);
		p = Vector3.zero;

		Vector3 diff = Vector3.Scale(targetMesh.bounds.extents, last_p - p); //Calculate difference in 3d position
		obj.transform.position -= Vector3.Scale(diff, obj.transform.localScale); //Move object position by taking localScale into account
		//Iterate over all vertices and move them in the opposite direction of the object position movement
		Vector3[] verts = targetMesh.vertices; 
		for(int i=0; i<verts.Length; i++) {
			verts[i] += diff;
		}
		targetMesh.vertices = verts; //Assign the vertex array back to the mesh
		targetMesh.RecalculateBounds(); //Recalculate bounds of the mesh, for the renderer's sake


		Debug.Log ("@@@@@@@@@  "+targetMesh.vertexCount);

		return obj;
	}

	component process (XmlNode currentNode)
	{
		component currentbone = new component ();

		if (currentNode.NodeType == XmlNodeType.Element && currentNode.Name.ToString() == bone)
		{	
			// first node should be bone_properties
			XmlNode node = currentNode.ChildNodes[0];
		
			if (node.Name.ToString() == bone_properties)
			{

				if (node.ChildNodes[0].Name.ToString() == bone_name)
				{
					currentbone.setName(node.ChildNodes[0].InnerText);
					Debug.Log("Bone name: "+node.ChildNodes[0].InnerText);
				}

				if (node.ChildNodes[1].Name.ToString() == bone_size)
				{
					string sizeStr = node.ChildNodes[1].InnerText;

					string[] sizeArr = sizeStr.Split(spliter, System.StringSplitOptions.RemoveEmptyEntries);

					Vector3 cubeSize = new Vector3();
					cubeSize.x = float.Parse(sizeArr[0], CultureInfo.InvariantCulture.NumberFormat);
					cubeSize.y = float.Parse(sizeArr[1], CultureInfo.InvariantCulture.NumberFormat);
					cubeSize.z = float.Parse(sizeArr[2], CultureInfo.InvariantCulture.NumberFormat);

					currentbone.cubeScaleV3(cubeSize);
					Debug.Log("Bone size: "+node.ChildNodes[1].InnerText);
				}

				if (node.ChildNodes[2].Name.ToString() == joint_pos_parent)
				{
					currentJoint = new joint();

					string joint_pos_pa = node.ChildNodes[2].InnerText;
					string[] sizeJPP = joint_pos_pa.Split(spliter, System.StringSplitOptions.RemoveEmptyEntries);

					currentJoint.rawJointPosOfParentBasedPa.x= float.Parse(sizeJPP[0], CultureInfo.InvariantCulture.NumberFormat);
					currentJoint.rawJointPosOfParentBasedPa.z= float.Parse(sizeJPP[1], CultureInfo.InvariantCulture.NumberFormat);
					currentJoint.rawJointPosOfParentBasedPa.y= float.Parse(sizeJPP[2], CultureInfo.InvariantCulture.NumberFormat);

					Debug.Log("joint_pos_parent: "+node.ChildNodes[2].InnerText);
				}

				if (node.ChildNodes[3].Name.ToString() == joint_pos_child)
				{
					string joint_pos_ch = node.ChildNodes[3].InnerText;
					string[] sizeJPH = joint_pos_ch.Split(spliter, System.StringSplitOptions.RemoveEmptyEntries);
					
					currentJoint.rawJointPosOfChildBasedPa.x= float.Parse(sizeJPH[0], CultureInfo.InvariantCulture.NumberFormat);
					currentJoint.rawJointPosOfChildBasedPa.z= float.Parse(sizeJPH[1], CultureInfo.InvariantCulture.NumberFormat);
					currentJoint.rawJointPosOfChildBasedPa.y= float.Parse(sizeJPH[2], CultureInfo.InvariantCulture.NumberFormat);

					Debug.Log("joint_pos_child: "+node.ChildNodes[3].InnerText);
				}

				if (node.ChildNodes[4].Name.ToString() == bone_type)
				{
					currentbone.setType(node.ChildNodes[4].InnerText);

					Debug.Log("bone_type: "+node.ChildNodes[4].InnerText);
				}

				if (node.ChildNodes[5].Name.ToString() == rotation_about_xyz)
				{
					string rotation = node.ChildNodes[5].InnerText;
					string[] rotationArr = rotation.Split(spliter, System.StringSplitOptions.RemoveEmptyEntries);
					
					currentJoint.setRotateAngleByX(float.Parse(rotationArr[0], CultureInfo.InvariantCulture.NumberFormat));
					currentJoint.setRotateAngleByZ(float.Parse(rotationArr[1], CultureInfo.InvariantCulture.NumberFormat));
					currentJoint.setRotateAngleByY(float.Parse(rotationArr[2], CultureInfo.InvariantCulture.NumberFormat));
					
					Debug.Log("rotation_about_xyz: "+node.ChildNodes[5].InnerText);
				}


			} else {
				Debug.Log("Bone don't have properties.");
			}
			// after finish one bone_properties
			// if mystack is empty, this bone is root, do not save joint, put this bone into mystack
			if (mystack.Count==0)
			{
				componentMap.Add(currentbone.getName(), currentbone);
			}
			else{

				component tempParent = (component) mystack.Peek();

				tempParent.pushChild(ref currentbone);
				currentbone.setParent(ref tempParent);

				currentJoint.setJointParent(ref tempParent);
				currentJoint.setJointChild(ref currentbone);

				componentMap.Add(currentbone.getName(), currentbone);
				if(jointMap.ContainsKey(currentJoint.getJointParentName())){
					jointMap[currentJoint.getJointParentName()].Add(currentJoint.getJointChildName(), currentJoint);
				}
				else {
					Dictionary<string, joint> tempMap = new Dictionary<string, joint>();
					tempMap.Add(currentJoint.getJointChildName(), currentJoint);
					jointMap.Add(currentJoint.getJointParentName(), tempMap);
				}

			}
			
			
			// if bone has children
			if (currentNode.ChildNodes.Count == 2)
			{
				Debug.Log("!Has children!");

				mystack.Push(currentbone);

				XmlNode node2 = currentNode.ChildNodes[1];
				if (node2.Name.ToString() == children)
				{
					foreach( XmlNode n in node2.ChildNodes)
					{
						if (n.Name.ToString()== bone)
						{
							process(n);
						}
					}
				}

				mystack.Pop();
			}

			
		} // finish bone 
	
		return currentbone;
	} // finish process method


	void DebugTest(){
		Debug.Log ("==============================");
		
		showBone (root);

		Debug.Log ("------------------------------");

		foreach (joint temp in jointList) {
			Debug.Log( "JointList: parent and child: " +temp.getJointParentName()+" "+temp.getJointChildName());	
		}
		
		
	}
	void showBone(component bone){
		
		Debug.Log ("Bone name: "+bone.getName());
		Debug.Log ("Bone type: "+bone.getBoneType());
		Debug.Log ("Bone size: " + bone.componentSize);
		Debug.Log ("Bone mesh: " + bone.mesh.name);

		for (int i=0; i<bone.childrenList.Count; i++) {
			showBone (bone.childrenList[i]);		
		}
	}
}




