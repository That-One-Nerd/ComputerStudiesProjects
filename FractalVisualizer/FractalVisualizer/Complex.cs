namespace FractalVisualizer;

public readonly struct Complex
{
    public double MagSq => r * r + i * i;

    public readonly double r, i;

    public Complex(double r, double i)
    {
        this.r = r;
        this.i = i;
    }

    public static Complex operator +(Complex a, Complex b) => new(a.r + b.r, a.i + b.i);
    public static Complex operator -(Complex a, Complex b) => new(a.r - b.r, a.i - b.i);
    public static Complex operator *(Complex a, Complex b) => new(a.r * b.r - a.i * b.i, a.r * b.i + a.i * b.r);
}
