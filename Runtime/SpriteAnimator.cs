using System;
using UnityEngine;

namespace BBUnity {

    /// <summary>
    /// ISpriteAnimator
    /// The interface for SpriteAnimator
    /// </summary>
    public interface ISpriteAnimator {
        void OnAnimationComplete(SpriteAnimator effect);
        void OnAnimationChangedFrame(SpriteAnimator effect, int frame);
    }

    /// <summary>
    /// SpriteAnimator
    /// A simple sprite animator for use without the Unity animation system
    /// </summary>
    [AddComponentMenu("BBUnity/2D/SpriteAnimator")]
    public class SpriteAnimator : MonoBehaviour {

        [Tooltip("The frames per second at which the animation will run")]
        [SerializeField]
        private int _framesPerSecond = 60;

        [Tooltip("Should the animation start when the Component / GameObject is Started / Enabled")]
        [SerializeField]
        private bool _startAutomatically = true;

        [Tooltip("Should the animation loop?")]
        [SerializeField]
        protected bool _loop = false;

        [Tooltip("Should the animation restart from frame 0 when the Component is re-enabled")]
        [SerializeField]
        private bool _restartOnEnable = true;

        [SerializeField]
        [HideInInspector]
        private Sprite[] _frames = null;

        private SpriteRenderer _spriteRenderer;
        private bool _isPlaying = false;
        private int _currentFrame = 0;
        private float _timePerFrame, _lastFrameChange = 0.0f;

        BehaviourDelegate<ISpriteAnimator> _delegate;

        public bool IsPlaying { get { return _isPlaying; } set { _isPlaying = value; } }
        public bool ShouldLoop { get { return _loop; } set { _loop = value; } }
        public bool StartAutomatically { get { return _startAutomatically; } set { _startAutomatically = value; } }
        public bool RestartOnEnable { get { return _restartOnEnable; } set { _restartOnEnable = value; } }

        public int CurrentFrame { get { return _currentFrame; } }
        public bool IsComplete { get { return _currentFrame == (_frames.Length - 1); } }

        private int NextFrame { get { return _currentFrame + 1; } }
        private bool HasFrames { get { return _frames.Length > 0; } }

        public Sprite[] Frames {
            get { return _frames; }
        }

        /*
         * Unity Overrides
         */

        private void Awake() {
            _delegate =  new BehaviourDelegate<ISpriteAnimator>(this);
            _spriteRenderer = GetComponent<SpriteRenderer>();

            if(_spriteRenderer == null) {
                Debug.LogError("A SpriteRenderer is required to use SpriteAnimator");
            }

            if(_frames == null) {
                _frames = new Sprite[0];
            }

            CalculateTimePerFrame();
            ResetAnimation();
        }

        private void Start() {
            if(_startAutomatically) {
                StartAnimation();
            }
        }

        private void OnEnable() {
            if(_restartOnEnable) {
                ResetAnimation();
            }

            StartAnimation();
        }

        private void Update() {
            if(IsPlaying) {
                _lastFrameChange += Time.deltaTime;
                if(_lastFrameChange >= _timePerFrame) {
                    _lastFrameChange = _lastFrameChange - _timePerFrame;
                    IncrementCurrentFrame();
                }
            }
        }

        private void ChangeFrame(int frame) {
            _currentFrame = frame;
            _spriteRenderer.sprite = _frames[_currentFrame];
            _delegate.Process(CallbackOnAnimationChangedFrame);
        }

        private void CalculateTimePerFrame() {
            _timePerFrame = 1.0f / _framesPerSecond;
        }

        private void ResetAnimation() {
            _currentFrame = 0;

            if(_spriteRenderer != null && HasFrames) {
                _spriteRenderer.sprite = _frames[_currentFrame];
            }
        }

        private void AnimationCompleted() {
            _delegate.Process(CallbackOnAnimationComplete);

            if (ShouldLoop) {
                ChangeFrame(0);
            } else {
                StopAnimation();
            }
        }

        private void CallbackOnAnimationChangedFrame(ISpriteAnimator animator) {
            animator.OnAnimationChangedFrame(this, _currentFrame);
        }

        private void CallbackOnAnimationComplete(ISpriteAnimator animator) {
            animator.OnAnimationComplete(this);
        }

        /// <summary>
        /// Increaments the current frame, this will always force a frame
        /// skip even if the animation is paused
        /// </summary>
        public void IncrementCurrentFrame() {
            if(IsComplete) {
                AnimationCompleted();
            } else {
                ChangeFrame(NextFrame);
            }
        }

        public void IncrementCurrentFrame(bool forceIncrement) {
            if(forceIncrement) {
                IncrementCurrentFrame();
            } else {
                if(IsPlaying) {
                    IncrementCurrentFrame();
                }
            }
        }

        /// <summary>
        /// Starts the animation if its in a stopped state
        /// </summary>
        public void StartAnimation() {
            _isPlaying = true;
        }

        /// <summary>
        /// Stops the animation if its in a playable state
        /// </summary>
        public void StopAnimation() {
            _isPlaying = false;
        }

        public void SetShouldLoop(bool loop) {
            _loop = loop;
        }

        /// <summary>
        /// Sets the frame of the animation
        /// </summary>
        /// <param name="frame"></param>
        public void SetCurrentFrame(int frame) {
            ChangeFrame(frame);
        }

        /// <summary>
        /// Adds a single sprite to the end of the frames array
        /// </summary>
        /// <param name="sprite"></param>
        public void AddFrame(Sprite sprite) {
            Array.Resize(ref _frames, _frames.Length + 1);
            _frames[_frames.Length - 1] = sprite;
        }

        /// <summary>
        /// Replaces the entire frames array
        /// </summary>
        /// <param name="sprites"></param>
        public void SetFrames(Sprite[] sprites) {
            _frames = (Sprite[])sprites.Clone();
        }

        public void AddCallback(ISpriteAnimator callback) {
            _delegate.AddDelegate(callback);
        }

        /*
         * Editor
         */

        private void OnValidate() {
            CalculateTimePerFrame();
        }
    }
}