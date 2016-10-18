using UnityEditor;
using UnityEngine;

namespace ModU3DToolkit.TransitionFX
{
    [CustomEditor(typeof(CameraTransitionsFX))]
    public class CameraTransitionsFXEditor : Editor
    {
        GUIContent _tooltip;

        MonoScript _script;

        void OnEnable()
        {
            _script = MonoScript.FromMonoBehaviour((CameraTransitionsFX)target);
        }

        public override void OnInspectorGUI()
        {
            var proCamera2DTransitionsFX = (CameraTransitionsFX)target;

            serializedObject.Update();

            // Show script link
            GUI.enabled = false;
            _script = EditorGUILayout.ObjectField("Script", _script, typeof(MonoScript), false) as MonoScript;
            GUI.enabled = true;

            EditorGUILayout.Space();

            // Enter
            EditorGUI.indentLevel = 1;

            _tooltip = new GUIContent("Duration", "The duration of the animation");
            EditorGUILayout.PropertyField(serializedObject.FindProperty("DurationEnter"), _tooltip);

            _tooltip = new GUIContent("Delay", "A delay for the start of the animation");
            EditorGUILayout.PropertyField(serializedObject.FindProperty("DelayEnter"), _tooltip);

            _tooltip = new GUIContent("Fade", "Fade the animation");
            EditorGUILayout.PropertyField(serializedObject.FindProperty("FadeEnter"), _tooltip);

            _tooltip = new GUIContent("IgnoreTransitionTex", "Ignore the transition texture");
            EditorGUILayout.PropertyField(serializedObject.FindProperty("IgnoreTransitionTexEnter"), _tooltip);

            _tooltip = new GUIContent("Ease Type", "The ease type of the animation");
            EditorGUILayout.PropertyField(serializedObject.FindProperty("EaseTypeEnter"), _tooltip);

            _tooltip = new GUIContent("Background Color", "The background color of the animation");
            EditorGUILayout.PropertyField(serializedObject.FindProperty("BackgroundColorEnter"), _tooltip);

            _tooltip = new GUIContent("Texture", "The texture to use as a mask. Should be a black and white texture.");
            EditorGUILayout.PropertyField(serializedObject.FindProperty("TextureEnter"), _tooltip);

            _tooltip = new GUIContent("Texture Smoothing", "The amount of fade smoothing to apply on the texture.");
            EditorGUILayout.PropertyField(serializedObject.FindProperty("TextureSmoothingEnter"), _tooltip);

            EditorGUI.indentLevel = 0;

            EditorGUILayout.Space();

            // Exit
            EditorGUI.indentLevel = 1;

            _tooltip = new GUIContent("Duration", "The duration of the animation");
            EditorGUILayout.PropertyField(serializedObject.FindProperty("DurationExit"), _tooltip);

            _tooltip = new GUIContent("Delay", "A delay for the start of the animation");
            EditorGUILayout.PropertyField(serializedObject.FindProperty("DelayExit"), _tooltip);

            _tooltip = new GUIContent("Fade", "Fade the animation");
            EditorGUILayout.PropertyField(serializedObject.FindProperty("FadeExit"), _tooltip);

            _tooltip = new GUIContent("IgnoreTransitionTex", "Ignore the transition texture");
            EditorGUILayout.PropertyField(serializedObject.FindProperty("IgnoreTransitionTexExit"), _tooltip);

            _tooltip = new GUIContent("Ease Type", "The ease type of the animation");
            EditorGUILayout.PropertyField(serializedObject.FindProperty("EaseTypeExit"), _tooltip);

            _tooltip = new GUIContent("Background Color", "The background color of the animation");
            EditorGUILayout.PropertyField(serializedObject.FindProperty("BackgroundColorExit"), _tooltip);

            _tooltip = new GUIContent("Texture", "The texture to use as a mask. Should be a black and white texture.");
            EditorGUILayout.PropertyField(serializedObject.FindProperty("TextureExit"), _tooltip);

            _tooltip = new GUIContent("Texture Smoothing", "The amount of fade smoothing to apply on the texture.");
            EditorGUILayout.PropertyField(serializedObject.FindProperty("TextureSmoothingExit"), _tooltip);

            EditorGUI.indentLevel = 0;

            // Start scene on enter state
            EditorGUILayout.Space();
            _tooltip = new GUIContent("Start Scene On Enter State", "If selected, on scene start the Enter FX will be loaded.");
            EditorGUILayout.PropertyField(serializedObject.FindProperty("StartSceneOnEnterState"), _tooltip);

			// Start scene on enter state
			EditorGUILayout.Space();
			_tooltip = new GUIContent("Auto Start", "If selected, start transition when on scene start.");
			EditorGUILayout.PropertyField(serializedObject.FindProperty("AutoStart"), _tooltip);


            // Limit values
            if (proCamera2DTransitionsFX.DurationEnter < 0)
                proCamera2DTransitionsFX.DurationEnter = 0;
            if (proCamera2DTransitionsFX.DurationExit < 0)
                proCamera2DTransitionsFX.DurationExit = 0;

            if (proCamera2DTransitionsFX.DelayEnter < 0)
                proCamera2DTransitionsFX.DelayEnter = 0;
            if (proCamera2DTransitionsFX.DelayExit < 0)
                proCamera2DTransitionsFX.DelayExit = 0;

            // Apply properties
            serializedObject.ApplyModifiedProperties();

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            // Detect changes
            if (GUI.changed && Application.isPlaying)
            {
                proCamera2DTransitionsFX.UpdateTransitionsProperties();
                proCamera2DTransitionsFX.UpdateTransitionsColor();
            }


            // Test buttons
            GUI.enabled = Application.isPlaying;
            if (GUILayout.Button("Transition Enter"))
            {
                proCamera2DTransitionsFX.TransitionEnter();
            }

            if (GUILayout.Button("Transition Exit"))
            {
                proCamera2DTransitionsFX.TransitionExit();
            }
            GUI.enabled = true;
        }
    }
}