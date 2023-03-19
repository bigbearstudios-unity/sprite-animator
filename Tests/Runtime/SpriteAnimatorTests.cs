using System;

using NUnit.Framework;
using UnityEngine;

using BBUnity.Sprites;
using BBUnity.TestSupport;

public class SpriteAnimatorTests {
    [Test]
    public void CurrentFrame_ShouldGetTheCurrentFrame() {
        CreateThenDestorySpriteAnimator((SpriteAnimator animator) => {
            Assert.AreEqual(0, animator.CurrentFrame);
        });
    }

    [Test]
    public void IncrementCurrentFrame_ShouldIncrementTheFrame() {
        CreateThenDestorySpriteAnimator((SpriteAnimator animator) => {
            animator.IncrementCurrentFrame();
            Assert.AreEqual(1, animator.CurrentFrame);
        });
    }

    [Test]
    public void IncrementCurrentFrame_WhenStopped_ShouldIncrementTheFrame() {
        CreateThenDestorySpriteAnimator((SpriteAnimator animator) => {
            animator.StopAnimation();
            animator.IncrementCurrentFrame();
            Assert.AreEqual(1, animator.CurrentFrame);
        });
    }

    [Test]
    public void IncrementCurrentFrame_WithTrue_WhenStopped_ShouldIncrementTheFrame() {
        CreateThenDestorySpriteAnimator((SpriteAnimator animator) => {
            animator.StopAnimation();
            animator.IncrementCurrentFrame(false);
            Assert.AreEqual(0, animator.CurrentFrame);
        });
    }

    [Test]
    public void IncrementCurrentFrame_WithFalse_WhenStopped_ShouldNotIncrementTheFrame() {
        CreateThenDestorySpriteAnimator((SpriteAnimator animator) => {
            animator.StopAnimation();
            animator.IncrementCurrentFrame(false);
            Assert.AreEqual(0, animator.CurrentFrame);
        });
    }

    [Test]
    public void SpriteAnimator_ShouldResetOnceAtEndOfFrame_WhenLooped() {
        CreateThenDestorySpriteAnimator((SpriteAnimator animator) => {
            animator.SetOnLoop(SpriteAnimator.OnLoopTypes.Loop);

            Assert.AreEqual(0, animator.CurrentFrame);

            animator.IncrementCurrentFrame();
            Assert.AreEqual(1, animator.CurrentFrame);

            animator.IncrementCurrentFrame();
            Assert.AreEqual(2, animator.CurrentFrame);

            animator.IncrementCurrentFrame();
            Assert.AreEqual(0, animator.CurrentFrame);
        });
    }

    [Test]
    public void SpriteAnimator_ShouldTransitionToStoppedAtEndOfFrames_WhenNotLooped() {
        CreateThenDestorySpriteAnimator((SpriteAnimator animator) => {
            animator.SetOnLoop(SpriteAnimator.OnLoopTypes.Stop);

            Assert.AreEqual(0, animator.CurrentFrame);

            animator.IncrementCurrentFrame();
            Assert.AreEqual(1, animator.CurrentFrame);

            animator.IncrementCurrentFrame();
            Assert.AreEqual(2, animator.CurrentFrame);

            animator.IncrementCurrentFrame();
            Assert.AreEqual(2, animator.CurrentFrame);
            Assert.IsTrue(animator.IsComplete);
            Assert.IsFalse(animator.IsPlaying);
        });
    }

    public class SpriteAnimatorTestCallbackHandler : ISpriteAnimator {
        public bool onAnimationChangedFrameCalled = false;
        public bool onAnimationCompleteCalled = false;

        public void OnAnimationChangedFrame(SpriteAnimator effect, int frame) {
            onAnimationChangedFrameCalled = true;
        }

        public void OnAnimationComplete(SpriteAnimator effect) {
            onAnimationCompleteCalled = true;
        }
    }

    [Test]
    public void AddCallback_ShouldAddANewCallbackToTheAnimator() {
        CreateThenDestorySpriteAnimator((SpriteAnimator animator) => {
            SpriteAnimatorTestCallbackHandler handler = new SpriteAnimatorTestCallbackHandler();

            animator.OnAnimationChangedFrameEvent += handler.OnAnimationChangedFrame;
            animator.OnAnimationCompleteEvent += handler.OnAnimationComplete;

            animator.IncrementCurrentFrame();
            Assert.IsTrue(handler.onAnimationChangedFrameCalled);

            animator.IncrementCurrentFrame();
            animator.IncrementCurrentFrame();
            Assert.IsTrue(handler.onAnimationCompleteCalled);
        });
    }

    [Test]
    public void AddCallbackUsingInterface_ShouldAddANewCallbackToTheAnimator() {
        CreateThenDestorySpriteAnimator((SpriteAnimator animator) => {
            SpriteAnimatorTestCallbackHandler handler = new SpriteAnimatorTestCallbackHandler();

            animator.AddCallbackListener(handler);

            animator.IncrementCurrentFrame();
            Assert.IsTrue(handler.onAnimationChangedFrameCalled);

            animator.IncrementCurrentFrame();
            animator.IncrementCurrentFrame();
            Assert.IsTrue(handler.onAnimationCompleteCalled);
        });
    }

    private void CreateThenDestorySpriteAnimator(Action<SpriteAnimator> func) {
        TestUtilities.CreateThenDestroyGameObject<SpriteAnimator>("Object", new System.Type[] { typeof(SpriteRenderer) }, (SpriteAnimator animator) => {
            animator.SetFrames(Fixtures.Sprites(3));
            func(animator);
        });
    }
}
