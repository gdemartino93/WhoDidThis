using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;
using System.ComponentModel.Composition;

namespace WhoDidThis
{
    [Export(typeof(IWpfTextViewCreationListener))]
    [ContentType("text")]
    [TextViewRole(PredefinedTextViewRoles.Document)]
    internal sealed class BlameTextViewCreationListener : IWpfTextViewCreationListener
    {
        /// <summary>
        /// this is called by VS every time a text editor is created (file opened) and attach e adornement to display git info
        /// </summary>
        /// <param name="textView">the new text view instance</param>
        public void TextViewCreated(IWpfTextView textView)
        {
            // get or create a adornement for opened filed
            textView.Properties.GetOrCreateSingletonProperty(() => new BlameAdornment(textView));
        }
    }
}
