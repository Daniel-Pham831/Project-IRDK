using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace ToolBox.Tags
{
	[DisallowMultipleComponent, DefaultExecutionOrder(-9000), ExecuteInEditMode]
	internal sealed class Taggable : MonoBehaviour
	{
		[SerializeField] private List<Tag> _tags = new List<Tag>();
		public List<Tag> Tags => _tags;
		public bool ShouldApplyToAllChildren;

		private void Awake()
		{
			gameObject.AddTags(_tags);
		}

		private void OnDestroy()
		{
			gameObject.RemoveTags(_tags);
		}

#if UNITY_EDITOR
		internal void Add(Tag tag)
		{
			_tags.Add(tag);
		}

		internal void Remove(Tag tag)
		{
			_tags.Remove(tag);
		}

		internal bool Contains(Tag tag)
		{
			return _tags.Contains(tag);
		}
#endif
	}
}