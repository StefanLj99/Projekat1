using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Stefan_Ljubinkovic_PZ1.Models
{
    public class SubstationEntity : PowerEntity
    {

        public override void ResetColor()
        {
            this.Ellipse.Fill = Brushes.Blue;
        }
    }
}
