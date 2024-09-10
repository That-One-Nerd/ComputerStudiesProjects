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
