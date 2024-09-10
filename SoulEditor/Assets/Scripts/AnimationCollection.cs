using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class AnimationCollection
{
    // Start is called before the first frame update
    public static double EaseInSine(double x) => 1 - Math.Cos((x * Math.PI) / 2);
    public static double EaseOutSine(double x) => Math.Sin((x * Math.PI) / 2);
    public static double EaseInOutSine(double x) => -(Math.Cos(Math.PI * x) - 1) / 2;

    // Quad
    public static double EaseInQuad(double x) => x * x;
    public static double EaseOutQuad(double x) => 1 - Math.Pow((1 - x), 2);
    public static double EaseInOutQuad(double x) => x < 0.5 ? 2 * x * x : 1 - Math.Pow(-2 * x + 2, 2) / 2;

    // Cubic
    public static double EaseInCubic(double x) => x * x * x;
    public static double EaseOutCubic(double x) => 1 - Math.Pow((1 - x), 3);
    public static double EaseInOutCubic(double x) => x < 0.5 ? 4 * x * x * x : 1 - Math.Pow(-2 * x + 2, 3) / 2;

    // Quart
    public static double EaseInQuart(double x) => x * x * x * x;
    public static double EaseOutQuart(double x) => 1 - Math.Pow((1 - x), 4);
    public static double EaseInOutQuart(double x) => x < 0.5 ? 8 * x * x * x * x : 1 - Math.Pow(-2 * x + 2, 4) / 2;

    // Quint
    public static double EaseInQuint(double x) => x * x * x * x * x;
    public static double EaseOutQuint(double x) => 1 - Math.Pow((1 - x), 5);
    public static double EaseInOutQuint(double x) => x < 0.5 ? 16 * x * x * x * x * x : 1 - Math.Pow(-2 * x + 2, 5) / 2;

    // Expo
    public static double EaseInExpo(double x) => x == 0 ? 0 : Math.Pow(2, 10 * (x - 1));
    public static double EaseOutExpo(double x) => x == 1 ? 1 : 1 - Math.Pow(2, -10 * x);
    public static double EaseInOutExpo(double x) => x == 0 ? 0 : x == 1 ? 1 : x < 0.5 ? Math.Pow(2, 20 * x - 10) / 2 : (2 - Math.Pow(2, -20 * x + 10)) / 2;

    // Circ
    public static double EaseInCirc(double x) => 1 - Math.Sqrt(1 - Math.Pow(x, 2));
    public static double EaseOutCirc(double x) => Math.Sqrt(1 - Math.Pow(x - 1, 2));
    public static double EaseInOutCirc(double x) => x < 0.5 ? (1 - Math.Sqrt(1 - Math.Pow(2 * x, 2))) / 2 : (Math.Sqrt(1 - Math.Pow(-2 * x + 2, 2)) + 1) / 2;

    // Back
    public static double EaseInBack(double x)
    {
        const double c1 = 1.70158;
        const double c3 = c1 + 1;
        return c3 * Math.Pow(x, 3) - c1 * Math.Pow(x, 2);
    }
    public static double EaseOutBack(double x)
    {
        const double c1 = 1.70158;
        const double c3 = c1 + 1;
        return 1 + c3 * Math.Pow(x - 1, 3) + c1 * Math.Pow(x - 1, 2);
    }
    public static double EaseInOutBack(double x)
    {
        const double c1 = 1.70158;
        const double c2 = c1 * 1.525;
        return x < 0.5 ? (Math.Pow(2 * x, 2) * ((c2 + 1) * 2 * x - c2)) / 2 : (Math.Pow(2 * x - 2, 2) * ((c2 + 1) * (x * 2 - 2) + c2) + 2) / 2;
    }

    // Elastic
    public static double EaseInElastic(double x)
    {
        const double c4 = (2 * Math.PI) / 3;
        return x == 0 ? 0 : x == 1 ? 1 : -Math.Pow(2, 10 * (x - 1)) * Math.Sin(((x - 1) * 10 - 10.75) * c4);
    }
    public static double EaseOutElastic(double x)
    {
        const double c4 = (2 * Math.PI) / 3;
        return x == 0 ? 0 : x == 1 ? 1 : Math.Pow(2, -10 * x) * Math.Sin((x * 10 - 0.75) * c4) + 1;
    }
    public static double EaseInOutElastic(double x)
    {
        const double c5 = (2 * Math.PI) / 4.5;
        return x == 0 ? 0 : x == 1 ? 1 : x < 0.5 ? -(Math.Pow(2, 20 * x - 10) * Math.Sin((20 * x - 11.125) * c5)) / 2 : (Math.Pow(2, -20 * x + 10) * Math.Sin((20 * x - 11.125) * c5)) / 2 + 1;
    }

    // Bounce
    public static double EaseInBounce(double x) => 1 - EaseOutBounce(1 - x);
    public static double EaseOutBounce(double x)
    {
        const double n1 = 7.5625;
        const double d1 = 2.75;
        if (x < 1 / d1) return n1 * x * x;
        if (x < 2 / d1) return n1 * (x - 1.5 / d1) * (x - 1.5 / d1) + 0.75;
        if (x < 2.5 / d1) return n1 * (x - 2.25 / d1) * (x - 2.25 / d1) + 0.9375;
        return n1 * (x - 2.625 / d1) * (x - 2.625 / d1) + 0.984375;
    }
    public static double EaseInOutBounce(double x) => x < 0.5 ? (1 - EaseOutBounce(1 - 2 * x)) / 2 : (1 + EaseOutBounce(2 * x - 1)) / 2;
    
}
