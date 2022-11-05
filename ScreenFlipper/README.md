# ScreenFlipper

A simple .NET application for automatically rotating your screen before and after launching another program.

This was built because a few of my Steam games on my vertical arcade cabinet do not like to start in portrait orientation, so this will rotate X number of 90° counter-clockwise rotations, run a game, then set the rotation back when the game exits.

Edit the `ScreenFlipper.exe` file with the # of 90° counter-clockwise rotations from the current orientation and the command to run:

```xml
{
    "Settings": {
        "RotateCount": 1,
        "Command": "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Danmaku Unlimited 3\\DU3-app.exe"
    }
}
```