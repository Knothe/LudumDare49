using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "TileData/HumanMade")]
public class HumanTile : TileData {
    public override float ContaminationByExisting() => contamination;
}