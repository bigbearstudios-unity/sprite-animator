# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [0.0.1] - 2020-07-31

### Added

- Added basic SpriteAnimator

## [0.0.3] - 2020-09-20

### Added

- Added Callback Handler

## [1.0.0] - 2020-09-20

### Changed

- Upgraded to using BBUnity Core 0.3.0
- Moved to the Process CallbackHandler API

## [1.0.1] - 2020-11-03

### Changed

- Upgraded to using BBUnity Core 1.0.0
- Moved to use the BehaviourDelegate

## [1.1.0] - 2020-11-03

### Changed

- Renamed ISpriteAnimatorCallback to ISpriteAnimator

## [1.1.1] - 2020-11-08

### Changed

- Fixed issues with tests

## [1.1.2] - 2020-11-25

### Changed

- Moved component from BBUnity/SpriteAnimator to BBUnity/2D/SpriteAnimator

## [1.2.0] - 2021-01-24

### Changed

- Moved away from the older callback system, ISpriteAnimator is still supported
- Renamed OnAnimationCompleteHandler to OnAnimationCompleteEventHandler
- Renamed OnAnimationChangedFrameHandler to OnAnimationChangedFrameEventHandler
- Removed loop boolean property and swapped with onLoop which is of the type OnLoopTypes
- Renamed OnLoopTypes.Disable to OnLoopTypes.DisableGameObject

### Added

- OnLoopTypes enum with the values, Loop, Stop, Disable
- README.md

### Fixed

- Made sure _startAutomatically was honoured when onEnable was called

## [2.0.0]

### Changed

- Added the .Sprites namespace

## [3.0.0]

### Changed

- Changed the .Sprites namespace to .SpriteAnimation
- Version of BBUnityCore to 4.0.0

### Added

- More validation which now will throw exceptions

## [3.1.0]

### Changed

- Version of BBUnityTestSupport to 2.1.0
- Version of BBUnityCore to 5.1.0
- Refactored the callback handlers