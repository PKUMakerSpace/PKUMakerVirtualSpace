using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSystem : MonoBehaviour {
    public List<PlayerController> players;
    public List<Objects> objects;
	// Use this for initialization
	void Start () {
        foreach(PlayerController p in players)
            p.mainSystem = this;
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
