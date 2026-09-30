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