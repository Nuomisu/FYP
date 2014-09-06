using UnityEngine;
using System.Collections;
using Component;

namespace Joint{
	public class joint{

		public static float moveSpeed = 10.0f;
		public static float rotateSpeed = 100.0f;

		component parent;
		component child;

		private Vector3 jointLocalPosOfParent;// = new Vector3(0.6F, 0, 0.3F);
		private Vector3 jointLocalPosOfChild;// = new Vector3(0, 0, -0.5F);
	
		public Vector3 jointRawLocalPosOfParent;
		public Vector3 jointRawLocalPosOfChild;

		Vector3 jointWorldPosParent;// = new Vector3(3.0F, 3.0F, 3.0F);
		Vector3 jointWorldPosChild;// = new Vector3(4.0F, 3.0F, -5.0F);

		Vector3 jointPosOfPaInPa;
		Vector3 jointPosOfChiInPa;

		bool enable = true;
		bool enableMove = true;
		bool enableRotate = false;
		//public bool startRotate = false;
		float distanceInPa = 0.0F;
		float distance = 0.0F;
		float distaneX, distancY, distanceZ;

		float rotateAngleByX;
		float rotateAngleByY;
		float rotateAngleByZ;
		bool rotateByX; // rotateVector = (1, 0, 0)
		bool rotateByY; // rotateVector = (0, 1, 0)
		bool rotateByZ; // rotateVector = (0, 0, 1)
		Vector3 rotateXVector, rotateYVector, rotateZVector;

		float angleCovered = 0;

		Vector3 parentPosition;
		Vector3 childPosition;

		public joint(){
				
		}

		public joint(component pa, component chi){
			parent = pa;
			child = chi;
			rotateAngleByX = 0.0F;
			rotateAngleByY = 0.0F;
			rotateAngleByZ = 0.0F;
			rotateByX = false;
			rotateByY = false;
			rotateByZ = false;
		}

		public joint(component pa, component chi, Vector3 jointLocalPosPa, Vector3 jointLocalPosChi,
		             float rotateAByX, float rotateAByY, float rotateAByZ
		             ){
			parent = pa;
			child = chi;
			jointLocalPosOfParent = jointLocalPosPa;
			jointLocalPosOfChild = jointLocalPosChi;
			rotateAngleByX = rotateAByX;
			rotateAngleByY = rotateAByY;
			rotateAngleByZ = rotateAByZ;

			rotateByX = true;
			rotateByY = true;
			rotateByZ = true;

			chi.setParent (pa);
		}

		public void prepare(){

			jointWorldPosParent = parent.localToWorld (jointLocalPosOfParent);
			jointWorldPosChild = child.localToWorld (jointLocalPosOfChild);

			//jointWorldPosChild = new Vector3 (0,0 ,0);
			//jointWorldPosParent = new Vector3 (0, 20f, 40f);

			jointPosOfPaInPa = jointLocalPosOfParent;
			jointPosOfChiInPa = parent.worldToLocal(child.localToWorld(jointLocalPosOfChild));
			// Since we want the child move in parent's coordinates, we need to find each joint connect point's position in parent's coordinates.

			parent.testDraw (jointLocalPosOfParent);
			//Debug.Log (jointWorldPosParent);
			//Debug.Log (jointWorldPosChild);

			distanceInPa =  Vector3.Distance(jointPosOfPaInPa, jointPosOfChiInPa);

			distance = Vector3.Distance (jointWorldPosParent, jointWorldPosChild);
			parentPosition = parent.getPosition ();
			childPosition = child.getPosition ();

			//Debug.Log ("Joint Prepare: Rotate X angles: " + rotateAngleByX);
			//Debug.Log ("Joint Prepare: Rotate Y angles: " + rotateAngleByY);
			//Debug.Log ("Joint Prepare: Rotate Z angles: " + rotateAngleByZ);
			rotateAngleByX = rotateAngleByX + parent.getRotationAngleX();
			rotateAngleByY = rotateAngleByY + parent.getRotationAngleY();
			rotateAngleByZ = rotateAngleByZ + parent.getRotationAngleZ();
			Debug.Log ("Rotation X "+rotateAngleByX);
			Debug.Log ("Rotation Y "+rotateAngleByY);
			Debug.Log ("Rotation Z "+rotateAngleByZ);
			child.setRotationXYZ (rotateAngleByX, rotateAngleByY, rotateAngleByZ);

			if (rotateAngleByX > 0) {
				rotateXVector = Vector3.down;
			} else {
				rotateXVector = Vector3.up;
				rotateAngleByX = Mathf.Abs(rotateAngleByX);
			}

			if (rotateAngleByY > 0) {
				rotateYVector = Vector3.left;
			} else {
				rotateYVector = Vector3.right;
				rotateAngleByY = Mathf.Abs(rotateAngleByY);
			}

			if (rotateAngleByZ > 0) {
				rotateZVector = Vector3.back;
			} else {
				rotateZVector = Vector3.forward;
				rotateAngleByZ = Mathf.Abs(rotateAngleByZ);
			}

			rotateByX = true;
			rotateByY = false;
			rotateByZ = false;
			angleCovered = 0.0f;

			Vector3 endpos = childPosition + (jointWorldPosParent - jointWorldPosChild);

			Debug.Log ("Start at: " + childPosition);
			Debug.Log ("End at: " + endpos);
			//Debug.Log ("Joint Prepare: Rotate X Vector: " + rotateXVector);
			//Debug.Log ("Joint Prepare: Rotate Y Vector: " + rotateYVector);
			//Debug.Log ("Joint Prepare: Rotate Z Vector: " + rotateZVector);
		}

