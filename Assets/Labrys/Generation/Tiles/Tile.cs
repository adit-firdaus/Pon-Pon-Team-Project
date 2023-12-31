﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Labrys.Generation;

namespace Labrys.Tiles
{
    /// <summary>
    /// A physical, individual 1x1 grid element to be placed in the world.
    /// </summary>
    //[System.Serializable]
    public class Tile : MonoBehaviour
    {
        /// <summary>
        /// The type of the connection.
        /// </summary>
        // TODO: Make a custom inspector so that this shows up in the inspector. See SectionInspector.
        [Tooltip("The connection type.")]
        [SerializeField]
        public TileType type;

        /// <summary>
        /// The variant of this Tile. This is used with the TileType to uniquely
        /// determine this Tile in TileSet.
        /// </summary>
        [SerializeField]
        public string variant;

        // TODO remove me; this is debug
        [SerializeField]
        public Section section;

        public override string ToString()
        {
            return $"Tile type {type.Name}, variant {variant}, prefab {gameObject.name}";
        }
    }
}
