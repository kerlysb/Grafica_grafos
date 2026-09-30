using System.IO;

namespace GrafosVisual
{
    public static class LectorGrafo
    {
        public static Grafo CargarDesdeArchivo(string ruta)
        {
            string[] lineas = File.ReadAllLines(ruta);
            bool esDirigido = false;
            int indice = 0;

            while (indice < lineas.Length)
            {
                string linea = lineas[indice].Trim();
                if (linea.StartsWith("DIRIGIDO"))
                {
                    string valor = linea.Split('=')[1].Trim().ToUpper();
                    esDirigido = valor == "SI";
                    indice++;
                    break;
                }
                indice++;
            }

            Grafo grafo = new Grafo(esDirigido);

            while (indice < lineas.Length && lineas[indice].Trim() != "NODOS")
                indice++;
            indice++;

            if (indice < lineas.Length)
            {
                string[] nombresNodos = lineas[indice].Split(',');
                foreach (string nombre in nombresNodos)
                    grafo.AgregarNodo(nombre.Trim());
                indice++;
            }

            while (indice < lineas.Length && lineas[indice].Trim() != "ARISTAS")
                indice++;
            indice++;

            while (indice < lineas.Length)
            {
                string linea = lineas[indice].Trim();
                if (!string.IsNullOrWhiteSpace(linea))
                {
                    string[] partes = linea.Split('-');
                    if (partes.Length == 3)
                    {
                        string origen = partes[0].Trim();
                        string destino = partes[1].Trim();
                        int peso = int.Parse(partes[2].Trim());
                        grafo.AgregarArista(origen, destino, peso);
                    }
                }
                indice++;
            }

            return grafo;
        }
    }
}