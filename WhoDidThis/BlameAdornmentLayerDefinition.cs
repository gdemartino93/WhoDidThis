using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;
using System.ComponentModel.Composition;

namespace WhoDidThis
{
    [Export(typeof(AdornmentLayerDefinition))]
    [Name("BlameAdornmentLayer")]
    [Order(After = PredefinedAdornmentLayers.Text)]
    internal sealed class BlameAdornmentLayerDefinition
    {
    }
}
