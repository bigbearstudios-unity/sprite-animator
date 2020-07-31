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

    [Serializable]
    public class SpriteAnimatorFrame {
        public SpriteAnimatorFrame() {
            sprite = null;
        }

        public SpriteAnimatorFrame(Sprite newSprite) {
            sprite = newSprite;
        }

        public Sprite sprite;
    }

    /// <summary>
    /// SpriteAnimator
    /// A simple sprite animator for use without the Unity animation system
    /// </summary>
    [AddComponentMenu("BBUnity/SpriteAnimator")]
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
        private Sprite[] _frames;

        private SpriteRenderer _spriteRenderer;
        private bool _isPlaying = false;
        private int _currentFrame = 0;
        private float _timePerFrame, _lastFrameChange = 0;
        private ISpriteAnimator _callback;

        public bool IsPlaying { get { return _isPlaying; } }

        public bool Loop { get { return _loop; } }

        public bool StartAutomatically { get { return _startAutomatically; } }
        public bool RestartOnEnable { get { return _restartOnEnable; } }

        public int CurrentFrame { get { return _currentFrame; } }
        public bool IsComplete { get { return _currentFrame == _frames.Length; } }

        private int NextFrame { get { return _currentFrame + 1; } }

        public Sprite[] Frames { get { return _frames; } }

        /*
         * Unity Overrides
         */

        private void Awake() {
            _callback = GetComponent<ISpriteAnimator>();
            _spriteRenderer = GetComponent<SpriteRenderer>();

            if(_spriteRenderer == null) {
                Debug.LogError("A SpriteRenderer is required to use SpriteAnimator");
            }

            if(_frames.Length > 0) {
                ResetAnimation();
            }

            _timePerFrame = 1.0f / _framesPerSecond;
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
                if (_lastFrameChange >= _timePerFrame) {
                    _lastFrameChange = _lastFrameChange - _timePerFrame;
                    IncrementCurrentFrame();
                }
            }
        }

        private void IncrementCurrentFrame() {
            if(IsComplete) {
                AnimationCompleted();
            } else {
                ChangeFrame(NextFrame);
            }
        }

        private void ChangeFrame(int frame) {
            _currentFrame = frame;
            if (_callback != null) {
                _callback.OnAnimationChangedFrame(this, frame);
            }

            _spriteRenderer.sprite = _frames[_currentFrame];
        }

        private void ResetAnimation() {
            _currentFrame = 0;
            _spriteRenderer.sprite = _frames[_currentFrame];
        }

        private void AnimationCompleted() {
            if (_callback != null) {
                _callback.OnAnimationComplete(this);
            }

            if (Loop) {
                ChangeFrame(0);
            } else {
                StopAnimation();
            }
        }

        public void AddFrame(Sprite sprite) {
            Array.Resize(ref _frames, _frames.Length + 1);
            _frames[_frames.Length - 1] = null;

            Debug.Log(_frames.Length);
        }

        public void AddFrameAt(Sprite sprite, int index) {
            
        }

        public void RemoveFrame(int index) {

        }

        public void ReorderFrame(int oldIndex, int newIndex) {

        }

        public void StartAnimation() {
            _isPlaying = true;
        }

        public void StopAnimation() {
            _isPlaying = false;
        }

        public void SetFrame(int frame) {
            ChangeFrame(frame);
        }
    }
}