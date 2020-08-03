using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vision.Difference.Chunkers;

namespace Vision.Difference.Builder {

    public interface IInlineBuilder {

        DiffPane BuildDiffModel(string oldText, string newText);

        /// <summary>
        /// 获取单行文本差异报告
        /// </summary>
        /// <param name="oldText">源文本</param>
        /// <param name="newText">修改后的文本</param>
        /// <param name="ignoreWhiteSpace">是否忽略行中空格</param>
        /// <param name="ignoreCase">是否对大小写敏感</param>
        /// <param name="chunker">分区方式</param>
        /// <returns>差异报告</returns>
        DiffPane BuildDiffModel(string oldText, string newText, bool ignoreWhitespace, bool ignoreCase, IChunker chunker);
    }

    public class InlineBuilder : IInlineBuilder {
        private readonly IDiffer differ;
        public static InlineBuilder Instance { get; } = new InlineBuilder();

        public InlineBuilder(IDiffer differ = null) {
            this.differ = differ ?? Differ.Instance;
        }

        public DiffPane BuildDiffModel(string oldText, string newText)
            => BuildDiffModel(oldText, newText, ignoreWhitespace: true);

        public DiffPane BuildDiffModel(string oldText, string newText, bool ignoreWhitespace) {
            var chunker = new LineChunker();
            return BuildDiffModel(oldText, newText, ignoreWhitespace, false, chunker);
        }

        public DiffPane BuildDiffModel(string oldText, string newText, bool ignoreWhitespace, bool ignoreCase, IChunker chunker) {
            if (oldText == null) throw new ArgumentNullException(nameof(oldText));
            if (newText == null) throw new ArgumentNullException(nameof(newText));

            var model = new DiffPane();
            var diffResult = differ.CreateDiffs(oldText, newText, ignoreWhitespace, ignoreCase: ignoreCase, chunker);
            BuildDiffPieces(diffResult, model.Lines);

            return model;
        }

        /// <inheritdoc/>
        public static DiffPane Diff(string oldText, string newText, bool ignoreWhiteSpace = true, bool ignoreCase = false, IChunker chunker = null) {
            return Diff(Differ.Instance, oldText, newText, ignoreWhiteSpace, ignoreCase, chunker);
        }

        /// <inheritdoc/>
        public static DiffPane Diff(IDiffer differ, string oldText, string newText, bool ignoreWhiteSpace = true, bool ignoreCase = false, IChunker chunker = null) {
            if (oldText == null) throw new ArgumentNullException(nameof(oldText));
            if (newText == null) throw new ArgumentNullException(nameof(newText));

            var model = new DiffPane();
            var diffResult = (differ ?? Differ.Instance).CreateDiffs(oldText, newText, ignoreWhiteSpace, ignoreCase, chunker ?? LineChunker.Instance);
            BuildDiffPieces(diffResult, model.Lines);

            return model;
        }

        private static void BuildDiffPieces(DiffResult diffResult, List<DiffPiece> pieces) {
            int bPos = 0;

            foreach (var diffBlock in diffResult.DiffBlocks) {
                for (; bPos < diffBlock.InsertStartB; bPos++)
                    pieces.Add(new DiffPiece(diffResult.PiecesNew[bPos], ChangeType.Unchanged, bPos + 1));

                int i = 0;
                for (; i < Math.Min(diffBlock.DeleteCountA, diffBlock.InsertCountB); i++)
                    pieces.Add(new DiffPiece(diffResult.PiecesOld[i + diffBlock.DeleteStartA], ChangeType.Deleted));

                i = 0;
                for (; i < Math.Min(diffBlock.DeleteCountA, diffBlock.InsertCountB); i++) {
                    pieces.Add(new DiffPiece(diffResult.PiecesNew[i + diffBlock.InsertStartB], ChangeType.Inserted, bPos + 1));
                    bPos++;
                }

                if (diffBlock.DeleteCountA > diffBlock.InsertCountB) {
                    for (; i < diffBlock.DeleteCountA; i++)
                        pieces.Add(new DiffPiece(diffResult.PiecesOld[i + diffBlock.DeleteStartA], ChangeType.Deleted));
                } else {
                    for (; i < diffBlock.InsertCountB; i++) {
                        pieces.Add(new DiffPiece(diffResult.PiecesNew[i + diffBlock.InsertStartB], ChangeType.Inserted, bPos + 1));
                        bPos++;
                    }
                }
            }

            for (; bPos < diffResult.PiecesNew.Length; bPos++)
                pieces.Add(new DiffPiece(diffResult.PiecesNew[bPos], ChangeType.Unchanged, bPos + 1));
        }
    }
}
