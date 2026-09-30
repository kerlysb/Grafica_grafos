using System;
using System.Collections.Generic;

namespace GrafosVisual
{
    public class Arista
    {
        public string Origen { get; set; }
        public string Destino { get; set; }
        public int Peso { get; set; }

        public Arista(string origen, string destino, int peso)
        {
            Origen = origen;
            Destino = destino;
            Peso = peso;
        }
    }

    