using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace GrafosVisual
{
    public partial class Form1 : Form
    {
        private Grafo grafoActual;
        private Panel pnlGrafico;
        private ComboBox cmbEjemplo;
        private Button btnCargar;
        private ComboBox cmbNodoConsulta;
        private Button btnConsultar;
        private Button btnReporteCompleto;
        private TextBox txtResultadoConsulta;
        private ListBox lstReporte;
        private Dictionary<string, Point> posicionesNodos;

        public Form1()
        {
            InitializeComponent();
            ConfigurarInterfaz();
        }

        private void ConfigurarInterfaz()
        {
            this.Text = "Visualizador de Grafos";
            this.Width = 950;
            this.Height = 650;

            cmbEjemplo = new ComboBox();
            cmbEjemplo.Location = new Point(10, 10);
            cmbEjemplo.Width = 250;
            cmbEjemplo.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbEjemplo.Items.Add("Ejemplo 1: Rutas entre ciudades");
            cmbEjemplo.Items.Add("Ejemplo 2: Red social (seguidores)");
            cmbEjemplo.SelectedIndex = 0;
            this.Controls.Add(cmbEjemplo);

            btnCargar = new Button();
            btnCargar.Text = "Cargar y graficar";
            btnCargar.Location = new Point(270, 9);
            btnCargar.Width = 130;
            btnCargar.Click += BtnCargar_Click;
            this.Controls.Add(btnCargar);

            cmbNodoConsulta = new ComboBox();
            cmbNodoConsulta.Location = new Point(410, 10);
            cmbNodoConsulta.Width = 150;
            cmbNodoConsulta.DropDownStyle = ComboBoxStyle.DropDownList;
            this.Controls.Add(cmbNodoConsulta);

            btnConsultar = new Button();
            btnConsultar.Text = "Consultar nodo";
            btnConsultar.Location = new Point(570, 9);
            btnConsultar.Width = 120;
            btnConsultar.Click += BtnConsultar_Click;
            this.Controls.Add(btnConsultar);

            btnReporteCompleto = new Button();
            btnReporteCompleto.Text = "Reporte completo";
            btnReporteCompleto.Location = new Point(700, 9);
            btnReporteCompleto.Width = 130;
            btnReporteCompleto.Click += BtnReporteCompleto_Click;
            this.Controls.Add(btnReporteCompleto);

            pnlGrafico = new Panel();
            pnlGrafico.Location = new Point(10, 45);
            pnlGrafico.Size = new Size(600, 550);
            pnlGrafico.BorderStyle = BorderStyle.FixedSingle;
            pnlGrafico.BackColor = Color.White;
            pnlGrafico.Paint += PnlGrafico_Paint;
            this.Controls.Add(pnlGrafico);

            txtResultadoConsulta = new TextBox();
            txtResultadoConsulta.Location = new Point(620, 45);
            txtResultadoConsulta.Size = new Size(300, 100);
            txtResultadoConsulta.Multiline = true;
            txtResultadoConsulta.ReadOnly = true;
            txtResultadoConsulta.ScrollBars = ScrollBars.Vertical;
            this.Controls.Add(txtResultadoConsulta);

            lstReporte = new ListBox();
            lstReporte.Location = new Point(620, 155);
            lstReporte.Size = new Size(300, 440);
            this.Controls.Add(lstReporte);

            posicionesNodos = new Dictionary<string, Point>();
        }
        private void BtnCargar_Click(object sender, EventArgs e)
        {
            string ruta;
            if (cmbEjemplo.SelectedIndex == 0)
                ruta = "rutas_ciudades.txt";
            else
                ruta = "red_social.txt";

            if (!System.IO.File.Exists(ruta))
            {
                MessageBox.Show("No se encontró el archivo: " + ruta);
                return;
            }

            grafoActual = LectorGrafo.CargarDesdeArchivo(ruta);
            CalcularPosicionesNodos();

            cmbNodoConsulta.Items.Clear();
            foreach (string nodo in grafoActual.Nodos)
                cmbNodoConsulta.Items.Add(nodo);
            if (cmbNodoConsulta.Items.Count > 0)
                cmbNodoConsulta.SelectedIndex = 0;

            lstReporte.Items.Clear();
            txtResultadoConsulta.Clear();
            pnlGrafico.Invalidate();
        }

        private void CalcularPosicionesNodos()
        {
            posicionesNodos.Clear();
            int cantidad = grafoActual.Nodos.Count;
            int centroX = pnlGrafico.Width / 2;
            int centroY = pnlGrafico.Height / 2;
            int radio = Math.Min(centroX, centroY) - 60;

            for (int i = 0; i < cantidad; i++)
            {
                double angulo = 2 * Math.PI * i / cantidad;
                int x = centroX + (int)(radio * Math.Cos(angulo));
                int y = centroY + (int)(radio * Math.Sin(angulo));
                posicionesNodos[grafoActual.Nodos[i]] = new Point(x, y);
            }
        }
        private void PnlGrafico_Paint(object sender, PaintEventArgs e)
        {
            if (grafoActual == null) return;

            Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            Pen lapizArista = new Pen(Color.SteelBlue, 2);
            Font fuenteEtiqueta = new Font("Segoe UI", 9, FontStyle.Bold);
            Font fuentePeso = new Font("Segoe UI", 8);
            Brush pincelNodo = new SolidBrush(Color.LightSkyBlue);
            Brush pincelTexto = new SolidBrush(Color.Black);

            List<Arista> aristas = grafoActual.ObtenerTodasLasAristas();
            foreach (Arista arista in aristas)
            {
                Point p1 = posicionesNodos[arista.Origen];
                Point p2 = posicionesNodos[arista.Destino];
                g.DrawLine(lapizArista, p1, p2);

                if (grafoActual.EsDirigido)
                    DibujarFlecha(g, p1, p2);

                Point puntoMedio = new Point((p1.X + p2.X) / 2, (p1.Y + p2.Y) / 2);
                if (arista.Peso > 1)
                    g.DrawString(arista.Peso.ToString(), fuentePeso, pincelTexto, puntoMedio);
            }

            int radioNodo = 22;
            foreach (string nodo in grafoActual.Nodos)
            {
                Point p = posicionesNodos[nodo];
                Rectangle rect = new Rectangle(p.X - radioNodo, p.Y - radioNodo, radioNodo * 2, radioNodo * 2);
                g.FillEllipse(pincelNodo, rect);
                g.DrawEllipse(Pens.SteelBlue, rect);

                SizeF tamanoTexto = g.MeasureString(nodo, fuenteEtiqueta);
                g.DrawString(nodo, fuenteEtiqueta, pincelTexto, p.X - tamanoTexto.Width / 2, p.Y - tamanoTexto.Height / 2);
            }
        }

        private void DibujarFlecha(Graphics g, Point origen, Point destino)
        {
            double angulo = Math.Atan2(destino.Y - origen.Y, destino.X - origen.X);
            int radioNodo = 22;
            Point puntaFlecha = new Point(
                destino.X - (int)(radioNodo * Math.Cos(angulo)),
                destino.Y - (int)(radioNodo * Math.Sin(angulo)));

            double anguloAla = Math.PI / 7;
            int largoAla = 10;

            Point ala1 = new Point(
                puntaFlecha.X - (int)(largoAla * Math.Cos(angulo - anguloAla)),
                puntaFlecha.Y - (int)(largoAla * Math.Sin(angulo - anguloAla)));

            Point ala2 = new Point(
                puntaFlecha.X - (int)(largoAla * Math.Cos(angulo + anguloAla)),
                puntaFlecha.Y - (int)(largoAla * Math.Sin(angulo + anguloAla)));

            g.FillPolygon(Brushes.SteelBlue, new Point[] { puntaFlecha, ala1, ala2 });
        }
        private void BtnConsultar_Click(object sender, EventArgs e)
        {
            if (grafoActual == null || cmbNodoConsulta.SelectedItem == null) return;

            string nodo = cmbNodoConsulta.SelectedItem.ToString();
            int grado = grafoActual.ObtenerGrado(nodo);
            List<string> vecinos = grafoActual.ObtenerVecinos(nodo);

            string resultado = "Nodo: " + nodo + Environment.NewLine;
            resultado += "Grado: " + grado + Environment.NewLine;
            resultado += "Vecinos: " + (vecinos.Count > 0 ? string.Join(", ", vecinos) : "Ninguno");

            txtResultadoConsulta.Text = resultado;
        }

        private void BtnReporteCompleto_Click(object sender, EventArgs e)
        {
            if (grafoActual == null) return;

            lstReporte.Items.Clear();
            lstReporte.Items.Add("Tipo: " + (grafoActual.EsDirigido ? "Dirigido" : "No dirigido"));
            lstReporte.Items.Add("Total de nodos: " + grafoActual.ContarNodos());
            lstReporte.Items.Add("Total de aristas: " + grafoActual.ContarAristas());
            lstReporte.Items.Add("---- Nodos y grado ----");

            foreach (string nodo in grafoActual.Nodos)
                lstReporte.Items.Add(nodo + " (grado: " + grafoActual.ObtenerGrado(nodo) + ")");

            lstReporte.Items.Add("---- Lista de aristas ----");
            foreach (Arista arista in grafoActual.ObtenerTodasLasAristas())
            {
                string texto = arista.Origen + (grafoActual.EsDirigido ? " -> " : " -- ") + arista.Destino;
                if (arista.Peso > 1)
                    texto += " (peso: " + arista.Peso + ")";
                lstReporte.Items.Add(texto);
            }
        }
    }
}