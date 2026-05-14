using System;
using System.IO;
using System.Globalization;

namespace EsercitazionePercettrone
{
    class Percettrone
    {
        private const int Features = 5;
        private const double Threshold = 0.5;
        private double[] weights = new double[Features];
        private double bias;

        // 1. Funzione di attivazione (Step function)
        private int Activation(double x)
        {
            return x > Threshold ? 1 : 0;
        }

        // 2. Funzione per caricare i pesi da file
        public bool CaricaPesi(string filename)
        {
            if (!File.Exists(filename))
            {
                Console.WriteLine($"Errore: file {filename} non trovato!");
                return false;
            }

            try
            {
                string[] linee = File.ReadAllLines(filename);
                for (int i = 0; i < Features; i++)
                {
                    // Estrae il valore numerico dalla riga "Peso X: valore"
                    string valoreStr = linee[i].Split(':')[1].Trim();
                    weights[i] = double.Parse(valoreStr, CultureInfo.InvariantCulture);
                }
                // Legge il Bias (ultima riga)
                string biasStr = linee[Features].Split(':')[1].Trim();
                bias = double.Parse(biasStr, CultureInfo.InvariantCulture);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Errore nella lettura del file: {ex.Message}");
                return false;
            }
        }

        // 3. Funzione di previsione
        public int Prevedi(int[] input)
        {
            double somma = bias;
            for (int i = 0; i < Features; i++)
            {
                somma += input[i] * weights[i];
            }
            return Activation(somma);
        }

        // 4. Funzione di Allenamento (Training / Backpropagation semplificata)
        public void Allena(int[][] trainingInputs, int[] targets, double learningRate, int epoche)
        {
            for (int e = 0; e < epoche; e++)
            {
                for (int i = 0; i < trainingInputs.Length; i++)
                {
                    int previsione = Prevedi(trainingInputs[i]);
                    int errore = targets[i] - previsione;

                    if (errore != 0)
                    {
                        // Aggiornamento pesi: wi = wi + η * errore * xi
                        for (int j = 0; j < Features; j++)
                        {
                            weights[j] += learningRate * errore * trainingInputs[i][j];
                        }
                        // Aggiornamento bias: b = b + η * errore
                        bias += learningRate * errore;
                    }
                }
            }
            Console.WriteLine("Allenamento completato.");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Percettrone mioPercettrone = new Percettrone();

            // Tentativo di caricamento pesi
            if (!mioPercettrone.CaricaPesi("pesi_concerto.txt"))
            {
                Console.WriteLine("Impossibile avviare il programma senza i pesi.");
                return;
            }

            Console.WriteLine("--- Percettrone Pronto ---");
            string[] domande = {
                "Artista famoso?",
                "Bel meteo?",
                "Amici presenti?",
                "Cibo buono?",
                "Alcool disponibile?"
            };

            int[] inputUtente = new int[5];

            // Ciclo di interazione
            for (int i = 0; i < domande.Length; i++)
            {
                Console.Write($"{domande[i]} (1=Si, 0=No): ");
                if (int.TryParse(Console.ReadLine(), out int valore))
                    inputUtente[i] = valore;
            }

            int decisione = mioPercettrone.Prevedi(inputUtente);

            Console.WriteLine("\n----------------------------");
            if (decisione == 1)
                Console.WriteLine("🎯 Risultato: Vai al concerto!");
            else
                Console.WriteLine("🏠 Risultato: Resta a casa!");
            Console.WriteLine("----------------------------");
        }
    }
}
