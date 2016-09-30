using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace ModU3DToolkit.TransitionFX
{
    public class CameraTransitionsFX : MonoBehaviour
    {
        public static string ExtensionName = "TransitionsFX";

        /// <summary>Fires whenever a TransitionEnter starts</summary>
        public Action OnTransitionEnterStarted;
        /// <summary>Fires whenever a TransitionEnter ends</summary>
        public Action OnTransitionEnterEnded;

        /// <summary>Fires whenever a TransitionExit starts</summary>
        public Action OnTransitionExitStarted;
        /// <summary>Fires whenever a TransitionExit ends</summary>
        public Action OnTransitionExitEnded;

        /// <summary>Fires whenever a TransitionEnter or a TransitionExit starts</summary>
        public Action OnTransitionStarted;
        /// <summary>Fires whenever a TransitionEnter or a TransitionExit ends</summary>
        public Action OnTransitionEnded;

        static CameraTransitionsFX _instance;

        public static CameraTransitionsFX Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = Camera.main.GetComponent<CameraTransitionsFX>();

                    if (_instance == null)
                        throw new UnityException("Main Camera does not have a TransitionFX extension.");
                }

                return _instance;
            }
        }

        public float DurationEnter = .5f;
        public float DelayEnter = 0f;
        public bool FadeEnter = false;
        public bool IgnoreTransitionTexEnter = false;
        public EaseType EaseTypeEnter = EaseType.EaseOut;
        public Color BackgroundColorEnter = Color.black;
        [Range(2, 128)]
        public int BlindsEnter = 16;
        public Texture TextureEnter;
        [Range(0, 1)]
        public float TextureSmoothingEnter = .2f;

        public float DurationExit = .5f;
        public float DelayExit = 0f;
        public bool FadeExit = false;
        public bool IgnoreTransitionTexExit = false;
        public EaseType EaseTypeExit = EaseType.EaseOut;
        public Color BackgroundColorExit = Color.black;
        [Range(2, 128)]
        public int BlindsExit = 16;
        public Texture TextureExit;
        [Range(0, 1)]
        public float TextureSmoothingExit = .2f;

        public bool StartSceneOnEnterState = true;

        Coroutine _transitionCoroutine;
        float _step;

        Material _transitionEnterMaterial;
        Material _transitionExitMaterial;

        BasicBlit _blit;

        int _material_StepID;
        int _material_BackgroundColorID;
        int _material_FadeID;
        int _material_IgnoreTransitionTexID;

        void Awake()
        {
            _material_StepID = Shader.PropertyToID("_Cutoff");
            _material_BackgroundColorID = Shader.PropertyToID("_Color");
            _material_FadeID = Shader.PropertyToID("_Fade");
            _material_IgnoreTransitionTexID = Shader.PropertyToID("_IgnoreTransitionTex");

            _transitionEnterMaterial = new Material(Shader.Find("Hidden/TransitionsFX"));
            _transitionExitMaterial = new Material(Shader.Find("Hidden/TransitionsFX"));

            _blit = gameObject.AddComponent<BasicBlit>();
            _blit.enabled = false;

            UpdateTransitionsProperties();
            UpdateTransitionsColor();

            if (StartSceneOnEnterState)
            {
                _step = 1f;
                _blit.CurrentMaterial = _transitionEnterMaterial;
                _blit.CurrentMaterial.SetFloat(_material_StepID, _step);
                _blit.CurrentMaterial.SetFloat(_material_FadeID, _step);
                _blit.enabled = true;
            }
        }

        /// <summary>
        /// Transition enter
        /// </summary>
        public void TransitionEnter()
        {
            Transition(_transitionEnterMaterial, DurationEnter, DelayEnter, 1f, 0f, EaseTypeEnter);
        }

        /// <summary>
        /// Transition exit
        /// </summary>
        public void TransitionExit()
        {
            Transition(_transitionExitMaterial, DurationExit, DelayExit, 0f, 1f, EaseTypeExit);
        }

        /// <summary>
        /// Updates the transitions properties.
        /// Use only if you changed the following properties during runtime: Direction, Side, Blinds, Texture, Texture Smoothing.
        /// For updating the background color, use the method UpdateTransitionsColor
        /// </summary>
        public void UpdateTransitionsProperties()
        {
            // Enter
            _transitionEnterMaterial.SetTexture("_TransitionTex", TextureEnter);
            _transitionEnterMaterial.SetFloat("_Smoothing", TextureSmoothingEnter);


            // Exit
            _transitionExitMaterial.SetTexture("_TransitionTex", TextureExit);
            _transitionExitMaterial.SetFloat("_Smoothing", TextureSmoothingExit);
        }

        /// <summary>
        /// Updates the transitions color.
        /// Use only if you changed the BackgroundColorEnter or BackgroundColorExit during runtime.
        /// </summary>
        public void UpdateTransitionsColor()
        {
            _transitionEnterMaterial.SetColor(_material_BackgroundColorID, BackgroundColorEnter);
            _transitionExitMaterial.SetColor(_material_BackgroundColorID, BackgroundColorExit);
        }

        /// <summary>
        /// Clears the current transition
        /// </summary>
        public void Clear()
        {
            _blit.enabled = false;
        }

        void Transition(Material material, float duration, float delay, float startValue, float endValue, EaseType easeType)
        {
            if (_transitionEnterMaterial == null)
            {
                Debug.LogWarning("TransitionsFX not initialized yet. You're probably calling TransitionEnter/Exit from an Awake method. Please call it from a Start method instead.");
                return;
            }

            if (_transitionCoroutine != null)
                StopCoroutine(_transitionCoroutine);

            _transitionCoroutine = StartCoroutine(TransitionRoutine(material, duration, delay, startValue, endValue, easeType));
        }

        IEnumerator TransitionRoutine(Material material, float duration, float delay, float startValue, float endValue, EaseType easeType)
        {
            _blit.enabled = true;

            _step = startValue;
            _blit.CurrentMaterial = material;
            _blit.CurrentMaterial.SetFloat(_material_StepID, _step);            

            if (endValue == 0)
            {
                if (FadeEnter)
                    _blit.CurrentMaterial.SetFloat(_material_FadeID, _step);
                else
                    _blit.CurrentMaterial.SetFloat(_material_FadeID, 1);

                if (IgnoreTransitionTexEnter)
                    _blit.CurrentMaterial.SetFloat(_material_IgnoreTransitionTexID, 1);
                else
                    _blit.CurrentMaterial.SetFloat(_material_IgnoreTransitionTexID, 0);

                if (OnTransitionEnterStarted != null)
                    OnTransitionEnterStarted();
            }
            else if (endValue == 1)
            {
                if (FadeExit)
                    _blit.CurrentMaterial.SetFloat(_material_FadeID, _step);
                else
                    _blit.CurrentMaterial.SetFloat(_material_FadeID, 1);

                if (IgnoreTransitionTexExit)
                    _blit.CurrentMaterial.SetFloat(_material_IgnoreTransitionTexID, 1);
                else
                    _blit.CurrentMaterial.SetFloat(_material_IgnoreTransitionTexID, 0);

                if (OnTransitionExitStarted != null)
                    OnTransitionExitStarted();
            }

            if (OnTransitionStarted != null)
                OnTransitionStarted();

            if (delay > 0)
                yield return new WaitForSeconds(delay);

            var t = 0f;
            while (t <= 1.0f)
            {
                t += Time.deltaTime / duration;

                _step = t.EaseFromTo(startValue, endValue, easeType);

                material.SetFloat(_material_StepID, _step);

                if (endValue == 0)
                {
                    if (FadeEnter)
                        material.SetFloat(_material_FadeID, _step);
                    else
                        material.SetFloat(_material_FadeID, 1);
                }
                else if (endValue == 1)
                {
                    if (FadeExit)
                        material.SetFloat(_material_FadeID, _step);
                    else
                        material.SetFloat(_material_FadeID, 1);
                }

                yield return null;
            }

            _step = endValue;
            material.SetFloat(_material_StepID, _step);

            if (endValue == 0)
            {
                if (FadeEnter)
                    material.SetFloat(_material_FadeID, _step);
                else
                    material.SetFloat(_material_FadeID, 1);

                if (OnTransitionEnterEnded != null)
                    OnTransitionEnterEnded();
            }
            else if (endValue == 1)
            {
                if (FadeExit)
                    material.SetFloat(_material_FadeID, _step);
                else
                    material.SetFloat(_material_FadeID, 1);

                if (OnTransitionExitEnded != null)
                    OnTransitionExitEnded();
            }

            if (OnTransitionEnded != null)
                OnTransitionEnded();

            if(endValue == 0)
                _blit.enabled = false;
        }
    }
}