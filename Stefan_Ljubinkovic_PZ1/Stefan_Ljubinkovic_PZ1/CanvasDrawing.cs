using Stefan_Ljubinkovic_PZ1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Xml;

namespace Stefan_Ljubinkovic_PZ1
{
    public class CanvasDrawing
    {
        public Block[,] Grid { get; set; }
        public List<SubstationEntity> Substations { get; set; }
        public List<SwitchEntity> Switches { get; set; }
        public List<NodeEntity> Nodes { get; set; }
        public List<LineEntity> Lines { get; set; }
		Dictionary<LineEntity, Polyline> coverLines { get; set; } = new Dictionary<LineEntity, Polyline>();

		List<Ellipse> switches { get; set; } = new List<Ellipse>();
        public List<Point> UsedPoints { get; set; }

		private readonly static List<int> dr = new List<int> { -1, +1, 0, 0 };
		private readonly static List<int> dc = new List<int> { 0, 0, +1, -1 };

		public CanvasDrawing()
        {
            Grid = new Block[501, 501];

            Substations = new List<SubstationEntity>();

            Switches = new List<SwitchEntity>();

            Nodes = new List<NodeEntity>();

            Lines = new List<LineEntity>();

            UsedPoints = new List<Point>();

		}

		

		public void Init()
        {
			for (int i = 0; i < 501; i++)
			{
				for (int j = 0; j < 501; j++)
				{
					Grid[i, j] = new Block(i * 10, j * 10, new PowerEntity(), EBlockType.EMPTY);
				}
			}
		}

		public void ChangeSwitches()
        {
            foreach (var item in switches)
            {
                if (item.ToolTip.ToString().Contains("Open"))
                {
					item.ToolTip = item.ToolTip.ToString().Replace("Open", "Closed");
				}
				else
				{
					item.ToolTip  = item.ToolTip.ToString().Replace("Closed", "Open");
				}
			}
			foreach (var item in Switches)
			{
				if (item.Status=="Open")
				{
					item.Status = "Closed";
				}
				else
				{
					item.Status =  "Open";
				}
			}

		}

		public void ChangeColour(SolidColorBrush brush , ImageBrush image)
        {
			if(brush != null)
            {
				foreach (var item in switches)
				{
					item.Fill = brush;
				}
            }
			if (image != null)
			{
				foreach (var item in switches)
				{
					item.Fill = image;
				}
			}
				
		}



		List<Polyline> backLines { get; set; }

		public void notActiveNetwork()
        {
			List<long> ends = new List<long>();
            backLines = new List<Polyline>(); 
			foreach (var item in coverLines)
                {
					
					try
					{
						if (Switches.Where(x => x.Id == item.Key.FirstEnd).First().Status == "Open")
						{
							
							long second = Switches.Where(x => x.Id == item.Key.SecondEnd).First().Id;
							item.Value.Stroke = Brushes.Transparent;
							ends.Add(second);
							backLines.Add(item.Value);
						}
					} catch (Exception) { }
					
				}

                foreach (var item in coverLines)
                {
					try
					{

						if (ends.Contains(item.Key.SecondEnd))
						{
							item.Value.Stroke = Brushes.Transparent;
							backLines.Add(item.Value);
						}
					}
					catch (Exception) { }
                }
            
     
            
        }
		
		public void BackLines()
        {
            foreach (var item in backLines)
            {
				item.Stroke = Brushes.Black;
            }
        }

