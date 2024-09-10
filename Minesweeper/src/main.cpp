#pragma region Boilerplate

#pragma region Disregard
// Removes some errors. Weird.
typedef int int24_t;
typedef unsigned int uint24_t;
#pragma endregion

#include <sys/lcd.h>
#include <sys/rtc.h>
#include <ti/getcsc.h>
#include <ti/getkey.h>
#include <ti/screen.h>
#include <ti/ui.h>
#include <ti/vars.h>

#include <math.h>
#include <stdint.h>
#include <stdlib.h>
#include <string.h>
#pragma endregion

#include "libext.h"
#include "sprites.h"

// Useful macros.
#define VRAM_START ((uint16_t*)0xD40000)
#define SCREEN_WIDTH 320
#define SCREEN_HEIGHT 240
#define HEADER_SIZE 30
#define VRAM_SIZE (SCREEN_WIDTH * SCREEN_HEIGHT)

#define GET_TILE_INDEX(c, r) ((r) * game.columns + (c))
#define GET_TILE_AT(c, r) (tiles[(r) * game.columns + (c)])

enum DirectionType : uint8_t
{
    DIR_UP,
    DIR_LEFT,
    DIR_RIGHT,
    DIR_DOWN
};
struct GameInfo
{
    uint8_t columns = 16; // Width
    uint8_t rows = 12;    // Height
    uint16_t mines = 48;
};
struct ScreenInfo
{
    int8_t scroll_x = 0;
    int8_t scroll_y = 0;

    int8_t selection_x = 0;
    int8_t selection_y = 0;
};
enum TileFlags : uint8_t
{
    TILE_NO_FLAGS = 0x00,
    TILE_IS_MINE  = 0x01,
    TILE_REVEALED = 0x02,
    TILE_FLAGGED  = 0x04
};
struct TileInfo
{
    TileFlags flags = TILE_NO_FLAGS;
    uint8_t mines_around = 0;
};

GameInfo game;
ScreenInfo screen;
TileInfo* tiles = nullptr;
int tile_count;
bool lost = false, won = false;
int remaining_tiles;

void initialize()
{
    game = GameInfo();
    screen = ScreenInfo();

    srand(rtc_Time());
}
void setup_tiles()
{
    if (tiles != nullptr) delete[] tiles;
    tile_count = game.columns * game.rows;
    tiles = new TileInfo[tile_count];

    if (game.mines >= tile_count) game.mines = tile_count;
    remaining_tiles = tile_count - game.mines;

    // Randomize mine positions.
    for (int i = 0; i < game.mines; i++)
    {
    _choose_pos:
        int mine_pos = rand() % tile_count;

        // Using = instead of & and | because we're setting up for the first time.
        TileInfo& tile = tiles[mine_pos];
        if (tile.flags & TILE_IS_MINE) goto _choose_pos; // Mine already there, try again.
        tile.flags = TILE_IS_MINE;
    }

    // Another loop. Calculate the amount of mines around each tile.
    for (int r = 0; r < game.columns; r++)
    {
        for (int c = 0; c < game.rows; c++)
        {
            TileInfo& tile = GET_TILE_AT(r, c);
            if (tile.flags & TILE_IS_MINE) continue; // This is a mine, we know what that means.

            int adjacent_rows[8] =
            {
                r - 1,
                r - 1,
                r,
                r + 1,
                r + 1,
                r + 1,
                r,
                r - 1
            };
            int adjacent_columns[8] =
            {                
                c,
                c + 1,
                c + 1,
                c + 1,
                c,
                c - 1,
                c - 1,
                c - 1
            };
            for (int i = 0; i < 8; i++)
            {
                int ind_row = adjacent_rows[i],
                    ind_column = adjacent_columns[i];
                if (ind_row < 0 || ind_row >= game.columns ||
                    ind_column < 0 || ind_column >= game.rows) continue; // Tile is hugging the side, ignore this one.
                TileInfo& adjTile = GET_TILE_AT(ind_row, ind_column);
                if (adjTile.flags & TILE_IS_MINE) tile.mines_around++; // Mine around here.
            }
        }
    }
}

// Colors and Formatting
constexpr uint16_t background_color = 0xB5D6; // #BDBDBD

constexpr uint16_t tile_shadow_color = 0x7BEF;
constexpr uint16_t tile_highlight_color = 0xFFFF;
constexpr uint8_t tile_size = 20;
constexpr uint8_t tile_3d_effect_size = 3;

