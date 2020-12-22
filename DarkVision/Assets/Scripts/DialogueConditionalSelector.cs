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

	public class DialogueConditionaEntry {
		public string DialogueKey;
		public Condition condition;

	}

	public int StartingValue = 0;
	private int value;

}