		public void Load(string path)
		{
			XmlDocument xmlDoc = new XmlDocument();

			xmlDoc.Load(path);

			double latLonX, latLonY;

			foreach (XmlNode n in xmlDoc.SelectNodes("/NetworkModel/Substations/SubstationEntity"))
			{
				ToLatLon(double.Parse(n.SelectSingleNode("X").InnerText), double.Parse(n.SelectSingleNode("Y").InnerText), 34, out latLonX, out latLonY);

				SubstationEntity temp = new SubstationEntity()
				{
					Id = long.Parse(n.SelectSingleNode("Id").InnerText),
					Name = n.SelectSingleNode("Name").InnerText,
					X = latLonX,
					Y = latLonY
				};

				Substations.Add(temp);
			}

			foreach (XmlNode n in xmlDoc.SelectNodes("/NetworkModel/Switches/SwitchEntity"))
			{
				ToLatLon(double.Parse(n.SelectSingleNode("X").InnerText), double.Parse(n.SelectSingleNode("Y").InnerText), 34, out latLonX, out latLonY);

				SwitchEntity temp = new SwitchEntity()
				{
					Id = long.Parse(n.SelectSingleNode("Id").InnerText),
					Status = n.SelectSingleNode("Status").InnerText,
					Name = n.SelectSingleNode("Name").InnerText,
					X = latLonX,
					Y = latLonY
				};

				Switches.Add(temp);
			}

			foreach (XmlNode n in xmlDoc.SelectNodes("/NetworkModel/Nodes/NodeEntity"))
			{
				ToLatLon(double.Parse(n.SelectSingleNode("X").InnerText), double.Parse(n.SelectSingleNode("Y").InnerText), 34, out latLonX, out latLonY);

				NodeEntity temp = new NodeEntity()
				{
					Id = long.Parse(n.SelectSingleNode("Id").InnerText),
					Name = n.SelectSingleNode("Name").InnerText,
					X = latLonX,
					Y = latLonY
				};

				Nodes.Add(temp);
			}

			foreach (XmlNode n in xmlDoc.SelectNodes("/NetworkModel/Lines/LineEntity"))
			{

				LineEntity l = new LineEntity()
				{
					ConductorMaterial = n.SelectSingleNode("ConductorMaterial").InnerText,
					FirstEnd = long.Parse(n.SelectSingleNode("FirstEnd").InnerText),
					Id = long.Parse(n.SelectSingleNode("Id").InnerText),
					IsUnderground = bool.Parse(n.SelectSingleNode("IsUnderground").InnerText),
					LineType = n.SelectSingleNode("LineType").InnerText,
					Name = n.SelectSingleNode("Name").InnerText,
					R = float.Parse(n.SelectSingleNode("R").InnerText),
					SecondEnd = long.Parse(n.SelectSingleNode("SecondEnd").InnerText),
					ThermalConstantHeat = long.Parse(n.SelectSingleNode("ThermalConstantHeat").InnerText)
				};

				bool suprotni_smer = false;

				foreach (LineEntity line in Lines)
				{
					if ((l.FirstEnd == line.SecondEnd && l.SecondEnd == line.FirstEnd) || (line.FirstEnd == l.FirstEnd && line.SecondEnd == l.SecondEnd))
					{
						suprotni_smer = true;
						break;
					}
				}
				if (suprotni_smer == false)
				{
					Lines.Add(l);
				}
			}
		}


		public void ScaleToCanvas(double canvasWidth, double canvasHeight)
		{
			Tuple<double, double, double, double> size = FindCanvasSizeInBlocks(canvasWidth, canvasHeight);

			double sizeX = size.Item1;
			double sizeY = size.Item2;
			double minX = size.Item3;
			double minY = size.Item4;

			foreach (SubstationEntity sub in Substations)
			{
				double x = Math.Round((sub.X - minX) * sizeX / 10) * 10;
				double y = Math.Round((sub.Y - minY) * sizeY / 10) * 10;
				Point p = FindUnusedPoint(x, y);
				sub.X = p.X;
				sub.Y = p.Y;
				Grid[(int)p.X / 10, (int)p.Y / 10] = new Block((int)p.X, (int)p.Y, new SubstationEntity(), EBlockType.ENTITY);


			}

			foreach (NodeEntity n in Nodes)
			{
				double x = Math.Round((n.X - minX) * sizeX / 10) * 10;
				double y = Math.Round((n.Y - minY) * sizeY / 10) * 10;
				Point p = FindUnusedPoint(x, y);
				n.X = p.X;
				n.Y = p.Y;
				Grid[(int)p.X / 10, (int)p.Y / 10] = new Block((int)p.X, (int)p.Y, new NodeEntity(), EBlockType.ENTITY);
			}

			foreach (SwitchEntity sw in Switches)
			{
				double x = Math.Round((sw.X - minX) * sizeX / 10) * 10;
				double y = Math.Round((sw.Y - minY) * sizeY / 10) * 10;
				Point p = FindUnusedPoint(x, y);
				sw.X = p.X;
				sw.Y = p.Y;
				Grid[(int)p.X / 10, (int)p.Y / 10] = new Block((int)p.X, (int)p.Y, new SwitchEntity(), EBlockType.ENTITY);
			}
		}