constexpr uint8_t arrow_size = 14;
constexpr uint8_t arrow_distance = 10;
constexpr uint16_t arrow_color = 0x000;

constexpr uint16_t selection_color = 0xF800;
constexpr uint16_t win_selection_color = 0x07E0;
constexpr uint16_t selection_width = 3;

void draw_sprite(int row, int column, uint16_t* sprite, int sprite_width, int sprite_height)
{
    uint16_t pos_x = row + screen.scroll_x,
             pos_y = column + screen.scroll_y;

    int pix_x = pos_x * tile_size,
        pix_y = pos_y * tile_size;

    if (pix_x < 0 || pix_x >= SCREEN_WIDTH ||
        pix_y < 0 || pix_y >= SCREEN_HEIGHT) return; // Sprite is off-screen.

    // Find middle of tile and offset by sprite size.
    pix_x += (tile_size - sprite_width) / 2;
    pix_y += (tile_size - sprite_height) / 2;

    uint16_t* vram = VRAM_START + pix_y * SCREEN_WIDTH + pix_x;
    for (int y = 0; y < sprite_height; y++)
    {
        for (int x = 0; x < sprite_width; x++)
        {
            memcpy(vram, sprite, sprite_width * sizeof(uint16_t));
        }
        vram += SCREEN_WIDTH;
        sprite += sprite_width;
    }
}

