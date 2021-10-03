using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "TileData/Neutral")]
public class TileData : ScriptableObject
{
    public TileBase[] tiles;
    public int id;
    public int hp;
    public float contamination;

    [Header("Possible actions")]
    [SerializeField] protected bool recolect;
    [SerializeField] protected bool build;
    [SerializeField] protected bool destroy;
    [SerializeField] protected bool repair;

    public int actionValue { get; private set; }

    public void Initialize() {
        actionValue = StaticData.GenerateActionValue(
            recolect, build, destroy, repair);
    }

    virtual public float ContaminationByExisting() => 0;
    virtual public float ContaminationByDead() => 0;
}
