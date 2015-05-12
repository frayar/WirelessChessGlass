using UnityEngine;
using System.Collections;
using System;

public class FractureObject : MonoBehaviour {
	
	public bool fractureToPoint = false;
	public float totalMaxFractures = 3;
	public float forcePerDivision = 20.0f;
	public float minBreakingForce = 0.0f;
	public float maxFracturesPerCall = 5;
	public float randomOffset = 0.0f;
	public Vector3 minFractureSize;
	public Vector3 grain;
	public float useCollisionDirection = 0.0f;
	public bool fractureAtCenter = false;
	public bool smartJoints = false;
	public float destroyAllAfterTime = 0.0f;
	public float destroySmallAfterTime = 0.0f;
	public GameObject instantiateOnBreak;
	public float totalMassIfStatic = 20.0f;
	public Joint[] joints;
	
	//Initialisation
	void Start () {
		minFractureSize = Vector3.zero;
		if (this.GetComponent<Rigidbody>()) {
			ArrayList temp = new ArrayList();
			foreach(Joint j in FindObjectsOfType(typeof(Joint))) {
				if (j.connectedBody == GetComponent<Rigidbody>()) {
					temp.Add(j);
					temp.Add(joints);
				}
			}
		}
	}
	
	public void FractureAtPoint(Vector3 hit, Vector3 force)
	{
		if (force.magnitude < Mathf.Max(minBreakingForce, forcePerDivision)) { return; }
		float iterations = Mathf.Min(Mathf.RoundToInt(force.magnitude / forcePerDivision), Mathf.Min(maxFracturesPerCall, totalMaxFractures));
		Vector3 point = transform.worldToLocalMatrix.MultiplyPoint(hit);
		StartCoroutine(Fracture(hit, force, iterations));
	}
	