void render_background()
{
    // Can't use memset!
    // The upper byte gets replaced by the lower one.

    // Draw sequential lines to make things faster.
    uint16_t* vram = VRAM_START;
    for (int x = 0; x < SCREEN_WIDTH; x++) vram[x] = background_color;

    for (int y = 1; y < SCREEN_HEIGHT; y++)
    {
        memcpy(vram + SCREEN_WIDTH, vram, SCREEN_WIDTH * sizeof(uint16_t));
        vram += SCREEN_WIDTH;
    }
}
void render_tile_3d_effect(int row, int column)
{
    uint16_t pos_x = row + screen.scroll_x,
             pos_y = column + screen.scroll_y;

    int pix_x = pos_x * tile_size,
        pix_y = pos_y * tile_size;

    // First render the highlight with two rectangles.
    uint16_t* vram = VRAM_START + pix_y * SCREEN_WIDTH + pix_x + tile_3d_effect_size;
    for (int x = 0; x < tile_size - tile_3d_effect_size; x++) vram[x] = tile_highlight_color;

    // Copy line for horizontal part.
    for (int y = 0; y < tile_3d_effect_size - 1; y++)
    {
        memcpy(vram + SCREEN_WIDTH, vram, (tile_size - tile_3d_effect_size) * sizeof(uint16_t));
        vram += SCREEN_WIDTH;
    }

    // Copy line for vertical part.
    uint16_t* vram_side = vram + (tile_size - tile_3d_effect_size * 2);
    for (int y = 0; y < tile_size - tile_3d_effect_size * 2; y++)
    {
        memcpy(vram_side + SCREEN_WIDTH, vram_side, tile_3d_effect_size * sizeof(uint16_t));
        vram_side += SCREEN_WIDTH;
    }

    // Now render the shadow with two rectangles.
    vram = VRAM_START + (pix_y + tile_size - tile_3d_effect_size) * SCREEN_WIDTH + pix_x + tile_3d_effect_size - 3;
    for (int x = 0; x < tile_size - 3; x++) vram[x] = tile_shadow_color;

    // Copy line for horizontal part.
    for (int y = 0; y < tile_3d_effect_size - 1; y++)
    {
        memcpy(vram + SCREEN_WIDTH, vram, tile_size * sizeof(uint16_t));
        vram += SCREEN_WIDTH;
    }

    // Copy line for vertical part.
    vram_side = vram - (tile_3d_effect_size - 1) * SCREEN_WIDTH;
    for (int y = 0; y < tile_size - tile_3d_effect_size * 2; y++)
    {
        memcpy(vram_side - SCREEN_WIDTH, vram_side, tile_3d_effect_size * sizeof(uint16_t));
        vram_side -= SCREEN_WIDTH;
    }
}
void render_tile_selection(int row, int column, uint16_t color)
{
    uint16_t pos_x = row + screen.scroll_x,
             pos_y = column + screen.scroll_y;

    int pix_x = pos_x * tile_size,
        pix_y = pos_y * tile_size;

    if (pix_x < 0 || pix_x >= SCREEN_WIDTH ||
        pix_y < 0 || pix_y >= SCREEN_HEIGHT) return; // Selection is off-screen.

    // Render selection with three rectangles.
    uint16_t* vram = VRAM_START + pix_y * SCREEN_WIDTH + pix_x;
    for (int x = 0; x < tile_size; x++) vram[x] = color;

    // Copy line for top part.
    for (int y = 1; y < selection_width; y++)
    {
        memcpy(vram + SCREEN_WIDTH, vram, tile_size * sizeof(uint16_t));
        vram += SCREEN_WIDTH;
    }
    
    // Copy line for left part.
    uint16_t* vram_side = vram + SCREEN_WIDTH;
    for (int y = 0; y < tile_size - selection_width * 2; y++)
    {
        memcpy(vram_side, vram, selection_width * sizeof(uint16_t));
        vram_side += SCREEN_WIDTH;
    }

    // Copy line for right part.
    vram_side = vram + SCREEN_WIDTH + (tile_size - selection_width);
    for (int y = 0; y < tile_size - selection_width * 2; y++)
    {
        memcpy(vram_side, vram, selection_width * sizeof(uint16_t));
        vram_side += SCREEN_WIDTH;
    }

    // Copy line for bottom part.
    uint16_t* new_vram = vram + SCREEN_WIDTH * (tile_size - selection_width * 2 + 1);
    for (int y = 0; y < selection_width; y++)
    {
        memcpy(new_vram, vram, tile_size * sizeof(uint16_t));
        new_vram += SCREEN_WIDTH;
    }
}
void render_tile(int row, int column)
{
    constexpr int tiles_visible_x = SCREEN_WIDTH / tile_size,
                  tiles_visible_y = SCREEN_HEIGHT / tile_size;

    const TileInfo& tile = GET_TILE_AT(row, column);
    
    int16_t pos_x = row + screen.scroll_x,
            pos_y = column + screen.scroll_y;

    if (pos_x < 0 || pos_x >= tiles_visible_x ||
        pos_y < 0 || pos_y >= tiles_visible_y) return; // Off screen.

    // No need to render the tile background because it's
    // just the regular background.
    if (tile.flags & TILE_REVEALED)
    {
        if (tile.flags & TILE_IS_MINE)
        {
            // Uh oh.
            draw_sprite(row, column, (uint16_t*)sprite_mine, 14, 14);
        }
        else if (tile.mines_around > 0)
        {
            // Draw number icon.
            uint16_t* sprite;
            switch (tile.mines_around)
            {
                case 1:
                    sprite = (uint16_t*)sprite_one;
                    break;

                case 2:
                    sprite = (uint16_t*)sprite_two;
                    break;

                case 3:
                    sprite = (uint16_t*)sprite_three;
                    break;

                case 4:
                    sprite = (uint16_t*)sprite_four;
                    break;

                case 5:
                    sprite = (uint16_t*)sprite_five;
                    break;

                case 6:
                    sprite = (uint16_t*)sprite_six;
                    break;

                case 7:
                    sprite = (uint16_t*)sprite_seven;
                    break;

                case 8:
                    sprite = (uint16_t*)sprite_eight;
                    break;
            }
            draw_sprite(row, column, sprite, 14, 14);
        }
    }
    else
    {
        render_tile_3d_effect(row, column);
        if (tile.flags & TILE_FLAGGED)
        {
            draw_sprite(row, column, (uint16_t*)sprite_flag, 12, 12);
        }
    }

    // If we lost, show all the mines.
    if (lost && (tile.flags & TILE_IS_MINE))
    {
        draw_sprite(row, column, (uint16_t*)sprite_mine, 14, 14);
    }
}
void render_arrow(DirectionType dir)
{
    uint16_t* vram = VRAM_START;
    switch (dir)
    {
        case DIR_UP:
            vram += SCREEN_WIDTH / 2 + arrow_distance * SCREEN_WIDTH;
            for (int y = 1; y <= arrow_size; y++)
            {
                vram += y % 2 == 1 ? SCREEN_WIDTH - 1 : SCREEN_WIDTH;
                for (int x = 0; x < 2 * ceil(y / 2.0); x++) vram[x] = arrow_color;
            }
            break;

        case DIR_LEFT:
            vram += SCREEN_WIDTH * SCREEN_HEIGHT / 2 + arrow_distance;
            for (int x = 1; x <= arrow_size; x++)
            {
                vram += x % 2 == 1 ? -SCREEN_WIDTH + 1 : 1;
                for (int y = 0; y < 2 * ceil(x / 2.0); y++) vram[SCREEN_WIDTH * y] = arrow_color;
            }
            break;

        case DIR_RIGHT:
            vram += SCREEN_WIDTH * SCREEN_HEIGHT / 2 + (SCREEN_WIDTH - arrow_distance);
            for (int x = 1; x <= arrow_size; x++)
            {
                vram -= x % 2 == 1 ? SCREEN_WIDTH + 1 : 1;
                for (int y = 0; y < 2 * ceil(x / 2.0); y++) vram[SCREEN_WIDTH * y] = arrow_color;
            }
            break;

        case DIR_DOWN:
            vram += SCREEN_WIDTH / 2 + (SCREEN_HEIGHT - arrow_distance) * SCREEN_WIDTH;
            for (int y = 1; y <= arrow_size; y++)
            {
                vram -= y % 2 == 1 ? SCREEN_WIDTH + 1 : SCREEN_WIDTH;
                for (int x = 0; x < 2 * ceil(y / 2.0); x++) vram[x] = arrow_color;
            }
            break;

        default: return;
    }
}

