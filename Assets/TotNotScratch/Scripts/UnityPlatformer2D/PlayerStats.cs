using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Platformer
{
	public class PlayerStats : MonoBehaviour
	{
		public Watchable<int> health = new Watchable<int>(3);

		private Watchable<int> _points = new Watchable<int>(0);
		public Watchable<int> points {
			get {
				return _points;
            }
            set {
				if (_points._value != value)
				{
					_points._value = value;
					PlayerPrefs.SetInt("Points", _points._value);
				}
            }
        }
    }
}
