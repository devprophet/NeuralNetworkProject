using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Pour la copie de tableau.
using System.Linq;

// Pour la serialization de données
using Newtonsoft.Json;

public class NeuralNetwork {

	// la valeur des neuronnes d'entrée
	private float[] 	Input;
	// la valeur des neuronnes de la couche cachée
	private float[][] Hidden;
	// la valeur des neuronnes de la couche de sortie
	private float[] 	Output;

	// la valeur des poids de la couche d'entrée
	private float[][] 	Wi;
	// la valeur des poids de la couche cachée
	private float[][][] Wh;

	// Le nombre de neurones en entrée
	public int InputsCount;
	// Le nombre de layers dans la couche cachée
	public int HiddenLayersCount;
	// Le nombre de neurones dans chaque layers
	public int[] HiddenLayersNeuronsCount;
	// Le nombre de neuronnes a la sortie
	public int OutputsCount;

	/// <summary>
	/// Crée une nouvelle instance de <see cref="NeuralNetwork"/>.
	/// <param name="InputsCount">Le nombre de neurones en entrée.</param>
	/// <param name="HiddenLayersCount">Le nombre de layers dans la couche cachée.</param>
	/// <param name="HiddenLayersNeuronsCount">Le nombre de neurones dans chaque layers.</param>
	/// <param name="OutputsCount">Le nombre de neuronnes a la sortie.</param>
	/// </summary>
	public NeuralNetwork(int InputsCount, int HiddenLayersCount, int[] HiddenLayersNeuronsCount, int OutputsCount) {

		this.InputsCount 				= InputsCount;
		this.HiddenLayersCount 			= HiddenLayersCount;
		this.HiddenLayersNeuronsCount 	= HiddenLayersNeuronsCount;
		this.OutputsCount 				= OutputsCount;

		/*
			Initialise le nombre de neurones dans le layers d'entrée.
		*/
		this.Input 	= new float[this.InputsCount];

		/* 
			Initialise le nombre de layers dans la partie cachée.
		*/
		this.Hidden = new float[this.HiddenLayersCount][];

		/*
			Initialise le nombre de neurones dans chaque layers de la partie cachée.
		*/
		for(int i = 0; i < this.Hidden.Length; i++) {
			this.Hidden[i] = new float[this.HiddenLayersNeuronsCount[i]];
		}

		/*
			Initialise le nombre de neurones dans le layers de sortie.
		*/
		this.Output = new float[OutputsCount];
		
		/* 
			Initialise le nombre de dentries qui relie le layers d'entrée et le premier layer de la couche cachée. 
		*/
		this.Wi = new float[this.InputsCount][];
		for(int i = 0; i < this.Wi.Length; i++) {
			this.Wi[i] = new float[this.Hidden[0].Length];
		}

		/* 
			Initialise le nombre de dentries qui relie chaque layers de la face cachée entre eux.
			NOTE : Initialise aussi les poids qui relie la derniere couche caché avec la couche de sortie!
		*/
		this.Wh = new float[this.HiddenLayersCount][][];
		for(int i = 0; i < this.HiddenLayersCount; i++) {
			this.Wh[i] = new float[this.HiddenLayersNeuronsCount[i]][];
			for(int j = 0; j < this.HiddenLayersNeuronsCount[i]; j++) {
				this.Wh[i][j] = new float[(i + 1 < this.Hidden.Length) ? this.Hidden[i + 1].Length : this.Output.Length];
			}
		}

	}

	/// <summary>
	/// Crée une nouvelle instance de <see cref="NeuralNetwork"/> a partire d'une sauvegarde.
	/// <param name="save">La sauvegarde</param>
	/// </summary>
	public NeuralNetwork(NeuralSave save) {
		this.InputsCount 				= save.InputsCount;
		this.HiddenLayersCount 			= save.HiddenLayersCount;
		this.HiddenLayersNeuronsCount 	= save.HiddenLayersNeuronsCount;
		this.OutputsCount 				= save.OutputsCount;

		/*
			Initialise le nombre de neurones dans le layers d'entrée.
		*/
		this.Input 	= new float[this.InputsCount];

		/* 
			Initialise le nombre de layers dans la partie cachée.
		*/
		this.Hidden = new float[this.HiddenLayersCount][];

		/*
			Initialise le nombre de neurones dans chaque layers de la partie cachée.
		*/
		for(int i = 0; i < this.Hidden.Length; i++) {
			this.Hidden[i] = new float[this.HiddenLayersNeuronsCount[i]];
		}

		/*
			Initialise le nombre de neurones dans le layers de sortie.
		*/
		this.Output = new float[OutputsCount];
		
		/* 
			Initialise le nombre de dentries qui relie le layers d'entrée et le premier layer de la couche cachée. 
		*/
		this.Wi = save.WeigthInputs;

		/* 
			Initialise le nombre de dentries qui relie chaque layers de la face cachée entre eux.
			NOTE : Initialise aussi les poids qui relie la derniere couche caché avec la couche de sortie!
		*/
		this.Wh = save.WeigthHidden;
	}

