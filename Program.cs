using System;

class Program
{
    static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        int n = 29;
        double fraction = 0.1;
        int a = 45;
        int b = 30;
        float c = -12.5f;

        Console.WriteLine("Варіант 1");
        Console.WriteLine(new string('=', 60));

        Console.WriteLine("1) Переведення цілого числа N у двійкову, вісімкову та шістнадцяткову системи");
        Console.WriteLine($"N = {n}");
        Console.WriteLine($"Двійкова: {ToBase(n, 2)}");
        Console.WriteLine($"Вісімкова: {ToBase(n, 8)}");
        Console.WriteLine($"Шістнадцяткова: {ToBase(n, 16)}");
        Console.WriteLine();

        Console.WriteLine("2) Переведення дробової частини F у двійковий вигляд (6 розрядів після коми)");
        Console.WriteLine($"F = {fraction}");
        Console.WriteLine($"F в двійковому форматі: {ToBinaryFraction(fraction, 6)}");
        Console.WriteLine();

        Console.WriteLine("3) Додатковий код для A і B у 6-бітному форматі");
        string aBits = ToTwosComplementBits(a, 6);
        string bBits = ToTwosComplementBits(b, 6);
        string sumBits = AddTwosComplement(aBits, bBits);

        int aDecoded = FromTwosComplement(aBits);
        int bDecoded = FromTwosComplement(bBits);
        int sumDecoded = FromTwosComplement(sumBits);
        bool overflow = HasOverflow(aDecoded, bDecoded, sumDecoded);

        Console.WriteLine($"A = {a} -> {aBits} -> десятково: {aDecoded}");
        Console.WriteLine($"B = {b} -> {bBits} -> десятково: {bDecoded}");
        Console.WriteLine($"Сума кодів: {sumBits} -> десятково: {sumDecoded}");
        Console.WriteLine($"Переповнення: {(overflow ? "так" : "ні")}");
        Console.WriteLine();

        Console.WriteLine("4) Представлення C у форматі IEEE 754 single");
        string ieee = FloatToIEEE754(c);
        float recovered = IEEE754ToFloat(ieee);
        Console.WriteLine($"C = {c}");
        Console.WriteLine($"IEEE 754 (32 біт): {ieee}");
        Console.WriteLine($"Відновлене число: {recovered}");
        Console.WriteLine($"Порівняння: {(c == recovered ? "збігається" : "не збігається")}");
        Console.WriteLine();

        Console.WriteLine("5) Порівняння 0.1 + 0.2 з 0.3");
        double x = 0.1 + 0.2;
        Console.WriteLine($"0.1 + 0.2 = {x}");
        Console.WriteLine($"0.1 + 0.2 == 0.3 -> {(x == 0.3 ? "true" : "false")}");
        Console.WriteLine("Причина: числа з плаваючою комою в IEEE 754 не завжди представлються точно, тому 0.1 і 0.2 мають неточний двійковий запис.");
        Console.WriteLine();
    }

    static string ToBase(int value, int radix)
    {
        if (value == 0)
            return "0";

        string digits = "0123456789ABCDEF";
        string result = string.Empty;
        int temp = Math.Abs(value);

        while (temp > 0)
        {
            result = digits[temp % radix] + result;
            temp /= radix;
        }

        return value < 0 ? "-" + result : result;
    }

    static string ToBinaryFraction(double value, int precision)
    {
        if (value < 0 || value >= 1)
            throw new ArgumentOutOfRangeException(nameof(value), "Дріб має бути у межах [0, 1). ");

        string result = "0.";
        double temp = value;

        for (int i = 0; i < precision; i++)
        {
            temp *= 2;
            if (temp >= 1)
            {
                result += "1";
                temp -= 1;
            }
            else
            {
                result += "0";
            }
        }

        return result;
    }

    static string ToTwosComplementBits(int value, int bits)
    {
        int modulo = 1 << bits;
        int wrapped = ((value % modulo) + modulo) % modulo;
        return Convert.ToString(wrapped, 2).PadLeft(bits, '0');
    }

    static int FromTwosComplement(string bits)
    {
        int value = Convert.ToInt32(bits, 2);
        int signBit = 1 << (bits.Length - 1);

        if ((value & signBit) != 0)
            value -= 1 << bits.Length;

        return value;
    }

    static string AddTwosComplement(string left, string right)
    {
        int x = FromTwosComplement(left);
        int y = FromTwosComplement(right);
        int sum = x + y;
        return ToTwosComplementBits(sum, left.Length);
    }

    static bool HasOverflow(int a, int b, int sum)
    {
        return (a > 0 && b > 0 && sum < 0) || (a < 0 && b < 0 && sum >= 0);
    }

    static string FloatToIEEE754(float value)
    {
        int bits = BitConverter.SingleToInt32Bits(value);
        return Convert.ToString(bits, 2).PadLeft(32, '0');
    }

    static float IEEE754ToFloat(string bits)
    {
        uint raw = Convert.ToUInt32(bits, 2);
        int sign = (raw >> 31) == 1 ? -1 : 1;
        int exponent = (int)((raw >> 23) & 0xFF);
        uint mantissa = raw & 0x7FFFFF;

        if (exponent == 0 && mantissa == 0)
            return 0f;

        if (exponent == 255)
            return sign < 0 ? float.NegativeInfinity : float.PositiveInfinity;

        double value = sign * (1.0 + mantissa / 8388608.0) * Math.Pow(2, exponent - 127);
        return (float)value;
    }
}
