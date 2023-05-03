#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace ToolBox.Tags.Editor
{
	[CustomEditor(typeof(Taggable))]
	internal class TaggableEditor : UnityEditor.Editor
	{
		private List<Tag> _tags = null;
		private bool _applyAllChildren = false;

		private void OnEnable()
		{
			_tags = GetAllTags();
		}

		public override void OnInspectorGUI()
		{
			var taggable = target as Taggable;

			if (taggable == null)
				return;

			var instance = taggable.gameObject;
			int hash = instance.GetHashCode();
			AddApplyAllTickBox();
			foreach (var tag in _tags)
			{
				bool contains = taggable.Contains(tag);
				EditorGUILayout.BeginHorizontal();
				GUI.enabled = false;

				GUI.color = contains ? Color.green : Color.red;
				EditorGUILayout.ObjectField(tag, typeof(Tag), false);
				GUI.color = Color.white;
				
				GUI.enabled = !contains;
				if (GUILayout.Button("Add", EditorStyles.miniButtonLeft))
				{
					tag.Add(instance, hash);
					taggable.Add(tag);
					contains = true;
					
					if(_applyAllChildren)
						ApplyTagToAllChildren();
					
					EditorUtility.SetDirty(taggable);
				}
				
				GUI.enabled = contains;
				if (GUILayout.Button("Remove", EditorStyles.miniButtonLeft))
				{
					tag.Remove(instance, hash);
					taggable.Remove(tag);
					
					contains = false;					
					if(_applyAllChildren)
						ApplyTagToAllChildren();
					
					EditorUtility.SetDirty(taggable);
				}

				GUI.enabled = true;
				EditorGUILayout.EndHorizontal();
			}
			
			AddAddTagsButton();
			AddRemoveTagsButton();
			AssetDatabase.SaveAssetIfDirty(taggable);
		}

		private void AddApplyAllTickBox()
		{
			var taggable = (target as Taggable);

			_applyAllChildren = GUILayout.Toggle(taggable.ShouldApplyToAllChildren, "Should Apply All Children");
			
			taggable.ShouldApplyToAllChildren = _applyAllChildren;
		}

		private void AddAddTagsButton()
		{
			if (GUILayout.Button("Add Tags To All Children", EditorStyles.miniButtonLeft))
			{
				ApplyTagToAllChildren();
			}
		}

		private void AddRemoveTagsButton()
		{
			if (GUILayout.Button("Remove Tags From All Children", EditorStyles.miniButtonLeft))
			{
				RemoveAllTagsFromChildren();
			}
		}

		public void ApplyTagToAllChildren()
		{
			var taggable = (target as Taggable);
			var children = taggable.GetComponentsInChildren<Transform>(true);
			for (int i = 0; i < children.Length; i++)
			{
				var child = children[i];
				if (child == taggable.transform)
					continue;

				child.gameObject.RemoveTags(_tags);
				child.gameObject.AddTags(taggable.Tags);
			}

			EditorUtility.SetDirty(taggable.gameObject);
		}

		public void RemoveAllTagsFromChildren()
		{
			var taggable = (target as Taggable);
			var children = taggable.GetComponentsInChildren<Transform>(true);
			for (int i = 0; i < children.Length; i++)
			{
				var child = children[i];
				if (child == taggable.transform)
					continue;

				child.gameObject.RemoveTags(_tags);
				var taggableChild = child.GetComponent<Taggable>();
				DestroyImmediate(taggableChild);
			}

			EditorUtility.SetDirty(taggable.gameObject);
		}

		private static List<Tag> GetAllTags()
		{
			var paths = AssetDatabase.FindAssets("t:Tag").Select(AssetDatabase.GUIDToAssetPath);
			return paths.Select(AssetDatabase.LoadAssetAtPath<Tag>).ToList();
		}
	}
}
#endif