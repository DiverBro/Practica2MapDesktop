//Jule Page Galocha y Estela Santos Mirete
namespace Practica2
{
    class Program
    {
        static Random rnd = new Random(); // generador de aleatorios para colocar los obstáculos verticales
        const bool DEBUG = true; // para sacar información de depuración en el Renderizado
        const int ANCHO = 22, ALTO = 14, // tamaño del área de juego
        SEP_OBS = 6, // separación horizontal entre obstáculos
        HUECO = 7, // hueco de los obstáculos (en vertical)
        COL_BIRD = ANCHO / 3, // columna fija del pájaro
        IMPULSO = 3, // unidades de ascenso por aleteo
        DELTA = 300; // retardo entre frames (ms)
        static void Main()
        { // programa principal
            int[] suelo, techo;
            int fil, ascenso, frame, puntos;
            bool colision, pausa = false;

            Console.WriteLine("Si quieres cargar un archivo pulsa(c), si no, cualquier letra.");
            string respuesta = Console.ReadKey(true).Key.ToString();

            if (respuesta == "c")
            {
                Console.WriteLine("Si quieres cargar un archivo pulsa(c), si no, cualquier letra?");
                string name = Console.ReadKey(true).Key.ToString();
                CargaJuego(name, file, suelo, techo, fil, ascenso, frame, puntos, colision);
            }
            else
            {
                Inicializa(out suelo, out techo, out fil, out ascenso, out frame, out puntos, out colision);
            }
            Render(suelo, techo, fil, frame, puntos, colision);
            while (!colision && continua)//continua
            {
                char teclaPulsada = LeeInput();
                if (teclaPulsada == 'q')
                {
                    continua = false;
                }
                else
                {
                    if (teclaPulsada == 'p')
                    {
                        pausa = !pausa;
                    }
                    Mueve(teclaPulsada, ref fil, ref ascenso, ref pausa, ref colision);
                    Avanza(suelo, techo, frame, teclaPulsada, pausa);
                    colision = Colision(suelo, techo, fil);
                    frame++;
                    Puntua(suelo, techo, ref puntos);
                    // Salir(teclaPulsada, ref colision, pausa);
                    Render(suelo, techo, fil, frame, puntos, colision);
                    System.Threading.Thread.Sleep(DELTA);//para que no vaya muy rapido y despues del render para que de tiempo que lo juegue
                }
            }

            //guardar
        }
        static void Inicializa(out int[] suelo, out int[] techo, out int fil, out int ascenso, out int frame, out int puntos, out bool colision)
        {
            suelo = new int[ANCHO];
            techo = new int[ANCHO];
            for (int i = 0; i < suelo.Length; i++)// es menor porque intenta llegar a la posicion 14 pero no hay porque el ultimo elemento es el 13 ya que empieza a contar desde el 0 hasta el 13
            {
                suelo[i] = 0;
                techo[i] = ALTO - 1;
            }
            fil = ALTO / 2;
            ascenso = -1;
            frame = 0;
            puntos = 0;
            colision = false;
        }

