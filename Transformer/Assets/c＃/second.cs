#if UNITY_EDITOR

using UnityEngine;
using System.Collections;
using Component;
using Joint;
using System.Xml;
using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEditor;

public class second : MonoBehaviour {

	private float startTime;
	private bool resetTime = false;
	private bool othersCanMove = true;
	
	private List<string> nameList = new List<string>();
	private List<Vector3> position = new List<Vector3>();
	private List<Vector3> scale = new List<Vector3>();

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
	
	/// <summary>
	/// The mystack: help to store children infor in component
	/// </summary>
	private Stack mystack = new Stack();
	/// <summary>
	/// The component map: help to find component by name
	/// </summary>
	private Dictionary<string, component> componentMap = new Dictionary<string, component> ();
	private List<component> componentList = new List<component>();
	private component currentbone = null;
	/// <summary>
	/// The root: component tree structure 
	/// </summary>
	private component root = null;
	
	/// <summary>
	/// The joint map: help to find joint by name
	/// </summary>
	private Dictionary<string, Dictionary<string, joint>> jointMap = new Dictionary<string, Dictionary<string, joint>> ();
	private List<joint> jointList = new List<joint>();
	private joint currentJoint = null;
	
	public Texture2D btnTexture;
	public Texture2D btnTexture2;
	private bool start = false;
	
	void Start() {
		readXML_skeleton ();
		readXML_infor ();

		// store joint by the sequence into jointList
		getJointSequence (root.getName(), jointMap);
		
		//testAndDebug ();
		
		
		startTime = Time.time;
		
		jointList [0].prepare();
		jointList [0].setJointEnableTrue ();
		
	}
	
	// resume and top button
	public void OnGUI(){
		
		if (GUI.Button (new Rect (70, 10, 50, 50), btnTexture)) {
			start = true;
		}
		
		if (GUI.Button (new Rect (130, 10, 50, 50), btnTexture2)) {
			start = false;
		}
	}
	
	
	
	void Update(){
		if (start){
			for (int i = 0; i< jointList.Count; i++) {
				joint temp = jointList[i];
				
				if (temp.jointEnable()){
					
					if (temp.jointMoveEnable()){
						temp.move(startTime);
					}
					
					if (temp.jointRotateEnable()){
						if (i < jointList.Count-1){
							
							if (temp.rotate()){
								jointList[i+1].prepare();
								jointList[i+1].setJointEnableTrue();
								startTime = Time.time;
							}
						} else {
							temp.rotate();
						}
					}
				}
				
			}	 
		}
	} 
	
