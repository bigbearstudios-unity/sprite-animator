# BBUnity Sprite Animator

The SpriteAnimator class allows simple animations to be played in Unity without the need for a complete Animation Controller or having to fallback to the Unity Legacy Animator.

Its worth noting that this is a 'Sprite' animator. E.g. It allows the changing of Unity Sprites, it doesn't allow the animation of any objects on the component like the Unity Animator would.

## Namespaces

``` csharp

using BBUnity.SpriteAnimation;

```

## SpriteAnimator

To get started using the SpriteAnimator you simply need to include the SpriteAnimator as a component on the object. The object will need to provide its own Sprite Renderer or else an error will be thrown to tell you to include one.

The SpriteAnimator has the following properties which can be set via the inspector:

- Frames Per Second: The frames per second at which the animation will run.
- Start Automatically: Should the animation start automatically upon Start / Enable.
- Restart on Enable: When the component is enabled, should the animation frame be reset to 0.
- On Loop: What should happen upon loop (Loop, Stop, DisableGameObject).
- Frames: The sprites which will be animated through.

### Callbacks

At some point you might want to know when a frame has changed or when the animation has ended, there is currently two ways to do this.

#### Using the ISpriteAnimator Interface

If you use the ISpriteAnimator interface on another component which is attached to the same object as the SpriteAnimator then the SpriteAnimator will automatically pick up that interface and add event listeners for the following methods:

``` csharp

  void OnAnimationComplete(SpriteAnimator spriteAnimator);
  void OnAnimationChangedFrame(SpriteAnimator spriteAnimator, int frame);

```

#### Using Manual Events

The alternative method is to directly attach to the event listeners using the SpriteAnimator:

``` csharp

//Inside of your component
private Start() {
  SpriteAnimator spriteAnimator = GetComponent<SpriteAnimator>();

  spriteAnimator.OnAnimationChangedFrameEvent += OnAnimationChangedFrame;
  spriteAnimator.OnAnimationCompleteEvent += OnAnimationComplete;
}

private  OnAnimationComplete(SpriteAnimator spriteAnimator) {

}

private OnAnimationChangedFrame(SpriteAnimator spriteAnimator, int frame) {

}

```