		public Tuple<double, double, double, double> FindCanvasSizeInBlocks(double canvasWidth, double canvasHeight)
		{

			List<PowerEntity> all = new List<PowerEntity>();

			foreach (SubstationEntity sub in Substations)
			{
				all.Add(sub);
			}

			foreach (SwitchEntity sw in Switches)
			{
				all.Add(sw);
			}

			foreach (NodeEntity n in Nodes)
			{
				all.Add(n);
			}

			double minX = all.Min(x => x.X);
			double maxX = all.Max(x => x.X);

			double minY = all.Min(x => x.Y);
			double maxY = all.Max(x => x.Y);

			double sizeX = (canvasWidth / 2) / (maxX - minX);
			double sizeY = (canvasHeight / 2) / (maxY - minY);

			return new Tuple<double, double, double, double>(sizeX, sizeY, minX, minY);

		}

		public Point FindUnusedPoint(double x, double y)
		{
			bool pointUsed = PointUsed(x, y);

			if (pointUsed == false)
			{
				Point p = new Point();
				p.X = x;
				p.Y = y;
				UsedPoints.Add(p);
				return new Point() { X = x * 2, Y = y * 2 };

			}
			double closestX = x - 10;
			closestX = (closestX < 0) ? closestX + 10 : closestX;
			double closestY = y - 10;
			closestY = (closestY < 0) ? closestY + 10 : closestY;

			while (PointUsed(closestX, closestY))
			{
				for (int i = 0; i < 2; i++)
				{
					for (int j = 0; j < 2; j++)
					{
						if (!PointUsed(closestX, closestY))
						{
							UsedPoints.Add(new Point() { X = closestX, Y = closestY });
							return new Point() { X = closestX * 2, Y = closestY * 2 };
						}
						closestY = closestY + 10;
					}
					if (!PointUsed(closestX, closestY))
					{
						UsedPoints.Add(new Point() { X = closestX, Y = closestY });
						return new Point() { X = closestX * 2, Y = closestY * 2 };
					}
					closestX = closestX + 10;
					closestY = closestY - 2 * 10;
				}

			}

			UsedPoints.Add(new Point() { X = closestX, Y = closestY });
			return new Point() { X = closestX * 2, Y = closestY * 2 };
		}


		public bool PointUsed(double x, double y)
		{
			foreach (Point p in UsedPoints)
			{
				if (p.X == x && p.Y == y)
				{
					return true;
				}
			}
			return false;
		}
		public void DrawSubNSw(Canvas canvas)
		{
			foreach (SubstationEntity sub in Substations)
			{
				Ellipse e = new Ellipse() { Height = 10, Width = 10, Fill = Brushes.Blue };
				e.ToolTip = "ID:" + sub.Id + "\nName: " + sub.Name;
				sub.Ellipse = e;
				Canvas.SetLeft(sub.Ellipse, sub.X);
				Canvas.SetTop(sub.Ellipse, sub.Y);
				canvas.Children.Add(sub.Ellipse);
			}

			foreach (SwitchEntity sw in Switches)
			{
				Ellipse e = new Ellipse() { Height = 10, Width = 10, Fill = Brushes.Green };
				ImageBrush imageBrush = new ImageBrush();
				imageBrush.ImageSource = new System.Windows.Media.Imaging.BitmapImage(new Uri("images.png", UriKind.Relative));
				e.Fill = imageBrush;
				e.ToolTip = "ID:" + sw.Id + "\nName: " + sw.Name +"Status :" + sw.Status;
				sw.Ellipse = e;
				switches.Add(e);
				Canvas.SetLeft(sw.Ellipse, sw.X);
				Canvas.SetTop(sw.Ellipse, sw.Y);
				canvas.Children.Add(sw.Ellipse);
			}

			foreach (NodeEntity n in Nodes)
			{
				Ellipse e = new Ellipse() { Height = 10, Width = 10, Fill = Brushes.Red };
				e.ToolTip = "ID:" + n.Id + "\nName: " + n.Name;
				n.Ellipse = e;
				Canvas.SetLeft(n.Ellipse, n.X);
				Canvas.SetTop(n.Ellipse, n.Y);
				canvas.Children.Add(n.Ellipse);
			}
		}

