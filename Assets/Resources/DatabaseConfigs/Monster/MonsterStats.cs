using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maniac.DataBaseSystem
{/// <summary>
 ///  Thông tin quái
 /// </summary>

    [Serializable]
    public class MonsterStats
    {
        public int Level;
        public int Health;
        public int MoveSpeed;
        public MonsterType TypeMonter;
    }
    /// <summary>
    /// Kiểu quái
    /// </summary>
    [Serializable]
    public enum MonsterType
    {
        None = 0,
        Normal = 1,
        Boss = 2,
    }
}