		public void move(float startTime){
			float distCovered = (Time.time - startTime) * moveSpeed;
			float fracJourney = distCovered / distance;

			//Debug.Log (fracJourney);
			Vector3 endpos = childPosition + (jointWorldPosParent - jointWorldPosChild);
			//Debug.Log ("Child position: "+ childPosition);
			//Debug.Log ("Child end position: "+ endpos);

			child.lerp (fracJourney, childPosition, endpos);

			if (fracJourney > 1.0F){
				//Debug.Log("I am here.");
				setJointMoveEnableFalse();
				setJointRotateEnableTrue();
				//setStartRotate();
			}
		}

		public void newMove(float startTime){

		}

		public bool rotate(){

			if (rotateByX && !rotateByY && !rotateByZ){
				//Debug.Log("Rotate by x");
				angleCovered = angleCovered + rotateSpeed * Time.deltaTime;

				child.rotateAround(jointWorldPosParent, rotateXVector, rotateSpeed * Time.deltaTime);	

				if (angleCovered >= rotateAngleByX) {
					rotateByX = false;
					rotateByY = true;
					angleCovered = 0.0f;
				}
			}
			if (!rotateByX && rotateByY && !rotateByZ){
				//Debug.Log("Rotate by y");
				angleCovered = angleCovered + rotateSpeed * Time.deltaTime;

				child.rotateAround(jointWorldPosParent, rotateYVector, rotateSpeed * Time.deltaTime);	
				
				if (angleCovered >= rotateAngleByY) {
					rotateByY = false;
					rotateByZ = true;
					angleCovered = 0.0f;
				}
			}
			if (!rotateByX && !rotateByY && rotateByZ){
				//Debug.Log("Rotate by z");
				angleCovered = angleCovered + rotateSpeed * Time.deltaTime;

				child.rotateAround(jointWorldPosParent, rotateZVector, rotateSpeed * Time.deltaTime);	
				
				if (angleCovered >= rotateAngleByZ) {
					rotateByZ = false;
					angleCovered = 0.0f;
					setJointEnableFalse();
					setJointRotateEnableFalse();
					return true;
				}
			}
			return false;
		}

		public bool jointEnable(){
			return enable;
		}
		public void setJointEnableTrue(){
			enable = true;
		}
		public void setJointEnableFalse(){
			enable = false;
		}

		public bool jointMoveEnable(){
			return enableMove;
		}
		public void setJointMoveEnableTrue(){
			enableMove = true;
		}
		public void setJointMoveEnableFalse(){
			enableMove = false;
		}

		public bool jointRotateEnable(){
			return enableRotate;
		}
		public void setJointRotateEnableTrue(){
			enableRotate = true;
		}
		public void setJointRotateEnableFalse(){
			enableRotate = false;
		}

		/*public void setStartRotate(){
			startRotate = true;
		}

		public bool getStartRotate(){
			return startRotate;
		} */

		public string getJointParentName(){
			if (parent != null) {
				return parent.getName();			
			}
			return "";
		}
		public string getJointChildName(){
			if (child != null) {
				return child.getName();			
			}
			return "";
		}

		public void setJointParent(component pa){
			parent = pa;
		}

		public void setJointChild(component chi){
			child = chi;
		}

		public void prepareLocalJointPosOfParent(){

				Debug.Log ("Enter prepare");
				jointLocalPosOfParent.x = jointRawLocalPosOfParent.x / parent.componentSize.x;
				jointLocalPosOfParent.y = (jointRawLocalPosOfParent.y - parent.componentSize.y/2)/parent.componentSize.y;
				jointLocalPosOfParent.z = jointRawLocalPosOfParent.z / parent.componentSize.z;

		}

		public void prepareLocalJointPosOfChild(){

				jointLocalPosOfChild.x = 0f / child.componentSize.x;
				jointLocalPosOfChild.y = (0f - parent.componentSize.y/2)/child.componentSize.y;
				jointLocalPosOfChild.z = 0f / child.componentSize.z;

		}

		public Vector3 getJointLocalPosOfParent(){
			return jointLocalPosOfParent;
		}
		public Vector3 getJointLocalPosOfChild(){
			return jointLocalPosOfChild;
		}

		public Vector3 getJointRawLocalPosOfParent(){
			return jointRawLocalPosOfParent;
		}
		public Vector3 getJointRawLocalPosOfChild(){
			return jointRawLocalPosOfChild;
		}
	}
}
