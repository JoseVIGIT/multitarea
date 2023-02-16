using System;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Multitarea
{
    /// <summary>
    /// Se muestran dos listados de valores alfanumerico (A..J) y enteros (0..4) 
    /// Función Main async para poder ejecutar usar await en la llamada a Task.WhenAll para controlar finalización de procesos
    /// Si no se define de esta manera la ejecución en paralelo se realiza igualmente 
    /// pero no podremos controlar finalización de todos los procesos al no poder usar await (resultados inesperados)
    /// Se definen las funciones de tipo async para poder simular bloqueos y usar await para liberar proceso
    /// </summary>
    class Program
    {
        static async Task Main(string[] args)
        {
            int iteraciones = 5;
            Stopwatch tiempo = new Stopwatch();

            do
            {
                Console.Clear();

                #region Tarea sincrona
                Console.WriteLine("Impresión A..J y 1..5 usando un único proceso:");
                TimeSpan tsincrono = TimeSpan.Zero;
                for (int i = 0; i < iteraciones; i++)
                {
                    tiempo.Reset();
                    tiempo.Start();
                    ProcesoNumerosLetras();
                    tiempo.Stop();
                    Console.WriteLine(tiempo.Elapsed);
                    tsincrono += tiempo.Elapsed;
                }
                #endregion

                tiempo.Reset();
                Console.WriteLine();

                #region Tarea asincrona
                Console.WriteLine("Impresión A..J y 1..5 usando dos procesos asincronos en paralelo:");
                TimeSpan tasincrono = TimeSpan.Zero;
                for (int i = 0; i < iteraciones; i++)
                {
                    tiempo.Reset();
                    tiempo.Start();
                    Task p1 = Task.Run(() => ProcesoNumeros());
                    Task p2 = Task.Run(() => ProcesoLetras());
                    await Task.WhenAll(p1, p2);
                    tiempo.Stop();
                    Console.WriteLine(tiempo.Elapsed);
                    tasincrono += tiempo.Elapsed;
                }
                #endregion

                Console.WriteLine();
                Console.WriteLine("Sincrono  finalizado en {0} de media", TimeSpan.FromTicks(tsincrono.Ticks / iteraciones));
                Console.WriteLine("Asincrono finalizado en {0} de media", TimeSpan.FromTicks(tasincrono.Ticks / 5));
                Console.WriteLine("Diferencia de tiempo {0}\n", TimeSpan.FromTicks((tsincrono.Ticks - tasincrono.Ticks) / iteraciones));
                Console.WriteLine("Pulse R para repetir o cualquier otra tecla para cerrar...");

            } 
            while (Console.ReadKey().Key.ToString().ToUpper() == "R");
        }

        static async Task ProcesoNumeros()
        {
            int a = 0;
            while (a<5)
            {
                Console.Write(++a + ", ");
                Thread.Sleep(1); //Simulando que el proceso es largo
                await Task.Delay(25); //Simulando que se ejecuta una acción bloqueante
            }
        }

        static async void ProcesoLetras()
        {
            int a = 0;
            while (a < 10)
            {
                Console.Write((char)(65 + a++) + ", ");
                Thread.Sleep(1);
            }
        }

        static void ProcesoNumerosLetras()
        {
            int a = 0;
            while (a < 5)
            {
                Console.Write(++a + ", ");
                Thread.Sleep(1+25); //Milisegundos añadidos para simular la misma espera que los añadidos en su versión asincrona
            }
            int b = 0;
            while (b < 10)
            {
                Console.Write((char)(65 + b++) + ", ");
                Thread.Sleep(1);
            }
        }
    }
}
