using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogicTile
{
    public int id { get; private set; }
    public float contaminationByExisting { get; private set; }
    public float contaminationByDead { get; private set; }
    public int actionValue { get; private set; }
    float health, maxHealth;
    public float healthPercentage => health / maxHealth;

    public LogicTile(TileData data) {
        SetNewValues(data);
    }

    public void SetNewValues(TileData data) {
        maxHealth = data.hp;
        health = (maxHealth < 0) ? 1 : maxHealth;
        id = data.id;
        actionValue = data.actionValue;
        if (data.id >= 4)
            SetValues((HumanTile)data);
        else if (data.id >= 1)
            SetValues((NaturalTile)data);
        else
            SetValues(data);
    }

    void SetValues(HumanTile tile) {
        contaminationByExisting = tile.ContaminationByExisting();
        contaminationByDead = tile.ContaminationByDead();
    }

    void SetValues(NaturalTile tile) {
        contaminationByExisting = tile.ContaminationByExisting();
        contaminationByDead = tile.ContaminationByDead();
    }

    void SetValues(TileData tile) {
        contaminationByExisting = tile.ContaminationByExisting();
        contaminationByDead = tile.ContaminationByDead();
    }

    /// <summary> Genera daño a un tile </summary>
    /// <param name="value">Cantidad de daño a inflingir</param>
    /// <returns>True si se muere</returns>
    public bool DealDamage(float value) {
        health -= value;
        return health <= 0;
    }
}
