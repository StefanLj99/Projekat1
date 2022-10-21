using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stefan_Ljubinkovic_PZ1.Models
{
    public enum EBlockType { ENTITY, LINE, EMPTY, CROSS}
    public class Block
    {
        public Block(int x, int y, PowerEntity entity, EBlockType type)
        {
            X = x;
            Y = y;
            Entity = entity;
            Type = type;
        }

        public int X { get; set; }
        public int Y { get; set; }
        public PowerEntity Entity { get; set; }
        public EBlockType Type { get; set; }
    }
}
