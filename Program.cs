internal class Program
{
    /* POPIS - HLEDANI MIN.
    vlozit parametry minoveho pole (ctverec).
    vygenerovat prazdne pole.
    zvolit prvni souradnice.
    vygenerovat miny -> prvni krok tak bude vzdy mimo minu.
    herni smycka.
        zvolit souradnice (krom prvni smycky).
        odkryt zvolene souradnice.
        vyhodnotit konec hry.
        vykreslit aktualni minove pole.
    vyhodnotit hru.

    HODNOTY POLICEK.
    0-8 - pocet min v okoli.
    10-18 - mina.
    20+ - chranene policko.
    kladne hodnoty - skryto.
    zaporne hodnoty - odkryto, vyjimka 0 se prevede na -9.
    */

    /* TO DO.
    smrsknout k sobe sloupceky, sloupcove souradnice vypsat na dva radky.
    algoritmus na shluky min.
    oznacovani min.
    oznacovani otazniky.
    
    */

    public static void Main(string[] args)
    {
        // DEFINICE KONSTANT pro zadani ulohy.
        const int VELIKOST_POLE_MIN = 5;
        const int VELIKOST_POLE_MAX = 20;
        //const int POCET_MIN_MIN = 10;
        //const int POCET_MIN_MAX = 30;
        // predchozi dva radky nejsou potreba - roysah se stanovi podle velikosti pole.

        // DEFINICE VELIKOKSTI POLE uzivatelem.
        Console.Write("Definice velikost pole: ");
        int velikostPole = KontrolaVstupuInt(VELIKOST_POLE_MIN, VELIKOST_POLE_MAX);
        Console.WriteLine();

        // NASTAVENI ROZSAHU POCTU MIN dle velikosti pole.
        int pocetMinMin = velikostPole*velikostPole/10;
        int pocetMinMax = velikostPole*velikostPole/5;

        // NASTAVENI POCTU MIN uzivatelem.
        Console.Write("Zadej pocet min: ");
        int miny = KontrolaVstupuInt(pocetMinMin, pocetMinMax);
        Console.WriteLine();

        // VYCISTENI KONZOLE pro ciste vykresleni hraciho pole.
        Console.Clear();

        // VYTVORENI MINOVEHO POLE.
        // Definuje se o 2 radky a 2 sloupce vetsi -> neni treba resit podminku pro "dostani se mimo hraci pole".
        // Hodnota vsem polickum = 0.
        int[,] minovePole = new int[velikostPole + 2, velikostPole + 2];
        Array.Clear(minovePole, 0, minovePole.Length);

/* delsi alternativa.
        for (int i = 0; i < velikostPole + 2; i++)
        {
            for (int j = 0; j < velikostPole + 2; j++)
            {
                minovePole[i, j] = 0;
            }
        }
*/

        // VYKRESLENI HRACIHO POLE, aby uzivatel mohl zvolit prvni souradnice.
        // 'hra' = definice konce hry, 0-pokracovani, 1-prohra, 2-vyhra.
        int hra = 0;
        VykresleniPole(velikostPole, minovePole, hra);

        // PRVNI VOLBA SOURADNIC.
        // Deklarace pro vstup souradnic.
        int radek, sloupec;
        VstupRadekSloupec(velikostPole, out radek, out sloupec);

        // OCHRANA PRVNIHO VSTUPU pred generovanim min.
        // Zajisti, aby prvni krok neskoncil na mine, ale naopak odkryl cast hraci plochy -> vybrane pole a jeho okoli je bez miny.
        // Hodnota policka = 20.
        for (int i = radek - 1; i < radek + 2; i++)
        {
            for (int j = sloupec - 1; j < sloupec + 2; j++)
            {
                minovePole[i, j] = 20;
            }
        }

        // GENEROVANI MIN.
        // Voli se nahodnoe souradnice.
        // Pokud je hodnota policka >= 10 (jiz umistena mina nebo chranene pole), mina se neumistuje.
        // Jinak je hodnota policka nastavena na 10 a vsem okolnim polickum je upravena hodnota o +1.
        Random random = new Random();
        int randomR;
        int randomS;
        int generMiny = miny;

        while (generMiny > 0)
        {
            randomR = random.Next(velikostPole) + 1;
            randomS = random.Next(velikostPole) + 1;

            if (minovePole[randomR, randomS] < 10)
            {
                minovePole[randomR, randomS] = 10;

                for (int i = randomR - 1; i < randomR + 2; i++)
                {
                    for (int j = randomS - 1; j < randomS + 2; j++)
                    {
                        minovePole[i, j] += 1;
                    }
                }

                generMiny--;
            }
        }

        // UHLAZENI MINOVEHO POLE.
        // Srovnani hodnot, ktere jsou > 10.
        // Dosazeni jednotnych hodnot na hranice.
        for (int i = 1; i < velikostPole + 1; i++)
        {
            for (int j = 1; j < velikostPole + 1; j++)
            {
                // srovnani hodnot uvnitr pole - chranena policka.
                if (minovePole[i, j] >= 20)
                    minovePole[i, j] -= 20;
                // srovnani hodnot uvnitr pole - policka s minou.
                else if (minovePole[i, j] >= 10)
                    minovePole[i, j] = 10;
            }

            // dosazeni hranic - sloupce.
            minovePole[i, 0] = 1;
            minovePole[i, velikostPole + 1] = 1;
        }

        // dosazeni hranic - radky.
        for (int j = 0; j < velikostPole + 2; j++)
        {
            minovePole[0, j] = 1;
            minovePole[velikostPole + 1, j] = 1;
        }

        // HRA - cyklus do konce hry.
        // rozliseni prvniho vstupu, jiz mame prvni souradnice i vykreslene pole.
        int start = 0;

        while (hra == 0)
        {
            // souradnice zadavat az pri druhe smycce, v prvnim kole se pouziji jiz existujici hodnoty z prvniho zadani.
            if (start != 0)
                VstupRadekSloupec(velikostPole, out radek, out sloupec);
            else
                start += 1;

            // vycisteni konzole a odkryti pole/poli.
            Console.Clear();
            hra = Odkryvani(minovePole, radek, sloupec, hra);

            // kontrola prohry.
            if (hra == 1)
            {
                VykresleniPole(velikostPole, minovePole, hra);
                break;
            }

            // kontrola vyhry.
            // nejdrive nastavit vyhru, ale pokud je nejake pole jeste neodkryte, nastavit zpet hra=0 a pokracovat.
            // jakmile je nalezeno prvni neodkryte pole, ukoncuje hledani.
            hra = 2;
            for (int i = 0; i < velikostPole; i++)
            {
                for (int j = 0; j < velikostPole; j++)
                {
                    if (minovePole[i + 1, j + 1] > 0 && minovePole[i + 1, j + 1] != 10)
                    {
                        hra = 0;
                        break;
                    }
                }
                if (hra == 0)
                    break;
            }

            // vykresleni pole.
            VykresleniPole(velikostPole, minovePole, hra);
        }

        // informace o vysledku hry.
        if (hra == 1)
            Console.WriteLine("Mina. X-( Zkus to jeste jednou!");
        else if (hra == 2)
            Console.WriteLine("Odkryl jsi vsechna pole, gratuluji! :-)");

        // VIZUALIZACNI TEST - kontrola dat pri psani kodu.
        // OdkrytiPole(velikostPole, minovePole);

        Console.ReadKey();
    }


    //------------------
    // M E T O D Y
    //------------------

    // KONTROLA VSTUPU, INT + ROZSAH.
    // Dokud neni vstup Int32 v rozsahu od min do max, metoda nepusti dal.
    private static int KontrolaVstupuInt(int _min, int _max)
    {
        Console.Write($"Zadej cele cislo v rozsahu od {_min} do {_max} vcetne: ");
        string _vstup = Console.ReadLine();

        int _cislo;

        while (!(int.TryParse(_vstup, out _cislo) && _cislo >= _min && _cislo <= _max))
        {
            Console.Write($"Spatny vstup. zadej cele cislo od {_min} do {_max} vcetne: ");
            _vstup = Console.ReadLine();
        }

        return _cislo;
    }


    // ZADANI RADKU A SLOUPCE.
    private static void VstupRadekSloupec(int _velikostPole, out int _radek, out int _sloupec)
    {
        Console.WriteLine("Vyber radek:");
        _radek = KontrolaVstupuInt(1, _velikostPole);
        Console.WriteLine("Vyber sloupec:");
        _sloupec = KontrolaVstupuInt(1, _velikostPole);
    }


    // ODKRYVANI POLICEK.
    // odkryti pole => prevedeni na zapornou hodnotu.
    // v pripade odkryti miny -> ukonceni hry (hra=1).
    private static int Odkryvani(int[,] minovePole, int _radek, int _sloupec, int _hra)
    {
        // v pripade, ze v okoli neni mina (policko==0), odkryje vsechna okolni pole, pokud jiz nejsou odkryta a zmen hondotu pole do zaporu.
        if (minovePole[_radek, _sloupec] == 0)
        {
            minovePole[_radek, _sloupec] = -9;
            for (int i = -1; i < 2; i++)
            {
                for (int j = -1; j < 2; j++)
                {
                    // rekurze, odkryvani celeho okoli 0.
                    _hra = Odkryvani(minovePole, _radek + i, _sloupec + j, _hra);
                }
            }
        }
        // pole v blizkosti miny/min.
        else if (minovePole[_radek, _sloupec] > 0 && minovePole[_radek, _sloupec] < 9)
            minovePole[_radek, _sloupec] *= (-1);
        // mina => konec hry - prohra.
        else if (minovePole[_radek, _sloupec] == 10)
            _hra = 1;

        return _hra;
    }


    // VYKRESLENI POLE.
    // Vykresli hraci pole -> jiz odkryta policka oznac cislem, neodkryta '?'.
    // Vykresluje souradnice radku a sloupcu.
    private static void VykresleniPole(int _velikostPole, int[,] _minovePole, int _hra)
    {
        // vykresleni popisu sloupcu.
        Console.Write("S   ");
        for (int i = 0; i < _velikostPole; i++)
        {
            if (i < 9)
                Console.Write(i + 1 + "  ");
            else
                Console.Write(i + 1 + " ");
        }
        Console.WriteLine();

        // vykresleni popisu radku + vlastni minove pole.
        for (int i = 0; i < _velikostPole; i++)
        {
            // popis radku.
            if (i < 9)
                Console.Write("R" + (i + 1) + "  ");
            else
                Console.Write("R" + (i + 1) + " ");
            
            // vykresleni hodnot v poli.
            for (int j = 0; j < _velikostPole; j++)
            {
                switch (_minovePole[i + 1, j + 1])
                {
                    // case -9 -> pozice bez miny v okoli (odkryta 0).
                    case -9:
                        Obarveni(_minovePole, i, j, ConsoleColor.Gray, ConsoleColor.Black);
                        break;
                    case -1:
                        Obarveni(_minovePole, i, j, ConsoleColor.Blue, ConsoleColor.White);
                        break;
                    case -2:
                        Obarveni(_minovePole, i, j, ConsoleColor.DarkGreen, ConsoleColor.White);
                        break;
                    case -3:
                        Obarveni(_minovePole, i, j, ConsoleColor.Red, ConsoleColor.White);
                        break;
                    case -4:
                        Obarveni(_minovePole, i, j, ConsoleColor.DarkBlue, ConsoleColor.White);
                        break;
                    case -5:
                        Obarveni(_minovePole, i, j, ConsoleColor.DarkRed, ConsoleColor.White);
                        break;
                    case -6:
                        Obarveni(_minovePole, i, j, ConsoleColor.DarkCyan, ConsoleColor.White);
                        break;
                    case -7:
                        Obarveni(_minovePole, i, j, ConsoleColor.DarkMagenta, ConsoleColor.White);
                        break;
                    case -8:
                        Obarveni(_minovePole, i, j, ConsoleColor.DarkGray, ConsoleColor.White);
                        break;
                    case 10:
                        if (_hra==0)
                            Console.Write("?  ");
                        else
                            Obarveni(_minovePole, i, j, ConsoleColor.Yellow, ConsoleColor.DarkRed);
                        break;
                    default:
                        Console.Write("?  ");
                        break;
                }
            }
            Console.WriteLine();
        }
        Console.WriteLine();
    }


    // OBARVENI POLI pri vykresleni.
    // zmena barvy pozadi i textu.
    // pouze pro odkryta policka.
    // je treba prevest zaporne hodnoty zpet na kladne, -9 na 0.
    private static void Obarveni(int[,] _minovePoleObarv, int _i, int _j, ConsoleColor barvaPozadi, ConsoleColor barvaTextu)
    {
        Console.BackgroundColor = barvaPozadi;
        Console.ForegroundColor = barvaTextu;
        if (_minovePoleObarv[_i + 1, _j + 1] == -9)
            Console.Write('0');
        else if (_minovePoleObarv[_i + 1, _j + 1] == 10)
            Console.Write('X');
        else
            Console.Write((_minovePoleObarv[_i + 1, _j + 1] * (-1)));
        Console.BackgroundColor = ConsoleColor.Black;
        Console.ForegroundColor = ConsoleColor.White;
        Console.Write("  ");
    }


    // VYKRESLENI POLE.
    // vykresli cele pole vcetne hranic.
    // vypise aktualni hodnoty pole.
    // pro testing pri psani kodu, znamenko '-' rozhodi formatovani.
    private static void OdkrytiPole(int _velikostPole, int[,] _minovePole)
    {
        // popis sloupcu.
        Console.Write("S   ");
        for (int i = 0; i < _velikostPole + 2; i++)
        {
            if (i < 10)
                Console.Write(i + "  ");
            else
                Console.Write(i + " ");
        }

        // popis radku a samotneho pole.
        Console.WriteLine();
        for (int i = 0; i < _velikostPole + 2; i++)
        {
            if (i < 10)
                Console.Write("R" + i + "  ");
            else
                Console.Write("R" + i + " ");
            for (int j = 0; j < _velikostPole + 2; j++)
            {
                if (_minovePole[i, j] > 9)
                    Console.Write(_minovePole[i, j] + " ");
                else if (_minovePole[i, j] >= 0)
                    Console.Write(_minovePole[i, j] + "  ");
                else
                    Console.Write(_minovePole[i, j] + " ");
            }
            Console.WriteLine();
        }
        Console.WriteLine();
    }


    // NAPLNENI POLE, PRES VSECHNA POLICKA
    // Nevhodny zpusob, lepsi vkladat miny primo
    /*
    public static char[,] Pole(int velikostPole, int miny)
    {
        int pocetPoli = velikostPole*velikostPole;
        int pravdepodobnost;

        Random random = new Random();
        int randomRange;

        // Naplneni pole
        char[,] minovePole = new char[velikostPole,velikostPole];
        for (int i=0; i<velikostPole; i++)
        {
            for (int j=0; j<velikostPole; j++)
            {
                pravdepodobnost = 100*miny/pocetPoli;
                randomRange = random.Next(100);

                if (pravdepodobnost>randomRange)
                {
                    minovePole[i,j]='x';
                    miny--;
                }
                else
                {
                    minovePole[i,j]='o';
                }
                pocetPoli--;
                //Console.WriteLine(randomRange+" vs "+pravdepodobnost);
            }
        }
        return minovePole;
    }
    */

}