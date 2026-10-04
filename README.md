<h1>Intransigente (Code name: Dazed)</h1>
<p>WIP</p>
<h2>Brief description</h2>
Intransigente is a point and click 3D adventure, with dystopic horror elements.<br>
This game's main plot intents to show the problems of prioritizing convenience and efficiency over anything else, even the very own humanity of mankind.<br>
The player is the only change agent, the world moves nowhere without his active actions, but the inhabitants of the futile land they live on will not doubt on helping their cause.<br>

<h2>Game technical architecture</h2>
The game is divided as follows:
<ul>
  <li>Vitals (Engine functions)</li>
  <ul>
    <li>Runtime (Played Game)</li>
      <ul>
      <li>Editor (Specialized tools programmed for the game's sysyems)</li>
      </ul>
    </ul>
</ul>
<p>If we disect each part:</p>
<h3>Vitals</h3>
<p>  Contains every engine functions, whether that is rendering, audio, input, gameplay, etc.</p>
<p>  Handles most of the logic of the game, allowing things like entities(dynamic, non map geometry, objects) to easily be implemented through common commands, while mantaining a certain degree of freedom to the user, then the engine becoming
a intermediate between the player + computer and game.</p>
<h3>Runtime / Game</h3>
<p>  Depends on Vitals.</p>
<p>  Contains all game dependent logic and, in simple terms, gets spoon-fed by the Vitals all the serialized data that then interprets and proceess.</p>
<p>  The vitals do all the heavy-lifting, providing the runtime with the required objects to make entities or dynamic brushes(dynamic map geomertry) act acordingly.</p>
<p>  Many functions are still not obfuscated from the user, as things like rendering are controlled individually by the user, having to dispose those objects to asure their removal from the rendering list.</p>
<h3>Editor</h3>
<p>  Depends on Vitals and Runtime / Game</p>
<p>  It is specifically made for the game, ensuring ease of tool development and implementation, as there is little to no abstraction from the game's components to the tools.</p>
<p>  Is in charge of serializing and storing the game's dynamic data, exposing files to the game with certain IDs, creating bitmap font data from user, creating localization data, etc.</p>
<p>  Also has (will have) a level editor for all the game's geometry and entity spawn points, featuring "portals" which serve as separation between sections, and determinates if an area is drawn or not.</p>
