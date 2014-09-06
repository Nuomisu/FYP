using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace Component
{
	
	public class component 
	{
		public GameObject cube;
		public component parent;
		private float rotationAnglebyX, rotationAnglebyY, rotationAnglebyZ;
		public List<component> childrenList = new List<component>();
		//private List<Vector3> jointPoint = new List<Vector3> ();

		public Vector3 componentSize;

		public component(){
			cube = GameObject.CreatePrimitive (PrimitiveType.Cube);	
			rotationAnglebyX = 0f;
			rotationAnglebyY = 0f;
			rotationAnglebyZ = 0f;
		}

		public component(string name,  Color co){
			cube = GameObject.CreatePrimitive (PrimitiveType.Cube);
			cube.name = name;
			cube.renderer.material.color = co;
			rotationAnglebyX = 0f;
			rotationAnglebyY = 0f;
			rotationAnglebyZ = 0f;
		}

		public component(string name, GameObject obj){
			cube = obj;
			cube.name = name;
			Debug.Log ("In the component: "+cube.transform.position);
			rotationAnglebyX = 0f;
			rotationAnglebyY = 0f;
			rotationAnglebyZ = 0f;
		}
		public component(GameObject obj){
			cube = obj;
			//Debug.Log ("In the component: "+cube.transform.position);
			rotationAnglebyX = 0f;
			rotationAnglebyY = 0f;
			rotationAnglebyZ = 0f;
		}

		public void positionXYZ(Vector3 pos){
			cube.transform.position = pos;
		}

		public Vector3 getPosition(){
			return cube.transform.position;		
		}

		public Vector3 getLocalPosition(){
			return cube.transform.localPosition;	
		}

		public void translateXYZ(float x, float y, float z, Space relativeTo){
			cube.transform.Translate(x, y, z, relativeTo);	
		}
		public void translateXYZ(float x, float y, float z){
			cube.transform .Translate(x, y, z);	
		}

		public void rotationXYZ(float xAngle, float yAngle, float zAngle, Space relativeTo)
		{
			cube.transform.Rotate (xAngle, yAngle, zAngle, relativeTo);
		}
		public void rotationXYZ(float xAngle, float yAngle, float zAngle)
		{
			cube.transform.Rotate (xAngle, yAngle, zAngle);

		}
		public void scaleV3(Vector3 scale){
			cube.transform.localScale = scale;
		}

		public void newScaleV3(){
			cube.transform.localScale = componentSize;
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

		public void lerp(float fracJourney, Vector3 start, Vector3 end){
			cube.transform.position = Vector3.Lerp(start, end, fracJourney);	
		}

		//public void newLerp()

		public void rotateAround(Vector3 point, Vector3 axis, float angle){
			cube.transform.RotateAround(point, axis, angle);	
		}

		public void changeLocalPosition(Vector3 newPos){
			cube.transform.localPosition = newPos;
		}

		public Transform getTransform(){
			return cube.transform;		
		}

 		public void setParent(component pa){
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

		public void pushChild(component child){
			childrenList.Add (child);
		}

		public void setName(string name){
			cube.name = name;
		}

		public string getName(){
			return cube.name;
		}
	}
	
}