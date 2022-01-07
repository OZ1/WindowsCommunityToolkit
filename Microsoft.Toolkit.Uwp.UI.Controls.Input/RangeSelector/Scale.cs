using System;

using static System.Math;

namespace Microsoft.Toolkit.Uwp.UI.Controls;

/// <summary>
/// Нелинейное преобразование [0;1] ↔ [0;1]
/// </summary>
public interface IScale
{
    /// <summary>
    /// Прямое нелинейное преобразование
    /// </summary>
    /// <param name="x">[0;1]</param>
    void Convert(ref double x);

    /// <summary>
    /// Обратное нелинейное преобразование
    /// </summary>
    /// <param name="y">[0;1]</param>
    void Inverse(ref double y);

    /// <summary>
    /// Предустановленный тип нелинейного преобразования
    /// </summary>
    ScaleType ScaleType { get; }
}

/// <summary>
/// Предустановленный тип нелинейного преобразования
/// </summary>
public enum ScaleType
{
    /// <summary>
    /// Настроенный тип нелинейного преобразования
    /// </summary>
    Custom = -1,

    /// <summary>
    /// Без преобразования
    /// </summary>
    Linear,

    /// <summary>
    /// y = sun x
    /// </summary>
    /// <seealso cref="SineScale"/>
    Sine,

    /// <summary>
    /// Нижний правый квадрант единичной окружности
    /// </summary>
    /// <seealso cref="CircleScale"/>
    Circle,

    /// <summary>
    /// y = x²
    /// </summary>
    /// <seealso cref="SquareScale"/>
    Square,

    /// <summary>
    /// y = √x
    /// </summary>
    /// <seealso cref="RootScale"/>
    Root,

    /// <summary>
    /// y = 2 ^ e
    /// </summary>
    /// <seealso cref="PowerScale"/>
    Power,

    /// <summary>
    /// y = (−1 / x) в диапазонѣ [−n ; −1 / n]
    /// </summary>
    /// <seealso cref="HyperbolicScale"/>
    Hyperbolic,
}

/// <summary>
/// Обратное нелинейное преобразование
/// </summary>
public class InverseScale : IScale
{
    /// <summary>
    /// Обращаемое нелинейное преобразование
    /// </summary>
    public IScale Scale { get; set; }

    public ScaleType ScaleType => ScaleType.Custom;

    public void Convert(ref double x) => Scale.Inverse(ref x);

    public void Inverse(ref double y) => Scale.Convert(ref y);

    public override string ToString() => $"({Scale})ˉ¹";
}

/// <summary>
/// Синусоидальное нелинейное преобразование
/// </summary>
public class SineScale : IScale
{
    public ScaleType ScaleType => ScaleType.Sine;

    public void Convert(ref double x) => x = Sin(x * PI);

    public void Inverse(ref double y) => y = Asin(y) / PI;

    public override string ToString() => $"sin x";
}

/// <summary>
/// Нижний правый квадрант единичной окружности
/// </summary>
public class CircleScale : IScale
{
    public ScaleType ScaleType => ScaleType.Circle;

    public void Convert(ref double x) => x = Sqrt(1 - (x * x)) + 1;

    public void Inverse(ref double y) => y = Sqrt(1 - ((y - 1) * (y - 1)));

    public override string ToString() => $"sin x";
}

/// <summary>
/// Квадратичное нелинейное преобразование
/// </summary>
public class SquareScale : IScale
{
    public ScaleType ScaleType => ScaleType.Square;

    public void Convert(ref double x) => x *= x;

    public void Inverse(ref double y) => y = Sqrt(y);

    public override string ToString() => $"x²";
}

/// <summary>
/// Обратное квадратичное нелинейное преобразование
/// </summary>
public class RootScale : IScale
{
    public ScaleType ScaleType => ScaleType.Root;

    public void Convert(ref double y) => y = Sqrt(y);

    public void Inverse(ref double x) => x *= x;

    public override string ToString() => $"√x";
}

/// <summary>
/// Показательное нелинейное преобразование
/// </summary>
public class PowerScale : IScale
{
    private const double DefaultPower = E;

    /// <summary>
    /// Степень, (0;∞)
    /// </summary>
    public double Power { get; set; }

    public ScaleType ScaleType => Power == DefaultPower ? ScaleType.Power : ScaleType.Custom;

    public void Convert(ref double x) => Pow(x, Power);

    public void Inverse(ref double y) => Pow(y, 1.0 / Power);

    /// <summary>
    /// y = x ^ e
    /// </summary>
    public PowerScale()
        : this(DefaultPower)
    {
    }

    /// <summary>
    /// y = x ^ <paramref name="power"/>
    /// </summary>
    /// <exception cref="ArgumentException"><paramref name="power"/> ≤ 0</exception>
    /// <param name="power">(0;∞)</param>
    public PowerScale(double power)
    {
        if (power <= 0)
        {
            throw new ArgumentException(nameof(power) + " ≤ 0", nameof(power));
        }

        Power = power;
    }

    public override bool Equals(object obj) => obj is PowerScale other && Power == other.Power;

    public override int GetHashCode() => Power.GetHashCode();

    public override string ToString() => $"x^{Power}";
}

/// <summary>
/// Гиперболическое нелинейное преобразование
/// </summary>
public class HyperbolicScale : IScale
{
    private const double DefaultMax = 2;

    /// <summary>
    /// Насколько далеко заходит преобразование вдоль оси, (1;∞)
    /// </summary>
    public double Max { get; set; }

    public ScaleType ScaleType => Max == DefaultMax ? ScaleType.Hyperbolic : ScaleType.Custom;

    public void Convert(ref double x) => x = ((1.0 / ((x * ((1.0 - Max) / Max)) + (1.0 / Max))) - (1.0 / Max)) / ((1.0 - Max) / Max);

    public void Inverse(ref double y) => Inverse(ref y);

    /// <summary>
    /// [½;2]
    /// </summary>
    public HyperbolicScale()
        : this(DefaultMax)
    {
    }

    /// <summary>
    /// [1/<paramref name="max"/> ; <paramref name="max"/>]
    /// </summary>
    /// <param name="max">(1;∞)</param>
    /// <exception cref="ArgumentException"><paramref name="max"/> ≤ 1</exception>
    public HyperbolicScale(double max)
    {
        if (max <= 1)
        {
            throw new ArgumentException(nameof(max) + " ≤ 1", nameof(max));
        }

        Max = max;
    }

    public override bool Equals(object obj) => obj is HyperbolicScale other && Max == other.Max;

    public override int GetHashCode() => Max.GetHashCode();

    public override string ToString() => $"1/x";
}
