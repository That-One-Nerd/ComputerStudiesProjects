# Computer Studies Projects

Just a few assignments I made for my 12th grade computer studies class. You get to pick your projects, so these are mine.

I have about 1-2 weeks for each project. Check the Git commits for specific dates or whatever.

## Projects

- Minesweeper/
    - I made Minesweeper for the TI-84 calculator. I tried to be somewhat true to the original Windows one.
    - 2 is down, 4 is left, 6 is right, 8 is up for your selection. Enter is to mine and the decimal point places a flag. The arrow keys move the board.
    - No additional libraries were used, only the built in TI libraries and the [TI CE toolchain](https://github.com/CE-Programming/toolchain).
    - Doesn't run great. It uses 16-bit color mode, so the graphics are somewhat slow to render. I attempted to find the way to switch to 4-bit color mode, but I didn't find enough useful info (the best I've found so far is to dissect the GraphX assembly code). Still runs decent though, I've made as many optimizations as I easily could with the renderer, and everything else is fast.
    - **WARNING**: This one can't be built without reconfiguring the `.clangd` file to include the path to the toolchain.
- Blackjack/
    - This program simulates casino blackjack rules. It allows for customizable player strategies.
    - I made a few default strategies (draw until 17, simple card counting, simple probabilities).
    - No additional libraries were used.
    - It has two custom-rendered graphs on the console display. I haven't figured out how to use XTerm yet, so I'm generating individual characters.
- BonusTicTacToe/
    - Plays tic-tac-toe. I made the game in Windows Forms.
    - The game allows for customization for the board size (rows/columns), number of players (up to 8), amount needed in a row to win, and amount of wins needed to finish the game.
    - Nice colors and sprites for each player. Scales seamlessly with a higher DPI.
    - The only component used is the menu component. The board and sprites are rendered myself with OnPaint.
- CentralizedChatRoom/
    - A live chat room system I wrote myself. It does not use HTTP, it instead uses my own TCP packet system.
    - Data transfer is automatically encrypted behind the scenes (though the server decrypts it when it gets it).
    - Allows for as many people to connect as need be.
    - The client is somewhat janky, but the server has zero issues from my testing.
- TypingTest/
    - A small game I made in like 2 hours. Using a list of words, it picks one at random and the user has to type it out.
    - It has a one-minute timer, and highlights the letters you got right.
    - Shows results as characters per minute, words per minute, and accuracy percentage at the end.
- AirTrajectoryBuilder/
    - A program I wrote that simulates the air trajectory of a projectile. Finished a while ago.
    - Create a `.sce` file (a somewhat easy to use plain text format)
    - Nice colors for each object. Scales seamlessly with a higher DPI.
    - Sweeps possible angles and speeds to try and find the path that brings the ball closest to the end point.
- Fractal Visualizer/
    - A program that can be used to visualize fractals.
    - Allows you to zoom in and drag the screen around in real time.
    - Renders in multiple resolution scales so as to be as responsive as possible. Upscales over time.
    - Currently does the mandlebrot set. It has support for any complex iterative fractal, but you have to code it yourself.
- Ciphers/
	- Command-line tool that enciphers and deciphers text.
	- Small thing. Not super good. I used it to complete homework for a different class.
	- Uses an argument parsing library I also wrote.
- BasicProjectionRenderer/
	- A program that parses and renders an OBJ file.
	- Not super optimized, but I've made a few tweaks to speed it up.
	- All calculations are my own, from sin and cosine to matrix multiplication.
	- Math code comes from a library I wrote, Nerd_STF.
- InverseKinematics/
    - A program that calculates inverse kinematics for a series of joints.
    - Took me less than an hour to write. Faster than I expected.
    - Handles hundreds of nodes with very little optimization.
    - Might improve in the future to work around obstacles.
- GreenRemover/
	- A python script that removes green from an image while keeping white looking normal.
	- Not perfect, for example yellow is changed, but it's pretty good otherwise.
- DeepAnagramSearch/
	- A program that searches for anagrams that are more than one word long.
	- Took about an afternoon to code, and about as much time finding a word list that was sufficiently reasonable.
	- The anagram solver is recursive and pretty fun to watch, though it's not particularly effective.
	- It searches to the maximum depth immediately, rather than all one-depth, then all two-depth, etc. It could be improved.