        static void Render(int[] suelo, int[] techo, int fil, int frame, int puntos, bool colision)
        {
            Console.Clear();
            for (int i = ALTO - 1; i >= 0; i--)
            {
                for (int j = 0; j < ANCHO; j++)
                {
                    if (suelo[j] >= i || techo[j] <= i)
                    {
                        Console.BackgroundColor = ConsoleColor.Blue;
                    }
                    else
                    {
                        Console.BackgroundColor = ConsoleColor.Black;
                    }
                    Console.Write("  ");
                }
                Console.WriteLine();
            }

            if (colision)
            {
                Console.SetCursorPosition(COL_BIRD, fil);
                Console.BackgroundColor = ConsoleColor.Red;
                Console.WriteLine("**");
            }
            else
            {
                //PÁJARO
                Console.SetCursorPosition(COL_BIRD, fil);
                Console.BackgroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine("->");
            }

            //PUNTOS
            Console.BackgroundColor = ConsoleColor.Black;
            Console.SetCursorPosition(0, ALTO + 1);
            Console.WriteLine("Puntos:" + puntos);
            if (DEBUG)
            {     //suelo y techo
                Console.SetCursorPosition(0, ALTO + 2);
                Console.Write("Suelo: ");
                for (int i = 0; i < ANCHO; i++)
                {
                    Console.Write(suelo[i] + " ");
                }
                Console.SetCursorPosition(0, ALTO + 3);
                Console.Write("Techo: ");
                for (int j = 0; j < ANCHO; j++)
                {
                    Console.Write(techo[j] + " ");
                }
                Console.SetCursorPosition(0, ALTO + 4);
                Console.WriteLine("Frame: " + frame + "  Pos Bird: " + fil);
            }
        }
        static void Avanza(int[] suelo, int[] techo, int frame, char ch, bool pausa)
        {
            int t, s;

            if (!pausa)
            {
                for (int i = 0; i < suelo.Length - 1; i++)
                {
                    suelo[i] = suelo[i + 1];
                    techo[i] = techo[i + 1];
                }
                if (frame % SEP_OBS == 0)
                {
                    s = rnd.Next(0, ALTO / 2 - 1);//dar valores aleatorios a s para el suelo
                    t = HUECO - 1 + s;
                }
                else
                {
                    s = 0;
                    t = ALTO - 1;
                }
                suelo[suelo.Length - 1] = s;// es lenght-1 porque el array mide 9 porque empieza contando desde 0 entonces la posicion del ultimo es 9-1 es decir la 8  
                techo[techo.Length - 1] = t;

            }
        }
        static void Mueve(char ch, ref int fil, ref int ascenso, ref bool pausa, ref bool colision)// son ref porque las modifico y las uso fuera y dentro
        {
            if (ch == 'p')
            {
                pausa = !pausa;
            }

            if (!pausa)
            {
                if (ch == 'i')
                {
                    ascenso = IMPULSO;
                }
                if (ascenso >= 0)
                {
                    fil--;
                    ascenso--;
                }
                else
                {
                    fil++;
                }
            }

        }

        static void Salir(char ch, ref bool colision, bool pausa)
        {
            if (ch == 'q')
            {
                colision = true;
                pausa = true;
            }
        }

        static bool Colision(int[] suelo, int[] techo, int fil)
        {
            //  bool colision = fil <= suelo[COL_BIRD] || fil >= techo[COL_BIRD];// cuando no es asi, es falsa, cuando se cumple, se vulve verdadera
            bool colision = false;
            if (ALTO - 1 - suelo[COL_BIRD / 2 + 1] <= fil || ALTO - 1 - techo[COL_BIRD / 2 + 1] >= fil)//hay que invertir las cordenadas porque para fil el 00 esta arriba a la izquierda entonvces le restas la altura-1 al suelo y al techo para invertir el valor 13-13=0 y asi en vez de valer 13 vale 0, y luego el fil tiene que ser o mayor o menor segfun quieras que sea el techo o el suelo
            {
                colision = true;
            }
            return colision;
        }

        static void Puntua(int[] suelo, int[] techo, ref int puntos)
        {
            if (suelo[COL_BIRD] != 0 && techo[COL_BIRD] != ALTO - 1)
            {
                puntos++;
            }
        }
        static char LeeInput()
        {
            char ch = ' ';
            if (Console.KeyAvailable)
            {
                string s = Console.ReadKey(true).Key.ToString();
                if (s == "X" || s == "Spacebar") ch = 'i'; // impulso                  
                else if (s == "P") ch = 'p'; // pausa
                else if (s == "Q" || s == "Escape") ch = 'q'; // salir
                while (Console.KeyAvailable) Console.ReadKey();
            }
            return ch;
        }
        void GuardaJuego(string file, int[] suelo, int[] techo, int fil, int ascenso, int frame, int puntos)
        {

        }

        void CargaJuego(string file, int[] suelo, int[] techo, int fil, int ascenso, int frame, int puntos, bool colision)
        {

        }
    }
}