		public void DrawLines(Canvas canvas)
		{
			List<Tuple<double, double, double, double, PowerEntity, PowerEntity, LineEntity>> lines = GetLines();


			foreach (Tuple<double, double, double, double, PowerEntity, PowerEntity, LineEntity> line in lines)
			{
				PowerEntity startEntity = line.Item5, endEntity = line.Item6;
				double x1 = line.Item1, y1 = line.Item2, x2 = line.Item3, y2 = line.Item4;
				LineEntity lineEnt = line.Item7;

				if ((x1 == -1 || x2 == -1 || y1 == -1 || y2 == -1) || (x1 == x2 && y1 == y2))
				{
					continue;
				}

				List<Block> lineBlocks = BFSAlgorythm(x1, y1, x2, y2, false); 

				if (lineBlocks.Count < 2)
				{
					lineBlocks = BFSAlgorythm(x1, y1, x2, y2, true);
				}

				Polyline polyline = new Polyline();

				polyline.Stroke = new SolidColorBrush(Colors.Black);
				polyline.StrokeThickness = 2;

				for (int i = 0; i < lineBlocks.Count; i++)
				{
					if (Grid[lineBlocks[i].X / 10, lineBlocks[i].Y / 10].Type == EBlockType.LINE)
					{
						Grid[lineBlocks[i].X / 10, lineBlocks[i].Y / 10].Type = EBlockType.CROSS;

					}
					else if (Grid[lineBlocks[i].X / 10, lineBlocks[i].Y / 10].Type == EBlockType.EMPTY)
					{
						Grid[lineBlocks[i].X / 10, lineBlocks[i].Y / 10].Type = EBlockType.LINE;
					}


					System.Windows.Point linePoint = new System.Windows.Point(lineBlocks[i].X + 5, lineBlocks[i].Y + 5);
					polyline.Points.Add(linePoint);



				}
				
				polyline.MouseRightButtonDown += ResetColors;
				polyline.MouseRightButtonDown += endEntity.OnClick;
				polyline.MouseRightButtonDown += startEntity.OnClick;

				polyline.ToolTip = "ID: " + lineEnt.Id + "\nName: " + lineEnt.Name;
				coverLines.Add(lineEnt, polyline);
				canvas.Children.Add(polyline);


			}
		}





		public List<Block> BFSAlgorythm(double x1, double y1, double x2, double y2, bool cross)
		{
			List<Block> path = new List<Block>();
			Queue<Block> queue = new Queue<Block>();
			bool pathFound = false;
			Block[,] visited = new Block[Grid.GetLength(0), Grid.GetLength(1)];

			x1 /= 10;
			x2 /= 10;
			y1 /= 10;
			y2 /= 10;

			int X1 = (int)x1;
			int X2 = (int)x2;
			int Y1 = (int)y1;
			int Y2 = (int)y2;


			Block start = Grid[X1, Y1];
			Block end = Grid[X2, Y2];
			visited[X1, Y1] = start;
			queue.Enqueue(start);



			while (queue.Count > 0)
			{
				Block temp = queue.Dequeue();
				if (temp.X / 10 == X2 && temp.Y / 10 == Y2)
				{
					pathFound = true;
					break;
				}
				for (int i = 0; i < 4; i++)
				{
					int nextX = temp.X / 10 + dr[i];
					int nextY = temp.Y / 10 + dc[i];

					if (nextX < 0 || nextY < 0 || nextX >= Grid.GetLength(0) || nextY >= Grid.GetLength(1))
					{
						continue;
					}

					if (visited[nextX, nextY] != null) 
					{
						continue;
					}

					if (!(nextX == end.X / 10 && nextY == end.Y / 10) && (Grid[nextX, nextY].Type != EBlockType.EMPTY) && cross == false)
					{
						continue;
					}


					if (!(nextX == end.X / 10 && nextY == end.Y / 10) && (Grid[nextX, nextY].Type == EBlockType.ENTITY) && cross == true)
					{
						continue;
					}

					queue.Enqueue(Grid[nextX, nextY]);
					visited[nextX, nextY] = temp; 
				}
			}

			if (pathFound)
			{
				path.Add(end);
				Block previousBlock = visited[end.X / 10, end.Y / 10];
				while (previousBlock.X / 10 > 0 && !(previousBlock.X / 10 == start.X / 10 && previousBlock.Y / 10 == start.Y / 10))
				{

					if (Grid[previousBlock.X / 10, previousBlock.Y / 10].Type == EBlockType.EMPTY)
					{
						Grid[previousBlock.X / 10, previousBlock.Y / 10].Type = EBlockType.LINE;

					}
					path.Add(previousBlock);
					previousBlock = visited[previousBlock.X / 10, previousBlock.Y / 10];
				}
				path.Add(previousBlock);
			}
			return path;

		}


