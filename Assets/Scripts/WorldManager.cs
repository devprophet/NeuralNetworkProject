using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;

public class WorldManager : MonoBehaviour {

	/* Les dimmensions du terrain */
	public float xMin,xMax, yMin, yMax;

	/* Le nombre de cellules au départ */
	public int CellsStartCount 		= 10;

	/* Le nombre de cellules a selectioner parmis les meilleurs pour chaque generation */
	public int CellsSelectionCount 	= 4;

	/* Le nombre de nourriture a generer aléatoirement sur le terrain */
	public int FoodsCount 			= 15;

	/* Le nombre de poison a generer aléatoirement sur le terrain */
	public int PoisonsCount 		= 15;

	/* Liste ou et stocker tous les Transform de nourritures sur le terrain */
	public List<Transform> Foods	{ get; private set; } = new List<Transform>();

	/* Liste ou et stocker tous les Transform de poisons sur le terrain */
	public List<Transform> Poisons  { get; private set; } = new List<Transform>();

	/* Liste ou et stocker tous les Transform de cellules sur le terrain */
	public List<Transform> Cells 	{ get; private set; } = new List<Transform>();

	/* Liste contenant les cellules a détruire */
	private List<Transform> CellsToDestroy = new List<Transform>();

	private Dictionary<NeuralNetwork, float> dictionary = new Dictionary<NeuralNetwork, float>();

	public int Generation { get; private set; } = 0;

	/* Ajoute de la nourriture sur le terrain */
	private void AddFood(){
		Transform Food = ((GameObject)Instantiate(Resources.Load<GameObject>("Food"), GetRandomPosition(), Quaternion.identity)).transform;
		Foods.Add(Food);
	}

	/* Ajoute du poison sur le terrain */
	private void AddPoison(){
		Transform Poison = ((GameObject)Instantiate(Resources.Load<GameObject>("Poison"), GetRandomPosition(), Quaternion.identity)).transform;
		Poisons.Add(Poison);
	}

	/* Ajoute une cellule sur le terrain */
	public Cell AddCell(){
		Transform Cell = ((GameObject)Instantiate(Resources.Load<GameObject>("Cell"), GetRandomPosition(), Quaternion.identity)).transform;
		Cells.Add(Cell);
		return Cell.gameObject.AddComponent<Cell>();
	}

	private void RemoveFood (Transform Food) {
		Foods.Remove(Food);
		Destroy(Food.gameObject);
	}

	private void RemovePoison (Transform Poison) {
		Poisons.Remove(Poison);
		Destroy(Poison.gameObject);
	}

	public void RemoveAndAddFood (Transform Food) {
		RemoveFood(Food);
		AddFood();
	}

	public void RemoveAndAddPoison (Transform Poison) {
		RemovePoison(Poison);
		AddPoison();
	}

	public void RemoveCell (Transform cell) {
		cell.gameObject.SetActive(false);
		Cells.Remove(cell);
		CellsToDestroy.Add(cell);
		dictionary.Add(cell.GetComponent<Cell>().neuralNetwork, cell.GetComponent<Cell>().fitness);
	}

	private void GenerateFoodAndPoison () {

		for ( int i = 0; i < FoodsCount; i++ )
			AddFood();

		for ( int i = 0; i < PoisonsCount; i++ )
			AddPoison();

	}

	private void GeneratePopulation (bool isFirst = false) {
		Generation ++;
		
		if (isFirst /* || (!isFirst && GameObject.FindObjectOfType<UIManager>().bestFitLocal == 0) */) {
			for ( int i = 0; i < CellsStartCount; i++ ) { 
				var neuralNetwork = new NeuralNetwork(5, 2, new int[]{ 5, 4 }, 4);
				neuralNetwork.InitializeWeight();
				AddCell().Initialize(neuralNetwork);
			}
		} else {
			
			IEnumerable<KeyValuePair<NeuralNetwork, float>> arr = dictionary.OrderByDescending( t => t.Value );
			var arr2 = arr.ToList();
			arr2.RemoveRange(CellsSelectionCount, arr2.Count - CellsSelectionCount);
			
			List<NeuralNetwork> neuralNetworks = new List<NeuralNetwork>();

			foreach(var x in arr2) {
				neuralNetworks.Add(x.Key);
			}

			for (int i = 0; i < neuralNetworks.Count; i++ ) {
				for (int j = 0; j < neuralNetworks.Count; j++ ) {
					NeuralNetwork child = Genetic.CrossOver(neuralNetworks[i], neuralNetworks[j]);
					var save = child.Save();
					Genetic.Mutate(ref save.dna);
					child = new NeuralNetwork(save);
					AddCell().Initialize(child);
				}
			}

			dictionary.Clear();

			for (int i = 0; i < CellsToDestroy.Count; i++ )
				Destroy(CellsToDestroy[i].gameObject);

			CellsToDestroy.Clear();
		}
		GameObject.FindObjectOfType<UIManager>().resetBestFitLocal();
	}

	void Start () {
		GenerateFoodAndPoison ();
		GeneratePopulation ( true );
	}

	void Update () {
		if (Cells.Count == 0) {
			GeneratePopulation();
		}
	}

	private Vector3 GetRandomPosition() {
		int iteration = 0;
		Vector3 position;
		do{
			var x 	 = Random.Range(xMin, xMax);
			var y 	 = Random.Range(yMin, yMax);
			position = new Vector3(x, y);
			if (iteration > 10000)
				break;
			iteration++;
		} while( CollideWith(position, Foods) || CollideWith(position, Poisons) || CollideWith(position, Cells) );

		return position;
	}

	private bool CollideWith(Vector3 a, List<Transform> array, float distance = .6f){
		foreach(var x in array) {
			if(a.x > x.position.x - distance && a.x < x.position.x + distance)
				return true;
			if(a.y > x.position.y - distance && a.y < x.position.y + distance)
				return true;
		}
		return false;
	}

}
