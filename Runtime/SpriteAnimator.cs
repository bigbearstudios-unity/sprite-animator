using System;
using UnityEngine;

namespace BBUnity.Sprites {
    /// <summary>
    /// SpriteAnimator
    /// A simple sprite animation component 
    /// </summary>
    [AddComponentMenu("BBUnity/2D/SpriteAnimator")]
    public class SpriteAnimator : MonoBehaviour {

        public delegate void OnAnimationCompleteEventHandler(SpriteAnimator spriteAnimator);
        public delegate void OnAnimationChangedFrameEventHandler(SpriteAnimator spriteAnimator, int currentFrame);

        public enum OnLoopTypes {
            Loop,
            Stop,
            DisableGameObject
        }

        [Tooltip("The frames per second at which the animation will run")]
        [SerializeField]
        private int _framesPerSecond = 60;

        [Tooltip("Should the animation start when the Component / GameObject is Started / Enabled")]
        [SerializeField]
        private bool _startAutomatically = true;

        [Tooltip("Should the animation restart from frame 0 when the Component is re-enabled")]
        [SerializeField]
        private bool _restartOnEnable = true;

        [Tooltip("Upon looping what should happen")]
        [SerializeField]
        private OnLoopTypes _onLoop = OnLoopTypes.Loop;

        [SerializeField]
        private Sprite[] _frames = null;

        private SpriteRenderer _spriteRenderer;
        private bool _isPlaying = false;
        private int _currentFrame = 0;
        private float _timePerFrame, _lastFrameChange = 0.0f;

        public event OnAnimationCompleteEventHandler OnAnimationCompleteEvent;
        public event OnAnimationChangedFrameEventHandler OnAnimationChangedFrameEvent;

        public bool IsPlaying { get { return _isPlaying; } set { _isPlaying = value; } }
        public bool ShouldLoop { get { return _onLoop == OnLoopTypes.Loop; } }
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
            _spriteRenderer = GetComponent<SpriteRenderer>();
            if(_spriteRenderer == null) {
                Debug.LogError("A SpriteRenderer is required to use SpriteAnimator");
            }

            if(_frames == null) {
                _frames = new Sprite[0];
            }

            AssignCallbackInterfaceEvents();
            CalculateTimePerFrame();
            ResetAnimation();
        }

        protected void Start() {
            _isPlaying = _startAutomatically;
        }

        protected void OnEnable() {
            if(_restartOnEnable) {
                ResetAnimation();
            }

            StartAnimation(_startAutomatically);
        }

        protected void Update() {
            if(IsPlaying) {
                _lastFrameChange += Time.deltaTime;
                if(_lastFrameChange >= _timePerFrame) {
                    _lastFrameChange = _lastFrameChange - _timePerFrame;
                    IncrementCurrentFrame();
                }
            }
        }

        /*
         * Private Methods
         */

        /// <summary>
        /// Iterates through all of the attached components to find ISpriteAnimator,
        /// foreach one found assigns an onCreateEvent, onSpawnEvent
        /// </summary>
        private void AssignCallbackInterfaceEvents() {
            ISpriteAnimator[] callbacks = GetComponents<ISpriteAnimator>();
            foreach(ISpriteAnimator behaviour in callbacks) {
                AddCallbackListener(behaviour);
            }
            
        }

        /// <summary>
        /// Changes the current frame to the frame number provided
        /// </summary>
        private void ChangeFrame(int frame) {
            _currentFrame = frame;
            _spriteRenderer.sprite = _frames[_currentFrame];
            OnAnimationChangedFrameEvent?.Invoke(this, _currentFrame);
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
            OnAnimationCompleteEvent?.Invoke(this);

            if(_onLoop == OnLoopTypes.Loop) {
                ChangeFrame(0);
            } else if(_onLoop == OnLoopTypes.Stop) {
                StopAnimation();
            } else if(_onLoop == OnLoopTypes.DisableGameObject) {
                StopAnimation();
                gameObject.SetActive(false);
            }
        }

        /*
         * Public Methods
         */

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
        public void StartAnimation(bool play = true) {
            _isPlaying = true;
        }

        /// <summary>
        /// Stops the animation if its in a playable state
        /// </summary>
        public void StopAnimation() {
            _isPlaying = false;
        }

        public void SetOnLoop(OnLoopTypes onLoop) {
            _onLoop = onLoop;
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

        public void AddCallbackListener(ISpriteAnimator listener) {
            OnAnimationCompleteEvent += listener.OnAnimationComplete;
            OnAnimationChangedFrameEvent += listener.OnAnimationChangedFrame;
        }

        /*
         * Editor
         */

        private void OnValidate() {
            CalculateTimePerFrame();
        }
    }
}