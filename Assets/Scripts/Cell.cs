using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*

Entrées (5) : 
0 - Présence de nourriture en face de soi
1 - Précence d'autre cellule en face de soi
2 - Présence de choses mortel en face de soi
3 - Précence d'un obstacle en face de soi
4 - Si l'individu peut se reproduire

Sorties (4) :
0 - Tourner a gauche
1 - Tourner a droite
2 - Avancer
3 - Reproduire

*/

public class Cell : MonoBehaviour {

	public float fitness { get; private set; }
	public NeuralNetwork neuralNetwork { get; private set; }

	public bool Initialized = false;
	private float life = 30f;
	private int FoodStock = 0;

	private UIManager uIManager;

	public void Initialize (NeuralNetwork neuralNetwork) {
		this.neuralNetwork = neuralNetwork;
		fitness = 0f;
		Initialized = true;
	}

	void Start () {
		uIManager = GameObject.FindObjectOfType<UIManager>();
	}

	void Update () {

		if(!Initialized)
			return;

		float foodPresence 		= -1f;
		float cellPresence 		= -1f;
		float poisonPresence 	= -1f;
		float obstaclePresence 	= -1f;
		float canReproduce 		= -1f;

		float turnLeft 			= 0f;
		float turnRight 		= 0f;
		float moving 			= 0f;
		float reproduce 		= 0f;

		Cell NearCell = null;

		RaycastHit2D hit2D;
		
		var baseLinePosition = transform.position + (transform.up * (GetComponent<CircleCollider2D>().radius + .1f));

		if( (hit2D = Physics2D.Raycast(baseLinePosition, transform.up, Mathf.Infinity)) ) {

			Debug.DrawLine(baseLinePosition, hit2D.point, Color.blue);

			if(hit2D.collider.tag == "Food")
				foodPresence = 1f;
			else if (hit2D.collider.tag == "Cell") {
				cellPresence = 1f;
				if (hit2D.distance < .5f) 
					NearCell = hit2D.collider.transform.GetComponent<Cell>();
			}
			else if (hit2D.collider.tag == "Poison")
				poisonPresence = 1f;
			else if (hit2D.collider.tag == "Obstacle")
				obstaclePresence = 1f;
			else
				Debug.Log("Unknow object tag!");

		}

		if(FoodStock >= 2)
			canReproduce = 1f;

		var Inputs = new float[]{ foodPresence, cellPresence, poisonPresence, obstaclePresence, canReproduce };
		neuralNetwork.SetInput(Inputs);
		neuralNetwork.Propagate();
		var Outputs = neuralNetwork.GetOutput();

		turnLeft 	= (Outputs[0] + 1f) / 2f;
		turnRight 	= (Outputs[1] + 1f) / 2f;
		moving 		= (Outputs[2] + 1f) / 2f;
		reproduce   = Outputs[3];

		if (reproduce > 0 && NearCell != null && canReproduce == 1f) {
			NeuralNetwork child = Genetic.CrossOver(neuralNetwork, NearCell.neuralNetwork);
			var save = child.Save();
			Genetic.Mutate(ref save.dna);
			child = new NeuralNetwork(save);
			GameObject.FindObjectOfType<WorldManager>().AddCell().Initialize(child);
			FoodStock -= 2;
			fitness += 1f;
		}


		transform.Rotate(Vector3.forward * turnLeft  * -80f * Time.deltaTime);
		transform.Rotate(Vector3.forward * turnRight *  80f * Time.deltaTime);
		if(moving > .5f)
			transform.Translate(Vector3.up * 3f * Time.deltaTime);

		//fitness += turnLeft / 100;
		//fitness += turnRight / 100;

		//fitness += moving / 100;

		life -= Time.deltaTime;

		if (life <= 0) {
			GameObject.FindObjectOfType<WorldManager>().RemoveCell(this.transform);
		}

		uIManager.SetLocalBestFit(fitness);

	}

	void OnCollisionEnter2D (Collision2D coll) {
		if (coll.transform.tag == "Food") {
			fitness += 1f;
			life = 30f;
			FoodStock ++;
			GameObject.FindObjectOfType<WorldManager>().RemoveAndAddFood(coll.transform);
		}

		if (coll.transform.tag == "Poison") {
			fitness -= 1f;
			GameObject.FindObjectOfType<WorldManager>().RemoveAndAddPoison(coll.transform);
			GameObject.FindObjectOfType<WorldManager>().RemoveCell(this.transform);
		}
	}
}
