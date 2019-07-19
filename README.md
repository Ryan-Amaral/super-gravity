# super-gravity
An experiment in Unity to create gravity effects like in Super Mario Galaxy and the real world. Performs efficiently by baking a gravity vector field in the world at the start of the level.

## Still a Work in Progress
Once mostly complete I will likely release a tutorial on YouTube, and upload to the Unity Asset Store.

## TODO
- Gravity vectors calculated from within a certain threshold distance should use ClosestPoint to a GravitySource Mesh to avoid weird directioning (main cause of jankyness).
- Allow multiple instances of GravityField to have effects like gravity from planets orbiting each other.
- To allow moving GravityFields as described above, the internal coordinates of GravitySectors must be made relative the the field location.

## More Info To Come Later

## Visuals
![The player walking around a cube.](https://github.com/Ryan-Amaral/super-gravity/blob/master/media/clip1.gif)
![The player orbiting around a cube.](https://github.com/Ryan-Amaral/super-gravity/blob/master/media/clip2.gif)
