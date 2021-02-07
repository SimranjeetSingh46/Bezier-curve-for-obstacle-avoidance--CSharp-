using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.DataVisualization.Charting;
using System.Windows.Forms;
using System.IO;



namespace Motion_Primitive
{
    public partial class Form1 : Form

    {
        public Form1()
        {
            InitializeComponent();
        }
        // Data point of double data type 
        public struct DPoint
        {
            public DPoint(Double x, Double y)
                : this()
            {
                this.X = x;
                this.Y = y;
            }
            public Double X { get; set; }
            public Double Y { get; set; }
            public static implicit operator DPoint(Point p)
            {
                return new DPoint(p.X, p.Y);
            }
        }
        // Convertor function
        public class GlobalMercator
        {
            public Int32 TileSize = 256;
            private const Int32 EarthRadius = 6378137;
            public Double InitialResolution = 2 * Math.PI * EarthRadius / 256;
            public Double OriginShift = 2 * Math.PI * EarthRadius / 2;
            public struct DPoint
            {
                public DPoint(Double x, Double y)
                    : this()
                {
                    this.X = x;
                    this.Y = y;
                }
                public Double X { get; set; }
                public Double Y { get; set; }
                public static implicit operator DPoint(Point p)
                {
                    return new DPoint(p.X, p.Y);
                }
            }
            public Double Resolution(Int32 zoom)
            {
                return InitialResolution / (Math.Pow(zoom, 2));
            }
            //public DPoint PixelsToMeters(DPoint p, Int32 zoom)
            public DPoint PixelsToMeters(DPoint p)

