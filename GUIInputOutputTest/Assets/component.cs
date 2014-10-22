using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace Component
{
	
	public class component 
	{
		private string componentName;
		public GameObject cube;
		public GameObject mesh;
		public component parent;
		private float rotationAnglebyX, rotationAnglebyY, rotationAnglebyZ;
		public List<component> childrenList = new List<component>();
		public List<Vector3> childrenPosStartList = new List<Vector3>();
		public List<Vector3> childrenPosEndList = new List<Vector3>();

		private string boneType = "";
		public Vector3 componentSize;

		public component(){
			cube = GameObject.CreatePrimitive (PrimitiveType.Cube);	
			rotationAnglebyX = 0f;
			rotationAnglebyY = 0f;
			rotationAnglebyZ = 0f;
			cubeUnEnable ();
		}

		public component(string name,  Color co){
			cube = GameObject.CreatePrimitive (PrimitiveType.Cube);
			cube.name = name;
			cube.renderer.material.color = co;
			rotationAnglebyX = 0f;
			rotationAnglebyY = 0f;
			rotationAnglebyZ = 0f;
		}
	
		public void setMesh(GameObject m){
			mesh = m;
		}

		public void cubeUnEnable(){
			cube.renderer.enabled = false;	
			cube.collider.enabled = false;
		}

		// set the name
		public void setName(string name){
			componentName = name;
			cube.name = name;
		}

		// set the cube size
		public void cubeScaleV3(Vector3 cubeSize){
			cube.transform.localScale = cubeSize;
			componentSize = cubeSize;
		}

		// set the bone type
		public void setType(string t){
			boneType = t;		
		}

		public string getBoneType(){
			return boneType;		
		}

		// get name
		public string getName(){
			return componentName;
		}


		/* may be useful
		public void meshPrepare(Vector3 loc){
			//Debug.Log ("~~~~~~~~~~~~"+getPosition());
			meshPosition (loc);
			mesh.transform.Rotate (-90, 0,180, Space.Self);
		} */

		private void meshPosition(Vector3 loc){
			mesh.transform.position = loc;
		}

		public void setPositionXYZ(Vector3 pos){
			cube.transform.position = pos;
		}

		public Vector3 getPosition(){
			return mesh.transform.position;		
		}

		public Vector3 getLocalPosition(){
			return cube.transform.localPosition;	
		}



		public Vector3 localToWorld(Vector3 local){
			return cube.transform.TransformPoint(local);
		}

		public Vector3 worldToLocal(Vector3 world){
			//GameObject testSpere = GameObject.CreatePrimitive (PrimitiveType.Sphere);
			//testSpere.transform.position = cube.transform.InverseTransformPoint (world);
			return cube.transform.InverseTransformPoint (world);		
		}
	
		public void testDraw(Vector3 pos){
			GameObject sphere = GameObject.CreatePrimitive (PrimitiveType.Sphere);
			sphere.transform.position = cube.transform.TransformPoint(pos);
		}


		public void prepareChildStartPosList(){
			if (childrenList != null) {
				for(int i = 0;i<childrenList.Count;i++){
					childrenPosStartList.Add(childrenList[i].getPosition());
				}
			}
		}

		public void prepareChildEndPosList(Vector3 dis){
			if (childrenList != null) {
				for(int i = 0;i<childrenList.Count;i++){
					childrenPosEndList.Add(childrenList[i].getPosition()+dis);
				}
			}
		}
		public void lerp(float fracJourney, Vector3 start, Vector3 end, bool prepareChildPos){
			cube.transform.position = Vector3.Lerp(start, end, fracJourney);
			mesh.transform.position = Vector3.Lerp (start, end, fracJourney);

			if (prepareChildPos) {
				prepareChildStartPosList();
				prepareChildEndPosList(end - start);	
			}

			if (childrenList.Count != 0){
				for (int i = 0; i < childrenList.Count; i++) {

					childrenList[i].lerp(fracJourney, childrenPosStartList[i], childrenPosEndList[i], true);
				}
			}
		}



		public void rotateAround(Vector3 point, Vector3 axis, float angle){
			cube.transform.RotateAround(point, axis, angle);
			mesh.transform.RotateAround(point, axis, angle);
			if (childrenList.Count != 0){
				for (int i = 0; i < childrenList.Count; i++) {
					childrenList[i].rotateAround(point, axis, angle);
				}
			}
		}

		public void changeLocalPosition(Vector3 newPos){
			cube.transform.localPosition = newPos;
		}

		public Transform getTransform(){
			return cube.transform;		
		}

 		public void setParent(ref component pa){
			parent = pa;
			//cube.transform.parent = pa.getTransform ();
		}

		public component getParent(){
			return parent;
		}

		public void setRotationXYZ(float x, float y, float z){
			rotationAnglebyX = x;
			rotationAnglebyY = y;
			rotationAnglebyZ = z;
		}
		public float getRotationAngleX(){
			return rotationAnglebyX;		
		}
		public float getRotationAngleY(){
			return rotationAnglebyY;		
		}
		public float getRotationAngleZ(){
			return rotationAnglebyZ;		
		}

		public void pushChild(ref component child){
			childrenList.Add (child);
		}



		public void hideCube(){
			cube.renderer.enabled = false;
		}

	}
	
}