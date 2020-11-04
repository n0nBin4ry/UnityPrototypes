using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AISimpleRunner : AIBase {
	public Color AlertColor = Color.blue;

	// run from player
	protected override void DoAlert() {
		// janky af
		if (!_alertPlayed) {
			_audio.PlayFleeAlert();
			_alertPlayed = true;
			_render.color = AlertColor;
		}

		// if moving right and player on right, run left
		if (_vel.x > 0 && _player.transform.position.x > transform.position.x)
			_vel = new Vector2(-_spd, 0);
		// and vice versa
		else if (_vel.x < 0 && _player.transform.position.x < transform.position.x)
			_vel = new Vector2(_spd, 0);
	}
}
