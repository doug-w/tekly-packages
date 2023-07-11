using System;
using System.Linq;
using Tekly.Common.Timers;
using UnityEditor;
using UnityEngine;

namespace Tekly.TwoD.Cells
{
	[CustomEditor(typeof(CellAnimator), true)]
	public class CellAnimatorEditor : Editor
	{
		private SerializedProperty m_sprite;
		private SerializedProperty m_animName;
		private SerializedProperty m_loop;
		private SerializedProperty m_playOnEnable;
		private SerializedProperty m_randomizeTime;
		private SerializedProperty m_timer;

		private CellAnimator m_cellAnimator;

		private string[] m_anims;
		
		private void OnEnable()
		{
			m_cellAnimator = target as CellAnimator;

			m_sprite = serializedObject.FindProperty("m_sprite");
			m_animName = serializedObject.FindProperty("m_animName");
			m_loop = serializedObject.FindProperty("m_loop");
			m_playOnEnable = serializedObject.FindProperty("m_playOnEnable");
			m_randomizeTime = serializedObject.FindProperty("m_randomizeTime");
			m_timer = serializedObject.FindProperty("m_timer");

			GrabAnims();
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			using (var check = new EditorGUI.ChangeCheckScope()) {
				EditorGUILayout.PropertyField(m_sprite);

				if (check.changed) {
					var cellSprite = m_sprite.objectReferenceValue as CellSprite;
					if (cellSprite != null) {
						m_cellAnimator.SetSprite(cellSprite.Icon);
					} else {
						m_cellAnimator.SetSprite(null);
					}
					
					GrabAnims();
				}
			}

			DrawAnims();

			EditorGUILayout.PropertyField(m_loop);
			EditorGUILayout.PropertyField(m_playOnEnable);
			EditorGUILayout.PropertyField(m_randomizeTime);
			EditorGUILayout.PropertyField(m_timer);

			if (m_timer.objectReferenceValue == null) {
				m_timer.objectReferenceValue = GetTimer();
			}
			
			serializedObject.ApplyModifiedProperties();
		}

		private void DrawAnims()
		{
			if (m_anims.Length == 0) {
				EditorGUILayout.LabelField("No Anims Found");
				return;
			}

			if (string.IsNullOrEmpty(m_animName.stringValue)) {
				m_animName.stringValue = m_anims[0];
			}

			var oldIndex = Array.IndexOf(m_anims, m_animName.stringValue);
			if (oldIndex == -1) {
				EditorGUILayout.LabelField($"Anim [{m_animName.stringValue}] wasn't found.");
				return;
			}
			
			var index = EditorGUILayout.Popup("Animation", oldIndex, m_anims);

			if (index != oldIndex) {
				m_animName.stringValue = m_anims[index];
			}
		}

		private void GrabAnims()
		{
			if (m_sprite.objectReferenceValue == null) {
				m_anims = Array.Empty<string>();
			} else {
				var cellSprite = m_sprite.objectReferenceValue as CellSprite;
				m_anims = cellSprite.Animations.Select(x => x.name).ToArray();
			}
		}

		private TimerRef GetTimer()
		{
			var monoScript = MonoScript.FromMonoBehaviour(m_cellAnimator);
			var monoImporter = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(monoScript)) as MonoImporter;
			var timerRef = monoImporter.GetDefaultReference("m_timer");

			return timerRef as TimerRef;
		}
		
		public override bool HasPreviewGUI() => true;

		public override void OnPreviewGUI(Rect r, GUIStyle background)
		{
			if (m_sprite.objectReferenceValue == null) {
				GUI.Label(r, "No Sprite");
				return;
			}

			var cellSprite = m_sprite.objectReferenceValue as CellSprite;
			DrawSprite(r, cellSprite.Icon);
		}

		private void DrawSprite(Rect r, Sprite sprite)
		{
			var uv = sprite.rect;
			uv.x /= sprite.texture.width;
			uv.width /= sprite.texture.width;
			
			uv.y /= sprite.texture.height;
			uv.height /= sprite.texture.height;

			var aspect = sprite.textureRect.width / sprite.textureRect.height;

			if (r.width < r.height) {
				r.height = r.width / aspect; 
			} else {
				r.width = r.height * aspect;
			}

			GUI.DrawTextureWithTexCoords(r, sprite.texture, uv, true);
		}
	}
}