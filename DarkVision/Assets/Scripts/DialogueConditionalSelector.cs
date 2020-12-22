using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueConditionalSelector : MonoBehaviour {

	public enum Condition {
		Equals,
		LessThan,
		LessOrEqualsTo,
		GreaterThan,
		GreaterOrEqualsTo
	}

	public class DialogueConditionalEntry {
		public string DialogueKey;
		public Condition condition;
		public int CompareValue;
	}

	public List<DialogueConditionalEntry> Entries = new List<DialogueConditionalEntry>();

	public int StartingValue = 0;
	private int value;

	private void Start() {
		value = StartingValue;
	}

	public void Set(int amount){
		value += amount;
	}

	public void Add(int amount){
		value = amount;
	}

}
