## Git Repository
● You will again be required to use your Git repository in this assessment. You can continue to use your repository from Assessment 3.
● The marks for the repository will be determined in relation to the band you reach. that is, the number of branches and commits you have should be proportional to the grade band that you reach.
● You should have a master (main) branch, a dev branch, and at least one feature branch per grade band that you complete.
  o The dev branch should stem from the master/main branch.
  o Each feature branch should stem from dev, unless you have multiple feature branches for a single grade band, in which case subsequent feature branches may stem from the first feature branch in that band.
  o You should make multiple commits per feature branch and merge the feature branches back into dev after the band is completed.
● Dev should be merged back into master/main before submitting your assessment.
  o Make sure the master/main branch is checked-out before submitting so that your final files are visible to markers.
● The repository must be connected to a remote online server (e.g. GitHub or BitBucket). You do not need to share this or provide account details, it will be evident from the Git folder whether you have a remote server reference.

Additional resources and suggestions:
● For more on this style of branching structure, see the Git Workflow branching strategy. You may have more branches if you wish.
● It is highly recommended that you work on one branch/feature/grade band at a time. E.g. checkout dev -> branch Feature-Audio -> work on the audio while committing frequently -> finish the audio and merge Feature-Audio back into dev -> branch Feature-Visual -> repeat
  o This will help you to avoid merge conflicts. Git helps you recover from poor planning (e.g. resolving merge conflicts), but it is much harder and time consuming than just planning your project flow ahead of time.
● If something goes wrong - https://dangitgit.com/ (thanks Peregrine7 and Michael on the Discord server for this resource).
  o Git is there to protect from catastrophic failure, as long as you commit and push often.
  o If you mess up a merge, reset back to a previous commit.
  o If you PC malfunctions, clone the repository from your remote (e.g. GitHub) to another computer.
  o With this in mind, no extension requests will be granted for reasons of having PC, Unity, project, or Git issues/crashes.

## Catch-up from Assessment 3
● If you were unable to finish Assessment 3, complete as much as possible in any way possible, up to and including the 75% D band (Manual Level Layout).
● The Ghosts should be placed in their center starting area, where they will start the game.
● PacStudent should be placed in the top-left hand corner, where the player will start the game
● The power pellet flashing animation should be playing.
● Remove the movement component for PacStudent from Assessment 3.
● Remove the ghost animation cycles (but don’t delete the animator from the Project View, you may need it later)
● Remove any excess sprites that aren’t part of the core level (PacStudent, Ghosts, Walls, Pellets, Power Pellets)

## Start Screen UI Appearance and Functionality
● Create a new scene called “StartScene”. This will be the first scene of the game and it will be your title screen.
● Create a title screen that has at least the following elements (though you can add more if you wish. As always, this should be in your own distinct style, not a replication of the original Pacman title screen):
  o The title “PacStudent” wording
  o A creative subtitle for your game
  o Your “PacStudent”
  o Your “Ghost” sprites
  o An animated border
    ▪ For example, moving dots around the title or anything else you can think of.
  o “Level 1” and “Level 2” buttons.
    ▪ When Level 1 is pressed, load up you Assessment 3 scene (that you will build upon in this assessment)
    ▪ When the Level 2 button is pressed, load the Design Iteration scene (see the 100% HD below). If you do not get to the 100% stage, this button does not need to do anything.
  o Previous high score and time
    ▪ These can just be 0 and 00:00:00 respectively for now. You will need to adjust them further below when doing high score saving and loading.
● This scene should be playing the “Game Intro” background music from Assessment 3, on a loop.

## In-game UI Appearance
● Continue to create your PacStudent re-creation by building upon the scene you were working on in Assessment 3.
● Add a Screen Space Canvas called “HUD” and add the following elements to the canvas. Aside from the Exit button, nothing else needs to be functional yet, just appear on screen:
  o Lives: A lives counter using the “Life Indicator” sprite from Assessment 3
    ▪ The player should start the game with 3 lives.
  o Score: A text field to shows the numeric score and that starts at 0
  o Game Timer: A text field that shows a numeric timer in the form of mm:ss:msms (e.g. 01:32:25 is 1 minute, 32 seconds, and 250 milliseconds). This should display 00:00:00 at the start of the game.
  o Ghost Scared Timer: A text field that shows seconds as an integer that will appear and count down when the ghosts are in the scared state. This should be invisible at the start of the game.
  o Exit Button: When pressed, the game should go back to the Start Scene from the previous section.
● Aspect Ratio: Both the Start Screen menu and this in-game HUD should scaleto fit both 16:9 and 4:3 aspect ratios in landscape mode.
● Add four World Space Canvases called “Ghost1Canvas” through to “Ghost4Canvas”:
  o Each ghost should have one of these world space canvas as a child object.
  o Each canvas should have a single text box that has a number from 1 to 4 in it such that your ghosts are labeled 1, 2, 3, and 4 in the game view.

