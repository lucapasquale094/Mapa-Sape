using GMap.NET;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using System;
using System.Collections.Generic;
using SharpKml.Dom;
using SharpKml.Engine;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace mapaGPS
{
    public partial class Form1 : Form
    {
        public GMapOverlay markersOverlay = new GMapOverlay();
        List<PontoModel> pontos = new List<PontoModel>();
        GMarkerGoogle[] marker = new GMarkerGoogle[500];

        public Form1()
        {
            InitializeComponent();

            //Cria o mapa e overlay
            gmap.MapProvider = GMap.NET.MapProviders.BingMapProvider.Instance;
            GMap.NET.GMaps.Instance.Mode = GMap.NET.AccessMode.ServerOnly;
            gmap.Position = new PointLatLng(-21.7270, -55.4018);

            //Le CSV com o horario, intensidade e posição
            LerCSV();

            //Para cada ponto gerar um marker no mapa
            GerarMarkers();

            //Cria poligono base da fazenda
            CriaPoligonos();

            GMapMarker marker = new GMarkerGoogle(new PointLatLng(-21.7270, -55.4018), GMarkerGoogleType.blue_dot);
            markersOverlay.Markers.Add(marker);
            gmap.Overlays.Add(markersOverlay);
        }

        void GerarMarkers()
        {
            markersOverlay.Clear();
            for (int i = 0; i < pontos.Count; i++)
            {
                //Dependendo da intensidade mudar a cor do marker
                GMarkerGoogleType markerColor = GMarkerGoogleType.red;

                if (pontos[i].intensidade >= -70)
                    markerColor = GMarkerGoogleType.green;
                if (pontos[i].intensidade < -70 && pontos[i].intensidade >= -110)
                    markerColor = GMarkerGoogleType.yellow;
                if (pontos[i].intensidade < -110)
                    markerColor = GMarkerGoogleType.red;

                //Inserir um label para cada ponto com a intensidade
                marker[i] = new GMarkerGoogle(new PointLatLng(pontos[i].lat, pontos[i].lng), markerColor);
                marker[i].ToolTip = new GMapToolTip(marker[i]);
                marker[i].ToolTipText = String.Format("Int: {0}\nDist: {1}\nLat: {2}\nLng: {3}", pontos[i].intensidade,
                    distance(pontos[i].lat, pontos[i].lng, -21.7270, -55.4018, 'K').ToString("n3"),
                    pontos[i].lat, pontos[i].lng);
                markersOverlay.Markers.Add(marker[i]);
            }

            gmap.Overlays.Add(markersOverlay);
        }

        void CriaPoligonos()
        {
            GMapOverlay polyOverlay = new GMapOverlay("polygons");
            List<PointLatLng> points = new List<PointLatLng>();
            points.Add(new PointLatLng(-21.72329, -55.43594));
            points.Add(new PointLatLng(-21.70768, -55.43298));
            points.Add(new PointLatLng(-21.69667, -55.41263));
            points.Add(new PointLatLng(-21.74403, -55.35306));
            points.Add(new PointLatLng(-21.74581, -55.38809));
            points.Add(new PointLatLng(-21.74581, -55.38809));
            points.Add(new PointLatLng(-21.73053, -55.40953));
            GMapPolygon polygon = new GMapPolygon(points, "mypolygon");
            polygon.Fill = new SolidBrush(Color.FromArgb(50, Color.Red));
            polygon.Stroke = new Pen(Color.Red, 1);
            polyOverlay.Polygons.Add(polygon);
            gmap.Overlays.Add(polyOverlay);
        }

        void LerCSV()
        {
            var reader = new StreamReader(File.OpenRead(@"C:\Users\lucap\Documents\intensidade.csv"));
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                var values = line.Split(';');

                pontos.Add(new PontoModel()
                {
                    timestamp = values[0],
                    //horario = DateTime.Parse(values[0]),
                    intensidade = Int32.Parse(values[1]),
                    lat = float.Parse(values[2].Replace('.', ',')),
                    lng = float.Parse(values[3].Replace('.', ','))
                });
            }

            label1.Text = "Total de pontos: " + pontos.Count;
        }

        private double distance(double lat1, double lon1, double lat2, double lon2, char unit)
        {
            double theta = lon1 - lon2;
            double dist = Math.Sin(deg2rad(lat1)) * Math.Sin(deg2rad(lat2)) + Math.Cos(deg2rad(lat1)) * Math.Cos(deg2rad(lat2)) * Math.Cos(deg2rad(theta));
            dist = Math.Acos(dist);
            dist = rad2deg(dist);
            dist = dist * 60 * 1.1515;
            if (unit == 'K')
            {
                dist = dist * 1.609344;
            }
            else if (unit == 'N')
            {
                dist = dist * 0.8684;
            }
            return (dist);
        }

        private double deg2rad(double deg)
        {
            return (deg * Math.PI / 180.0);
        }

        private double rad2deg(double rad)
        {
            return (rad / Math.PI * 180.0);
        }
    }
}
