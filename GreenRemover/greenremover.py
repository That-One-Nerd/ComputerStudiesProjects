#**********722871**********
# Date: 1/4/2025
# Programmer: Kyle Gilbert
# Program Name: GreenRemover
# Program Description: Removes green from an image without making the image ugly.
#**************************

import math
import sys
from PIL import Image

def print_usage():
    print("Usage: greenremover <image> <output>")
    print("")
    print("        <image>: The path of an image to remove green from.")
    print("       <output>: The output path for the new image without green.")
    print("")
    print("  The output is the same as the original image, but with no green.")

def lerp_color(colorA: tuple[int, int, int], colorB: tuple[int, int, int], time: float):
    diffR = colorB[0] - colorA[0]
    diffG = colorB[1] - colorA[1]
    diffB = colorB[2] - colorA[2]
    
    return (int(colorA[0] + diffR * time),
            int(colorA[1] + diffG * time),
            int(colorA[2] + diffB * time))

def convert_color(color: tuple[int, int, int]) -> tuple[int, int, int]:
    red = color[0]
    blue = color[2]
    
    # arithmetic mean: (a+b)/2. works well (and looks nice), but red becomes orange and blue becomes cyan.
    #mean = (red + blue) // 2
    
    # geometric mean: sqrt(ab). keeps the hues how they should be
    mean = int(math.sqrt((red / 255) * (blue / 255)) * 255)
    
    return (red, mean, blue)

def convert_image(in_image: Image.Image, out_image: Image.Image):
    PRINT_EVERY = 150_000
    total_size = in_image.size[0] * in_image.size[1]
    print(f"Converting... ({in_image.size[0]}x{in_image.size[1]}, {total_size} pix)")
    
    in_pix = in_image.getdata()
    counter = 0
    for y in range(in_image.size[1]):
        for x in range(in_image.size[0]):
            index = y * in_image.size[0] + x
            out_image.putpixel((x, y), convert_color(in_pix[index]))
            counter += 1
            if counter % PRINT_EVERY == PRINT_EVERY - 1:
                percent = round(100.0 * counter / total_size, 1)
                print(f"\r{percent}% ", end="")
                sys.stdout.flush()
    print("\r ! Done")

def main(argv: list[str]):
    argc: int = len(argv)
    if argc < 3:
        print_usage()
        return
    elif argc > 3:
        print("Too many arguments!\n")
        print_usage()
        return
    
    image_path: str = argv[1]
    output_path: str = argv[2]
    
    in_image = Image.open(image_path).convert("RGB")
    out_image = Image.new("RGB", in_image.size)

    convert_image(in_image, out_image)
    
    out_image.save(output_path)
    
if __name__ == "__main__": main(sys.argv)
