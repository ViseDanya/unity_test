using UnityEngine;
using System.Collections.Generic;

public class OneThing : DynamicObject
{
    // players is ordered left to right if horizontal and down to up if vertical
    public List<Player> players;

    public override float mass => players.Count;
}
