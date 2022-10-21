using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Stefan_Ljubinkovic_PZ1.Models
{
    public class SwitchEntity : PowerEntity
    {
        private string status;

        public string Status
        {
            get
            {
                return status;
            }

            set
            {
                status = value;
            }
        }
        public override void ResetColor()
        {
            this.Ellipse.Fill = Brushes.Green;
        }

    }
}
