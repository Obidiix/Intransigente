# Intransigente (Code name: Dazed)
**WIP**
## Brief description
  - Intransigente is a point and click 3D adventure, with dystopic horror elements.
  - This game's main plot intents to show the problems of prioritizing convenience and efficiency over anything else, even the very own humanity of mankind.<br>
  - The player is the only change agent, the world moves nowhere without his active actions, but the inhabitants of the futile land they live on will not doubt on helping their cause.<br>

## Game technical architecture
The game is divided as follows:
  - Vitals (Engine functions)
    - Runtime (Played Game)
      - Editor (Specialized tools programmed for the game's systems)
#### If we disect each part:
### Vitals
  - Handles most of the heavy-duty logic (rendering, audio, input, level creation, gameplay, etc.), making the game specific implementations easier, whilst not compromising the user's freedom. 
### Runtime / Game
  - Depends on *Vitals*.
  - Contains all game dependent logic, reliying on the internal workings of the engine.
  - Many functions are still not obfuscated from the user, as actions like rendering and asset disposal are controlled individually by the user, having to manually dispose those objects and making asset management much easier internally.
### Editor
  - Depends on *Vitals* and *Runtime* / *Game*
  -  It is specifically made for the game, ensuring ease of tool development and implementation, as there is little to no abstraction from the game's components to the tools.
  -  Is in charge of serializing and storing the game's dynamic data, exposing files to the game with certain IDs, creating bitmap font data from user, creating localization data, etc.
  -  Also has (will have) a level editor for all the game's geometry and entity spawn points, featuring "portals" which serve as separation between sections, and determinates if an area is drawn or not.
