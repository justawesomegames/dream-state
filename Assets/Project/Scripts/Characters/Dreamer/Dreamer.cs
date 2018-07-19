using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dreamer : MovableCharacter {
	[SerializeField] private float lowerBlinkRangeTime;
	[SerializeField] private float upperBlinkRangeTime;

	private float nextBlink;
	private float blinkTimer;

	protected override void Awake() {
		base.Awake();
		setBlink(true);
	}

	protected override void Update() {
		base.Update();
		blinkTimer += Time.deltaTime;
		if (blinkTimer > nextBlink) {
			setBlink(false);
		}
	}

	private void setBlink(bool init) {
		if (!init) {
			animator.SetTrigger("Blink");
		}
		nextBlink = Random.Range(lowerBlinkRangeTime, upperBlinkRangeTime);
		blinkTimer = 0.0f;
	}
}
