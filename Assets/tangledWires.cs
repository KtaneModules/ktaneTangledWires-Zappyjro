using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using System.Linq;
using Newtonsoft.Json;
using KModkit;


public class tangledWires : MonoBehaviour {
	public KMAudio Audio;
	public KMBombModule Module;
	public KMBombInfo Info;
	public KMSelectable[] swapButtons;
	public TextMesh movesCounterText;
	public Material[] wireMats;
	public GameObject[] wirePointsOne;
	public GameObject[] wirePointsTwo;
	public GameObject[] wirePointsThree;
	public GameObject[] wirePointsFour;
	public LineRenderer[] lines;
	public GameObject[] wireStarts;
	public GameObject[] wireEnds;
	public KMSelectable green;
	public GameObject[] squares;
	public KMSelectable moduleSelectable;
	public KMSelectable wireZero;
	public GameObject[] buttonObjects;
	public GameObject movesBack;
	public GameObject[] finalWires;
	public MeshRenderer[] finalWiresMesh;
	public KMSelectable[] finalClickables;
	public GameObject[] cutWires;
	public MeshRenderer[] cutWireMain;

	private static int _moduleIdCounter = 1;
	private int _moduleId = 0;
	private int moves;
	private bool _isSolved = false;
	private int[] rowOne = new int[4];
	private int[] rowTwo = new int[4];
	private int[] rowThree = new int[4];
	private int[] rowFour = new int[4];
	private int finalTotal = 0;
	private LineRenderer blackWire, blueWire, redWire, yellowWire;

	int[] swap(int x, int y, int[] array){
		// swap 2 things in an array
		int temp = array[x];
		array[x] = array[y];
		array[y] = temp;    
		return array;
	}
	void Start () {
		blackWire = lines[0];
		blueWire = lines[1];
		redWire = lines[2];
		yellowWire = lines[3];

		_moduleId = _moduleIdCounter++;

		for (int i = 0; i < 4; i++) {
			finalWires [i].SetActive (false);
			cutWires [i].SetActive (false);
		}

		//Choose wires start positions
		int[] zott = {0, 1, 2, 3};
		int[][] rows = { rowOne, rowTwo, rowThree, rowFour };

		for (int p = 0; p < 4; p++) {
			int[] numArray = zott;
			int[] newArray = zott;
			for (int i = 0; i < 4; i++) {
				numArray = newArray;
				int rand = Random.Range (0, (4 - i));
				rows[p] [i] = numArray [rand];
				newArray = new int[3 - i];
				int count = 0;
				for (int j = 0; j < numArray.Length; j++) {
					if (!(j == rand)) {
						newArray [count] = numArray [j];
						count++;
					}
				}
			}
		}

		//Wire settings
		blackWire.material = wireMats[0];
		blackWire.startColor = Color.black;
		blackWire.endColor = Color.black;
		blackWire.positionCount = 6;
		blackWire.useWorldSpace = false;
		blueWire.material = wireMats[1];
		blueWire.startColor = Color.blue;
		blueWire.endColor = Color.blue;
		blueWire.positionCount = 6;
		blueWire.useWorldSpace = false;
		redWire.material = wireMats[2];
		redWire.startColor = Color.red;
		redWire.endColor = Color.red;
		redWire.positionCount = 6;
		redWire.useWorldSpace = false;
		yellowWire.material = wireMats[3];
		yellowWire.startColor = Color.yellow;
		yellowWire.endColor = Color.yellow;
		yellowWire.positionCount = 6;
		yellowWire.useWorldSpace = false;

		updateWires ();


		for (int i = 0; i < 9; i++) {
			int j = i;
			swapButtons [i].OnInteract += delegate () {
				handlePress (j);
				return false;
			};
		}
		green.OnInteract += delegate () {
			handlePress (9);
			return false;
		};
	}

