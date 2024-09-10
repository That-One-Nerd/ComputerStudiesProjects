import math
import sys

def print_usage():
    print("Usage: colorconv.py <from> <to> <color_from>")
    print("")
    print("        <from>: The color format to convert from.")
    print("          <to>: The color format to convert to.")
    print("  <color_from>: The color to convert the format of.")
    print("")
    print("  Color formats are formatted as their individual channels.")
    print("    Ex.")
    print("      r8g8b8")
    print("      r5g6b5")
    print("")
    print("  Color data is represented as a number. Must be hexadecimal.")
    print("  The output is a single color value in hexadecimal.")

def total_format_bytes(format: tuple[int, int, int]):
    bits = sum(format)
    return math.ceil(bits / 8)

def determine_format(format: str) -> tuple[int, int, int]:
    r_ind: int = format.index("r")
    g_ind: int = format.index("g")
    b_ind: int = format.index("b")
    
    r_sub: str = format[(r_ind + 1):g_ind]
    g_sub: str = format[(g_ind + 1):b_ind]
    b_sub: str = format[(b_ind + 1):]

    r_val = int(r_sub)
    g_val = int(g_sub)
    b_val = int(b_sub)

    return (r_val, g_val, b_val)

def convert_color(format_in: tuple[int, int, int], format_out: tuple[int, int, int], color_in):
    in_r_max = 2 ** format_in[0] - 1
    in_g_max = 2 ** format_in[1] - 1
    in_b_max = 2 ** format_in[2] - 1

    out_r_max = 2 ** format_out[0] - 1
    out_g_max = 2 ** format_out[1] - 1
    out_b_max = 2 ** format_out[2] - 1

    in_r: int
    in_g: int
    in_b: int

    if isinstance(color_in, int):
        in_b = color_in & in_b_max
        in_g = (color_in >> format_in[2]) & in_g_max
        in_r = (color_in >> (format_in[2] + format_in[1])) & in_r_max
    else:
        in_r = color_in[0]
        in_g = color_in[1]
        in_b = color_in[2]

    r_float = in_r / float(in_r_max)
    g_float = in_g / float(in_g_max)
    b_float = in_b / float(in_b_max)

    out_r = int(r_float * out_r_max)
    out_g = int(g_float * out_g_max)
    out_b = int(b_float * out_b_max)

    color_out = out_b + (out_g << format_out[2]) + (out_r << (format_out[2] + format_out[1]))
    return color_out

def main(argv: list[str]):
    argc: int = len(argv)
    if argc < 4:
        print_usage()
        return
    elif argc > 4:
        print("Too many arguments!\n")
        print_usage()
        return
    
    # The first argument is this file. Happens when calling python.exe.
    format_in_str: str = argv[1]
    format_out_str: str = argv[2]
    color_str: str = argv[3]
    
    format_in: tuple[int, int, int] = determine_format(format_in_str)
    format_out: tuple[int, int, int] = determine_format(format_out_str)
    color = int(color_str, 16)

    result: int = convert_color(format_in, format_out, color)
    formatter: str = "0x{:0" + str(total_format_bytes(format_out) * 2) + "X}"
    print(formatter.format(result))

if __name__ == "__main__": main(sys.argv)