		public List<Tuple<double, double, double, double, PowerEntity, PowerEntity, LineEntity>> GetLines()
		{
			List<Tuple<double, double, double, double, PowerEntity, PowerEntity, LineEntity>> list = new List<Tuple<double, double, double, double, PowerEntity, PowerEntity, LineEntity>>();


			List<PowerEntity> allEntites = new List<PowerEntity>();

			foreach (SubstationEntity sub in Substations)
			{
				allEntites.Add(sub);
			}

			foreach (SwitchEntity sw in Switches)
			{
				allEntites.Add(sw);
			}

			foreach (NodeEntity n in Nodes)
			{
				allEntites.Add(n);
			}

			foreach (LineEntity line in Lines)
			{
				PowerEntity startEntity = new PowerEntity();
				PowerEntity endEntity = new PowerEntity();
				double x1 = -1, y1 = -1, x2 = -1, y2 = -1;

				foreach (PowerEntity temp in allEntites)
				{
					if (temp.Id == line.FirstEnd)
					{
						x1 = temp.X;
						y1 = temp.Y;
						startEntity = temp;
					}
					if (temp.Id == line.SecondEnd)
					{
						x2 = temp.X;
						y2 = temp.Y;
						endEntity = temp;
					}
				}

				Tuple<double, double, double, double, PowerEntity, PowerEntity, LineEntity> tuple = new Tuple<double, double, double, double, PowerEntity, PowerEntity, LineEntity>(x1, y1, x2, y2, startEntity, endEntity, line);

				list.Add(tuple);
			}

			return list;
		}


		private void ResetColors(object sender, EventArgs e)
		{
			List<PowerEntity> allEntites = new List<PowerEntity>();

			foreach (SubstationEntity sub in Substations)
			{
				sub.ResetColor();
			}

			foreach (SwitchEntity sw in Switches)
			{
				sw.ResetColor();
			}

			foreach (NodeEntity n in Nodes)
			{
				n.ResetColor();
			} 
		}




		//From UTM to Latitude and longitude in decimal
		public static void ToLatLon(double utmX, double utmY, int zoneUTM, out double latitude, out double longitude)
		{
			bool isNorthHemisphere = true;

			var diflat = -0.00066286966871111111111111111111111111;
			var diflon = -0.0003868060578;

			var zone = zoneUTM;
			var c_sa = 6378137.000000;
			var c_sb = 6356752.314245;
			var e2 = Math.Pow((Math.Pow(c_sa, 2) - Math.Pow(c_sb, 2)), 0.5) / c_sb;
			var e2cuadrada = Math.Pow(e2, 2);
			var c = Math.Pow(c_sa, 2) / c_sb;
			var x = utmX - 500000;
			var y = isNorthHemisphere ? utmY : utmY - 10000000;

			var s = ((zone * 6.0) - 183.0);
			var lat = y / (c_sa * 0.9996);
			var v = (c / Math.Pow(1 + (e2cuadrada * Math.Pow(Math.Cos(lat), 2)), 0.5)) * 0.9996;
			var a = x / v;
			var a1 = Math.Sin(2 * lat);
			var a2 = a1 * Math.Pow((Math.Cos(lat)), 2);
			var j2 = lat + (a1 / 2.0);
			var j4 = ((3 * j2) + a2) / 4.0;
			var j6 = ((5 * j4) + Math.Pow(a2 * (Math.Cos(lat)), 2)) / 3.0;
			var alfa = (3.0 / 4.0) * e2cuadrada;
			var beta = (5.0 / 3.0) * Math.Pow(alfa, 2);
			var gama = (35.0 / 27.0) * Math.Pow(alfa, 3);
			var bm = 0.9996 * c * (lat - alfa * j2 + beta * j4 - gama * j6);
			var b = (y - bm) / v;
			var epsi = ((e2cuadrada * Math.Pow(a, 2)) / 2.0) * Math.Pow((Math.Cos(lat)), 2);
			var eps = a * (1 - (epsi / 3.0));
			var nab = (b * (1 - epsi)) + lat;
			var senoheps = (Math.Exp(eps) - Math.Exp(-eps)) / 2.0;
			var delt = Math.Atan(senoheps / (Math.Cos(nab)));
			var tao = Math.Atan(Math.Cos(delt) * Math.Tan(nab));

			longitude = ((delt * (180.0 / Math.PI)) + s) + diflon;
			latitude = ((lat + (1 + e2cuadrada * Math.Pow(Math.Cos(lat), 2) - (3.0 / 2.0) * e2cuadrada * Math.Sin(lat) * Math.Cos(lat) * (tao - lat)) * (tao - lat)) * (180.0 / Math.PI)) + diflat;
		}
	}



}