	void handlePress(int numberPos){
		if (numberPos == 0) {
			rowOne = swap (0, 1, rowOne);
			rowTwo = swap (0, 1, rowTwo);
		} else if (numberPos == 1) {
			rowOne = swap (1, 2, rowOne);
			rowTwo = swap (1, 2, rowTwo);
		} else if (numberPos == 2) {
			rowOne = swap (2, 3, rowOne);
			rowTwo = swap (2, 3, rowTwo);
		} else if (numberPos == 3) {
			rowTwo = swap (0, 1, rowTwo);
			rowThree = swap (0, 1, rowThree);
		} else if (numberPos == 4) {
			rowTwo = swap (1, 2, rowTwo);
			rowThree = swap (1, 2, rowThree);
		} else if (numberPos == 5) {
			rowTwo = swap (2, 3, rowTwo);
			rowThree = swap (2, 3, rowThree);
		} else if (numberPos == 6) {
			rowThree = swap (0, 1, rowThree);
			rowFour = swap (0, 1, rowFour);
		} else if (numberPos == 7) {
			rowThree = swap (1, 2, rowThree);
			rowFour = swap (1, 2, rowFour);
		} else if (numberPos == 8) {
			rowThree = swap (2, 3, rowThree);
			rowFour = swap (2, 3, rowFour);
		} else if (numberPos == 9) {
			rowOne = swap (1, 2, rowOne);
			moves += 4;
		}
		moves++;
		updateWires ();
		if (rowOne[0] == rowTwo[0] && rowOne[0] == rowThree[0] && rowOne[0] == rowFour[0] && rowOne[1] == rowTwo[1] && rowOne[1] == rowThree[1] && rowOne[1] == rowFour[1] && rowOne[2] == rowTwo[2] && rowOne[2] == rowThree[2] && rowOne[2] == rowFour[2] && rowOne[3] == rowTwo[3] && rowOne[3] == rowThree[3] && rowOne[3] == rowFour[3]) {
			Audio.PlaySoundAtTransform ("untangled", Module.transform);
			untangled ();
		} else {
			Audio.PlayGameSoundAtTransform (KMSoundOverride.SoundEffect.ButtonPress, Module.transform);
		}
	}

	void untangled() {
		float repeats = (moves / 4);
		if (moves % 4 == 0) {
			repeats -= 1;
		}
		repeats -= repeats % 1;
		int repeatsInt = (int)repeats;
		int serialFirst = Info.GetSerialNumberNumbers ().First ();
		int serialLast = Info.GetSerialNumberNumbers ().Last ();
		int current = serialFirst;
		for (int i = 0; i <= repeatsInt; i++) {
			current += serialLast;
			current = current * 2;
			if (current > 500) {
				current = current / 10;
			}
			Debug.LogFormat ("[Tangled Wires #{0}] After {1} steps the sum is {2}.", _moduleId, i+1, current);
		}
		finalTotal = current;
		for (int i = 0; i < 4; i++) {
			finalWires [i].SetActive (true);
			finalWiresMesh [i].material = wireMats [rowOne [i]];
			cutWireMain [2 * i].material = wireMats [rowOne [i]];
			cutWireMain [(2 * i)+1].material = wireMats [rowOne [i]];
		}

		for (int i = 0; i < buttonObjects.Count(); i++) {
			buttonObjects [i].SetActive (false);
		}
		for (int i = 0; i < 4; i++) {
			int j = i;
			lines [i].enabled = false;
			finalClickables[i].OnInteract += delegate () {
				handleWireCut (j);
				return false;
			};
		}
		moduleSelectable.Children [0] = finalClickables[0];
		moduleSelectable.Children [1] = finalClickables[1];
		moduleSelectable.Children [2] = finalClickables[2];
		moduleSelectable.Children [3] = finalClickables[3];
		moduleSelectable.Children [4] = null;
		moduleSelectable.Children [5] = null;
		moduleSelectable.Children [6] = null;
		moduleSelectable.Children [7] = null;
		moduleSelectable.Children [8] = null;
		moduleSelectable.Children [9] = null;
		moduleSelectable.Children [10] = null;
		moduleSelectable.Children [11] = null;
		moduleSelectable.Children [12] = null;
		moduleSelectable.Children [13] = null;
		moduleSelectable.Children [14] = null;
		moduleSelectable.Children [15] = null;
		moduleSelectable.UpdateChildrenProperly(wireZero);
	}