            {
                var zoom = 3;
                //var res = Resolution(zoom);
                var res = InitialResolution / (Math.Pow(zoom, 2));
                var met = new DPoint();
                met.X = p.X * res - OriginShift;
                met.Y = p.Y * res - OriginShift;
                return met;
            }

        }
        // Bezier Equation for X Coordinate 
        public static float X(float t, float x0, float x1, float x2, float x3)
        {
            return (float)(
                x0 * Math.Pow((1 - t), 3) +
                x1 * 3 * t * Math.Pow((1 - t), 2) +
                x2 * 3 * Math.Pow(t, 2) * (1 - t) +
                x3 * Math.Pow(t, 3)
            );
        }
        // Bezier Equation for Y Coordinate 
        public static float Y(float t, float y0, float y1, float y2, float y3)
        {
            return (float)(
                y0 * Math.Pow((1 - t), 3) +
                y1 * 3 * t * Math.Pow((1 - t), 2) +
                y2 * 3 * Math.Pow(t, 2) * (1 - t) +
                y3 * Math.Pow(t, 3)
            );
        }
        // Draw the Bezier curve. 
        public static void DrawBezier(Graphics gr, Pen the_pen, float dt, PointF pt0, PointF pt1, PointF pt2, PointF pt3)
        {
            // Draw the curve.
            List<PointF> points = new List<PointF>();

            for (float t = 0.0f; t < 1.0; t += dt)
            {
                points.Add(new PointF(
                    X(t, pt0.X, pt1.X, pt2.X, pt3.X),
                    Y(t, pt0.Y, pt1.Y, pt2.Y, pt3.Y)));
            }

            // Connect to the final point.
            points.Add(new PointF( X(1.0f, pt0.X, pt1.X, pt2.X, pt3.X),
                                   Y(1.0f, pt0.Y, pt1.Y, pt2.Y, pt3.Y)));

            // Draw the curve.
            gr.DrawLines(the_pen, points.ToArray());
            //Pen yellow = new Pen(Color.Yellow, 7);
            //Pen pink = new Pen(Color.Pink, 7);
            //Pen orange = new Pen(Color.Orange, 7);


            // Draw lines connecting the control points.
            //gr.DrawLine(yellow, pt0, pt1);
            //gr.DrawLine(pink, pt1, pt2);
            //gr.DrawLine(orange, pt2, pt3);


        }
       // Bezier Equation_Chart for X Coordinate
        public static double XChart(double t, double x0, double x1, double x2, double x3)
        {
            return (double)(
                x0 * Math.Pow((1 - t), 3) +
                x1 * 3 * t * Math.Pow((1 - t), 2) +
                x2 * 3 * Math.Pow(t, 2) * (1 - t) +
                x3 * Math.Pow(t, 3)
            );
        }
        // Added by intelegence c#
        public static double[] XChart(double x0, double x1, double x2, double x3, int totalPoints)
        {
            List<double> points = new List<double>();
            for (float t = 0.0f; t < 1.0f; t += (1 / (float)totalPoints))
                points.Add(XChart(t, x0, x1, x2, x3));

            return points.ToArray();
        }
        // Bezier Equation_Chart for Y Coordinate
        private static double YChart(double t, double yValues1, double yValues2, double yValues3, double yValues4)
        {
            return (double)(
               yValues1 * Math.Pow((1 - t), 3) +
               yValues2 * 3 * t * Math.Pow((1 - t), 2) +
               yValues3 * 3 * Math.Pow(t, 2) * (1 - t) +
               yValues4 * Math.Pow(t, 3)
           );
        }
        // Added by intelegence c#
        public static double[] YChart(double x0, double x1, double x2, double x3, int totalPoints)
        {
            List<double> points = new List<double>();
            for (float t = 0.0f; t < 1.0f; t += (1 / (float)totalPoints))
                points.Add(XChart(t, x0, x1, x2, x3));

            return points.ToArray();
        }
        //Draw the Bezier_chart curve
        public static void DrawChartBezier(Graphics gr, Pen the_pen, double dt, DataPoint pt0, DataPoint pt1, DataPoint pt2, DataPoint pt3)
        { 
            List<DataPoint> dataPoints = new List<DataPoint>();
            List<DPoint> dPoints = new List<DPoint>();

            for (double t = 0.0f; t < 1.0; t += dt)
            {
                dataPoints.Add(new DataPoint(
                    XChart(t, pt0.XValue, pt1.XValue, pt2.XValue, pt3.XValue),
                    YChart(t, pt0.YValues[0],pt1.YValues[0], pt2.YValues[0], pt3.YValues[0])));
            }

            // Connect to the final point.
            dataPoints.Add(new DataPoint(
                XChart(1.0f, pt0.XValue, pt1.XValue, pt2.XValue, pt3.XValue),
                YChart(1.0f, pt0.YValues[0], pt1.YValues[0], pt2.YValues[0], pt3.YValues[0])));

            // Draw the curve.
            
         //  chart1.Series["Series1"].Points.AddXY(textBox8.Text, textBox7.Text);

          //gr.DrawLines(the_pen, dataPoints);
            Pen yellow = new Pen(Color.Yellow, 7);
            Pen pink = new Pen(Color.Pink, 7);
            Pen orange = new Pen(Color.Orange, 7);


           // Draw lines connecting the control points.
            //gr.DrawLine(yellow, pt0, pt1);
            //gr.DrawLine(pink, pt1, pt2);
            //gr.DrawLine(orange, pt2, pt3);

        }
        // Reset Buttons
        private void button1_Click_1(object sender, EventArgs e)
        {
            Source.Clear();
            Obstacle.Clear();
            Destination.Clear();
            panel1.Controls.Clear();
            panel1.Refresh();
            checkBox1.Refresh();
            checkBox2.Refresh();
            checkBox1.Checked = false;
            checkBox2.Checked = false;
            radioButton1.Checked = false;
            radioButton2.Checked = false;
            radioButton3.Checked = false;
            textBox2.Clear();

        }
        // Enter Button
        private void button1_Click(object sender, EventArgs e)
        {
            int velocity = Int32.Parse(textBox2.Text);
            int thinkingdistance;
            int breakingdistance;
            int stoppingdistance;

            var carX = sourceCtrl.Location.X;
            var carY = sourceCtrl.Location.Y;
            MessageBox.Show("carX" + carX + "carY" + carY);

            var desX = destCtrl.Location.X;
            var desY = destCtrl.Location.Y;
            MessageBox.Show("desX" + desX + "desX" + desY);


            thinkingdistance = (velocity / 10) * 3;
            breakingdistance = (velocity / 10) * (velocity / 10);
            stoppingdistance = thinkingdistance + breakingdistance;

            //MessageBox.Show("thinking distance :   " + thinkingdistance);
            //MessageBox.Show("breakingdistance :   " + breakingdistance);
            //MessageBox.Show("The Stopping Distnace is :   " + stoppingdistance.ToString());

            //var distance = Math.Sqrt(((carX - desX) ^ 2) + ((carX - carY) ^ 2));
            //MessageBox.Show("The Distance between source and destination :  " + distance);


        }
        // list and controls
        #region
        List<Point> Source = new List<Point>();
        List<Point> Obstacle = new List<Point>();
        List<Point> Destination = new List<Point>();

