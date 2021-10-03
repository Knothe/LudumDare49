using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class StaticData
{
    /// <summary> Genera un valor para resumir las acciones posibles por el tile </summary>
    /// <param name="recolect">Recolectar</param>
    /// <param name="build">Construir</param>
    /// <param name="destroy">Destruir</param>
    /// <param name="repair">Reparar</param>
    /// <returns>Valor de acción</returns>
    public static int GenerateActionValue(bool recolect, bool build, 
        bool destroy, bool repair) {
        int value = 0;
        if (recolect) value += 1;
        if (build) value += 2;
        if (destroy) value += 4;
        if (repair) value += 8;
        return value;
    }

    public static bool CanRecolect(int value) => (value & 1) != 0;
    public static bool CanBuild(int value) => (value & 2) != 0;
    public static bool CanDestroy(int value) => (value & 4) != 0;
    public static bool CanRepair(int value) => (value & 8) != 0;
}
