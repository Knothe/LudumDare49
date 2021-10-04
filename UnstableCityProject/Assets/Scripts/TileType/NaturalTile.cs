using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Los recursos al llegar a 0 de hp generan contaminación en ese turno
[CreateAssetMenu(menuName = "TileData/Natural")]
public class NaturalTile : TileData {
    public override int ContaminationByDead() => contamination;
}