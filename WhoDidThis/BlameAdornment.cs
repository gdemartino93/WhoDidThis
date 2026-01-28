using Microsoft.VisualStudio.Text.Editor;
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

        private void OnLayoutChanged(object sender, TextViewLayoutChangedEventArgs e)
        {
        }
    }
}