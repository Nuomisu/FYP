using UnityEngine;
using System.Collections;
using Component;
using Joint;
using System.Xml;
using System;
using System.Collections.Generic;
using System.Globalization;


public class first : MonoBehaviour {

	public GameObject obj0,obj1,obj2,obj3,obj4,obj5;

	private GameObject clone0, clone2;
	//public component Torso, head, hand2;
	public component Torso, head, hand, hand2, leg, leg2;
	public joint torsoHand, torsoHead, handHand2, torsoLeg, legLeg2;

	public GameObject handP;
	private float startTime;
	private bool resetTime = false;
	private bool othersCanMove = true;

	private List<string> nameList = new List<string>();
	private List<Vector3> position = new List<Vector3>();
	private List<Vector3> scale = new List<Vector3>();

	private component root = null;

	private string bone = "bone";
	private string bone_properties = "bone_properties";
	private string bone_name = "bone_name";
	private string bone_size = "bone_size";
	private string joint_pos_parent = "joint_pos_parent";
	private string joint_pos_child = "joint_pos_child";
	private string bone_type = "bone_type";
	private string rotation_about_xyz = "rotation_about_xyz";
	private string children = "children";
	private string x_ = "x";
	private string y_ = "y";
	private string z_ = "z";
	

	private Stack mystack = new Stack();
	private component currentbone = null;
	//private component tempBone = null;
	
	//private Dictionary<string, joint> jointMap = new Dictionary<string, joint> ();
	private Dictionary<string, Dictionary<string, joint>> jointMap = new Dictionary<string, Dictionary<string, joint>> ();
	private List<joint> jointList = new List<joint>();
	private joint currentJoint = null;

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


	void Start() {

		readXML ();

		getJointSequence (root.getName(), jointMap);

		foreach (joint temp in jointList) {
			Debug.Log( "=====JointList: parent and child" +temp.getJointParentName()+" "+temp.getJointChildName());	
		}

		startTime = Time.time;

		jointList [0].prepare ();
		jointList [0].setJointEnableTrue ();

		//clone0 = (GameObject)Instantiate(obj0, new Vector3(0f,0f,0f), Quaternion.identity);
		//Instantiate(obj1, new Vector3(0,0,0), Quaternion.identity);
		//clone2 = (GameObject)Instantiate(obj2, new Vector3(0f,0f,0f), Quaternion.identity);
		//Instantiate(obj3, new Vector3(0,0,0), Quaternion.identity);
		//Instantiate(obj4, new Vector3(0,0,0), Quaternion.identity);
		//Instantiate(obj5, new Vector3(0,0,0), Quaternion.identity);

		//handP = new GameObject ();
		//handP.name = "handWrapper";

		//clone2.name = "hand";
		//clone2.transform.parent = handP.transform;
		//clone2.transform.localPosition = new Vector3 (0,0,50);

		//hand = new component (clone2);
		//hand.positionXYZ (new Vector3(0,0,0));
		//Debug.Log (hand.getPosition());
		/*

		Torso = new component("Torso", new Color(1, 0, 0));
		Torso.positionXYZ(new Vector3(-0.06565475f, -4.979904f, 0.9601841F));
		Torso.scaleV3 (new Vector3 (22.789801f,  49.377800f,34.184700f));

		head = new component("head", new Color(0, 1, 0));
		head.positionXYZ(new Vector3(-0.09509087f, 20.58524f, 3.715981f));
		head.scaleV3 (new Vector3 (22.789801f,  14.836999f, 22.789780f));

		hand = new component("hand", new Color(1, 1, 0));
		hand.positionXYZ (new Vector3(10.21297f, 16.50324f, -10.25298f));
		hand.scaleV3 (new Vector3 (20.881901f,  26.588009f, 20.905399f));

		hand2 = new component("hand2", new Color(1, 0.7f, 0));
		hand2.positionXYZ (new Vector3(9.646242f, -11.17308f, -10.06381f));
		hand2.scaleV3 (new Vector3 (20.882000f,  37.982880f, 20.911100f));

		leg = new component("leg", new Color(0 , 0, 1));
		leg.positionXYZ (new Vector3(10.15125f, 19.20372f, 9.9681f));
		leg.scaleV3 (new Vector3 (20.546900f,  23.123299f, 20.873301f));
		
		leg2 = new component("leg2", new Color(0 , 0.6f, 1));
		leg2.positionXYZ (new Vector3(10.41975f, -9.690727f, 10.45223f));
		leg2.scaleV3 (new Vector3 (20.874100f,  41.781197f, 20.903000f));
		//torsohand = new joint (Torso, hand);

		Vector3 localJointOnParent = new Vector3 ( 0/22.789801f, (50- 49.377800f/2)/50f, 0/34.184700f);
		Vector3 localJointOnChild = new Vector3 (0/22.789801f, (0- 14.836999f/2)/14.836999f, 0/22.789780f);
			//(joint.x / parent.length_x, (joint.y-parent.length_y/2)/parent.length_y, joint.z/parent.length_z)
		torsoHead = new joint (Torso, head, localJointOnParent, localJointOnChild, 0f, 0f, 0f);
		torsoHead.prepare ();
		torsoHead.setJointEnableTrue ();


		localJointOnParent = new Vector3 ( 0/22.789801f, (40 - 49.377800f/2)/50f, 20/34.184700f);
		localJointOnChild = new Vector3 (0/20.881901f, (0 - 26.588009f/2)/26.588009f, 0/20.905399f);
		torsoHand = new joint (Torso, hand, localJointOnParent, localJointOnChild, 0f, 0f, -45f);

		torsoHand.setJointEnableFalse ();


		localJointOnParent = new Vector3 ( 0/20.881901f, (28- 26.588009f/2)/26.588009f, 0/20.905399f);
		localJointOnChild = new Vector3 (0/20.8820f, (0- 37.982880f/2)/37.982880f, 0/20.911100f);
		Debug.Log ("localJointOnParent: "+localJointOnParent);
		Debug.Log ("localJointOnChild: "+localJointOnChild);
		handHand2 = new joint (hand, hand2, localJointOnParent, localJointOnChild, 0f, 0f, 0f);

		handHand2.setJointEnableFalse ();


		localJointOnParent = new Vector3 ( 0/22.789801f, (-11.6f-49.377800f/2)/49.377800f, 0/34.184700f);
		localJointOnChild = new Vector3 (0/20.546900f, (0- 23.123299f/2)/23.123299f, 0/20.903000f);
		torsoLeg = new joint (Torso, leg, localJointOnParent, localJointOnChild, 0f, 0f, 180f);
		Debug.Log ("Torso and leg: local joint parent "+localJointOnParent);
		Debug.Log ("Torso and leg: local joint child "+localJointOnChild);
		torsoLeg.setJointEnableFalse ();


		localJointOnParent = new Vector3 (0 / 20.546900f, (23 - 23.123299f / 2) / 23.123299f, 0/ 20.873301f);
		localJointOnChild = new Vector3 (0/20.874100f, (0- 41.781197f/2)/ 41.781197f, 0/20.905399f);
		legLeg2 = new joint (leg, leg2, localJointOnParent, localJointOnChild, 0f, 0f, 0f);

		legLeg2.setJointEnableFalse (); */
	}