        Control sourceCtrl;
        Control obstCtrl;
        Control destCtrl;
        #endregion
        // Radiobuttons (Source, Obstacle , Destination)
        private void panel1_MouseClick(object sender, MouseEventArgs e)
        {

            if (radioButton1.Checked == true)
            {
                if (Source.Count < 1)
                    Source.Add(new Point(e.X, e.Y));
                else
                    Source[0] = new Point(e.X, e.Y);

                panel1.Controls.Remove(sourceCtrl);
                foreach (var item in Source)
                {
                    sourceCtrl = new Control("Source", item.X, item.Y, 20, 20);

                    panel1.Controls.Add(sourceCtrl);
                    //MessageBox.Show("X...." + item.X + "Y...."  + item.Y );
                }

            }
            else if (radioButton2.Checked == true)
            {
                if (Obstacle.Count < 1)
                    Obstacle.Add(new Point(e.X, e.Y));
                else
                    Obstacle[0] = new Point(e.X, e.Y);

                panel1.Controls.Remove(obstCtrl);
                foreach (var item in Obstacle)
                {

                    obstCtrl = new Control("Obstacle", item.X, item.Y, 10, 10);
                    panel1.Controls.Add(obstCtrl);
                }
            }
            else if (radioButton3.Checked == true)
            {
                if (Destination.Count < 1)
                    Destination.Add(new Point(e.X, e.Y));
                else
                    Destination[0] = new Point(e.X, e.Y);

                panel1.Controls.Remove(destCtrl);
                foreach (var item in Destination)
                {
                    destCtrl = new Control("Point", item.X, item.Y, 20, 20);
                    panel1.Controls.Add(destCtrl);

                }
            }


        }
        // Start_Button 
        private void start_Click(object sender, EventArgs e)
        {
           // Moving straightpath no obstacle Source to destination 
            if (checkBox1.Checked == true)
            {

                //bool moving = false;
                //var car = sourceCtrl;
                //var des = destCtrl;

                //var currentXcordinateofCAR = car.Location.X;
                //int currentXcordinateofCAR;   
                //var currentYcordinateofCAR = car.Location.Y;
                //int currentYcordinateofCAR;
                //var currentXcordinateofDES = des.Location.X;
                //int currentXcordinateofDES;
                //var currentYcordinateofDES = des.Location.Y;
                //int currentYcordinateofDES;
                var velocity = Int32.Parse(textBox2.Text);
                float elapse = 0.1f;
                //int convertingelapsetointeger = (int)elapse;
                var carX = sourceCtrl.Location.X;
                var carY = sourceCtrl.Location.Y;
                //MessageBox.Show("ksdjklsdj" +carY);
                var desX = destCtrl.Location.X;
                var desY = destCtrl.Location.Y;
                //var distance = Math.Sqrt(((carX - desX) ^ 2) + ((carY - desY) ^ 2));
                //var directionX = (desX - carX) / distance;
                //var directionY = (desY - desX) / distance;

                if (Source.Count < 1)
                    Source.Add(new Point(carX, carY));
                else
                    Source[0] = new Point(carX, carY);
                panel1.Controls.Remove(sourceCtrl);
                foreach (var items in Source)
                {
                    sourceCtrl = new Control("Source", items.X, items.Y, 20, 20);
                    panel1.Controls.Add(sourceCtrl);

                }

                bool moving = true;
                int lastCarX = carX, lastCarY = carY;
                while (moving == true)
                {
                    //carX += Convert.ToInt32(directionX * velocity * elapse);
                    //carY += Convert.ToInt32(directionY * velocity * elapse);

                    //MessageBox.Show("Before X:" + carX + " Y:" + carY);
                    //MessageBox.Show("Dist X:" + (Convert.ToDouble(desX - carX) * elapse) + " Y:" + (Convert.ToDouble(desY - carY) * elapse));
                    carX += Convert.ToInt32(Convert.ToDouble(desX - carX) * elapse);
                    carY += Convert.ToInt32(Convert.ToDouble(desY - carY) * elapse);
                    //MessageBox.Show("X:" + carX + " Y:" + carY);


                    ////create pen
                    Pen RedPen = new Pen(Color.Red, 7);
                    //create points that define line 
                    List<Point> starightpoints = new List<Point>();

                    Point Point1 = new Point(lastCarX, lastCarY);
                    Point Point2 = new Point(carX, carY);
                    starightpoints.Add(Point1);
                    starightpoints.Add(Point2);

                    //draw line to screen 
                    foreach (var items in starightpoints)
                    {

                        var gp = panel1.CreateGraphics();
                        gp.DrawLine(RedPen, Point1, Point2);


                    }

                    //
                    if (Source.Count < 1)
                        Source.Add(new Point(carX, carY));
                    else
                        Source[0] = new Point(carX, carY);

                    panel1.Controls.Remove(sourceCtrl);
                    foreach (var items in Source)
                    {
                        sourceCtrl = new Control("Source", items.X, items.Y, 20, 20);
                        panel1.Controls.Add(sourceCtrl);

                    }

                    //if (Math.Sqrt(((carX - desX) ^ 2) + ((carY - desY) ^ 2)) >= distance)
                    //{
                    //    carX = desX;
                    //    carY = desY;
                    //    moving = false;
                    //}
                    //if(distance < 5)
                    //{
                    //    moving = false;
                    //}


                    var distance = Math.Sqrt(((carX - desX) ^ 2) + ((carY - desY) ^ 2));
                    if (distance <= 5)
                    {

                        moving = false;
                    }

                    lastCarX = carX;
                    lastCarY = carY;

                }



                if (Source.Count < 1)
                    Source.Add(new Point(lastCarX, lastCarY));
                else
                    Source[0] = new Point(lastCarX, lastCarY);
                panel1.Controls.Remove(sourceCtrl);
                foreach (var items in Source)
                {
                    sourceCtrl = new Control("Source", items.X, items.Y, 20, 20);
                    panel1.Controls.Add(sourceCtrl);

                }


            }
            // Moving straightpath with obstacle 
            else if (checkBox2.Checked == true)
            {
              
                var carX = sourceCtrl.Location.X;
                var carY = sourceCtrl.Location.Y;
                int last0CarX = carX, last1CarY = carY;

                MessageBox.Show("carX" + carX + "carY" + carY);
                var obsX = obstCtrl.Location.X;
                var obsY = obstCtrl.Location.Y;
                MessageBox.Show("obsX" + obsX + "obsY" + obsY);
                var desX = destCtrl.Location.X;
                var desY = destCtrl.Location.Y;
                MessageBox.Show("desX" + desX + "desX" + desY);

                int velocity = Int32.Parse(textBox2.Text);

                var DSO = Math.Sqrt(((carX - obsX) ^ 2) + ((carY - obsY) ^ 2));
                var DOD = Math.Sqrt(((obsX - desX) ^ 2) + ((obsY - desY) ^ 2));
                var DSD = Math.Sqrt(((carX - desX) ^ 2) + ((carY - desY) ^ 2));
                MessageBox.Show(" dso :" + DSO  + "dod :" + DOD  + "dsd  :" + DSD);

                List<Point> POINTS = new List<Point>();
                List<Point> POINTS2 = new List<Point>();
                var MSD = 10;


                if (DSO > MSD && ((velocity > 10) && (velocity < 200)))
                {

                    POINTS.Clear();


                    while (DSO > MSD && ((8 < obstCtrl.Height) && (obstCtrl.Height < 20))
                       && ((8 < obstCtrl.Width) && (obstCtrl.Width < 20)))
                    {
                        // start point

                        carX = carX;
                        carY = carY;
                        //carY = carY - 60;
                        DSO = Math.Sqrt(((carX - obsX) ^ 2) + ((carY - obsY) ^ 2));
                      
                        if (DSO > 7 && DSO < 20)
                        {
                            break;
                        }


                    }

                   POINTS.Add(new Point(carX, carY));
                   
                   
                    while ((carX < obsX + MSD))
                    {
                        carX = carX - 7*MSD;
                        carY = carY -6*MSD;
                       

                        if ((50 < carX) && (carX < 250))
                        {
                            break;
                        }
                    }
                    POINTS.Add(new Point(carX, carY));

                    while ((carY + MSD > obsY))
                    {
                        carX = carX + 70;
                        carY = carY - 8*MSD;
                       
                        break;

                    }
                    POINTS.Add(new Point(carX, carY));

                    while (DSO > MSD)
                    {
                        carX = obsX -8*MSD;
                        carY = obsY - 2 *MSD;
                        DSO = Math.Sqrt(((carX - obsX) ^ 2) + ((carY - obsY) ^ 2));
                        
                        break;

                    }
                    POINTS.Add(new Point(carX, carY));

                    MessageBox.Show("point[0].x" + POINTS[0].X + "point[0].y" + POINTS[0].Y);
                    MessageBox.Show("point[1].x" + POINTS[1].X + "point[1].y" + POINTS[1].Y);
                    MessageBox.Show("point[2].x" + POINTS[2].X + "point[2].y" + POINTS[2].Y);
                    MessageBox.Show("point[3].x" + POINTS[3].X + "point[3].y" + POINTS[3].Y);
               
                }

                if (DSD > MSD && ((velocity > 10) && (velocity < 200)))
                {

                    POINTS2.Clear();


                    while (DSD > MSD && ((8 < obstCtrl.Height) && (obstCtrl.Height < 20))
                       && ((8 < obstCtrl.Width) && (obstCtrl.Width < 20)))
                    {
                        // start point
                        carX = obsX - 8 * MSD; ;
                        carY = obsY - 2 * MSD; ;

                        DSD = Math.Sqrt(((carX - obsX) ^ 2) + ((carY - obsY) ^ 2));

                        if (DSD > 7 && DSD < 20)
                        {
                            break;
                        }


                    }

                    POINTS2.Add(new Point(carX, carY));

                    while ((carX < obsX + MSD))
                    {
                        obsX = obsX;
                        obsY = obsY - 10 * MSD;


                        if ((50 < carX) && (carX < 300))
                        {
                            break;
                        }
                    }
                    POINTS2.Add(new Point(obsX, obsY));

                    while ((carY + MSD > obsY))
                    {
                        obsX = obsX - 7 * MSD;
                        obsY = obsY - 7 * MSD;

                        break;

                    }
                    POINTS2.Add(new Point(obsX, obsY));

                    while (DOD > MSD)
                    {
                        obsX = desX;
                        obsY = desY;
                        DSO = Math.Sqrt(((carX - obsX) ^ 2) + ((carY - obsY) ^ 2));

                        break;

                    }
                    POINTS2.Add(new Point(obsX, obsY));

                }


                //create pen
                Pen RedPen = new Pen(Color.Red, 6);
                var abc = panel1.CreateGraphics();

                

                DrawBezier(abc,RedPen,0.01f,POINTS[0],POINTS[2], POINTS[1],POINTS[3]);
                DrawBezier(abc, RedPen, 0.01f, POINTS2[0], POINTS2[2], POINTS2[1], POINTS2[3]);

                for (int i = 0; i < 4;i++ )
                {
                  abc.FillRectangle(Brushes.White, POINTS[i].X - 3, POINTS[i].Y - 3, 6, 6);
                  abc.DrawRectangle(Pens.Blue, POINTS[i].X - 3, POINTS[i].Y - 3, 6, 6);

                }
                for (int i = 0; i < 4; i++)
                {
                    abc.FillRectangle(Brushes.White, POINTS2[i].X - 3, POINTS2[i].Y - 3, 6, 6);
                    abc.DrawRectangle(Pens.Blue, POINTS2[i].X - 3, POINTS2[i].Y - 3, 6, 6);

                }




            }
        }
      
