using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DialogueConditionalSelector : MonoBehaviour {

	public enum Condition {
		Equals,
		LessThan,
		LessOrEqualsTo,
		GreaterThan,
		GreaterOrEqualsTo
	}

	[Serializable]
	public class DialogueConditionalEntry {
		public string DialogueKey;
		public Condition Condition;
		public int CompareValue;
		public UnityEvent Event;
	}

	public List<DialogueConditionalEntry> Entries = new List<DialogueConditionalEntry>();

	public int StartingValue = 0;
	private int value;

	private void Start() {
		value = StartingValue;
	}

	public void Set(int amount) {
		value += amount;
	}

	public void Add(int amount) {
		value = amount;
	}

	public string GetPhrase() {
		foreach (var item in Entries) {
			switch (item.Condition) {
				case Condition.Equals:
					if (item.CompareValue == value) {
						item.Event.Invoke();
						return item.DialogueKey;
					}
					break;
				case Condition.GreaterOrEqualsTo:
					if (item.CompareValue <= value) {
						item.Event.Invoke();
						return item.DialogueKey;
					}
					break;
				case Condition.GreaterThan:
					if (item.CompareValue < value) {
						item.Event.Invoke();
						return item.DialogueKey;
					}
					break;
				case Condition.LessOrEqualsTo:
					if (item.CompareValue >= value) {
						item.Event.Invoke();
						return item.DialogueKey;
					}
					break;
				case Condition.LessThan:
					if (item.CompareValue > value) {
						item.Event.Invoke();
						return item.DialogueKey;
					}
					break;

				default:
					break;
			}
		}

		return "";
	}

}
