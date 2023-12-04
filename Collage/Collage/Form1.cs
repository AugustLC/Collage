using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Threading;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        //корень иерархического дерева построения коллажа
        public static Node root;
        //переменная для наименования промежуточных картинок коллажа
        public static int i_count;

        public Form1()
        {
            InitializeComponent();

            update();
            create();
        }

/*
0 root c
i root images/1.jpg
r r1 root
c c1 r1
i c1 images/2.jpg
i c1 images/1.jpg
c c2 r1
i c2 images/2.jpg
i c2 images/1.jpg



0 root r
i root images/1.jpg
c c1 root
i c1 images/2.jpg
i c1 images/3.jpg


*/
        

        //обработка введённых команд
        private void update()
        {
            string command;

            command = textBox1.Text;

            var result = command.Split(new[] { '\r', '\n' });

            foreach (string r in result)
            {
                if (r != "")
                {
                    var c = r.Split(new[] { ' ' });
                    if (c[0] == "i")
                    {
                        Zone zoneT = new Zone('i');
                        zoneT.filename = c[2];

                        root.setNode(c[1], "i", zoneT);
                    }
                    if (c[0] == "c")
                    {
                        Zone zoneT = new Zone('c');

                        root.setNode(c[2], c[1], zoneT);
                    }
                    if (c[0] == "r")
                    {
                        Zone zoneT = new Zone('r');

                        root.setNode(c[2], c[1], zoneT);
                    }
                    if (c[0] == "0")
                    {
                        Zone zone = new Zone(c[2][0]);
                        root = new Node(c[1], zone);
                    }
                }
            }

        }

        //рекурсивный поиск узла
        private TreeNode searchNode(TreeNodeCollection node, string key)
        {
            foreach (TreeNode child in node)
            {
                searchNode(child.Nodes, key);
                if (child.Text == key)
                    return child;
            }
            return null;
        }

        //генерация коллажа
        private Zone generate(Node node, char type)
        {
            List<Zone> images = new List<Zone>();
            Zone tempIm;

            foreach (Node n in node.nodes)
            {
                Zone temp = n.node;
                
                if (temp.type == 'i')
                {
                    tempIm = new Zone('0');
                    i_count++;
                    string path = "temp/" + i_count + ".jpg";
                    DrawRow(new List<Zone>() { temp }).Save(path);
                    tempIm.filename = path;
                    images.Add(tempIm);
                }
                else
                {
                    tempIm = new Zone('0');
                    tempIm = generate(n, temp.type);
                    if (tempIm != null) images.Add(tempIm);
                }
                
            }

            if (type == 'c' && images.Count != 0)
            {
                i_count++;
                string path = "temp/" + i_count + ".jpg";
                DrawColumn(images).Save(path);
                tempIm = new Zone('c');
                tempIm.filename = path;
                return tempIm;
            }
            if (type == 'r' && images.Count != 0)
            {
                i_count++;
                string path = "temp/" + i_count + ".jpg";
                DrawRow(images).Save(path);
                tempIm = new Zone('r');
                tempIm.filename = path;
                return tempIm;
            }

            return null;
            
        }

        //создание коллажа, отображение картинки и создание графического файла коллажа
        private void create()
        {
            int width = (int)numericUpDown1.Value;

            Bitmap image = new Bitmap(generate(root, root.node.type).filename);

            float tempScale = (float)image.Width / width;
            int newHeight = (int)(image.Height / tempScale);

            pictureBox1.Width = width;
            pictureBox1.Height = newHeight;

            pictureBox1.Image = new Bitmap(5000, 5000);

            Bitmap bmp;
            bmp = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            pictureBox1.Image = bmp;
            Graphics g = Graphics.FromImage(pictureBox1.Image);

            g.DrawImage(image, 0, 0, width, newHeight);

            string path = "temp/ttt.jpg";
            bmp.Save(path);

            image = null;

            GC.Collect();

            Thread.Sleep(500);

            DirectoryInfo dir = new DirectoryInfo("Temp");
            foreach (FileInfo f in dir.GetFiles())
            {
                if(f.Name != "ttt.jpg")
                    f.Delete();
            }
        }
        

        //изменение ширины экрана
        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            update();
            create();
        }

        //кнопка генерации
        private void generateBatton_Click(object sender, EventArgs e)
        {
            update();
            create();
        }

        //рисование столбца
        public Bitmap DrawColumn(List<Zone> zones)
        {
            Bitmap bmp;
            bmp = new Bitmap(5000, 5000);
            Graphics g = Graphics.FromImage(bmp);

            int wStandart = 0;
            int hStandart = 0;
            int hTempRow = 0;

            if (zones.Count > 0)
            {
                for (int i = 0; i < zones.Count; i++)
                {
                    Zone zone = zones[i];

                    Bitmap image = new Bitmap(zone.filename);
                    
                    if (i == 0)
                    {
                        wStandart = image.Width;
                        hStandart = image.Height;
                        hTempRow = 0;
                    }

                    float tempScale = (float)image.Width / wStandart;
                    int newHeight = (int)(image.Height / tempScale);

                    g.DrawImage(image, 0, hTempRow, wStandart, newHeight);

                    hTempRow += newHeight;
                }

                return bmp.Clone(new Rectangle(0, 0, wStandart, hTempRow), bmp.PixelFormat);
            }

            return null;
        }

        //рисование строки
        public Bitmap DrawRow(List<Zone> zones)
        {
            Bitmap bmp;
            bmp = new Bitmap(5000, 5000);
            Graphics g = Graphics.FromImage(bmp);

            int wStandart = 0;
            int hStandart = 0;
            int wTempRow = 0;
            
            if (zones.Count > 0)
            {
                for (int i = 0; i < zones.Count; i++ )
                {
                    Zone zone = zones[i];

                    Bitmap image = new Bitmap(zone.filename);

                    if (i == 0)
                    {
                        wStandart = image.Width;
                        hStandart = image.Height;
                        wTempRow = 0;
                    }

                    float tempScale = (float)image.Height / hStandart;
                    int newWidth = (int)(image.Width / tempScale);

                    g.DrawImage(image, wTempRow, 0, newWidth, hStandart);

                    wTempRow += newWidth;

                }

                return bmp.Clone(new Rectangle(0, 0, wTempRow, hStandart), bmp.PixelFormat);
            }
            
            return null;
        }

    }


    //класс зоны для расчёта размеров промежуточных картинок коллажа
    public class Zone
    {
        public Zone(char t) { type = t; }

        public Zone(char t, int _x, int _y, int _w, int _h)
        {
            type = t;
            x = _x;
            y = _y;
            w = _w;
            h = _h;
        }

        public char type;
        public string filename;
        public int x;
        public int y;
        public int w;
        public int h;
    }

    //класс узла иерархического дерева для создания коллажа
    public class Node
    {
        public Node(string _name, Zone _node) 
        {
            node = _node;
            nodes = new List<Node>();
            name = _name;
            node = _node; 
        }

        public string name;

        public Zone node;

        public List<Node> nodes;

        public void setNode(string _name, Zone _node)
        {
            Node n = new Node(_name, _node);
            nodes.Add(n);
        }

        public void setNode(string parent, string _name, Zone _node)
        {
            foreach (Node n in nodes)
            {
                n.setNode(parent, _name, _node);
            }
            if (name == parent)
            {
                Node n1 = new Node(_name, _node);
                nodes.Add(n1);
            }
        }

        public Node getNode(string _key)
        {
            if (name == _key) return this;
            return getNode(_key,  nodes);
        }

        public Node getNode(string _key, List<Node> nodes)
        {
            
            foreach (Node n in nodes)
            {
                getNode(_key, n.nodes);
                if (n.name == _key)
                {
                    return n;
                }
            }

            return null;
        }


        public void output()
        {
            MessageBox.Show(node.type.ToString());
            foreach (Node n in nodes)
            {
                n.output();
            }
        }

    }
}
