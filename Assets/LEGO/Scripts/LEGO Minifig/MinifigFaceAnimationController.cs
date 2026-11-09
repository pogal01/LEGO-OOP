// MinifigFaceAnimationController.cs
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Unity.LEGO.Minifig
{
    public class MinifigFaceAnimationController : MonoBehaviour
    {
        public bool IsEnabled;
        
        public enum FaceAnimation
        {
            Accept,
            Blink,
            BlinkTwice,
            Complain,
            Cool,
            Dissatisfied,
            Doubtful,
            Frustrated,
            Laugh,
            Mad,
            Sleepy,
            Smile,
            Surprised,
            Wink
        }

        [Serializable]
        class AnimationData
        {
            public Texture2D[] textures;
        }

        [SerializeField]
        [Tooltip("This is the tag name of your face object. Drag the prefab onto the scene to select the face object and assign a tag 'Face'.")]
        private string faceTag = "Face";

        [SerializeField]
        private Transform face;

        [SerializeField]
        [Tooltip("Frames per second for the animation. Can be set dynamically in PlayAnimation.")]
        private float framesPerSecond = 24.0f; // Default FPS

        [SerializeField]
        [Tooltip("The default texture of the minifigure")]
        private Texture2D defaultTexture;

        [SerializeField]
        private List<FaceAnimation> animations = new List<FaceAnimation>();

        [SerializeField]
        private List<AnimationData> animationData = new List<AnimationData>();

        private Material faceMaterial;
        private bool playing;
        private AnimationData currentAnimationData;
        private float currentFrame;
        private int showingFrame;

    

        private int shaderTextureId;

        void Start()
        {
            if (IsEnabled)
            {
                if (face == null)
                {
                    FindAndAssignFaceByTag();
                }

                if (face != null)
                {
                    Renderer faceRenderer = face.GetComponent<Renderer>();
                    if (faceRenderer != null)
                    {
                        faceMaterial = faceRenderer.material;
                        shaderTextureId = Shader.PropertyToID("_BaseMap");

                        // Set initial texture
                        if (defaultTexture != null)
                        {
                            faceMaterial.SetTexture(shaderTextureId, defaultTexture);
                        }
                    }
                    else
                    {
                        Debug.LogError($"No Renderer component found on face object '{face.name}'");
                    }
                }
                else
                {
                    Debug.LogError($"No face object found with tag '{faceTag}'");
                }
            }
        }

        void Update()
        {
            if (IsEnabled)
            {
                if (playing && currentAnimationData != null)
                {
                    currentFrame += Time.deltaTime * framesPerSecond;
                    var wholeFrame = Mathf.FloorToInt(currentFrame);

                    if (wholeFrame != showingFrame)
                    {
                        if (wholeFrame >= currentAnimationData.textures.Length)
                        {
                            if (defaultTexture != null)
                            {
                                faceMaterial.SetTexture(shaderTextureId, defaultTexture);
                            }
                            playing = false;
                        }
                        else if (currentAnimationData.textures[wholeFrame] != null)
                        {
                            faceMaterial.SetTexture(shaderTextureId, currentAnimationData.textures[wholeFrame]);
                            showingFrame = wholeFrame;
                        }
                    }
                }
            }
        }

        public void PlayAnimation(FaceAnimation animation, float fps = 24.0f)
        {
            if (IsEnabled)
            {
                if (!Application.isPlaying)
                {
                    Debug.LogWarning("Cannot play animations in edit mode");
                    return;
                }

                var animationIndex = animations.IndexOf(animation);
                if (animationIndex < 0)
                {
                    Debug.LogError($"Animation '{animation}' not found in the animations list");
                    return;
                }

                if (fps <= 0.0f)
                {
                    Debug.LogError("Frames per second must be positive");
                    return;
                }

                if (faceMaterial == null)
                {
                    Debug.LogError("Face material not initialized");
                    return;
                }

                playing = true;
                currentAnimationData = animationData[animationIndex];
                currentFrame = 0.0f;
                showingFrame = -1;
                framesPerSecond = fps; // Set the animation's FPS here
            }
        }

        private void FindAndAssignFaceByTag()
        {
            if (IsEnabled)
            {
                face = transform.Find(faceTag);
                if (face == null)
                {
                    Debug.LogError($"No face object found with tag '{faceTag}'");
                }
            }
        }

        // Helper methods for animation management
        public void AddAnimation(FaceAnimation animation, Texture2D[] textures)
        {
            if (IsEnabled)
            {
                if (!animations.Contains(animation))
                {
                    animations.Add(animation);
                    var newAnimationData = new AnimationData { textures = textures };
                    animationData.Add(newAnimationData);
                }
            }
        }

        public bool HasAnimation(FaceAnimation animation)
        {
            return animations.Contains(animation);
        }

        public void StopAnimation()
        {
            if (IsEnabled)
            {
                playing = false;
                if (defaultTexture != null && faceMaterial != null)
                {
                    faceMaterial.SetTexture(shaderTextureId, defaultTexture);
                }
            }
        }

        public void TriggerAnimation(FaceAnimation animation, float fps = 24.0f)
        {
            if (IsEnabled)
            {
                if (HasAnimation(animation))
                {
                    PlayAnimation(animation, fps);
                }
                else
                {
                    Debug.LogError($"Animation '{animation}' not found in the animations list");
                }
            }
        }

        public List<FaceAnimation> GetAvailableAnimations()
        {
            return animations;
        }
    }
}
