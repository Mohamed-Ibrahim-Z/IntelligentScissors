using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ConsoleApp2;

namespace IntelligentScissors
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        RGBPixel[,] ImageMatrix;
        Boolean isLoaded = false;
        Boolean isFinished = false;
        int isClicked = 0;

        int mouseX = 0;
        int mouseY = 0;
        
        Dictionary<Tuple<int,int>, List<Node>> wightedGraph;
        List<Tuple<int, int>> points = new List<Tuple<int, int>>();
        List<List<Tuple<int, int>>> pathes = new List<List<Tuple<int, int>>>();
        int width;
        int height ;

        private void btnOpen_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                //Open the browsed image and display it
                string OpenedFilePath = openFileDialog1.FileName;
                ImageMatrix = ImageOperations.OpenImage(OpenedFilePath);
                ImageOperations.DisplayImage(ImageMatrix, pictureBox1);
                isLoaded = true;
            }
            txtWidth.Text = ImageOperations.GetWidth(ImageMatrix).ToString();
            txtHeight.Text = ImageOperations.GetHeight(ImageMatrix).ToString();

            width = Convert.ToInt32(txtWidth.Text);
            height = Convert.ToInt32(txtHeight.Text);
            // Start of the code
            //Graph
            wightedGraph = new Dictionary<Tuple<int, int>, List<Node>>();
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    Tuple<int, int> vertex = new Tuple<int, int>(i,j);

                    int index = i * width + j;
                    


                    wightedGraph[vertex] = new List<Node>();

                    if (j != width - 1)
                    {
                        double weightRight = 1/ImageOperations.CalculatePixelEnergies(j, i, ImageMatrix).X;
                        wightedGraph[vertex].Add(new Node(i,j+1,double.IsInfinity(weightRight) ? 1e16 : weightRight,null));
                    }
                    if(j != 0)
                    {

                        double weightLeft = 1/ImageOperations.CalculatePixelEnergies(j-1, i, ImageMatrix).X;
                        wightedGraph[vertex].Add(new Node(i,j-1 , double.IsInfinity(weightLeft) ? 1e16 : weightLeft, null));
                    }
                    if(i != 0)
                    {
                        
                        double weightUp = 1/ImageOperations.CalculatePixelEnergies(j, i-1, ImageMatrix).Y;
                        wightedGraph[vertex].Add(new Node(i-1,j, double.IsInfinity(weightUp) ? 1e16 : weightUp, null));
                    }
                    if(i != height - 1)
                    {
                        double weightDown = 1/ImageOperations.CalculatePixelEnergies(j, i, ImageMatrix).Y;
                        wightedGraph[vertex].Add(new Node(i+1,j, double.IsInfinity(weightDown) ? 1e16 : weightDown, null));
                    }

                }
            }

        }



        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {

            if (!isLoaded || isFinished)
                return;
            

            
            isClicked++;

            Console.WriteLine("X: " + e.X + " Y: " + e.Y);





            if (e.Button == MouseButtons.Right)
            {
                isClicked--;
                points.RemoveAt(points.Count-1);
                pathes.RemoveAt(pathes.Count - 1);
            }
            else
            if (isClicked >= 1)
            {
                Tuple<int, int> click = new Tuple<int, int>(e.Y, e.X);
                points.Add(click);
                if(isClicked > 1)
                    pathes.Add(Dijkstra(points[points.Count - 2], points[points.Count - 1]));
                
                
            }



            //if(isClicked >1)
            //{
            //    Dijkstra(points[points.Count - 2].X * Convert.ToInt32(txtHeight.Text) + points[points.Count - 2].Y,
            //        points[points.Count - 1].X * Convert.ToInt32(txtHeight.Text) + points[points.Count - 1].Y);
            //}

            mouseX = e.X;
            mouseY = e.Y;


        }
        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (!isLoaded || isFinished)
                return;

            textBox1.Text = e.X.ToString();
            textBox2.Text = e.Y.ToString();

            Pen p = new Pen(Color.Red, 1.0f);
            textBox3.Text = e.X.ToString();
            textBox4.Text = e.Y.ToString();
            //textBox5.Text = wightedGraph[vector2][2].ToString();
            //textBox6.Text = wightedGraph[vector2][3].ToString();
            if (isClicked >= 1 )
            {
                using (Graphics g = pictureBox1.CreateGraphics())
                {
                    Refresh();
                    foreach (var node in points)
                        g.DrawEllipse(new Pen(Color.Red, 4), node.Item2, node.Item1, 1, 1);

                    foreach (var currentPath in pathes)
                    {
                        foreach(var node in currentPath)
                        {

                            g.DrawEllipse(p, node.Item2, node.Item1,1,1);
                        }
                    }
                    if (points.Count > 0)
                    {
                        List<Tuple<int, int>> ayAsm = Dijkstra(points[points.Count - 1], new Tuple<int, int>(e.Y, e.X));
                        foreach (var node in ayAsm)
                        {
                            g.DrawEllipse(p, node.Item2, node.Item1, 1, 1);
                        }
                    }
                }
            }
        }

        public List<Tuple<int,int>> Dijkstra(Tuple<int,int> src, Tuple<int, int> dist)
        {
            Console.WriteLine("Src" + src + "Dist" + dist);
            bool[,] visted = new bool[height, width];
            Dictionary < Tuple <int,int>, double> distance = new Dictionary<Tuple<int, int>, double>();
            Dictionary<Tuple<int, int>, Tuple<int, int>> parent = new Dictionary<Tuple<int, int>, Tuple<int, int>>();

            parent[src] = null;
            PrQueue prQueue = new PrQueue();
            List<Tuple<int,int>> path = new List<Tuple<int, int>>();


            prQueue.Enqueue(new Node(src.Item1,src.Item2, 0, null),0);
            distance[src] = 0;



            Node child = new Node();
            while (prQueue.Count() != 0)
            {
                Node node = prQueue.Dequeue();
                Tuple<int, int> vertex = new Tuple<int, int>(node.x, node.y);
                visted[node.x,node.y] = true;

                if (node.x == dist.Item1 && node.y == dist.Item2)
                {
                    Tuple<int, int> temp = new Tuple<int, int>(node.x, node.y);
                    while (parent[temp] != null)
                    {
                        path.Add(temp);
                        
                        temp = parent[temp];
                    }
                    path.Add(temp);
                    break;
                }
                
                foreach (var neighboor in wightedGraph[new Tuple<int,int>(node.x,node.y)])
                {
                    Tuple<int, int> tuple = new Tuple<int, int>(neighboor.x, neighboor.y);
                    if (!visted[tuple.Item1,tuple.Item2])
                    {
                        if(!distance.ContainsKey(tuple))
                        {
                            distance.Add(tuple,  distance[vertex] + neighboor.weight);
                            parent[tuple] = vertex;
                            prQueue.Enqueue(neighboor, distance[tuple]);
                        }
                        if (distance[vertex] + neighboor.weight < distance[tuple])
                        {
                            distance[tuple] =  distance[vertex] + neighboor.weight;
                            parent[tuple] = vertex;
                            prQueue.Enqueue(neighboor, distance[tuple]);
                        }

                    }

                }
                
                
            }

            return path;



        }
        
        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            if (!isLoaded)
                return;

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            isFinished = true;
            Pen p = new Pen(Color.Blue, 1.0f);

            using (Graphics g = pictureBox1.CreateGraphics())
            {
                Refresh();
                //List<Tuple<int, int>> ayAsm = Dijkstra(points[points.Count - 1], points[0]);
                foreach(var path in pathes)
                    foreach(var node in path)
                    g.DrawEllipse(p, node.Item2, node.Item1, 1, 1);

                g.DrawLine(p, points[points.Count - 1].Item2, points[points.Count - 1].Item1, points[0].Item2, points[0].Item1);
            }
        }
    }
}