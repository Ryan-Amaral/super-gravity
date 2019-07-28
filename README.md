# super-gravity
An experiment in Unity to create gravity effects like in Super Mario Galaxy and the real world. Performs efficiently by baking a gravity vector field in the world at the start of the level.

## [Online Demo](https://ryan-amaral.github.io/super-gravity/)
Not the best due to reducing resolution of the gravity field to work in WebGL.

## Still a Work in Progress
Once mostly complete I will likely release a tutorial on YouTube, and upload to the Unity Asset Store.

## TODO
- Make camera or adjustments to gravity vectors more smooth.
- Make gravity fields bakable in the editor and save to a file, for quicker loading and to still have higher resolution gravity fields on mobile and web.
- Use partial mass for sectors not completely in or out of a gravity source.
- Allow multiple instances of GravityField to have effects like gravity from planets orbiting each other.
- To allow moving GravityFields as described above, the internal coordinates of GravitySectors must be made relative the the field location.

## More Info To Come Later

## Visuals
![The player moving around some planets.](https://github.com/Ryan-Amaral/super-gravity/blob/master/media/clip3.gif)
![The player coming walking out of a hollow planet.](https://github.com/Ryan-Amaral/super-gravity/blob/master/media/clip4.gif)