## PacStudent and Cherry Movement
● Create a script called “PacStudentController.cs”. In this script, implement PacStudent’s movement in the following way:
  o Use a linear lerping/tweening approach to move from one grid position to another. This grid should align with the LevelGenerator.cs levelMap.
    I.e. PacStudent should be lerping from one pellet position (or empty   space) to another at a fixed speed and be frame-rate independent.
  o You may NOT use external tween libraries (such as DoTween). If these libraries are found in your project structure, you will be severely penalized.
  o In Update(), gather player input for moving with the W, A, S, and D key to move PacStudent up, left, down, and right respectively.
    ▪ Store the last key that the player pressed in a member variable called “lastInput”.
    ▪ Do not override/clear this variable’s value unless the player provides more input
  o In Update(), if PacStudent is not lerping (i.e. was not previously moving   or has just reached a grid position), then…
    ▪ Check lastInput and try to move in that direction to the  adjacent grid position.
    ▪ If the adjacent grid position from lastInput is walkable, then store lastInput in a member variable called “currentInput” and lerp to the adjacent grid position.
    ▪ If the adjacent grid position from lastInput is not a walkable area (e.g. a wall), then...
● Check currentInput (see below) and try to move in that direction to the adjacent grid position.
● If the adjacent grid position from currentInput is walkable, then lerp to the adjacent grid position.
● If the adjacent grid position from currentInput is not a walkable area (e.g. a wall), then do nothing (PacStudent stops moving).
  o With this setup, PacStudent’s movement should feel like the reference example except with a slight delay in reaction time if the player provides input while PacStudent is lerping https://www.google.com/search?q=play+pacman+doodle
  o See the “Part 4” video overview on the Assessment 4 page of Canvas.uts.edu.au for a clear visual example of how this should function.
    ▪ No, you may not do PacStudent’s movement in any other way.
    ▪ No, you may not use Rigidbody physics to handle movements.
    ▪ Yes, we know it isn’t the common way of doing things, that is the point. It is unique and we are checking to see if you can program unique systems without following the thousands of unimaginative Unity tutorials out there that lead to the same uninspired feeling in games.
● Make sure that PacStudent’s movement animations and audio play when they  are lerping, and stop when they are not moving
  o Remember that there are two movement audio clips – one for when PacStudent is eating a pellet (or about to eat one) and one for when they are moving but not about to eat a pellet in the next grid position.
● Create a custom dust particle effect and play it around PacStudent when they are moving so that it looks like they are running through dirt (or a similar surface)
● Create a script called “CherryController.cs” to implement the spawning and movement of the bonus cherry sprite created in the previous assessment.
  o The bonus cherry should spawn once every 10 seconds.
  o It should be instantiated at a random location just outside of the camera view on any side of the level. The starting location should be different every time.
  o It should move in a straight line, via linear lerping, from one side of the screen to the other, passing through the center point of the level and ignoring collisions with walls, ghosts, and pellets.
  o If the cherry reaches the other side of the level, outside of camera view, destroy it.
  o See below for what to do if PacStudent collides with the cherry.