	IEnumerator Fracture(Vector3 point, Vector3 force, float iterations) 
	{
		if (instantiateOnBreak) {
			Instantiate(instantiateOnBreak,transform.position,transform.rotation);
			instantiateOnBreak = null;
		}
		while (iterations > 1) {
			//If minFractureSize est atteint alors il n'y a plus de divisions
			if (totalMaxFractures == 0 || Vector3.Min(gameObject.GetComponent<MeshFilter>().mesh.bounds.size,minFractureSize) != minFractureSize) {
				if (destroySmallAfterTime >= 1) {
					Destroy(GetComponent("MeshCollider"),destroySmallAfterTime-1);
					Destroy(gameObject,destroySmallAfterTime);
				}
				totalMaxFractures = 0;
				yield return null;
				
			}
			totalMaxFractures -= 1;
			iterations -= 1;
			//Définition du plane de division
			if(fractureAtCenter) {
				point=GetComponent<MeshFilter>().mesh.bounds.center;
			}
			Vector3 vec = Vector3.Scale(grain,UnityEngine.Random.insideUnitSphere).normalized;
			Vector3 sub = transform.worldToLocalMatrix.MultiplyVector(force.normalized)*useCollisionDirection*Vector3.Dot(transform.worldToLocalMatrix.MultiplyVector(force.normalized),vec);
			Plane plane = new Plane(vec-sub,Vector3.Scale(UnityEngine.Random.insideUnitSphere,GetComponent<MeshFilter>().mesh.bounds.size)*randomOffset+point);
			//Création du clone
			GameObject newObject = (GameObject)Instantiate(gameObject,transform.position,transform.rotation);
			newObject.transform.SetParent(this.transform);
			if (GetComponent<Rigidbody>()) {
				newObject.GetComponent<Rigidbody>().velocity = GetComponent<Rigidbody>().velocity;
			}
			Vector3[] vertsA =  gameObject.GetComponent<MeshFilter>().mesh.vertices;
			Vector3[] vertsB =  newObject.GetComponent<MeshFilter>().mesh.vertices;
			Vector3 average = Vector3.zero;
			foreach(Vector3 i in vertsA) {
				average += i;
			}
			average /= gameObject.GetComponent<MeshFilter>().mesh.vertexCount;
			average -= plane.GetDistanceToPoint(average)*plane.normal;
			//-------------------------------------------------------------------
			float broken = 0;
			//Découpage le long du plan
			if (fractureToPoint) {
				for (int i=0; i < gameObject.GetComponent<MeshFilter>().mesh.vertexCount; i++) {
					if (plane.GetSide(vertsA[i])) {
						vertsA[i] = average;
						broken += 1;
					}
					else {
						vertsB[i] = average;
					}
				}
			}
			else {
				for (int i=0; i<gameObject.GetComponent<MeshFilter>().mesh.vertexCount; i++) {
					if (plane.GetSide(vertsA[i])) {
						vertsA[i] -= plane.GetDistanceToPoint(vertsA[i])*plane.normal;
						broken += 1;
					}
					else {
						vertsB[i] -= plane.GetDistanceToPoint(vertsB[i])*plane.normal;
					}
				}
			}
			if (broken == 0 || broken == gameObject.GetComponent<MeshFilter>().mesh.vertexCount) {
				totalMaxFractures += 1;
				iterations += 1;
				Destroy(newObject);
				yield break;
			}
			//Si le découpage est correctement réalisé, on applique les modifs aux meshs
			else {
				gameObject.GetComponent<MeshFilter>().mesh.vertices = vertsA;
				newObject.GetComponent<MeshFilter>().mesh.vertices = vertsB;
				gameObject.GetComponent<MeshFilter>().mesh.RecalculateNormals();
				newObject.GetComponent<MeshFilter>().mesh.RecalculateNormals();
				gameObject.GetComponent<MeshFilter>().mesh.RecalculateBounds();
				newObject.GetComponent<MeshFilter>().mesh.RecalculateBounds();
				if (gameObject.GetComponent<MeshCollider>()) {
					gameObject.GetComponent<MeshCollider>().sharedMesh = gameObject.GetComponent<MeshFilter>().mesh;
					newObject.GetComponent<MeshCollider>().sharedMesh = newObject.GetComponent<MeshFilter>().mesh;
				}
				//Dans le cas où il n'y a pas de convexhull on supprime l'objet afin d'éviter les comportements normaux
				else {
					Destroy(GetComponent<Collider>());
					Destroy(gameObject,1);
				}
				//Prise en compte des joints
				if (smartJoints) {
					Joint[] jointsb = (Joint[])GetComponents(typeof(Joint));
					if (jointsb[0] != null){
						//Attribut le joint à l'objet le plus proche du connectedbody
						for (int i=0; i<jointsb.Length; i++){
							if (jointsb[i].connectedBody != null && plane.GetSide(transform.worldToLocalMatrix.MultiplyPoint(jointsb[i].connectedBody.transform.position))) {
								Destroy(jointsb[i]);
							}
							else {
								Destroy(newObject.GetComponents<Joint>()[i]);
							}
						}
					}
					if (joints != null){
						for (int i=0; i<joints.Length; i++){
							if (joints[i] && plane.GetSide(transform.worldToLocalMatrix.MultiplyPoint(joints[i].transform.position))) {
								joints[i].connectedBody = newObject.GetComponent<Rigidbody>();
								ArrayList temp = new ArrayList(joints);
								temp.RemoveAt(i);
								temp.Add(joints);
							}
							else {
								ArrayList temp = new ArrayList(joints);
								temp.RemoveAt(i);
								temp.Add(newObject.GetComponent<FractureObject>().joints);
							}
						}
					}
				}
				//Permet de détruire les joints dans le cas où l'on utilise pas smartjoint
				else {
					if (GetComponent<Joint>()) {
						for (int i = 0; i < GetComponents<Joint>().Length; i++)
						{
							Destroy(GetComponents<Joint>()[i]);
							Destroy(newObject.GetComponents<Joint>()[i]);
						}
					}
					if (joints != null) {
						for (int i=0; i<joints.Length; i++){
							Destroy(joints[i]);
						}
						joints = null;
					}
				}
				//Dans le cas d'un objet STATIC, permet de générer un rigidbody sur les nouveaux objets
				if (!GetComponent<Rigidbody>()) {
					gameObject.AddComponent<Rigidbody>();
					newObject.AddComponent<Rigidbody>();
					GetComponent<Rigidbody>().mass = totalMassIfStatic;
					newObject.GetComponent<Rigidbody>().mass = totalMassIfStatic;
				}else{
					gameObject.AddComponent<Rigidbody>();
					newObject.AddComponent<Rigidbody>();
					GetComponent<Rigidbody>().mass = totalMassIfStatic;
					newObject.GetComponent<Rigidbody>().mass = totalMassIfStatic;
				}
				gameObject.GetComponent<Rigidbody>().mass *= 0.5f;
				newObject.GetComponent<Rigidbody>().mass *= 0.5f;
				gameObject.GetComponent<Rigidbody>().centerOfMass = transform.worldToLocalMatrix.MultiplyPoint3x4(gameObject.GetComponent<Collider>().bounds.center);
				newObject.GetComponent<Rigidbody>().centerOfMass = transform.worldToLocalMatrix.MultiplyPoint3x4(newObject.GetComponent<Collider>().bounds.center);
				
				newObject.GetComponent<FractureObject>().Fracture(point, force, iterations);
				
				if (destroyAllAfterTime >= 1) {
					Destroy(newObject.GetComponent<MeshCollider>(),destroyAllAfterTime-1);
					Destroy(GetComponent<MeshCollider>(), destroyAllAfterTime - 1);
					Destroy(newObject,destroyAllAfterTime);
					Destroy(gameObject,destroyAllAfterTime);
				}
				yield break;
			}
		}
		if (totalMaxFractures == 0)
		{
			if (destroySmallAfterTime >= 1) {
				Destroy(GetComponent<MeshCollider>(), destroySmallAfterTime - 1);
				Destroy(gameObject,destroySmallAfterTime);
			}
			totalMaxFractures = 0;
		}
	}
	
	float UsedMass (Collision collision) {
		if (collision.rigidbody) {
			if (GetComponent<Rigidbody>()) {
				if (collision.rigidbody.mass < GetComponent<Rigidbody>().mass) {
					return (collision.rigidbody.mass);
				}
				else {
					return (GetComponent<Rigidbody>().mass);
				}
			}
			else {
				return (collision.rigidbody.mass);
			}
		}
		else if (GetComponent<Rigidbody>()) {
			return (GetComponent<Rigidbody>().mass);
		}
		else {return (1);}
	}
	
}