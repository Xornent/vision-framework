using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vision.Difference.Chunkers;

namespace Vision.Difference.Builder {

    public interface ITwoSidedBuilder {

        /// <summary>
        /// 使用双文本编辑器展示差异结果
        /// </summary>
        /// <returns>双编辑器模型</returns>
        TwoSidedDiff BuildDiffModel(string oldText, string newText);
    }

    public class TwoSidedBuilder : ITwoSidedBuilder {
        private readonly IDiffer differ;
        private readonly IChunker lineChunker;
        private readonly IChunker wordChunker;

        private delegate void PieceBuilder(string oldText, string newText, List<DiffPiece> oldPieces, List<DiffPiece> newPieces);
        public static TwoSidedBuilder Instance { get; } = new TwoSidedBuilder();

        public TwoSidedBuilder(IDiffer differ, IChunker lineChunker, IChunker wordChunker) {
            this.differ = differ ?? Differ.Instance;
            this.lineChunker = lineChunker ?? throw new ArgumentNullException(nameof(lineChunker));
            this.wordChunker = wordChunker ?? throw new ArgumentNullException(nameof(wordChunker));
        }

        public TwoSidedBuilder(IDiffer differ = null) :
            this(differ, new LineChunker(), new WordChunker()) {
        }

        public TwoSidedBuilder(IDiffer differ, char[] wordSeparators)
            : this(differ, new LineChunker(), new DelimiterChunker(wordSeparators)) {
        }

        public TwoSidedDiff BuildDiffModel(string oldText, string newText)
            => BuildDiffModel(oldText, newText, ignoreWhitespace: true);

        public TwoSidedDiff BuildDiffModel(string oldText, string newText, bool ignoreWhitespace) {
            return BuildLineDiff(
                oldText ?? throw new ArgumentNullException(nameof(oldText)),
                newText ?? throw new ArgumentNullException(nameof(newText)),
                ignoreWhitespace);
        }

        public static TwoSidedDiff Diff(string oldText, string newText, bool ignoreWhiteSpace = true, bool ignoreCase = false) {
            if (oldText == null) throw new ArgumentNullException(nameof(oldText));
            if (newText == null) throw new ArgumentNullException(nameof(newText));

            var model = new TwoSidedDiff();
            var diffResult = Differ.Instance.CreateDiffs(oldText, newText, ignoreWhiteSpace, ignoreCase, LineChunker.Instance);
            BuildDiffPieces(diffResult, model.OldText.Lines, model.NewText.Lines, BuildWordDiffPiecesInternal);

            return model;
        }

        public static TwoSidedDiff Diff(IDiffer differ, string oldText, string newText, bool ignoreWhiteSpace = true, bool ignoreCase = false, IChunker lineChunker = null, IChunker wordChunker = null) {
            if (oldText == null) throw new ArgumentNullException(nameof(oldText));
            if (newText == null) throw new ArgumentNullException(nameof(newText));

            if (differ == null) return Diff(oldText, newText, ignoreWhiteSpace, ignoreCase);
            var model = new TwoSidedDiff();
            var diffResult = differ.CreateDiffs(oldText, newText, ignoreWhiteSpace, ignoreCase, lineChunker ?? LineChunker.Instance);
            BuildDiffPieces(diffResult, model.OldText.Lines, model.NewText.Lines, (ot, nt, op, np) => {
                var r = differ.CreateDiffs(oldText, newText, false, false, wordChunker ?? WordChunker.Instance);
                BuildDiffPieces(r, op, np, null);
            });

            return model;
        }

        private static void BuildWordDiffPiecesInternal(string oldText, string newText, List<DiffPiece> oldPieces, List<DiffPiece> newPieces) {
            var diffResult = Differ.Instance.CreateDiffs(oldText, newText, false, false, WordChunker.Instance);
            BuildDiffPieces(diffResult, oldPieces, newPieces, null);
        }

        private TwoSidedDiff BuildLineDiff(string oldText, string newText, bool ignoreWhitespace) {
            var model = new TwoSidedDiff();
            var diffResult = differ.CreateDiffs(oldText, newText, ignoreWhitespace, false, lineChunker);
            BuildDiffPieces(diffResult, model.OldText.Lines, model.NewText.Lines, BuildWordDiffPieces);

            return model;
        }

        private void BuildWordDiffPieces(string oldText, string newText, List<DiffPiece> oldPieces, List<DiffPiece> newPieces) {
            var diffResult = differ.CreateDiffs(oldText, newText, ignoreWhiteSpace: false, false, wordChunker);
            BuildDiffPieces(diffResult, oldPieces, newPieces, subPieceBuilder: null);
        }

        private static void BuildDiffPieces(DiffResult diffResult, List<DiffPiece> oldPieces, List<DiffPiece> newPieces, PieceBuilder subPieceBuilder) {
            int aPos = 0;
            int bPos = 0;

            foreach (var diffBlock in diffResult.DiffBlocks) {
                while (bPos < diffBlock.InsertStartB && aPos < diffBlock.DeleteStartA) {
                    oldPieces.Add(new DiffPiece(diffResult.PiecesOld[aPos], ChangeType.Unchanged, aPos + 1));
                    newPieces.Add(new DiffPiece(diffResult.PiecesNew[bPos], ChangeType.Unchanged, bPos + 1));
                    aPos++;
                    bPos++;
                }

                int i = 0;
                for (; i < Math.Min(diffBlock.DeleteCountA, diffBlock.InsertCountB); i++) {
                    var oldPiece = new DiffPiece(diffResult.PiecesOld[i + diffBlock.DeleteStartA], ChangeType.Deleted, aPos + 1);
                    var newPiece = new DiffPiece(diffResult.PiecesNew[i + diffBlock.InsertStartB], ChangeType.Inserted, bPos + 1);

                    if (subPieceBuilder != null) {
                        subPieceBuilder(diffResult.PiecesOld[aPos], diffResult.PiecesNew[bPos], oldPiece.SubPieces, newPiece.SubPieces);
                        newPiece.Type = oldPiece.Type = ChangeType.Modified;
                    }

                    oldPieces.Add(oldPiece);
                    newPieces.Add(newPiece);
                    aPos++;
                    bPos++;
                }

                if (diffBlock.DeleteCountA > diffBlock.InsertCountB) {
                    for (; i < diffBlock.DeleteCountA; i++) {
                        oldPieces.Add(new DiffPiece(diffResult.PiecesOld[i + diffBlock.DeleteStartA], ChangeType.Deleted, aPos + 1));
                        newPieces.Add(new DiffPiece());
                        aPos++;
                    }
                } else {
                    for (; i < diffBlock.InsertCountB; i++) {
                        newPieces.Add(new DiffPiece(diffResult.PiecesNew[i + diffBlock.InsertStartB], ChangeType.Inserted, bPos + 1));
                        oldPieces.Add(new DiffPiece());
                        bPos++;
                    }
                }
            }

            while (bPos < diffResult.PiecesNew.Length && aPos < diffResult.PiecesOld.Length) {
                oldPieces.Add(new DiffPiece(diffResult.PiecesOld[aPos], ChangeType.Unchanged, aPos + 1));
                newPieces.Add(new DiffPiece(diffResult.PiecesNew[bPos], ChangeType.Unchanged, bPos + 1));
                aPos++;
                bPos++;
            }
        }
    }
}
