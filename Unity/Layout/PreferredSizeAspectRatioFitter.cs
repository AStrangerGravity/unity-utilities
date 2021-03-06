﻿using UnityEngine;
using UnityEngine.UI;

namespace Utilities.Unity.Layout
{
    /// <summary>
    ///     An AspectRatioFitter extension that sets the aspect ratio from the preferredSize of this layout element. This
    ///     is useful for using image / content sizes as the aspect ratio.
    /// </summary>
    [ExecuteInEditMode]
    [RequireComponent(typeof(RectTransform))]
    [DisallowMultipleComponent]
    public class PreferredSizeAspectRatioFitter : AspectRatioFitter
    {
        protected new void Start()
        {
            UpdateAspect();
        }

        private void UpdateAspect()
        {
            var width = LayoutUtility.GetPreferredSize(GetComponent<RectTransform>(), 0);
            var height = LayoutUtility.GetPreferredSize(GetComponent<RectTransform>(), 1);
            var ratio = width / height;

            if (!float.IsNaN(ratio))
            {
                // ReSharper disable once CompareOfFloatsByEqualityOperator
                if (ratio != aspectRatio)
                {
                    aspectRatio = Mathf.Clamp(ratio, 1f / 1000f, 1000f);
                }
            }
        }

        protected override void OnRectTransformDimensionsChange()
        {
            UpdateAspect();
            base.OnRectTransformDimensionsChange();
        }

        /// <summary>
        ///     <para>Method called by the layout system.</para>
        /// </summary>
        public override void SetLayoutHorizontal()
        {
            UpdateAspect();
        }

        /// <summary>
        ///     <para>Method called by the layout system.</para>
        /// </summary>
        public override void SetLayoutVertical()
        {
            UpdateAspect();
        }
    }
}
