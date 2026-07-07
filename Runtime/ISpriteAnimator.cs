
namespace BBUnity.SpriteAnimation {

    /// <summary>
    /// The interface for SpriteAnimator. Implement this on a component on the same
    /// GameObject to receive animation callbacks without subscribing to events directly.
    /// Components added after Awake() must use Register() / Unregister() instead.
    /// </summary>
    public interface ISpriteAnimator {
        void OnAnimationComplete(SpriteAnimator spriteAnimator);
        void OnAnimationLoop(SpriteAnimator spriteAnimator);
        void OnAnimationChangedFrame(SpriteAnimator spriteAnimator, int frame);
    }
}