	/// <summary>
	/// Initialise les poids des neurones, en leurs donnant une valeur aléatoire comprise en <param ref="min"/> et <param ref="max"/>.
	/// <param name="min">La valeur minimum du poids de chaque neurones.</param>
	/// <param name="max">La valeur maximum du poids de chaque neurones.</param>
	/// </summary>
	public void InitializeWeigth(float min = -1f, float max = 1f) {

		/* 
			Initialise les poids qui relie le layer d'entrée au permier layer caché
		*/
		for(int i = 0; i < Wi.Length; i++ ) {
			for(int j = 0; j < Wi[i].Length; j++ ) {
				Wi[i][j] = Random.Range(min, max);
			}
		}

		/*
			Initilise tous les poids de la couche caché ainsi que les poids qui relie le dernier layers de la couche caché
			au layer de sortie.
		*/
		for(int i = 0; i < Wh.Length; i++ ) {
			for(int j = 0; j < Wh[i].Length; j++ ) {
				for(int k = 0; k < Wh[i][j].Length; k++) {
					Wh[i][j][k] = Random.Range(min, max);
				}
			}
		}

	}

	/// <summary>
	/// Présentes les valeurs stocker dans <see paramref="data"/> aux neurones du layers d'entrée.
	/// <param name="data">Les valeurs d'entrées</param>
	/// </summary>
	public void SetInput(float[] data){
		for(int i = 0; i < Input.Length; i++ )
			Input[i] = data[i];
	}

	/// <summary>
	/// Propage les données du layer d'entrée vers le layer de sortie.
	/// </summary>
	public void Propagate(){
		/* 
			Propagation du layers d'entrée vers le premier layer de la couche caché 
		*/
		for(int i = 0; i < Hidden[0].Length; i++) {
			for(int j = 0; j < Input.Length; j++) {
				Hidden[0][i] += Wi[j][i] * Input[j];
			}
			Hidden[0][i] = sigmoid(Hidden[0][i]);
		}

		if(Hidden.Length > 1) {
			/* 
				Propagation du premiers layer de la couche vers le layer de sortie
			*/
			for(int i = 1; i < Hidden.Length; i++) {
				for(int j = 0; j < Hidden[i].Length; j++) {
					for(int k = 0; k <  Hidden[i - 1].Length; k++) {
						Hidden[i][j] += Wh[i - 1][k][j] * Hidden[i - 1][j];
					}
					Hidden[i][j] = sigmoid(Hidden[i][j]);
				}
			}
		}
		else {
			/*
				Propagation du layer cahcée vers le layer de sortie.
			*/
			for(int i = 0; i < Output.Length; i++) {
				for(int j = 0; j < Hidden[0].Length; j++) {
					Output[i] += Wh[0][j][i] * Hidden[0][j];
				}
				Output[i] = sigmoid(Output[i]);
			}
		}
	}

	/// <summary>
	/// La fonction sigmoid (plus d'infos sur http://www.google.fr/ ;) )
	/// </summary>
	private float sigmoid(float x) {
		return 2f / (1f + Mathf.Exp(-2f * x)) - 1f;
	}

	/// <summary>
	/// Retourne un tableau de float contenant les valeurs de sortie du réseau de neurones.
	/// </summary>
	public float[] GetOutput() {
		float[] output = new float[Output.Length];
		for(int i = 0; i < Output.Length; i++ )
			output[i] = Output[i];
		return output;
	}

	public NeuralSave Save() {
		NeuralSave save = new NeuralSave();

		save.InputsCount 				= InputsCount;
		save.HiddenLayersCount 			= HiddenLayersCount;
		save.HiddenLayersNeuronsCount 	= HiddenLayersNeuronsCount;
		save.OutputsCount 				= OutputsCount;
		save.WeigthInputs 				= Wi;
		save.WeigthHidden 				= Wh;

		return save;
	}
}

[System.Serializable]
public struct NeuralSave {
	// Le nombre de neurones en entrée
	public int InputsCount;

	// Le nombre de layers dans la couche cachée
	public int HiddenLayersCount;

	// Le nombre de neurones dans chaque layers
	public int[] HiddenLayersNeuronsCount;
	
	// Le nombre de neuronnes a la sortie
	public int OutputsCount;

	// Tous les poids entre le layer d'entrée et le premier layer de la couche cachée
	public float[][] WeigthInputs;

	// Tous les poids entre tous les layers de la couche cachée ainsi que entre le dernier layer de la couche cacher et du layer de sortie.
	public float[][][] WeigthHidden;

	/// <summary>
	/// Retourne un json du contenue de cette structure.
	/// </summary>
	public string ToJson() {
		return JsonConvert.SerializeObject(this, Formatting.Indented);
	}

	/// <summary>
	/// Initialise les données via le contenue text d'un fichier json.
	/// <param name="json">Le contenue d'un fichier json</param>
	/// </summary>
	public void FromJson(string json) {
		NeuralSave s = JsonConvert.DeserializeObject<NeuralSave>(json);
		this.InputsCount 				= s.InputsCount;
		this.HiddenLayersCount 			= s.HiddenLayersCount;
		this.HiddenLayersNeuronsCount 	= s.HiddenLayersNeuronsCount;
		this.OutputsCount				= s.OutputsCount;
		this.WeigthInputs 				= s.WeigthInputs;
		this.WeigthHidden 				= s.WeigthHidden;
	}

}