	void handleWireCut(int position){
		if (_isSolved) {
			return;
		}
		finalWires [position].SetActive (false);
		Audio.PlayGameSoundAtTransform (KMSoundOverride.SoundEffect.WireSnip, Module.transform);
		cutWires [position].SetActive (true);
		if (rowOne [position] == 0) {
			Debug.LogFormat ("[Tangled Wires #{0}] Black wire removed.", _moduleId);
			if ((finalTotal % 10) == 0) {
				Debug.LogFormat ("[Tangled Wires #{0}] That was correct.", _moduleId);
				_isSolved = true;
				Module.HandlePass ();
			} else {
				Debug.LogFormat ("[Tangled Wires #{0}] That was incorrect, strike issued.", _moduleId);
				Module.HandleStrike ();
			}
		} else if (rowOne [position] == 1) {
			Debug.LogFormat ("[Tangled Wires #{0}] Blue wire removed.", _moduleId);
			if ((finalTotal % 10) == 2 || (finalTotal % 10) == 5 || (finalTotal % 10) == 8) {
				Debug.LogFormat ("[Tangled Wires #{0}] That was correct.", _moduleId);
				_isSolved = true;
				Module.HandlePass ();
			} else {
				Debug.LogFormat ("[Tangled Wires #{0}] That was incorrect, strike issued.", _moduleId);
				Module.HandleStrike ();
			}
		} else if (rowOne [position] == 2) {
			Debug.LogFormat ("[Tangled Wires #{0}] Red wire removed.", _moduleId);
			if ((finalTotal % 10) == 1 || (finalTotal % 10) == 6 || (finalTotal % 10) == 9) {
				Debug.LogFormat ("[Tangled Wires #{0}] That was correct.", _moduleId);
				_isSolved = true;
				Module.HandlePass ();
			} else {
				Debug.LogFormat ("[Tangled Wires #{0}] That was incorrect, strike issued.", _moduleId);
				Module.HandleStrike ();
			}
		} else if (rowOne [position] == 3) {
			Debug.LogFormat ("[Tangled Wires #{0}] Yellow wire removed.", _moduleId);
			if ((finalTotal % 10) == 3 || (finalTotal % 10) == 7 || (finalTotal % 10) == 4) {
				Debug.LogFormat ("[Tangled Wires #{0}] That was correct.", _moduleId);
				_isSolved = true;
				Module.HandlePass ();
			} else {
				Debug.LogFormat ("[Tangled Wires #{0}] That was incorrect, strike issued.", _moduleId);
				Module.HandleStrike ();
			}
		} else {
			Debug.LogFormat ("[Tangled Wires #{0}] An error occurred, solving module.", _moduleId);
			_isSolved = true;
			Module.HandlePass ();
		}
		return;
	}

