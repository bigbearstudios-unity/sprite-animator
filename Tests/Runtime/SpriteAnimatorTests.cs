using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

using BBUnity.SpriteAnimation;
using BBUnity.TestSupport;

namespace BBUnity.SpriteAnimation {
    public class SpriteAnimatorTests {

        private class TestCallbackHandler : ISpriteAnimator {
            public int completeCalled = 0;
            public int loopCalled = 0;
            public int frameCalled = 0;
            public void OnAnimationComplete(SpriteAnimator _) { completeCalled++; }
            public void OnAnimationLoop(SpriteAnimator _) { loopCalled++; }
            public void OnAnimationChangedFrame(SpriteAnimator _, int frame) { frameCalled++; }
        }

        private void WithAnimator(Action<SpriteAnimator> func, int frameCount = 3) {
            TestUtilities.CreateThenDestroyGameObject<SpriteAnimator>(animator => {
                animator.SetFrames(Fixtures.Sprites(frameCount));
                func(animator);
            }, components: new[] { typeof(SpriteRenderer) });
        }

        // --- SetFrames ---

        [Test]
        public void SetFrames_ShouldThrow_WhenNull() {
            TestUtilities.CreateThenDestroyGameObject<SpriteAnimator>(animator => {
                Assert.Throws<ArgumentNullException>(() => animator.SetFrames(null));
            }, components: new[] { typeof(SpriteRenderer) });
        }

        [Test]
        public void SetFrames_ShouldThrow_WhenEmpty() {
            TestUtilities.CreateThenDestroyGameObject<SpriteAnimator>(animator => {
                Assert.Throws<SpriteAnimatorNoFramesException>(() => animator.SetFrames(new Sprite[0]));
            }, components: new[] { typeof(SpriteRenderer) });
        }

        [Test]
        public void SetFrames_ShouldThrow_WhenPlaying() {
            WithAnimator(animator => {
                animator.Play();
                Assert.Throws<SpriteAnimatorImmutableWhilePlaying>(() => animator.SetFrames(Fixtures.Sprites(3)));
            });
        }

        [Test]
        public void SetFrames_ShouldReplaceFrames() {
            WithAnimator(animator => {
                Sprite[] newSprites = Fixtures.Sprites(5);
                animator.SetFrames(newSprites);
                Assert.AreEqual(5, animator.Frames.Length);
            });
        }

        // --- Frames property ---

        [Test]
        public void Frames_ShouldReturnNull_WhenNoFramesSet() {
            TestUtilities.CreateThenDestroyGameObject<SpriteAnimator>(animator => {
                Assert.IsNull(animator.Frames);
            }, components: new[] { typeof(SpriteRenderer) });
        }

        [Test]
        public void Frames_ShouldReturnCopyNotReference() {
            WithAnimator(animator => {
                Sprite[] copy = animator.Frames;
                Sprite original = copy[0];
                copy[0] = null;
                Assert.AreEqual(original, animator.Frames[0]);
            });
        }

        // --- SetCurrentFrame ---

        [Test]
        public void SetCurrentFrame_ShouldThrow_WhenFramesNotSet() {
            TestUtilities.CreateThenDestroyGameObject<SpriteAnimator>(animator => {
                Assert.Throws<SpriteAnimatorNoFramesException>(() => animator.SetCurrentFrame(0));
            }, components: new[] { typeof(SpriteRenderer) });
        }

        [Test]
        public void SetCurrentFrame_ShouldThrow_WhenFrameOutOfBounds() {
            WithAnimator(animator => {
                Assert.Throws<SpriteAnimatorInvalidFrame>(() => animator.SetCurrentFrame(99));
            });
        }

        [Test]
        public void SetCurrentFrame_ShouldThrow_WhenFrameIsNegative() {
            WithAnimator(animator => {
                Assert.Throws<SpriteAnimatorInvalidFrame>(() => animator.SetCurrentFrame(-1));
            });
        }

        [Test]
        public void SetCurrentFrame_ShouldChangeCurrentFrame() {
            WithAnimator(animator => {
                animator.SetCurrentFrame(2);
                Assert.AreEqual(2, animator.CurrentFrame);
            });
        }

        // --- IncrementCurrentFrame ---

        [Test]
        public void IncrementCurrentFrame_ShouldIncrementTheFrame() {
            WithAnimator(animator => {
                animator.Play();
                animator.IncrementCurrentFrame();
                Assert.AreEqual(1, animator.CurrentFrame);
            });
        }

        [Test]
        public void IncrementCurrentFrame_WhenStopped_ShouldStillIncrement() {
            WithAnimator(animator => {
                animator.IncrementCurrentFrame();
                Assert.AreEqual(1, animator.CurrentFrame);
            });
        }

        // --- Loop behaviour ---

