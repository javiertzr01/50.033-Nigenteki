# Nigenteki
 50.033 Game Development Project Group 11
 
## General Information
Nigenteki is an exhilarating team-based 2D shooter game. In this king-of-the-hill-esque gameplay, players form trios and engage in strategic battles to capture zones and emerge victorious.

### Key Features
1. **Tactical Gameplay**: Nigenteki places a heavy emphasis on tactical decision-making, challenging players to make strategic choices in fast-paced combat scenarios.
2. **Customisable Loadouts**: Each player can personalise their character by selecting up to 2 "arms," each offering unique Offensive, Defensive, or Support abilities. Players can experiment with different arm combinations to discover the perfect setup that aligns with their preferred playstyle.
3. **Shared Resource Pool**: Players collect resources that contribute to a shared pool. Teams must strategically decide how to distribute resources to enhance their collective effectiveness.
4. **Team-Centric Gameplay**: Our game mode and meticulously designed maps promote teamwork and exploration. Nigenteki is crafted to encourage collaboration among players as they strive for victory in capturing zones.

## How to use RTT Map Generator

0. Ignore if everything is already set up
1. Create a Tilemap gameobject (Right-Click > 2D Object > Tilemap > Isometric) -> Name it **Floor**
2. Create a Tilemap gameobject (Right-Click > 2D Object > Tilemap > Isometric) -> Name it **Walls**
3. In the **Walls** gameobject, create a *TilemapCollider2D* Component and a *Rigidbody2D(Static)* component 
3. Create a new gameobject called **TilemapVisualizer**, drag *TilemapVisualizer* script as component
	1. Set Floor Tilemap as The **floor** Tilemap gameobject
	2. Set Walls Tilemap as the **Walls** Tilemap gameobject
	3. Set Floor Tile to be the sprite
	4. Set Walls Tile to be the sprite
4. Create a new gameobject called **RTTGenerator**, drag *RTTGenerator* script as component
	1. Set Tilemap Visualizer as **TilemapVisualizer** gameobject
5. Click on **Generate Map**

### Options

- **Num Nodes**: Determines the number of nodes to place. More nodes = bigger map
- **Max Length**: Determines the max length between each node
- **Iterations**: Determines the number of iterations the Random Walk Algorithm goes through. More iterations = more *"filled"* it will be around each node
- **Walk Length**: Determines how long the Random Walk Algorithm will walk around each node. Higher walk length = higher density + more width