	void updateWires() {
		List<int> rowOneList = new List<int> () { rowOne [0], rowOne [1], rowOne [2], rowOne [3] };
		List<int> rowTwoList = new List<int> () { rowTwo [0], rowTwo [1], rowTwo [2], rowTwo [3] };
		List<int> rowThreeList = new List<int> () { rowThree [0], rowThree [1], rowThree [2], rowThree [3] };
		List<int> rowFourList = new List<int> () { rowFour [0], rowFour [1], rowFour [2], rowFour [3] };
		blackWire.SetPosition (0, wireStarts [rowOneList.IndexOf(0)].transform.localPosition);
		blackWire.SetPosition (1, wirePointsOne [rowOneList.IndexOf(0)].transform.localPosition);
		blackWire.SetPosition (2, wirePointsTwo [rowTwoList.IndexOf(0)].transform.localPosition);
		blackWire.SetPosition (3, wirePointsThree [rowThreeList.IndexOf(0)].transform.localPosition);
		blackWire.SetPosition (4, wirePointsFour [rowFourList.IndexOf(0)].transform.localPosition);
		blackWire.SetPosition (5, wireEnds [rowFourList.IndexOf(0)].transform.localPosition);
		blueWire.SetPosition (0, wireStarts [rowOneList.IndexOf(1)].transform.localPosition);
		blueWire.SetPosition (1, wirePointsOne [rowOneList.IndexOf(1)].transform.localPosition);
		blueWire.SetPosition (2, wirePointsTwo [rowTwoList.IndexOf(1)].transform.localPosition);
		blueWire.SetPosition (3, wirePointsThree [rowThreeList.IndexOf(1)].transform.localPosition);
		blueWire.SetPosition (4, wirePointsFour [rowFourList.IndexOf(1)].transform.localPosition);
		blueWire.SetPosition (5, wireEnds [rowFourList.IndexOf(1)].transform.localPosition);
		redWire.SetPosition (0, wireStarts [rowOneList.IndexOf(2)].transform.localPosition);
		redWire.SetPosition (1, wirePointsOne [rowOneList.IndexOf(2)].transform.localPosition);
		redWire.SetPosition (2, wirePointsTwo [rowTwoList.IndexOf(2)].transform.localPosition);
		redWire.SetPosition (3, wirePointsThree [rowThreeList.IndexOf(2)].transform.localPosition);
		redWire.SetPosition (4, wirePointsFour [rowFourList.IndexOf(2)].transform.localPosition);
		redWire.SetPosition (5, wireEnds [rowFourList.IndexOf(2)].transform.localPosition);
		yellowWire.SetPosition (0, wireStarts [rowOneList.IndexOf(3)].transform.localPosition);
		yellowWire.SetPosition (1, wirePointsOne [rowOneList.IndexOf(3)].transform.localPosition);
		yellowWire.SetPosition (2, wirePointsTwo [rowTwoList.IndexOf(3)].transform.localPosition);
		yellowWire.SetPosition (3, wirePointsThree [rowThreeList.IndexOf(3)].transform.localPosition);
		yellowWire.SetPosition (4, wirePointsFour [rowFourList.IndexOf(3)].transform.localPosition);
		yellowWire.SetPosition (5, wireEnds [rowFourList.IndexOf(3)].transform.localPosition);
	}
	
	// Update is called once per frame
	void Update () {
		if (!_isSolved) {
			movesCounterText.text = "" + moves;
		} else {
			movesCounterText.text = "✓";
		}
	}

	private readonly string TwitchHelpMessage = @"Press an orange peg by typing '!{0} 1', with 1 being replaced by the reading order position of the peg you want to press, e.g. bottom middle would be 8. Press the green peg by typing '!{0} green'. Cut a wire by typing '!{0} cut1' with 1 being replaced by the position of the wire you wish to cut.";
	private IEnumerator ProcessTwitchCommand(string command){
		command = command.ToLowerInvariant ();
		if (command.Equals ("1")) {
			yield return null;
			handlePress (0);
		} else if (command.Equals ("2")) {
			yield return null;
			handlePress (1);
		} else if (command.Equals ("3")) {
			yield return null;
			handlePress (2);
		} else if (command.Equals ("4")) {
			yield return null;
			handlePress (3);
		} else if (command.Equals ("5")) {
			yield return null;
			handlePress (4);
		} else if (command.Equals ("6")) {
			yield return null;
			handlePress (5);
		} else if (command.Equals ("7")) {
			yield return null;
			handlePress (6);
		} else if (command.Equals ("8")) {
			yield return null;
			handlePress (7);
		} else if (command.Equals ("9")) {
			yield return null;
			handlePress (8);
		} else if (command.Equals ("green")) {
			yield return null;
			handlePress (9);
		} else if (command.Equals ("cut1")) {
			yield return null;
			handleWireCut(0);
		} else if (command.Equals ("cut2")) {
			yield return null;
			handleWireCut(1);
		} else if (command.Equals ("cut3")) {
			yield return null;
			handleWireCut(2);
		} else if (command.Equals ("cut4")) {
			yield return null;
			handleWireCut(3);
		}
	}
}
