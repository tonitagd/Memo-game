using System;
using System.Threading;



class PlayList
{
    static int C = 264;
    static int D = 297;
    static int E = 330;
    static int F = 352;
    static int G = 396;
    static int A = 440;
    //    static int Bb = 466;
    static int B = 495;
    //    static int C2 = 528;

    static int note = 800;
    static int half = note / 2;
    static int quarter = note / 4;
    static int eighth = note / 8;

    public static void PlayGameOver()
    {
        //F F F G# G G F F E F
        Console.Beep(F, note);
        Console.Beep(F, note);
        Console.Beep(F, note);

        Console.Beep(G + 20, half);
        Console.Beep(G, half);
        Console.Beep(G, half);

        Console.Beep(F, quarter + 100);
        Console.Beep(F, half);
        Console.Beep(E, half);
        Console.Beep(F, note + 200);
    }

    public static void PlayWrongCard()
    {
        Console.Beep(F, half);
        Console.Beep(E, note + 200);
    }

    public static void PlayLevelUpVar2()
    {
        //From Super Mario

        Console.Beep((int)(G / 2), quarter);
        Console.Beep(C, quarter);
        Console.Beep(E, quarter);
        Console.Beep(G, quarter);
        Console.Beep(C * 2, quarter);
        Console.Beep(E * 2, quarter);
        Console.Beep(G * 2, note);

        Console.Beep(A, quarter);
        Console.Beep(C, quarter);
        Console.Beep(D + 20, quarter);
        Console.Beep(G + 20, quarter);
        Console.Beep(C * 2, quarter);
        Console.Beep(E * 2, quarter);
        Console.Beep(G * 2 + 20, quarter);
        Console.Beep(E * 2, note);

        Console.Beep((int)(A / 2) + 20, quarter);
        Console.Beep(D, quarter);
        Console.Beep(F, quarter);
        Console.Beep(A + 20, quarter);
        Console.Beep(D * 2, quarter);
        Console.Beep(F * 2, quarter);
        Console.Beep(A * 2 + 20, half);
        Console.Beep(A * 2 + 20, eighth);
        Console.Beep(A * 2 + 20, eighth);
        Console.Beep(A * 2 + 20, eighth);
        Console.Beep(B * 2, note);
    }
}