        [Test]
        public void IncrementCurrentFrame_WhenComplete_Loop_ShouldResetToFrame0() {
            WithAnimator(animator => {
                animator.SetOnLoop(OnLoopTypes.Loop);
                animator.Play();

                animator.IncrementCurrentFrame();  // 0 → 1
                animator.IncrementCurrentFrame();  // 1 → 2 (last)
                Assert.AreEqual(2, animator.CurrentFrame);
                Assert.IsTrue(animator.IsComplete);

                animator.IncrementCurrentFrame();  // loops → 0
                Assert.AreEqual(0, animator.CurrentFrame);
                Assert.IsTrue(animator.IsPlaying);
            });
        }

        [Test]
        public void IncrementCurrentFrame_WhenComplete_Stop_ShouldStopPlaying() {
            WithAnimator(animator => {
                animator.SetOnLoop(OnLoopTypes.Stop);
                animator.Play();

                animator.IncrementCurrentFrame();  // 0 → 1
                animator.IncrementCurrentFrame();  // 1 → 2 (last)
                animator.IncrementCurrentFrame();  // complete → stop

                Assert.AreEqual(0, animator.CurrentFrame);
                Assert.IsFalse(animator.IsPlaying);
            });
        }

        // --- Events ---

        [Test]
        public void OnAnimationLoopEvent_ShouldFire_WhenLooped() {
            WithAnimator(animator => {
                int count = 0;
                animator.OnAnimationLoopEvent += _ => count++;
                animator.SetOnLoop(OnLoopTypes.Loop);
                animator.Play();

                animator.IncrementCurrentFrame();
                animator.IncrementCurrentFrame();
                Assert.AreEqual(0, count);

                animator.IncrementCurrentFrame();  // loops
                Assert.AreEqual(1, count);
            });
        }

        [Test]
        public void OnAnimationLoopEvent_ShouldNotFire_WhenStop() {
            WithAnimator(animator => {
                int count = 0;
                animator.OnAnimationLoopEvent += _ => count++;
                animator.SetOnLoop(OnLoopTypes.Stop);
                animator.Play();

                animator.IncrementCurrentFrame();
                animator.IncrementCurrentFrame();
                animator.IncrementCurrentFrame();  // completes

                Assert.AreEqual(0, count);
            });
        }

        [Test]
        public void OnAnimationCompleteEvent_ShouldFire_WhenStop() {
            WithAnimator(animator => {
                int count = 0;
                animator.OnAnimationCompleteEvent += _ => count++;
                animator.SetOnLoop(OnLoopTypes.Stop);
                animator.Play();

                animator.IncrementCurrentFrame();
                animator.IncrementCurrentFrame();
                animator.IncrementCurrentFrame();  // completes

                Assert.AreEqual(1, count);
            });
        }

        [Test]
        public void OnAnimationCompleteEvent_ShouldNotFire_WhenLooped() {
            WithAnimator(animator => {
                int count = 0;
                animator.OnAnimationCompleteEvent += _ => count++;
                animator.SetOnLoop(OnLoopTypes.Loop);
                animator.Play();

                animator.IncrementCurrentFrame();
                animator.IncrementCurrentFrame();
                animator.IncrementCurrentFrame();  // loops, not completes

                Assert.AreEqual(0, count);
            });
        }

        [Test]
        public void OnAnimationChangedFrameEvent_ShouldFireOnFrameChange() {
            WithAnimator(animator => {
                List<int> frames = new List<int>();
                animator.OnAnimationChangedFrameEvent += (_, f) => frames.Add(f);
                animator.Play();

                animator.IncrementCurrentFrame();  // → 1
                animator.IncrementCurrentFrame();  // → 2

                Assert.AreEqual(2, frames.Count);
                Assert.AreEqual(1, frames[0]);
                Assert.AreEqual(2, frames[1]);
            });
        }

        [Test]
        public void OnAnimationChangedFrameEvent_ShouldNotFireAtLoopBoundary() {
            WithAnimator(animator => {
                List<int> frames = new List<int>();
                animator.OnAnimationChangedFrameEvent += (_, f) => frames.Add(f);
                animator.SetOnLoop(OnLoopTypes.Loop);
                animator.Play();

                animator.IncrementCurrentFrame();  // → 1 (fires)
                animator.IncrementCurrentFrame();  // → 2 (fires)
                animator.IncrementCurrentFrame();  // loops → 0 (should NOT fire)

                Assert.AreEqual(2, frames.Count);
                Assert.AreEqual(0, animator.CurrentFrame);
            });
        }

        // --- Pause / Resume ---

        [Test]
        public void PauseAnimation_ShouldStopPlayback() {
            WithAnimator(animator => {
                animator.Play();
                Assert.IsTrue(animator.IsPlaying);

                animator.PauseAnimation();
                Assert.IsFalse(animator.IsPlaying);
            });
        }

