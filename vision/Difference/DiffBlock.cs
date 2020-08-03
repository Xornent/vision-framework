using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vision.Difference {

    public class DiffBlock {

        /// <summary>
        /// 交集在 A 中开始的位置
        /// </summary>
        public int DeleteStartA { get; }

        /// <summary>
        /// A 中删除的字符数
        /// </summary>
        public int DeleteCountA { get; }

        /// <summary>
        /// 交集在 B 中开始的位置
        /// </summary>
        public int InsertStartB { get; }

        /// <summary>
        /// B 中重叠的字符数
        /// </summary>
        public int InsertCountB { get; }


        public DiffBlock(int deleteStartA, int deleteCountA, int insertStartB, int insertCountB) {
            DeleteStartA = deleteStartA;
            DeleteCountA = deleteCountA;
            InsertStartB = insertStartB;
            InsertCountB = insertCountB;
        }
    }
}
