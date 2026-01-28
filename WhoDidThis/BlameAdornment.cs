using LibGit2Sharp;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Formatting;
using System;
using System.IO;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;

namespace WhoDidThis
{
    internal sealed class BlameAdornment
    {
        private readonly IWpfTextView _textView;
        private readonly Canvas _canvas;

        public BlameAdornment(IWpfTextView textView)
        {
            _textView = textView;
            _canvas = new Canvas();

            // add canvas as layer on editor
            var layer = textView.GetAdornmentLayer("BlameAdornmentLayer");

            //add canvas to layer
            layer.AddAdornment(AdornmentPositioningBehavior.OwnerControlled, null, null, _canvas, null);
            // redraw on editor changes
            _textView.LayoutChanged += OnLayoutChanged;
        }

        /// <summary>
        /// called when the editor layout changes (scroll, resize, text modification)
        /// clears and redraws Git blame information for visible lines
        /// </summary>
        private void OnLayoutChanged(object sender, TextViewLayoutChangedEventArgs e)
        {
            _canvas.Children.Clear();
            string filePath = GetFilePath();
            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
            {
                return;
            }

            var repositoryPath = Repository.Discover(filePath);
            if (string.IsNullOrEmpty(repositoryPath))
            {
                return;
            }

            DrawBlameForVisibleLines(filePath, repositoryPath);
        }

        /// <summary>
        /// get git info and draws it for each visible lines in the editor
        /// </summary>
        /// <param name="filePath">Path to the file being edited</param>
        /// <param name="repoPath">Path to the Git repository</param>
        private void DrawBlameForVisibleLines(string filePath, string repoPath)
        {
            try
            {
                using (var repo = new Repository(repoPath))
                {
                    // get blame information
                    var blame = repo.Blame(filePath);

                    // Draw blame info for each visible line
                    foreach (var line in _textView.TextViewLines)
                    {
                        var lineNumber = line.Start.GetContainingLine().LineNumber;
                        if (lineNumber < blame.Count())
                        {
                            var hunk = blame[lineNumber];
                            DrawBlameInfo(line, hunk);
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// draws Git blame information at the end of a text line
        /// </summary>
        /// <param name="line">The text view line to annotate</param>
        /// <param name="hunk">The hunk containing commit information</param>
        private void DrawBlameInfo(ITextViewLine line, BlameHunk hunk)
        {
            var author = hunk.FinalCommit.Author.Name;
            var message = hunk.FinalCommit.MessageShort;
            //TODO: to fix for show 1 days ago, 5 days ago until a week then use the date
            var date = hunk.FinalCommit.Author.When.DateTime.ToShortDateString();
            var text = $"  {author} - {date} - {message}";

            var textBlock = new TextBlock
            {
                Text = text,
                Foreground = new SolidColorBrush(Colors.Gray),
                FontSize = 12,
                Opacity = 0.6
            };

            // write info at the end of line
            Canvas.SetLeft(textBlock, line.Right + 10);
            Canvas.SetTop(textBlock, line.Top);

            _canvas.Children.Add(textBlock);
        }

        private string GetFilePath()
        {
            var document = _textView.TextBuffer.Properties.TryGetProperty<ITextDocument>(typeof(ITextDocument), out var doc);
            return document ? doc.FilePath : null;
        }
    }
}