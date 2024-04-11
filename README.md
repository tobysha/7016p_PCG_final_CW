# 7016p_PCG_final_CW

# How to run
After press the start button, click the button at bottom left "Generate", it will generate map and agents, Which include a white background, brown walls, green Thiefs, pink Diamonds and gray Trolls.
It can be pressed many times, which will delete the Objects and map last time and generate a new combine.

# Map generate
Map generation is mainly divided into the following steps:
1.(document: LevelGrid.cs)
Using cellular automata to generate a 120*65 size grid.
GenerateInitialGrid() randomly set 1 or 0 in the grid.

SmoothInitialGrid() performs a smoothing operation by cycling the number of current values through a set value, specifically within 8 cells around the grid, if the number of walls is greater than 4, set as the wall, if less than 4, set as the ground.

ProcessMap() detect the space size of the internal wall and the space size of the passage, and remove the area where the space is too small. You can change the threshold of the space to be subtracted by changing the wallThresholdSize and roomThresholdSize.

PostProcessGrid() Expanded the grid and increased the thickness of the edge walls.

 meshGen.GenerateMesh(finalLevel, 1): The mesh is generated according to the grid generated above, and 16 different shapes are generated according to the square composed of four adjacent points. The wall outline is determined in the Meshgenerator, and the final figure of the map is finally outlined.

 ag.AgentsGenerate(): From AgentsLocationGenerate.cs; Divide game map into 4 part:topLeft, topRight, bottomLeft, bottomRight, define the number of Objects in these for place(Trolls, Thiefs and Diamonds), four diamonds, one Thief and one troll for each place.

 # Thief(square)
speed: 12f
(speed during Collecting and Running will be faster)

 State:(Color)
 1.Idle: Search the entire map for diamonds and trolls and avoid walls.(Green)
 2.Collecting: Diamonds detected and collected. (yellow)
 3.Running: Trolls detected and run.(red)
 4.TurnAround: In case the agents won't stuck in the same place all the time.

 Score Generate:(Which copy a few part of tankBT)
 Base on utilityScores array, thiefs will always select the highest score.
 1.Idle:always at score of 10 
 2.Collecting: Distance of the closest diamond, maximum of score is 30 (Score = 30f - distance)
 3.Running: Distance of the closest troll, maximum of score is 50 (Score = 50f - distance)
 4.TurnAround: When detect the wall or thief within a radius of 3f, Score = 100f, else Score = 0f

 # Troll
speed: 8f
(Speed during Chasing will be faster)

 State(Color):
 1.Idle: Search the entire map for thiefs and avoid walls.(gray)
 2.Chasing：Thiefs detected and chase the thiefs.(blue)
 3.TurnAround: In case the agents won't stuck in the same place all the time.

 Score Generate:(Which copy a few part of tankBT)
 Base on utilityScores array, thiefs will always select the highest score.
 1.Idle:always at score of 10 
 2.Running: Distance of the closest thief, maximum of score is 50 (Score = 50f - distance)
 3.TurnAround: When detect the wall or thief within a radius of 3f, Score = 100f, else Score = 0f

# Diamonds
Color: Pink
Each part of map generate 4 diamons.