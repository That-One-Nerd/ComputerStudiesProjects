import sys

INPUT_R = 8
INPUT_G = 8
INPUT_B = 8

OUTPUT_R = 5
OUTPUT_G = 6
OUTPUT_B = 5

def convert(color_in: int) -> int:
    global INPUT_R, INPUT_G, INPUT_B
    global OUTPUT_R, OUTPUT_G, OUTPUT_B

    in_r_max = 2 ** INPUT_R - 1
    in_g_max = 2 ** INPUT_G - 1
    in_b_max = 2 ** INPUT_B - 1

    out_r_max = 2 ** OUTPUT_R - 1
    out_g_max = 2 ** OUTPUT_G - 1
    out_b_max = 2 ** OUTPUT_B - 1

    in_b = color_in & in_b_max
    in_g = (color_in >> INPUT_B) & in_g_max
    in_r = (color_in >> INPUT_B + INPUT_G) & in_r_max

    r_float = in_r / float(in_r_max)
    g_float = in_g / float(in_g_max)
    b_float = in_b / float(in_b_max)

    out_r = int(r_float * out_r_max)
    out_g = int(g_float * out_g_max)
    out_b = int(b_float * out_b_max)

    color_out = out_b + (out_g << OUTPUT_B) + (out_r << (OUTPUT_B + OUTPUT_G))
    return color_out

if len(sys.argv) < 2:
    print("Requires color argument.")
elif len(sys.argv) > 2:
    print("Too many arguments!")
else:
    input_col = int(sys.argv[1], 16)
    output_col = convert(input_col)
    print(hex(output_col))