        [Test]
        public void PauseAnimation_ShouldPreserveCurrentFrame() {
            WithAnimator(animator => {
                animator.Play();
                animator.IncrementCurrentFrame();  // → 1
                animator.PauseAnimation();

                Assert.AreEqual(1, animator.CurrentFrame);
            });
        }

        [Test]
        public void ResumeAnimation_ShouldContinuePlayback() {
            WithAnimator(animator => {
                animator.Play();
                animator.PauseAnimation();
                animator.ResumeAnimation();
                Assert.IsTrue(animator.IsPlaying);
            });
        }

        [Test]
        public void ResumeAnimation_ShouldNotResetFrame() {
            WithAnimator(animator => {
                animator.Play();
                animator.IncrementCurrentFrame();  // → 1
                animator.PauseAnimation();
                animator.ResumeAnimation();
                Assert.AreEqual(1, animator.CurrentFrame);
            });
        }

        // --- Segment play ---

        [Test]
        public void Play_WithSegment_ShouldStartAtStartFrame() {
            WithAnimator(animator => {
                animator.Play(1, 2);
                Assert.AreEqual(1, animator.CurrentFrame);
            }, frameCount: 5);
        }

        [Test]
        public void Play_WithSegment_ShouldCompleteAtEndFrame() {
            WithAnimator(animator => {
                animator.SetOnLoop(OnLoopTypes.Stop);
                animator.Play(1, 2);

                animator.IncrementCurrentFrame();  // 1 → 2 (last in segment)
                Assert.IsTrue(animator.IsComplete);

                animator.IncrementCurrentFrame();  // completes → stop
                Assert.IsFalse(animator.IsPlaying);
                Assert.AreEqual(1, animator.CurrentFrame);  // loops back to segmentStart on complete
            }, frameCount: 5);
        }

        [Test]
        public void Play_WithSegment_ShouldThrow_WhenStartFrameOutOfBounds() {
            WithAnimator(animator => {
                Assert.Throws<SpriteAnimatorInvalidFrame>(() => animator.Play(10, 11));
            }, frameCount: 5);
        }

        [Test]
        public void Play_ShouldResetSegmentAndPlayFromFrame0() {
            WithAnimator(animator => {
                animator.Play(1, 2);
                animator.StopAnimation();
                animator.Play();

                // Full range restored — can advance past frame 2
                animator.IncrementCurrentFrame();  // 0 → 1
                animator.IncrementCurrentFrame();  // 1 → 2
                Assert.IsFalse(animator.IsComplete);  // full range: end is frame 4
            }, frameCount: 5);
        }

        // --- OnLoop getter ---

        [Test]
        public void OnLoop_ShouldReturnCurrentValue() {
            WithAnimator(animator => {
                animator.SetOnLoop(OnLoopTypes.Stop);
                Assert.AreEqual(OnLoopTypes.Stop, animator.OnLoop);
            });
        }

        // --- SpeedMultiplier ---

        [Test]
        public void SpeedMultiplier_ShouldThrow_WhenZero() {
            WithAnimator(animator => {
                Assert.Throws<ArgumentOutOfRangeException>(() => animator.SpeedMultiplier = 0f);
            });
        }

        [Test]
        public void SpeedMultiplier_ShouldThrow_WhenNegative() {
            WithAnimator(animator => {
                Assert.Throws<ArgumentOutOfRangeException>(() => animator.SpeedMultiplier = -1f);
            });
        }

        [Test]
        public void SpeedMultiplier_ShouldUpdate_WhenValid() {
            WithAnimator(animator => {
                animator.SpeedMultiplier = 2.0f;
                Assert.AreEqual(2.0f, animator.SpeedMultiplier);
            });
        }

        // --- Register / Unregister ---

        [Test]
        public void Register_ShouldThrow_WhenListenerIsNull() {
            WithAnimator(animator => {
                Assert.Throws<ArgumentNullException>(() => animator.Register(null));
            });
        }

        [Test]
        public void Register_ShouldReceiveCallbacks() {
            WithAnimator(animator => {
                TestCallbackHandler handler = new TestCallbackHandler();
                animator.Register(handler);
                animator.Play();

                animator.IncrementCurrentFrame();
                Assert.AreEqual(1, handler.frameCalled);
            });
        }

        [Test]
        public void Unregister_ShouldStopReceivingCallbacks() {
            WithAnimator(animator => {
                TestCallbackHandler handler = new TestCallbackHandler();
                animator.Register(handler);
                animator.Play();

                animator.IncrementCurrentFrame();
                Assert.AreEqual(1, handler.frameCalled);

                animator.Unregister(handler);
                animator.IncrementCurrentFrame();
                Assert.AreEqual(1, handler.frameCalled);  // no further calls
            });
        }
    }
}