        private void chart1_MouseClick(object sender, MouseEventArgs e)
        {
            Point mouseclick = new Point(e.X, e.Y);

            chart1.ChartAreas[0].CursorX.Interval = 0;
            chart1.ChartAreas[0].CursorY.Interval = 0;

            //chart1.ChartAreas[0].CursorX.SetCursorPixelPosition(mouseclick,true) ;
            //chart1.ChartAreas[0].CursorY.SetCursorPixelPosition(mouseclick, true);
            //var trc = chart1.ChartAreas[0].AxisX.PixelPositionToValue(e.X);
            //label1.Text = " X Postion " + trc;
           label1.Text = " X Postion "  + chart1.ChartAreas[0].AxisX.PixelPositionToValue(e.X)/*.ToString()*/;
            label2.Text = " Y Postion "  + chart1.ChartAreas[0].AxisY.PixelPositionToValue(e.Y).ToString();


            if (radioButton1.Checked == true)
            {
                if (Source.Count < 1)
                    Source.Add(new Point(e.X, e.Y));
                else
                    Source[0] = new Point(e.X, e.Y);

                chart1.Controls.Remove(sourceCtrl);
                foreach (var item in Source)
                {
                    sourceCtrl = new Control("Source", item.X, item.Y, 20, 20);

                    chart1.Controls.Add(sourceCtrl);
                }

            }
            else if (radioButton2.Checked == true)
            {
                if (Obstacle.Count < 1)
                    Obstacle.Add(new Point(e.X, e.Y));
                else
                    Obstacle[0] = new Point(e.X, e.Y);

                chart1.Controls.Remove(obstCtrl);
                foreach (var item in Obstacle)
                {

                    obstCtrl = new Control("Obstacle", item.X, item.Y, 10, 10);
                    chart1.Controls.Add(obstCtrl);
                }
            }
            else if (radioButton3.Checked == true)
            {
                if (Destination.Count < 1)
                    Destination.Add(new Point(e.X, e.Y));
                else
                    Destination[0] = new Point(e.X, e.Y);

                chart1.Controls.Remove(destCtrl);
                foreach (var item in Destination)
                {
                    destCtrl = new Control("Point", item.X, item.Y, 20, 20);
                    chart1.Controls.Add(destCtrl);

                }
            }
        }
        // Start_chart
        private void button2_Click(object sender, EventArgs e)
        {
            chart1.ChartAreas[0].AxisX.Maximum =10 ;
            chart1.ChartAreas[0].AxisX.Minimum = -10;
            chart1.ChartAreas[0].AxisY.Maximum = 20;
            chart1.ChartAreas[0].AxisY.Minimum = -3;


            // State vector ego vechile 
            var Xego = double.Parse(textBox8.Text);
            var Yego = double.Parse(textBox7.Text);
            var Wego = double.Parse(textBox16.Text);
            var Lego = double.Parse(textBox18.Text);
            var Vego = double.Parse(textBox15.Text);

            label1.Text = "Xego Postion" + Xego;
            label2.Text = "Yego Postion" + Yego;
            label5.Text = "Wego " + Wego;
            label6.Text = "Lego " + Lego;
            label7.Text = "Velocity " + Vego;

            Dictionary<string, double> EgoVehicle =new Dictionary<string, double>();
            EgoVehicle.Add("Xego", Xego);
            EgoVehicle.Add("Yego", Yego);
            EgoVehicle.Add("Wego", Wego);
            EgoVehicle.Add("Lego", Lego);
            EgoVehicle.Add("Vego", Vego);

            // State vector Obs 
            var Xobs = double.Parse(textBox6.Text);
            var Yobs = double.Parse(textBox5.Text);
            var Wobs = double.Parse(textBox9.Text);
            var Lobs = double.Parse(textBox10.Text);

            label3.Text = "Xobs Postion" + Xobs;
            label4.Text = "Yobs Postion" + Yobs;
            label8.Text = "Wego" + Wobs;
            label9.Text = "Lego" + Lobs;
          
            Dictionary<string, double> ObsVehicle = new Dictionary<string, double>();
            ObsVehicle.Add("Xobs", Xobs);
            ObsVehicle.Add("Yobs", Yobs);
            ObsVehicle.Add("Wobs", Wobs);
            ObsVehicle.Add("Lobs", Lobs);
            ObsVehicle.Add("Vobs", 0);
            double MSD_e = 0;
            double MSD_o = 0;
            
          
            if (Vego <= 20)
            {
                 MSD_e = 0.25;
                 MSD_o = 0.25;
            }
            else if (Vego <= 40)
            {
                MSD_e = 0.35;
                MSD_o = 0.35;
            }
            else if (Vego > 40 )
            {
                MSD_e = 0.5;
                MSD_o = 0.5;
            }
            var DEO = Math.Sqrt((Math.Pow(Xego - Xobs,2)) + (Math.Pow(Yego - Yobs,2)));

            List<DataPoint> dataPoints0 = new List<DataPoint>();


           if (DEO > MSD_e + MSD_o && ((Wego<3) && (Lego < 6)) && ((Wobs<3) && (Lobs < 20)))
            {
                dataPoints0.Clear();

                // First DataPoint.
                while(DEO > MSD_e + MSD_o)

                {
                    Xego = Xego;
                    Yego = Yego;
                    DEO = Math.Sqrt((Math.Pow(Xego - Xobs, 2)) + (Math.Pow(Yego - Yobs, 2)));
                    break;

                }
                dataPoints0.Add(new DataPoint(Xego, Yego));
                // Second DataPoint.
                while (DEO > MSD_e + MSD_o)

                {
                    Xego = Xego-(3 * MSD_e + MSD_o);
                    Yego = Yego +(2 * MSD_e + MSD_o);
                    DEO = Math.Sqrt((Math.Pow(Xego - Xobs, 2)) + (Math.Pow(Yego - Yobs, 2)));
                    break;

                }
                dataPoints0.Add(new DataPoint(Xego, Yego));
                // Third DataPoint.
                while (DEO > MSD_e + MSD_o)
                {
                    Xego = Xego - (MSD_e + MSD_o);
                    Yego = Yego + (3 * MSD_e + MSD_o);
                    DEO = Math.Sqrt((Math.Pow(Xego - Xobs, 2)) + (Math.Pow(Yego - Yobs, 2)));
                    break;

                }
                dataPoints0.Add(new DataPoint(Xobs,Yego));
                // Fourth DataPoint.
                while (DEO > MSD_e + MSD_o)
                {
                    Xego = Xobs - (3 * MSD_e + MSD_o);
                    Yego = Yobs;
                    DEO = Math.Sqrt((Math.Pow(Xego - Xobs, 2)) + (Math.Pow(Yego - Yobs, 2)));
                    break;
                }
                dataPoints0.Add(new DataPoint(Xego, Yego));


            }
            else
            {
             MessageBox.Show("Collison possible STOP");
            //MessageBoxButtons.OKCancel,
            //MessageBoxIcon.Warning,
            //MessageBoxDefaultButton.Button1,
            //MessageBoxOptions.RightAlign,
            //true);
            }

            var a = dataPoints0[0].XValue;
            var b = dataPoints0[1].XValue;
            var c = dataPoints0[2].XValue;
            var d = dataPoints0[3].XValue;

            var l = dataPoints0[0].YValues[0];
            var i = dataPoints0[1].YValues[0];
            var f = dataPoints0[2].YValues[0];
            var g = dataPoints0[3].YValues[0];

            double[] xPoints = XChart(a, c, b, d, 100);
            double[] yPoints = YChart(l, f, i, g, 100);
            if (xPoints.Length != yPoints.Length)
                throw new InvalidOperationException("The number of points between axes must match.");

            for (int v = 0; v < xPoints.Length; v++)
                chart1.Series["Series1"].Points.AddXY(xPoints[v], yPoints[v]);

            // chart1.Series["Series1"].IsValueShownAsLabel = true;

            Pen RedPen = new Pen(Color.Red, 6);
            var abc = chart1.CreateGraphics();

           DrawChartBezier(abc, RedPen, 0.1, dataPoints0[0], dataPoints0[1], dataPoints0[2], dataPoints0[3]);



            //Second beziere curve
          List<DataPoint> dataPoints1 = new List<DataPoint>();

            if (DEO > MSD_e + MSD_o && ((Wego < 3) && (Lego < 6)) && ((Wobs < 3) && (Lobs < 20)))
            {
                dataPoints1.Clear();

                // First DataPoint.
                while (DEO > MSD_e + MSD_o)
                {
                    Xego = Xobs - (3 * MSD_e + MSD_o);
                    Yego = Yobs;
                    DEO = Math.Sqrt((Math.Pow(Xego - Xobs, 2)) + (Math.Pow(Yego - Yobs, 2)));
                    break;

                }
                dataPoints1.Add(new DataPoint(Xego, Yego));


                // Second DataPoint.
                while (DEO > MSD_e + MSD_o)
                {
                    
                    Xobs = Xobs ;
                    Yobs = Yobs + (3 * MSD_e + MSD_o);
                    DEO = Math.Sqrt((Math.Pow(Xego - Xobs, 2)) + (Math.Pow(Yego - Yobs, 2)));
                    break;

                }
                dataPoints1.Add(new DataPoint(Xobs, Yobs));

                // Third DataPoint.
                while (DEO > MSD_e + MSD_o)
                {
                    
                    Xobs = Xobs - (3 * MSD_e + MSD_o);
                    Yobs = Yobs + (3 * MSD_e + MSD_o);
                    DEO = Math.Sqrt((Math.Pow(Xego - Xobs, 2)) + (Math.Pow(Yego - Yobs, 2)));
                    break;
                }
                dataPoints1.Add(new DataPoint(Xobs, Yobs));

                // Fourth DataPoint.
                while (DEO > MSD_e + MSD_o)
                {
                    Xobs = Xobs + ( 3 * MSD_e + MSD_o);
                    Yobs = Yobs + ( 4* MSD_e + MSD_o);
                    DEO = Math.Sqrt((Math.Pow(Xego - Xobs, 2)) + (Math.Pow(Yego - Yobs, 2)));
                    break;
                }
                dataPoints1.Add(new DataPoint(Xobs, Yobs));


            }

            var a1 = dataPoints1[0].XValue;
            var b1 = dataPoints1[1].XValue;
            var c1 = dataPoints1[2].XValue;
            var d1 = dataPoints1[3].XValue;

            var l1 = dataPoints1[0].YValues[0];
            var i1 = dataPoints1[1].YValues[0];
            var f1 = dataPoints1[2].YValues[0];
            var g1 = dataPoints1[3].YValues[0];

            double[] xPoints0 = XChart(a1, c1, b1, d1, 100);
            double[] yPoints0 = YChart(l1, f1, i1, g1, 100);
            if (xPoints.Length != yPoints.Length)
                throw new InvalidOperationException("The number of points between axes must match.");

            Pen RedPen1 = new Pen(Color.Red, 6);
            var abcd = chart1.CreateGraphics();

            for (int v = 0; v < xPoints.Length; v++)
                chart1.Series["Series1"].Points.AddXY(xPoints0[v], yPoints0[v]);

            // DrawChartBezier(abcd,RedPen1,0.1,dataPoints1[0],dataPoints1[1],dataPoints1[2],dataPoints1[3]);
         




        }

    
    }
}
