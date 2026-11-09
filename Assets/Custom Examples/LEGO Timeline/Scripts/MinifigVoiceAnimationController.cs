using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unity.LEGO.Minifig
{
    [System.Serializable]
    public struct VoiceAnimation
    {
        
        public AnimationClip animationClip;
        public AudioClip audioClip;
        public Texture2D[] frames; // Add an array of textures to represent animation frames
    }

    [Serializable]
    public class MinifigVoiceAnimationController : MonoBehaviour
    {
        [SerializeField]
        public bool IsEnabled;

        [SerializeField]
        [Tooltip("This is the tag name of your face object. Drag the prefab onto the scene to select the face object and assign a tag 'Face'.")]
        private string faceTag = "Face";

        [SerializeField]
        private Transform face;

        [SerializeField]
        [Tooltip("Frames per second for the animation. Can be set dynamically in PlayAnimation.")]
        private float framesPerSecond = 24.0f;

        [SerializeField]
        [Tooltip("The default texture of the minifigure")]
        private Texture2D defaultTexture;

        [SerializeField]
        private List<VoiceAnimation> voiceAnimations = new List<VoiceAnimation>();

        private Renderer faceRenderer;
        private Coroutine animationCoroutine;

        private void Awake()
        {
            
            FindFaceObject();
        }

        private void FindFaceObject()
        {
            if (IsEnabled)
            {


                // Attempt to find the GameObject by tag first
                GameObject faceObject = GameObject.FindGameObjectWithTag(faceTag);

                if (faceObject)
                {
                    face = faceObject.transform;
                }
                else if (face == null) // Backup to find by name if tag not found
                {
                    face = transform.Find("Face");
                }

                if (face)
                {
                    faceRenderer = face.GetComponent<Renderer>();
                }

                if (faceRenderer == null)
                {
                    Debug.LogWarning("No renderer found on the face GameObject.");
                }
            }
        }

        public List<VoiceAnimation> GetAvailableVoiceAnimations()
        {
            
            return voiceAnimations;
        }

        public void PlayVoiceAnimation(VoiceAnimation animation)
        {
            if (IsEnabled)
            {
                if (animationCoroutine != null)
                {
                    StopCoroutine(animationCoroutine);
                }

                animationCoroutine = StartCoroutine(PlayAnimationCoroutine(animation));
            }

        }

        public void PlayAllVoiceAnimations()
        {
            if (IsEnabled)
            {

                if (animationCoroutine != null)
                {
                    StopCoroutine(animationCoroutine);
                }

                animationCoroutine = StartCoroutine(PlayAllAnimationsCoroutine());
            }
        }

        private IEnumerator PlayAnimationCoroutine(VoiceAnimation animation)
        {
            if (IsEnabled)
            {
                if (animation.frames == null || animation.frames.Length == 0)
                {
                    Debug.LogWarning("No frames in the animation to play.");
                    yield break;
                }

                float frameDuration = 1.0f / framesPerSecond;
                int frameCount = animation.frames.Length;

                // Play each frame of the animation
                for (int i = 0; i < frameCount; i++)
                {
                    if (faceRenderer)
                    {
                        faceRenderer.material.mainTexture = animation.frames[i];
                    }
                    yield return new WaitForSeconds(frameDuration);
                }

                // Reset to default texture
                if (faceRenderer)
                {
                    faceRenderer.material.mainTexture = defaultTexture;
                }
            }
        }

        private IEnumerator PlayAllAnimationsCoroutine()
        {
            if (IsEnabled)
            {
                foreach (var animation in voiceAnimations)
                {
                    yield return PlayAnimationCoroutine(animation);
                }
            }
        }
    }
}
