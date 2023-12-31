﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using Labrys.Tiles;

namespace Labrys.Generation
{
    [CustomEditor(typeof(Tile))]
    public class TileInspector : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            Tile tile = (Tile)target;
            SerializedObject serializedTile = new SerializedObject(tile);

            TileType type = tile.type;

            // Make the TileType's Name visible, but not editable.
            // TODO make a good looking drop-down (not just Enum based one, but 
            // one that has images representing TileTypes), and make this editable then.
            // 
            // Or have a "generated" flag that gets set by default if it's automatically
            // made by a tool, and if it's false, allow editing.
            GUI.enabled = false;
            EditorGUILayout.TextField("Name", type.Name);
            GUI.enabled = true;

            EditorGUILayout.TextField("Variant", tile.variant);
            //EditorGUILayout.PropertyField(serializedTile.FindProperty("gameObject"));

            // TODO remove this debug
            GUI.enabled = false;
            EditorGUILayout.TextField("Internal Connections", tile.section.internalConnections.ToString());
            EditorGUILayout.TextField("External Connections", tile.section.externalConnections.ToString());
            GUI.enabled = true;
        }
    }
}
