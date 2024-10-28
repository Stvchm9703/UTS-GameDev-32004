**● Create a script called “PacStudentController.cs”. In this script, implement
PacStudent’s movement in the following way:
o Use a linear lerping/tweening approach to move from one grid position
to another. This grid should align with the LevelGenerator.cs levelMap.
I.e. PacStudent should be lerping from one pellet position (or empty
space) to another at a fixed speed and be frame-rate independent.
o You may NOT use external tween libraries (such as DoTween). If these
libraries are found in your project structure, you will be severely
penalized.
o In Update(), gather player input for moving with the W, A, S, and D key
to move PacStudent up, left, down, and right respectively.

▪ Store the last key that the player pressed in a member variable
called “lastInput”.
▪ Do not override/clear this variable’s value unless the player
provides more input
o In Update(), if PacStudent is not lerping (i.e. was not previously moving
or has just reached a grid position), then…
▪ Check lastInput and try to move in that direction to the
adjacent grid position.
▪ If the adjacent grid position from lastInput is walkable, then
store lastInput in a member variable called “currentInput” and
lerp to the adjacent grid position.
▪ If the adjacent grid position from lastInput is not a walkable
area (e.g. a wall), then...
● Check currentInput (see below) and try to move in that
direction to the adjacent grid position.
● If the adjacent grid position from currentInput is
walkable, then lerp to the adjacent grid position.
● If the adjacent grid position from currentInput is not a
walkable area (e.g. a wall), then do nothing (PacStudent
stops moving).
o With this setup, PacStudent’s movement should feel like the reference
example except with a slight delay in reaction time if the player
provides input while PacStudent is lerping
https://www.google.com/search?q=play+pacman+doodle
o See the “Part 4” video overview on the Assessment 4 page of
Canvas.uts.edu.au for a clear visual example of how this should
function.
▪ No, you may not do PacStudent’s movement in any other way.
▪ No, you may not use Rigidbody physics to handle movements.
▪ Yes, we know it isn’t the common way of doing things, that is
the point. It is unique and we are checking to see if you can
program unique systems without following the thousands of
unimaginative Unity tutorials out there that lead to the same
uninspired feeling in games.**