## Collisions, In-game UI Functionality and Saving High Scores
● Register collisions between PacStudent and the following elements and have the following effects:
  o As per to original rules in the Assessment 3 specifications, you may only use Rigidbody components to help with registering collisions (e.g. to allow OnTriggerEnter() method calls to between more objects, refer to the Unity collision/trigger matrix on https://docs.unity3d.com/Manual/CollidersOverview.html ).
    ▪ You may NOT use the Rigidbody to handle collision responses (e.g. to put force onto Pacman).
    ▪ Therefore, any Rigidbody components in your game must be set to IsKinematic = true in the Inspector Window (or Body Type set to Kinematic if you are using a Rigidbody2D).
  o Walls:
    ▪ Prevent movement in that direction and (if need be) make sure PacStudent is moved back to the previous lerp position before the collision happened.
    ▪ Create a small particle effect and play it from the point of collision of the first frame of the collision so it looks like PacStudent has bumped into the wall.
    ▪ Play the wall collision sound effect on the first frame that PacStudent collides with the wall.
  o Teleporters:
    ▪ When PacStudent gets to the end of one of the empty tunnels on the left and right side of the map, move them to the tunnel on the other side.
    ▪ After teleporting, PacStudent’s movement should continue from that point, moving inwards towards the rest of the level.
  o Pellets: Destroy the pellet and add 10 points to the player’s score and update the UI element for this.

  o Bonus Cherry: Destroy the cherry and add 100 points to the player’s score.
  o Power pills:
    ▪ Change the Ghost animator state to “Scared”.
    ▪ Change the background music to match this state.
    ▪ Start a timer for 10 seconds. Make the Ghost Timer UI element visible and set it to this timer.
    ▪ With 3 seconds left to go on this timer, change the Ghosts to the Recovering state.
    ▪ After 10 seconds have passed, set the Ghosts back to their Walking states and hide the Ghost Timer UI element.
  o Ghosts – Walking State:
    ▪ PacStudent dies and loses a life. Update the UI element for this.
    ▪ Play a particle effect around PacStudent the spot of PacStudent’s death.
    ▪ Respawn PacStudent by moving them to the top-left hand corner, where they started the game, and wait for player input.
  o Ghosts – Scared and Recovering States:
    ▪ The ghost dies and enters their Dead animator state.
    ▪ If the ghost is moving (e.g. in the 90% section below), they can either keep moving on their ghost-specific behaviour or stop in their current position.
    ▪ Change the background music to match this state.
    ▪ Add 300 points to the player’s score.
    ▪ Start a 5 second timer (this does not need to be visible). Once this 5 seconds has passed, transition the ghost back to the Walking state.

● Round Start:
  o At the start of the round, on the HUD canvas, show a big countdown of “3”, “2”, “1”, “GO!” (each displayed 1 second apart).
  o During this time, the Game Timer (below) should remain at 0 and the player and the ghosts should not be able to move (see previous and later sections).
  o After “GO!” has been shown for 1 second:
    ▪ Hide this UI element and start the game.
    ▪ Enable player control and ghost movement (if you complete the 90% section below)
    ▪ Start the background music for when ghosts are in their Walking state, which should loop if it finishes.

● Game Timer:
  o Make the timer functional. It should start at 00:00:00 and count upwards after “GO!” disappears from the screen

● Start Scene High Score and Time:
  o In the Start Scene, when the game starts, use PlayerPrefs to load the previous high score and associated time and update the UI element for this (see Game Over below for saving the score)

● Game Over:
  o When either every pellet is eaten, or PacStudent has no lives left, show “Game Over” text on the HUD canvas.
  o Stop all player and ghost movement and pause the Game Timer
  o Use PlayerPrefs to save the player’s current score and current time if the current score is greater than the previous high score or if the score is the same but the time is lower than the previous best time.
  o The “Game Over” text should remain for 3 seconds and then return the player to the Start Scene. If the high score has changed, make sure to update it on this scene.



## Ghost Movement and Artificial Intelligence
● Place all four ghosts in the center area of the map.
● Create a new script called “GhostController.cs” to handle ghost movement.
● This should work in the exact same way as the player movement, except instead of waiting for player input, at the end of each lerp each ghost should make a decision where to move next based on the following:
  o No Unity Pathfinding: You are NOT allowed to use the Unity Pathfinding tools.
  o No stopping: In all cases below, the ghosts never stop moving.
  o No backstep: In all cases below, ghosts cannot move back to a grid position that they just came from (i.e. they cannot reverse direction) unless there is no other choice.
  o No walking through walls: The ghost should only move to “valid” positions, such as those that PacStudent can walk on (e.g. the corridors).
  o Ghost cannot teleport: In all cases below, ghosts cannot use the teleporters.
  o Dead state ghosts: Move in a straight line, at a constant, frame-rate independent speed, towards the ghost spawn area.
    ▪ Move through walls and ignore all collisions with PacStudent
  o Ghosts respawning: If a ghost is in the spawn area and they are in a…
    ▪ Dead state:
      ● Reset the ghost to Walking, Scared, or Recovering to match the other ghosts.
      ● If no other ghosts are in the Dead state, change the background music to match the new state.
    ▪ Walking/Scared/Recovering state: Move directly to leave the spawn area out (one) of the gaps in the spawn area walls.
    ▪ After exiting the spawn area, ghosts cannot re-enter unless they are in their Dead state.
  o Walking state ghosts: Match the below ghost behaviors with the number above each ghost’s head (see in-game HUD – world space section above).
    ▪ Ghost 1: Move in a random valid direction such that PacStudent will be either further than or equal distance to the ghost (straight-line distance) when compared to the ghost’s current position.
    ▪ Ghost 2: Move in a random valid direction such that PacStudent will be either closer than or equal distance to the ghost (straight-line distance) when compared to the ghost’s current position.
    ▪ Ghost 3: Move in a randomly selected valid direction
    ▪ Ghost 4: Move clockwise around the map, following the outside wall
  o Scared and Recovering state ghosts: All ghosts use Ghost 1 behavior

## Design Innovation
This is an opportunity for students to spread their wings and get creative.

Once you have completed everything above, create a second scene called “InnovationScene” and link it with the “Level 2” button on the main menu.

In this scene, design some meaningful gameplay innovation on top of the existing PacStudent experience.

This may include (but is not limited to) ONE of the following:
  ● A weapon / abilities / pickup system that enables extra controls for PacStudent
  ● A small assortment of slightly altered game modes that change the player’s objective or rules of the game (see the game “Move or Die” as an example)
  ● A single significantly different game mode that fundamentally changes the PacStudent experience.
  ● Enhanced visuals, particle effects, audio, and camera movement for better immersion.
  ● Procedural generation of the level or other content in the game to increase replayability.
  ● Enhanced ghost AI that behaves more strategically
  ● A full 3D remake of the game with either a 1st or 3rd person camera angle
  ● Etc.

If you have another idea and you are unsure whether it is too small or too large for the scope of this assessment, talk to your lecturer or tutors about it.
