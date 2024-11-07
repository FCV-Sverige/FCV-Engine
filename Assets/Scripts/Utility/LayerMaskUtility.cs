using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LayerMaskUtility
{
    // check if gameObject is in a specified layer
    public static bool IsInLayerMask(GameObject obj, LayerMask mask) => (mask.value & (1 << obj.layer)) != 0;
    public static bool IsInLayerMask(int layer, LayerMask mask) => (mask.value & (1 << layer)) != 0;
}
