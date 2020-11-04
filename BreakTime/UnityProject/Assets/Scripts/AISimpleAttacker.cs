using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AISimpleAttacker : AIBase {
	public Color AlertColor = Color.red;

	// chase player
	protected override void DoAlert() {
		// janky af
		if (!_alertPlayed) {
			_audio.PlayAttackAlert();
			_alertPlayed = true;
			Color = AlertColor;
			_render.color = AlertColor;
		}

		// if moving right and player on left, chase left
		if (_vel.x > 0 && _player.transform.position.x < transform.position.x)
			_vel = new Vector2(-_spd, 0);
		// and vice versa
		else if (_vel.x < 0 && _player.transform.position.x > transform.position.x)
			_vel = new Vector2(_spd, 0);
	}
}