void full_render()
{
    constexpr int tiles_visible_x = SCREEN_WIDTH / tile_size,
                  tiles_visible_y = SCREEN_HEIGHT / tile_size;

    render_background();
    for (int r = 0; r < game.columns; r++)
    {
        for (int c = 0; c < game.rows; c++)
        {
            render_tile(r, c);
        }
    }

    render_tile_selection(screen.selection_x, screen.selection_y, selection_color);

    int min_tile_visible_x = -screen.scroll_x,
        max_tile_visible_x = tiles_visible_x - screen.scroll_x;
    int min_tile_visible_y = -screen.scroll_y,
        max_tile_visible_y = tiles_visible_y - screen.scroll_y;

    if (min_tile_visible_y > 0) render_arrow(DIR_UP);
    if (min_tile_visible_x > 0) render_arrow(DIR_LEFT);
    if (max_tile_visible_x < game.columns) render_arrow(DIR_RIGHT);
    if (max_tile_visible_y < game.rows) render_arrow(DIR_DOWN);
}

bool check_completion()
{
    return remaining_tiles == 0;
}

void reveal_tile(int row, int column)
{
    // We have a vector of tile pointers. we loop until every tile in the vector
    // is revealed. if it's revealed, remove it, otherwise add its neighbors and repeat.
    // This is double its possible size because I don't want to use additional memory for
    // storing the row and column for the tile.

    TileInfo& tile = GET_TILE_AT(row, column);
    if (tile.flags & TILE_IS_MINE)
    {
        if (tile.flags & TILE_FLAGGED) return; // Clicked on a mine, but it was flagged already. Don't show.

        // Otherwise, licked on a mine, we have lost.
        lost = true;
        return;
    }
    if (tile.flags & TILE_REVEALED) return; // Already revealed.

    vector<int> tile_rows = vector<int>(),
                tile_columns = vector<int>();
    tile_rows.add(row);
    tile_columns.add(column);
    while (tile_rows.size() > 0)
    {
        int active_row = tile_rows.at(0),
            active_column = tile_columns.at(0);
        TileInfo& active = GET_TILE_AT(active_row, active_column);
        if ((active.flags & TILE_REVEALED) ||
            (active.flags & TILE_FLAGGED))
        {
            // Already revealed or flagged and can't reveal, remove.
            tile_rows.remove_at(0);
            tile_columns.remove_at(0);
            continue;
        }
        else if (active.flags & TILE_IS_MINE)
        {
            // Mine, so just reveal.
            active.flags = (TileFlags)(active.flags | TILE_REVEALED);
            tile_rows.remove_at(0);
            tile_columns.remove_at(0);
            continue;
        }

        // Get neighbors and add them here too.
        // If this current tile is a zero tile, reveal all tiles around it.
        // Otherwise, only reveal zero tiles nearby, don't reveal other
        // non-zero tiles.
        int adjacent_rows[8] =
        {
            active_row - 1,
            active_row - 1,
            active_row,
            active_row + 1,
            active_row + 1,
            active_row + 1,
            active_row,
            active_row - 1
        };
        int adjacent_columns[8] =
        {
            active_column,
            active_column + 1,
            active_column + 1,
            active_column + 1,
            active_column,
            active_column - 1,
            active_column - 1,
            active_column - 1,
        };
        for (int i = 0; i < 8; i++)
        {
            int ind_row = adjacent_rows[i],
                ind_column = adjacent_columns[i];
            if (ind_row < 0 || ind_row >= game.columns ||
                ind_column < 0 || ind_column >= game.rows) continue; // Tile is hugging the side, ignore this one.
            TileInfo& adjTile = GET_TILE_AT(ind_row, ind_column);

            if (adjTile.flags & TILE_IS_MINE) continue; // Tile is mine, ignore.
            if (adjTile.flags & TILE_REVEALED) continue; // Already revealed, ignore.
            if (active.mines_around > 0 && adjTile.mines_around > 0) continue; // Non-zero tile can't reveal other non-zero tiles, ignore.
            tile_rows.add(ind_row);
            tile_columns.add(ind_column);
        }

        active.flags = (TileFlags)(active.flags | TILE_REVEALED);
        tile_rows.remove_at(0);
        tile_columns.remove_at(0);
        remaining_tiles--;
    }
}
void flag_tile(int row, int column)
{
    TileInfo& tile = GET_TILE_AT(row, column);
    if (tile.flags & TILE_REVEALED) return; // Already revealed, ignore.
    tile.flags = (TileFlags)(tile.flags ^ TILE_FLAGGED);
}

