using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

	public Text UITime;

	public Text UIBestFitLocal;
	public Text UIBestFitGlobal;

	public Text UIGainLocal;
	public Text UIGainGlobal;

	public Text UICpg;
	public Text UISpg;

	public Text UIGeneration;
	public Text UIPopulation;

	private float bestFitGlobal = 0;
	public float bestFitLocal = 0;

	private float gainLocal = 0;
	private float gainGlobal = 0;

	private WorldManager worldManager;

	private float bestFit;

	void Start() {
		worldManager = GameObject.FindObjectOfType<WorldManager>();
	}

	void Update () {

		UITime.text 		 = string.Format("Temp\n{2}h {1}m {0}s", (int)(Time.time) % 60, (int)(Time.time / 60f) % 60 , (int)((Time.time / 60f) / 60f) % 60);
		UIBestFitLocal.text  = string.Format("Meilleur fit (local)\n{0}", bestFitLocal);
		UIBestFitGlobal.text = string.Format("Meilleur fit (global)\n{0}", bestFitGlobal);
		UIGainLocal.text 	 = string.Format("Gain (local)\n{0}", gainLocal);
		UIGainGlobal.text 	 = string.Format("Gain (global)\n{0}", gainGlobal);
		UISpg.text 			 = string.Format("SPG\n{0}", worldManager.CellsSelectionCount);
		UICpg.text 			 = string.Format("CPG\n{0}", worldManager.CellsStartCount);
		UIGeneration.text	 = string.Format("Generations\n{0}", worldManager.Generation);
		UIPopulation.text	 = string.Format("Populations\n{0}", worldManager.Cells.Count);

	}

	public void SetLocalBestFit(float fit) {
		if(bestFitLocal < fit) {
			bestFitLocal = fit;
		}

		if(bestFitLocal > bestFitGlobal) {
			bestFitGlobal = bestFitLocal;
		}
	}

	public void resetBestFitLocal(){
		bestFitLocal = 0f;
	}

}
