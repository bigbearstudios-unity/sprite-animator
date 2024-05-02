
namespace BBUnity.SpriteAnimation {
    
    /// <summary>
    /// The interface for SpriteAnimator. This allows easier binding for animation events of
    /// OnAnimationComplete and OnAnimationChangedFrame when you don't wish to subclass
    /// the SpriteAnimator itself.
    /// </summary>
    public interface ISpriteAnimator {
        void OnAnimationComplete(SpriteAnimator spriteAnimator);
        void OnAnimationChangedFrame(SpriteAnimator spriteAnimator, int frame);
    }
}