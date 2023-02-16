using System;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Multitarea
{
    /// <summary>
    /// Se muestran dos listados de valores alfanumerico (A..J) y enteros (0..4)
    /// El listado de enteros acabará a priori antes que el de alfanuméricos
    /// Función Main async para poder ejecutar usar await en la llamada a Task.WhenAll para controlar finalización de procesos
    /// Si no se define de esta manera la ejecución en paralelo se realiza igualmente pero no podremos controlar finalización de todos los procesos al no poder usar await 
    /// Cada función es de tipo asincrono para poder usar internamente await que permita salto entre procesos.
    /// El tipo devuelto por funciones asincronas debe ser una Task
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

        static void ProcesoNumeros()
        {
            int a = 0;
            while (a<5)
            {
                Console.Write(++a + ", ");
                Thread.Sleep(1);
            }
        }

        static void ProcesoLetras()
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
                Thread.Sleep(1);
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
