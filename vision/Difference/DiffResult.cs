using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vision.Difference {

    public class DiffResult {

        /// <summary>
        /// 源文本中的分区
        /// </summary>
        public string[] PiecesOld { get; }

        /// <summary>
        /// 修改后文本的分区
        /// </summary>
        public string[] PiecesNew { get; }


        /// <summary>
        /// 修改位置列表
        /// </summary>
        public IList<DiffBlock> DiffBlocks { get; }

        public DiffResult(string[] peicesOld, string[] piecesNew, IList<DiffBlock> blocks) {
            PiecesOld = peicesOld;
            PiecesNew = piecesNew;
            DiffBlocks = blocks;
        }
    }
}
