﻿using System;
using UnityEngine;

namespace Utilities {
  public class NumUtils {
    /// <summary>
    /// Determine the signed angle between two vectors, with normal 'n'
    /// as the rotation axis.
    /// </summary>
    public static float AngleSigned(Vector3 v1, Vector3 v2, Vector3 n) {
      return Mathf.Atan2(
          Vector3.Dot(n, Vector3.Cross(v1, v2)),
          Vector3.Dot(v1, v2)) * Mathf.Rad2Deg;
    }

    public static float NearestMultiple(float value, float factor) {
      return (float) Math.Round(value / (double) factor, MidpointRounding.AwayFromZero) * factor;
    }
  }
}
