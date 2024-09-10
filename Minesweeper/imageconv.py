from colorconv import determine_format, convert_color, total_format_bytes
import sys
from PIL import Image

def print_usage():
    print("Usage: colorconv.py <to> <image>")
    print("")
    print("          <to>: The color format to convert to.")
    print("       <image>: The color to convert the format of.")
    print("")
    print("  Color formats are formatted as their individual channels.")
    print("    Ex.")
    print("      r8g8b8")
    print("      r5g6b5")
    print("")
    print("  Image data is represented as a path to an image file.")
    print("  The output is printed code that represents the sprite.")

def convert_image(format_out: tuple[int, int, int], image: Image.Image) -> str:
    output = "{\n"
    counter = 0
    size = image.size[0] * image.size[1]
    pixels = image.getdata()
    formatter: str = "0x{:0" + str(total_format_bytes(format_out) * 2) + "X}"
    for y in range(image.size[1]):
        output += "    "
        for x in range(image.size[0]):
            num = convert_color([8, 8, 8], format_out, pixels[y * image.size[0] + x])
            output += formatter.format(num)
            if counter < size - 1: output += ", "
            counter += 1
        output += "\n"
    output += "}"
    return output

def main(argv: list[str]):
    argc: int = len(argv)
    if argc < 3:
        print_usage()
        return
    elif argc > 3:
        print("Too many arguments!\n")
        print_usage()
        return
    
    # The first argument is this file. Happens when calling python.exe.
    format_out_str: str = argv[1]
    image_path: str = argv[2]
    
    format_out: tuple[int, int, int] = determine_format(format_out_str)
    image = Image.open(image_path).convert("RGB")
    
    result = convert_image(format_out, image)
    print(result)

if __name__ == "__main__": main(sys.argv)
