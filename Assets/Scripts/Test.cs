using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;

using System.IO;

public class Test : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
		/* Crée un réseau de neurones */
		NeuralNetwork n = new NeuralNetwork(4, 1, new int[]{ 4 }, 2);

		/* Initialise le poids des dentries */
		n.InitializeWeight();

		/* Présente a l'entrée des valeurs */
		n.SetInput(new float[]{0, 1, 0, 1});

		/* Propage les donnés de l'entrées vers la sortie */
		n.Propagate();

		/* Récupere les valeurs des neurones de sortie */
		var r = n.GetOutput();

		/* Affihe les valeurs */
		foreach(var result in r)
			Debug.Log(result);
		
		/* Sauvegarde le reseau de neurones */
		var save = n.Save();

		/* Le fichier JSON ou seras stocker la sauvegarde du reseau de neurones */
		string path = "/Users/alexis/Desktop/NeuralNetwork.json";

		/* Detruit le fichier si il existe deja */
		if(File.Exists(path))
			File.Delete(path);
		
		/* Crée et ouvre le fichier */
		var fs = File.OpenWrite(path);

		/* Recupere les données de la sauvegarde dans un tableau de byte */
		var data = System.Text.Encoding.UTF8.GetBytes(save.ToJson());

		/* Ecris dans le fichier les données */
		fs.Write(data, 0, data.Length);

		/* Libère les ressources */
		fs.Close();

		fs = File.OpenRead(path);
		List<byte> dataRead = new List<byte>();
		byte[] buffer = new byte[256];
		int readResult = 0;
		do{
			readResult = fs.Read(buffer, 0, buffer.Length);
			System.Array.Resize(ref buffer, readResult);
			dataRead.AddRange(buffer);
		}while(readResult > 0);

		save = new NeuralSave();
		save.FromJson(System.Text.Encoding.UTF8.GetString(dataRead.ToArray()));

		n = new NeuralNetwork(save);

		n.InitializeWeight(save.dna);

		/* Présente a l'entrée des valeurs */
		n.SetInput(new float[]{0, 1, 0, 1});

		/* Propage les donnés de l'entrées vers la sortie */
		n.Propagate();

		/* Récupere les valeurs des neurones de sortie */
		r = n.GetOutput();

		/* Affihe les valeurs */
		foreach(var result in r)
			Debug.Log(result);

	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
