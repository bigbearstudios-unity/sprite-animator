
namespace BBUnity {
    
    /// <summary>
    /// ISpriteAnimator
    /// The interface for SpriteAnimator. This allows easier binding for animation events of
    /// OnAnimationComplete and OnAnimationChangedFrame
    /// </summary>
    public interface ISpriteAnimator {
        void OnAnimationComplete(SpriteAnimator spriteAnimator);
        void OnAnimationChangedFrame(SpriteAnimator spriteAnimator, int frame);
    }
}