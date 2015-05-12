using System;
using UnityEngine;

namespace AssemblyCSharp
{
	public class Piece
	{
		public GameObject obj { get; set;}

		public string name { get; private set; }

		public string position { get; set;}
		
		public bool inMovement { get; set; }
		
		public Vector3 newPosition { get; set; }

		public Piece (string name, string pos)
		{
			this.name = name;
			this.position = pos;
			this.obj = GameObject.Find (name);
			this.inMovement = false;
		}
	}
}