	void Update(){
		//Debug.Log (torsoHead.jointEnable());
	//	Debug.Log (torsoHand.jointEnable());
	//	Debug.Log (handHand2.jointEnable());


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
							//Debug.Log("Enable torsoHand");
						}
					} else {
						temp.rotate();
					}
				}
			}
		}



		/*
		if ( torsoHead.jointEnable() ) {
			//Debug.Log ("joint 1");
			if (torsoHead.jointMoveEnable()){
				//Debug.Log ("joint 1 move");
				//Debug.Log (distCovered);
				torsoHead.move(startTime);
			}

			if (torsoHead.jointRotateEnable()){

				//Debug.Log(torsoHead.enableRotate);
				if (torsoHead.rotate()){
					torsoHand.prepare();
					torsoHand.setJointEnableTrue();
					startTime = Time.time;
					Debug.Log("Enable torsoHand");
				}

			}
		} 

		if (torsoHand.jointEnable() ) {
		
			//Debug.Log ("joint 2");
			if (torsoHand.jointMoveEnable()){
				
				//Debug.Log (distCovered);
				torsoHand.move(startTime);
			}
			
			if (torsoHand.jointRotateEnable()){
				//Debug.Log("I am in the rorate");
				//Debug.Log(torsoHead.enableRotate);
				//Debug.Log(torsoHead.enableRotate);
				if (torsoHand.rotate()){
					handHand2.prepare();
					handHand2.setJointEnableTrue();
					startTime = Time.time;
					Debug.Log("Enable handHand2");
				}
			}
		}

		if (handHand2.jointEnable() ) {
			//Debug.Log ("joint 3");
			if (handHand2.jointMoveEnable()){
				
				//Debug.Log (distCovered);
				handHand2.move(startTime);
			}
			
			if (handHand2.jointRotateEnable()){
				//Debug.Log(torsoHead.enableRotate);
				if (handHand2.rotate()){
					torsoLeg.prepare();
					torsoLeg.setJointEnableTrue();
					startTime = Time.time;
					Debug.Log("Enable torsoLog");
				}
			}
		}

		if (torsoLeg.jointEnable() ) {
			//Debug.Log ("joint 3");
			if (torsoLeg.jointMoveEnable()){
				
				//Debug.Log (distCovered);
				torsoLeg.move(startTime);
			}
			
			if (torsoLeg.jointRotateEnable()){
				//Debug.Log(torsoHead.enableRotate);
				if (torsoLeg.rotate()){
					legLeg2.prepare();
					legLeg2.setJointEnableTrue();
					startTime = Time.time;
					Debug.Log("Enable legLeg2");
				}
			}
		}

		if (legLeg2.jointEnable() ) {
			//Debug.Log ("joint 3");
			if (legLeg2.jointMoveEnable()){
				
				//Debug.Log (distCovered);
				legLeg2.move(startTime);
			}
			
			if (legLeg2.jointRotateEnable()){
				//Debug.Log(torsoHead.enableRotate);
				legLeg2.rotate();
			}
		}*/
	}

	private void readXML(){

		XmlTextReader reader = new XmlTextReader("skeleton.xml");
		while(reader.Read())
		{
			switch(reader.NodeType)
			{
			case XmlNodeType.Element: //The node is an element



				//Debug.Log(reader.Name.ToString());
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
					//current.name = reader.Name.tostring
					reader.Read();

					//Debug.Log("+++++++++" + reader.Value.ToString());
					if (reader.NodeType == XmlNodeType.Text){
						//Debug.Log("@@@@@@@@");

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

					int i =0;
					for (;i<3;){
						reader.Read();

						if (reader.NodeType == XmlNodeType.Text){

							if (i == 0){

								currentbone.componentSize.x = float.Parse(reader.Value.ToString(), CultureInfo.InvariantCulture.NumberFormat);
								i = i+1;
							}
							else if (i == 1){
								currentbone.componentSize.z = float.Parse(reader.Value.ToString(), CultureInfo.InvariantCulture.NumberFormat);

								i = i+1;
							}
							else if (i == 2){
								currentbone.componentSize.y = float.Parse(reader.Value.ToString(), CultureInfo.InvariantCulture.NumberFormat);

								i = i+1;
							}
						}
					}
					currentbone.newScaleV3();
				} 
				else if (reader.Name.ToString() == joint_pos_parent)
				{
					currentJoint = new joint();

					int i =0;
					for (;i<3;){
						reader.Read();
						
						if (reader.NodeType == XmlNodeType.Text){
							
							if (i == 0){
								
								currentJoint.jointRawLocalPosOfParent.x = float.Parse(reader.Value.ToString(), CultureInfo.InvariantCulture.NumberFormat);
								i = i+1;
							}
							else if (i == 1){
								currentJoint.jointRawLocalPosOfParent.z = float.Parse(reader.Value.ToString(), CultureInfo.InvariantCulture.NumberFormat);
								
								i = i+1;
							}
							else if (i == 2){
								currentJoint.jointRawLocalPosOfParent.y = float.Parse(reader.Value.ToString(), CultureInfo.InvariantCulture.NumberFormat);
								
								i = i+1;
							}
						}
					}

				}
				else if (reader.Name.ToString() == joint_pos_child)
				{
					int i =0;
					for (;i<3;){
						reader.Read();
						
						if (reader.NodeType == XmlNodeType.Text){
							
							if (i == 0){
								
								currentJoint.jointRawLocalPosOfChild.x = float.Parse(reader.Value.ToString(), CultureInfo.InvariantCulture.NumberFormat);
								i = i+1;
							}
							else if (i == 1){
								currentJoint.jointRawLocalPosOfChild.z = float.Parse(reader.Value.ToString(), CultureInfo.InvariantCulture.NumberFormat);
								
								i = i+1;
							}
							else if (i == 2){
								currentJoint.jointRawLocalPosOfChild.y = float.Parse(reader.Value.ToString(), CultureInfo.InvariantCulture.NumberFormat);
								
								i = i+1;
							}
						}
					}
					//currentJoint.prepareLocalJointPosOfParent();

				}
				else if (reader.Name.ToString() == bone_type)
				{
					// bone type = bone_type
				} 
				else if (reader.Name.ToString() == rotation_about_xyz)
				{
					int i =0;
					for (;i<3;){
						reader.Read();
						
						if (reader.NodeType == XmlNodeType.Text){
							
							if (i == 0){
								
								currentJoint.setRotateAngleByX(float.Parse(reader.Value.ToString(), CultureInfo.InvariantCulture.NumberFormat));
								i = i+1;
							}
							else if (i == 1){
								currentJoint.setRotateAngleByZ(float.Parse(reader.Value.ToString(), CultureInfo.InvariantCulture.NumberFormat));
								
								i = i+1;
							}
							else if (i == 2){
								currentJoint.setRotateAngleByY(float.Parse(reader.Value.ToString(), CultureInfo.InvariantCulture.NumberFormat));
								
								i = i+1;
							}
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
					}
					else//: put current as stack.peek's children   stack.peek().push(current)
					{	
						component temp = (component)mystack.Peek();
						temp.pushChild(currentbone);
						
						currentbone.setParent((component)mystack.Peek());

						currentJoint.setJointParent((component)mystack.Peek());
						currentJoint.setJointChild(currentbone);

						Debug.Log("~~~~~~~start prepare local pos of joint");
						currentJoint.prepareLocalJointPosOfParent();
						currentJoint.prepareLocalJointPosOfChild();
						//Debug.Log();


						//jointMap.Add(temp, currentJoint);
						//jointList.Add(currentJoint);


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

		Debug.Log ("------------------------");
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
		//Debug.Log (jointMap[new KeyValuePair<string, string>("torso", "hand")].getJointParentName());
	}


}
