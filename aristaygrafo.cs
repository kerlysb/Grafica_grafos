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

    public class Grafo
    {
        public bool EsDirigido { get; private set; }
        public List<string> Nodos { get; private set; }
        public Dictionary<string, List<Arista>> ListaAdyacencia { get; private set; }

        public Grafo(bool esDirigido)
        {
            EsDirigido = esDirigido;
            Nodos = new List<string>();
            ListaAdyacencia = new Dictionary<string, List<Arista>>();
        }

        public void AgregarNodo(string nombre)
        {
            if (!ListaAdyacencia.ContainsKey(nombre))
            {
                Nodos.Add(nombre);
                ListaAdyacencia[nombre] = new List<Arista>();
            }
        }

        public void AgregarArista(string origen, string destino, int peso)
        {
            AgregarNodo(origen);
            AgregarNodo(destino);
            ListaAdyacencia[origen].Add(new Arista(origen, destino, peso));
            if (!EsDirigido)
                ListaAdyacencia[destino].Add(new Arista(destino, origen, peso));
        }

        public List<Arista> ObtenerTodasLasAristas()
        {
            var resultado = new List<Arista>();
            var vistos = new HashSet<string>();
            foreach (var nodo in Nodos)
            {
                foreach (var arista in ListaAdyacencia[nodo])
                {
                    string clave1 = arista.Origen + "-" + arista.Destino;
                    string clave2 = arista.Destino + "-" + arista.Origen;
                    if (EsDirigido)
                    {
                        resultado.Add(arista);
                    }
                    else if (!vistos.Contains(clave1) && !vistos.Contains(clave2))
                    {
                        resultado.Add(arista);
                        vistos.Add(clave1);
                    }
                }
            }
            return resultado;
        }

        public int ObtenerGrado(string nodo)
        {
            if (!ListaAdyacencia.ContainsKey(nodo)) return 0;
            return ListaAdyacencia[nodo].Count;
        }

        public List<string> ObtenerVecinos(string nodo)
        {
            var vecinos = new List<string>();
            if (ListaAdyacencia.ContainsKey(nodo))
            {
                foreach (var arista in ListaAdyacencia[nodo])
                    vecinos.Add(arista.Destino);
            }
            return vecinos;
        }

        public int ContarNodos()
        {
            return Nodos.Count;
        }

        public int ContarAristas()
        {
            return ObtenerTodasLasAristas().Count;
        }
    }
}