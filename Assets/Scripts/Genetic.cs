using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Genetic : MonoBehaviour {
	/// <summary>
	/// Effectue un crossing-over uniforme avec deux NeuralNetwork
	/// </summary>
	public static NeuralNetwork CrossOver (NeuralNetwork a, NeuralNetwork b) {

		/* Sauvegarde le reseau de neurones a */
		var save_a = a.Save();

		/* Sauvegarde le reseau de neurones b */
		var save_b = b.Save();

		/* Crée une nouvelle adn basée sur l'adn du reseau de neurone a */
		var child_dna = new DNA(a);
		
		/* Effectue le crossing-over */
		for( int i = 0; i < child_dna.genes.Length; i ++ )
			if( Random.Range(-1f, 1f) > 0f )
				child_dna.genes[i] = save_a.dna.genes[i];
			else
				child_dna.genes[i] = save_b.dna.genes[i];

		/* Crée un nouveau reseau de neurones basée sur le reseau de neurone a */
		var child_neuralNetwork = new NeuralNetwork(save_a);
		
		/* Initialise les poids du nouveau reseau de neurones avec l'adn crée */
		child_neuralNetwork.InitializeWeight(child_dna);

		/* Retourne le nouveau reseau de neurones */
		return child_neuralNetwork;

	}

	/// <summary>
	/// Permet de muter l'adn.
	/// </summary>
	public static void Mutate (ref DNA a) {
		float s = a.genes.Length;
		float seuil = 1f / s;

		for(int i = 0; i < a.genes.Length; i++ )
			if(Random.Range(0f, s) <= seuil)
				a.genes[i] = Random.Range(-1f, 1f);
	}

}

[System.Serializable]
public struct DNA {
	/* 
		Les gènes
	*/
	public float[] genes;

	/// <summary>
	/// Crée une nouvelle instance de <see cref="DNA"/> a partir d'un <see cref="NeuralSave"/>.
	/// <param name="save">La sauvegarde neuronal</param>
	/// </summary>
	public DNA (NeuralNetwork neuralNetwork) {

		/* Liste qui va stocker les information de la sauvegarde neuronal */
		List<float> genes = new List<float>();

		/* Ajoute a la liste les poids entres les neurones d'entrée et le premier layer de la couche caché */
		for ( int i = 0; i < neuralNetwork.Wi.Length; i++ )
			for ( int j = 0; j < neuralNetwork.Wi[i].Length; j++ )
				genes.Add(neuralNetwork.Wi[i][j]);

		/* Ajoute a la liste les poids des entres les layers de la couche cachée ainsi que ceux entre le layer de sortie et la couche caché */
		for(int i = 0; i < neuralNetwork.Wh.Length; i++ )
			for( int j = 0; j < neuralNetwork.Wh[i].Length; j++ )
				for( int k = 0; k < neuralNetwork.Wh[i][j].Length; k++ )
					genes.Add(neuralNetwork.Wh[i][j][k]);

		/* stock la liste */
		this.genes = genes.ToArray();

	}
	
}
