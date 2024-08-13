using System;
using UnityEngine;

namespace BBUnity.SpriteAnimation {

    public class SpriteAnimatorRendererNotFoundException : Exception {
        public SpriteAnimatorRendererNotFoundException() : base("A SpriteRenderer could not be found. Please ensure your component has one.") { }
    }

    public class SpriteAnimatorNoFramesException : Exception {
        public SpriteAnimatorNoFramesException() : base("The sprite animator has attempted to be started without frames. At least one frame is required.") { }
    }

    public class SpriteAnimatorImmutableWhilePlaying : Exception {
        public SpriteAnimatorImmutableWhilePlaying() : base("The Sprite Animator is immutable when playing. Please stop the animation first") { }
    }

    public class SpriteAnimatorInvalidFrame : Exception {
        public SpriteAnimatorInvalidFrame() : base("The Sprite Animator attempted to set the frame to an invalid frame") { }
    }

    /// <summary>
    /// SpriteAnimator
    /// A simple sprite animation component 
    /// </summary>
    [AddComponentMenu("BBUnity/2D/SpriteAnimator")]
    public class SpriteAnimator : BBMonoBehaviour {

        public delegate void OnAnimationCompleteEventHandler(SpriteAnimator spriteAnimator);
        public delegate void OnAnimationChangedFrameEventHandler(SpriteAnimator spriteAnimator, int currentFrame);

        [SerializeField, Tooltip("The frames per second at which the animation will run")]
        private int _framesPerSecond = 60;

        [SerializeField, Tooltip("Sets if the animation should start when Start is called")]
        private bool _playOnStart = true;

        [SerializeField, Tooltip("Sets if the animation should start when OnEnable is called")]
        private bool _playOnEnable = true;

        [SerializeField, Tooltip("Sets if the animation should restart from frame 0 when OnEnable is called")]
        private bool _restartOnEnable = true;

        [SerializeField, Tooltip("Sets the action which should take place upon the animation completing")]
        private OnLoopTypes _onLoop = OnLoopTypes.Loop;

        [SerializeField, Tooltip("The frames which will make up the animation")]
        private Sprite[] _frames = null;

        private SpriteRenderer _spriteRenderer;
        private bool _isPlaying = false;
        private int _currentFrame = 0;
        private float _timePerFrame, _lastFrameChange = 0.0f;

        public event OnAnimationCompleteEventHandler OnAnimationCompleteEvent;
        public event OnAnimationChangedFrameEventHandler OnAnimationChangedFrameEvent;

        public bool IsPlaying { get { return _isPlaying; } }
        public bool PlayOnEnable { get { return _playOnEnable; } }
        public bool RestartOnEnable { get { return _restartOnEnable; } set { _restartOnEnable = value; } }

        public int CurrentFrame { get { return _currentFrame; } }
        public bool IsComplete { get { return _currentFrame == (_frames.Length - 1); } }

        public Sprite[] Frames {
            get { return _frames; }
        }

        /*
         * Unity Overrides
         *
         * All of these are marked as protected virtual incase a subclass
         * wishes to change the inner workings of these methods
         */

        protected virtual void Awake() {
            FindSpriteRenderer();
            AssignCallbackInterfaceEvents();
            CalculateTimePerFrame();
        }

        protected virtual void Start() {
            SetPlaying(_playOnStart);
        }

        protected virtual void OnEnable() {
            if(_restartOnEnable) {
                ResetAnimation();
            }

            SetPlaying(_playOnEnable);
        }

        protected virtual void Update() {
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

        private void FindSpriteRenderer() {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            if(_spriteRenderer == null) {
                throw new SpriteAnimatorRendererNotFoundException();
            }
        }

        private void ValidateFrames() {
            if(_frames == null || _frames.Length == 0) {
                throw new SpriteAnimatorNoFramesException();
            }
        }

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
            ValidateFrames();

            _currentFrame = 0;
            _spriteRenderer.sprite = _frames[_currentFrame];
        }

        private void AnimationCompleted() {
            OnAnimationCompleteEvent?.Invoke(this);

            if(_onLoop == OnLoopTypes.Loop) {
                ChangeFrame(0);
            } else if(_onLoop == OnLoopTypes.Stop) {
                StopAnimation();
            } else if(_onLoop == OnLoopTypes.DisableMonoBehaviour) {
                Disable();
            } else if(_onLoop == OnLoopTypes.DeactivateGameObject) {
                Deactivate();
            }
        }

        /*
         * Public Methods
         * 
         */

        public void SetPlaying(bool playing) {
            if(playing) {
                ValidateFrames();
            }

            _isPlaying = playing;
        }

        /// <summary>
        /// Starts the animation if its in a stopped state
        /// </summary>
        public void Play() {
            SetPlaying(true);
        }

        /// <summary>
        /// Stops the animation if its in a playable state
        /// </summary>
        public void StopAnimation() {
            SetPlaying(false);
        }

        /// <summary>
        /// Increments the current frame. Internally this will first check if the animation is complete and
        /// /// if it is then the AnimationCompleted method will be called. Otherwise the frame will be 
        /// </summary>
        public void IncrementCurrentFrame() {
            if(IsComplete) {
                AnimationCompleted();
            } else {
                ChangeFrame(_currentFrame + 1);
            }
        }

        

        /// <summary>
        /// 
        /// </summary>
        /// <param name="onLoop"></param>
        public void SetOnLoop(OnLoopTypes onLoop) {
            _onLoop = onLoop;
        }

        /// <summary>
        /// Sets the frame of the animation
        /// </summary>
        /// <param name="frame"></param>
        public void SetCurrentFrame(int frame) {
            if(frame > _frames.Length - 1) {
                throw new SpriteAnimatorInvalidFrame();
            }

            ChangeFrame(frame);
        }

        /// <summary>
        /// Replaces the entire frames array
        /// </summary>
        /// <param name="sprites"></param>
        public void SetFrames(Sprite[] sprites, bool resetAnimation = true) {
            if(_isPlaying) {
                throw new SpriteAnimatorImmutableWhilePlaying();
            }

            _frames = (Sprite[])sprites.Clone();

            if(resetAnimation) {
                ResetAnimation();
            }
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