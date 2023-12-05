# Nigenteki
 50.033 Game Development Project Group 11
 
## General Information
Nigenteki is an exhilarating team-based 2D shooter game. In this king-of-the-hill-esque gameplay, players form trios and engage in strategic battles to capture zones and emerge victorious.

### Key Features
1. **Tactical Gameplay**: Nigenteki places a heavy emphasis on tactical decision-making, challenging players to make strategic choices in fast-paced combat scenarios.
2. **Customisable Loadouts**: Each player can personalise their character by selecting up to 2 "arms," each offering unique Offensive, Defensive, or Support abilities. Players can experiment with different arm combinations to discover the perfect setup that aligns with their preferred playstyle.
3. **Shared Resource Pool**: Players collect resources that contribute to a shared pool. Teams must strategically decide how to distribute resources to enhance their collective effectiveness.
4. **Team-Centric Gameplay**: Our game mode and meticulously designed maps promote teamwork and exploration. Nigenteki is crafted to encourage collaboration among players as they strive for victory in capturing zones.

### Game Characters
1. Defender
2. Guardian
3. Syndicate

### Weapons
#### Silkworm
- **Design**: Bow-and-arrow
- **Basic Attack**: Fires an arrow that deals damage upon hit
- **Basic Ability**: Silk Web
- **Ultimate**: Silk Road

#### Beetle
- **Design**: Gauntlet
- **Basic Attack**: Beetle Shield/Laser
- **Basic Ability**: Shell Shock
- **Ultimate**: Jeweled Gauntlet

#### Honeybee
- **Design**: Spray Can
- **Basic Attack**: Honey Spray
- **Basic Ability**: Honeycomb
- **Ultimate**: Hivemind

#### Locust
- **Design**: Dagger
- **Basic Attack**: Locust Stab
- **Basic Ability**: Grasshopper
- **Ultimate**: Mantis

### Player Controls
- **Aim**: Mouse position determines direction of aim
- **Movement**: WASD keys to move the character
- **Normal Attack**: Left and right mouse buttons for the respective arms
- **Special Ability**: Q and E keys for the left and right arm respectively
- **Ultimate Activation**: 1 and 4 keys for the left and right arm respectively
- **Upgrade Weapon**: Press Z and C keys when in character team’s spawn to upgrade left and right arm respectively

## How to Win
### Resource Collection Phase (first 4 minutes)
- Objective: Gather resources, eliminate opponents, and scout the control point
- Teams engage in combat and resource collection

### Control Point Phase (next 6 minutes)
- Triggered after the resource collection phase
- Goal: Capture the control point within the six-minute timeframe
- Countdown: 1.5 minutes for an instant win

### Extended Match Resolution (beyond 10th minute)
- If the match surpasses the 10th minute, the team with the highest kill-death ratio wins

## How to Use RTT Map Generator

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
