﻿using System;
using System.Collections.Generic;
using UnityEngine.Assertions;

namespace AsgUtils.Unity
{
    /// <summary>
    ///     A value that can have various layers of overrides to set a final value. Adding a layer allows you to
    ///     temporarily shadow this value with a different value. Other systems can modify lower layers without touching the
    ///     current value. This allows us to easily layer changes of the value from different, independent systems.
    /// </summary>
    /// <remarks>
    ///     Override layers on top of the BaseValue are stored in a stack. The most recent override is used as the
    ///     'perceived' value.
    /// </remarks>
    public class LayeredValue<T>
    {
        /// <summary>
        ///     The base value. This value is never removed. If you want to make permanent changes to a value (rather than
        ///     shadowing it with a new value temporarily), change this value.
        /// </summary>
        public T BaseValue
        {
            set => layers[0].Value = value;
            get => layers[0].Value;
        }

        private readonly List<Layer> layers = new List<Layer>();
        private readonly Action<T> onValueUpdate;

        /// <param name="baseValue">The value this object starts with.</param>
        /// <param name="onValueUpdate">A callback that is run whenever the perceived value of this object may have changed.</param>
        public LayeredValue(T baseValue, Action<T> onValueUpdate = null)
        {
            this.onValueUpdate = onValueUpdate;
            layers.Add(new Layer(this, baseValue));
        }

        /// <summary>Gets the 'perceived' value, which is the value after all overrides have been applied.</summary>
        public T Get()
        {
            return layers[layers.Count - 1].Value;
        }

        /// <summary>
        ///     Adds a new override to this value. Until this layer is removed or another one is added on top, this layer
        ///     represents the "current perceived" value for this object.
        /// </summary>
        public Layer AddOverrideLayer(T newValue)
        {
            Layer l = new Layer(this, newValue);

            layers.Add(l);

            OnValuePossiblyChanged();
            return l;
        }

        /// <summary>Removes the given layer from the override system, returning the perceived value to the override below.</summary>
        public void RemoveOverrideLayer(Layer toRemove)
        {
            bool removed = layers.Remove(toRemove);
            Assert.IsTrue(removed);
            
            OnValuePossiblyChanged();
        }

        /// <summary>Checks to see if a layer is at the top of the override stack, and therefore acting as the 'perceived' value.</summary>
        public bool IsHighestOverride(Layer layer)
        {
            Assert.IsTrue(layers.Contains(layer));
            return layers.IndexOf(layer) == layers.Count - 1;
        }

        /// <summary>Called when the 'perceived' value may have changed.</summary>
        private void OnValuePossiblyChanged()
        {
            // Currently does not deduplicate changes, so this may be called even if the actual value does not change, just the underlying layers.
            onValueUpdate?.Invoke(Get());
        }

        /// <summary>
        ///     Single layer that overrides all values in the layers below. The value of this object is the value of the
        ///     highest layer override.
        /// </summary>
        public class Layer
        {
            /// <summary>Set or get the value in the override layer itself.</summary>
            public T Value
            {
                get => value;
                set
                {
                    this.value = value;
                    parent.OnValuePossiblyChanged();
                }
            }

            private readonly LayeredValue<T> parent;
            private T value;

            /// <summary>Create a new layer.</summary>
            internal Layer(LayeredValue<T> parent, T value)
            {
                this.value = value;
                this.parent = parent;
            }
        }
    }
}