void display_win()
{
    // Highlight mines in green.
    for (int r = 0; r < game.columns; r++)
    {
        for (int c = 0; c < game.rows; c++)
        {
            TileInfo& tile = GET_TILE_AT(r, c);
            if (!(tile.flags & TILE_IS_MINE)) continue; // Ignore non-mines.
            render_tile_selection(r, c, win_selection_color);
        }
    }
}

int main()
{
    // TODO: only full render once. check for empty space and fill that when scrolling.
    // TODO: replace the check for out-of-bounds cells with a more optimized for loop.
    // this should probably be for 2.0, not this one.

    // Initialize
    initialize();
    setup_tiles();

    while (true)
    {
        full_render();

        if (check_completion() || won)
        {
            won = true;
            display_win();
        }

    _getkey:
        uint16_t key = os_GetKey();
        
        switch (key)
        {
            case k_Up:
                if (lost || won) goto _getkey; // Lost, can't do anything.
                screen.scroll_y++;
                break;

            case k_Down:
                if (lost || won) goto _getkey; // Lost, can't do anything.
                screen.scroll_y--;
                break;

            case k_Right:
                if (lost || won) goto _getkey; // Lost, can't do anything.
                screen.scroll_x--;
                break;

            case k_Left:
                if (lost || won) goto _getkey; // Lost, can't do anything.
                screen.scroll_x++;
                break;

            case k_4:
                if (lost || won) goto _getkey; // Lost, can't do anything.
                screen.selection_x--;
                if (screen.selection_x < 0) screen.selection_x = 0;
                break;

            case k_6:
                if (lost || won) goto _getkey; // Lost, can't do anything.
                screen.selection_x++;
                if (screen.selection_x >= game.columns) screen.selection_x = game.columns - 1;
                break;

            case k_8:
                if (lost || won) goto _getkey; // Lost, can't do anything.
                screen.selection_y--;
                if (screen.selection_y < 0) screen.selection_y = 0;
                break;

            case k_2:
                if (lost || won) goto _getkey; // Lost, can't do anything.
                screen.selection_y++;
                if (screen.selection_y >= game.rows) screen.selection_y = game.rows - 1;
                break;

            case k_Enter:
                if (lost || won) goto _getkey; // Lost, can't do anything.
                reveal_tile(screen.selection_x, screen.selection_y);
                break;
            
            case k_DecPnt:
                if (lost || won) goto _getkey; // Lost, can't do anything.
                flag_tile(screen.selection_x, screen.selection_y);
                break;

            case k_Clear: return 0; // End.
            default: goto _getkey;  // Don't re-render if we press the wrong key.
        }
    }
}