	private joint getJointByTwoName(string parent, string child)
	{
		return jointMap[parent][child];
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
	
	
	
	private void readXML_infor(){
		XmlTextReader reader2 = new XmlTextReader("info.xml");
		
		string objName = null;
		string[] seperator = {","};
		
		string fileName = null;
		string boneName = null;
		Vector3 location = new Vector3 ();
		
		while (reader2.Read()){
			switch(reader2.NodeType)
			{
			case XmlNodeType.Element:
				if (reader2.Name.ToString() == original_mesh_path_name){
					reader2.Read();
					
					if (reader2.NodeType == XmlNodeType.Text){
						objName = reader2.Value.ToString();
						string[] objNameString = objName.Split(seperator, StringSplitOptions.RemoveEmptyEntries);
						objName = objNameString[0];
						Debug.Log("Object name: "+objName);
					}
					else 
						Debug.Log("Object name Error");
				} else if (reader2.Name.ToString() == polygon_name){
					reader2.Read();
					
					if (reader2.NodeType == XmlNodeType.Text){
						
						fileName = reader2.Value.ToString();
						Debug.Log("File name: "+original_mesh_path_name);
					}
					else 
						Debug.Log("File name Error");
				} else if (reader2.Name.ToString() == bone_name) {
					reader2.Read();
					
					if (reader2.NodeType == XmlNodeType.Text){
						
						boneName = reader2.Value.ToString();
					}
					else 
						Debug.Log("Bone name Error");				
					
				} else if (reader2.Name.ToString() == origin){
					reader2.Read();
					if (reader2.NodeType == XmlNodeType.Text){
						string tempString = reader2.Value.ToString();
						Debug.Log("Tempstring: "+tempString);
						string[] splitResult = tempString.Split(seperator, StringSplitOptions.RemoveEmptyEntries);
						if (splitResult.Length == 3){
							//Debug.Log(splitResult[0]);
							//Debug.Log(splitResult[1]);
							//Debug.Log(splitResult[2]);
							location.x = float.Parse(splitResult[0], CultureInfo.InvariantCulture.NumberFormat);
							location.y = float.Parse(splitResult[1], CultureInfo.InvariantCulture.NumberFormat);
							location.z = float.Parse(splitResult[2], CultureInfo.InvariantCulture.NumberFormat);
						}
					}
					
				} else if (reader2.Name.ToString() == local_x_repect_to_wolrd) {
					
				} else if (reader2.Name.ToString() == local_y_repect_to_wolrd) {
					
				} else if (reader2.Name.ToString() == local_z_repect_to_wolrd) {
					
				}
				
				break;
				
			case XmlNodeType.EndElement:
				if (reader2.Name.ToString() == mesh_cut_part){
					Debug.Log("filename:"+fileName);
					GameObject temp = AssetDatabase.LoadAssetAtPath("Assets/CuttingParts/uShape/"+fileName, typeof(GameObject)) as GameObject;
					GameObject temp2 =  (GameObject)Instantiate (temp);
					Debug.Log ("@@:" + temp2.name); //mesh_local_0
					
					if (componentMap.ContainsKey(boneName)){
						componentMap[boneName].setMesh(temp2);
						componentMap[boneName].meshPrepare(location);
					}
					
					if (location != null){
						//Debug.Log("!@#@!#!!#@!");
						componentMap[boneName].setPositionXYZ(location);
					}
				}
				
				break;
			}
		}
	}
	
	private void readXML_skeleton(){
		string[] seperator = {","};
		
		XmlTextReader reader = new XmlTextReader("skeleton.xml");
		while(reader.Read())
		{
			switch(reader.NodeType)
			{
			case XmlNodeType.Element: //The node is an element
				
				if (reader.Name.ToString() == (bone))
				{
					currentbone = new component();
					//Debug.Log("Create a new bone");
				}
				else if (reader.Name.ToString() == (bone_properties))
				{
				}
				else if (reader.Name.ToString() == (bone_name))
				{
					
					reader.Read();
				
					if (reader.NodeType == XmlNodeType.Text){		
						if (currentbone == null){
							Debug.Log("No currentBone");
						}
						currentbone.setName(reader.Value.ToString());		
					}
					else 
						Debug.Log("Bone name Error");

				}
				else if (reader.Name.ToString() == (bone_size))
				{
					reader.Read();
					if (reader.NodeType == XmlNodeType.Text){
						string tempString = reader.Value.ToString();
						string[] splitResult = tempString.Split(seperator, StringSplitOptions.RemoveEmptyEntries);
						if (splitResult.Length == 3){
							currentbone.componentSize.x = float.Parse(splitResult[0], CultureInfo.InvariantCulture.NumberFormat);
							currentbone.componentSize.z = float.Parse(splitResult[1], CultureInfo.InvariantCulture.NumberFormat);
							currentbone.componentSize.y = float.Parse(splitResult[2], CultureInfo.InvariantCulture.NumberFormat);
						}
						currentbone.newScaleV3();
					}
					
				} 
				else if (reader.Name.ToString() == joint_pos_parent)
				{
					currentJoint = new joint();
					
					reader.Read();
					if (reader.NodeType == XmlNodeType.Text){
						string tempString = reader.Value.ToString();
						string[] splitResult = tempString.Split(seperator, StringSplitOptions.RemoveEmptyEntries);
						if (splitResult.Length == 3){
							currentJoint.jointRawLocalPosOfParent.x = float.Parse(splitResult[0], CultureInfo.InvariantCulture.NumberFormat);
							currentJoint.jointRawLocalPosOfParent.z = float.Parse(splitResult[1], CultureInfo.InvariantCulture.NumberFormat);
							currentJoint.jointRawLocalPosOfParent.y = float.Parse(splitResult[2], CultureInfo.InvariantCulture.NumberFormat);
						}
					}
					
					
				}
				else if (reader.Name.ToString() == joint_pos_child)
				{
					reader.Read();
					if (reader.NodeType == XmlNodeType.Text){
						string tempString = reader.Value.ToString();
						string[] splitResult = tempString.Split(seperator, StringSplitOptions.RemoveEmptyEntries);
						if (splitResult.Length == 3){
							currentJoint.jointRawLocalPosOfChild.x = float.Parse(splitResult[0], CultureInfo.InvariantCulture.NumberFormat);
							currentJoint.jointRawLocalPosOfChild.z = float.Parse(splitResult[1], CultureInfo.InvariantCulture.NumberFormat);
							currentJoint.jointRawLocalPosOfChild.y = float.Parse(splitResult[2], CultureInfo.InvariantCulture.NumberFormat);
						}
					}	
				}
				else if (reader.Name.ToString() == bone_type)
				{
					// bone type = bone_type
				} 
				else if (reader.Name.ToString() == rotation_about_xyz)
				{
					reader.Read();
					if (reader.NodeType == XmlNodeType.Text){
						string tempString = reader.Value.ToString();
						string[] splitResult = tempString.Split(seperator, StringSplitOptions.RemoveEmptyEntries);
						if (splitResult.Length == 3){
							currentJoint.setRotateAngleByX(float.Parse(splitResult[0], CultureInfo.InvariantCulture.NumberFormat));
							currentJoint.setRotateAngleByZ(float.Parse(splitResult[1], CultureInfo.InvariantCulture.NumberFormat));
							currentJoint.setRotateAngleByY(float.Parse(splitResult[2], CultureInfo.InvariantCulture.NumberFormat));
						}
					}
					
				} 
				else if (reader.Name.ToString() == children)
				{
					mystack.Push(currentbone);
				}
				
				break;
				
			case XmlNodeType.EndElement: //Display the end of the element
				//Debug.Log(reader.Name.ToString());
				if (reader.Name.ToString() == children)
				{
					mystack.Pop ();
				}
				else if (reader.Name.ToString() == bone_properties)
				{
					//Debug.Log("!!!!!Current bone: "+currentbone.getName());
					
					if (root == null) {
						root = currentbone;
						
						componentMap.Add(currentbone.getName(), currentbone);
					}
					else//: put current as stack.peek's children   stack.peek().push(current)
					{	
						component temp = (component)mystack.Peek();
						temp.pushChild(currentbone);
						
						currentbone.setParent((component)mystack.Peek());
						
						// save the component of skeleton
						componentMap.Add(currentbone.getName(), currentbone);
						
						
						currentJoint.setJointParent((component)mystack.Peek());
						currentJoint.setJointChild(currentbone);
						
						//Debug.Log("~~~~~~~start prepare local pos of joint");
						currentJoint.prepareLocalJointPosOfParent();
						currentJoint.prepareLocalJointPosOfChild();
						//Debug.Log("Local pos of joint(pa):" + currentJoint.getJointLocalPosOfParent());
						//Debug.Log("Local pos of joint(chi):" + currentJoint.getJointLocalPosOfChild());
						
						// save the joint of skeleton
						if(jointMap.ContainsKey(currentJoint.getJointParentName())){
							jointMap[currentJoint.getJointParentName()].Add(currentJoint.getJointChildName(), currentJoint);
						}
						else {
							Dictionary<string, joint> tempMap = new Dictionary<string, joint>();
							tempMap.Add(currentJoint.getJointChildName(), currentJoint);
							jointMap.Add(currentJoint.getJointParentName(), tempMap);
						}
						
					}
					
				} else if (reader.Name.ToString() == joint_pos_child)
				{
					
				}
				
				break;
			}
		}
		
	}
	
	private void testAndDebug(){
		
		Debug.Log (root.getName());
		for (int i = 0; i< root.childrenList.Count; i++) {
			component temp = root.childrenList[i];
			Debug.Log(temp.getName());
			Debug.Log(temp.componentSize);
			for (int j=0; j< temp.childrenList.Count;j++){
				Debug.Log(temp.childrenList[j].getName());
				Debug.Log(temp.childrenList[j].componentSize);
			}
		} 
		
		foreach (joint temp in jointList) {
			Debug.Log( "=====JointList: parent and child" +temp.getJointParentName()+" "+temp.getJointChildName());	
		}
		
		Debug.Log ("####ComponetList");
		for (int i=0; i<componentList.Count; i++) {
			Debug.Log("##Name: "+componentList[i].getName());		
		}
		
		Debug.Log ("------------------------");
		foreach (KeyValuePair<string, component> entry in componentMap) {
			Debug.Log("Name(key): "+entry.Key);
			Debug.Log("Name(Value): "+entry.Value.getName());
		}
		
		foreach(KeyValuePair<string, Dictionary<string, joint>> entry in jointMap)
		{
			
			foreach(KeyValuePair<string, joint> entryInner in entry.Value)
			{
				Debug.Log ("****************************");
				Debug.Log("Parent key => " + entry.Key);
				Debug.Log("Child key => " + entryInner.Key);
				
				Debug.Log("joint parent&child => "+ entryInner.Value.getJointParentName()+ entryInner.Value.getJointChildName());
				Debug.Log("joint pos in parent => "+entryInner.Value.getJointRawLocalPosOfParent());
				Debug.Log("*joint pos in parent => "+entryInner.Value.getJointLocalPosOfParent());
				Debug.Log("joint pos in child =>"+entryInner.Value.getJointRawLocalPosOfChild());
				Debug.Log("*joint pos in child => "+entryInner.Value.getJointLocalPosOfChild());
				Debug.Log("joint rotation =>"+entryInner.Value.getRotateAngleByX()+" "+entryInner.Value.getRotateAngleByY()+" "+entryInner.Value.getRotateAngleByZ());
			}
		}
	}
}

#endif