# Nigenteki
 50.033 Game Development Project Group 11

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
