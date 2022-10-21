using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Stefan_Ljubinkovic_PZ1.Models
{
    public class PowerEntity
    {
        private long id;
        private string name;
        private double x;
        private double y;

        public Ellipse Ellipse { get; set; }


        public PowerEntity()
        {

        }

        public long Id
        {
            get
            {
                return id;
            }

            set
            {
                id = value;
            }
        }

        public string Name
        {
            get
            {
                return name;
            }

            set
            {
                name = value;
            }
        }

        public double X
        {
            get
            {
                return x;
            }

            set
            {
                x = value;
            }
        }

        public double Y
        {
            get
            {
                return y;
            }

            set
            {
                y = value;
            }
        }



        public void OnClick(object sender, EventArgs e)
        {
            this.Ellipse.Fill = Brushes.Black;
        }

        public virtual void ResetColor()
        {
           
        }
    }
}
