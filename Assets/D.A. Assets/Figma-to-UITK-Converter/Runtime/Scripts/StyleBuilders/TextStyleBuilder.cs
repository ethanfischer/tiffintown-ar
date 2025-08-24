#if FCU_EXISTS
using DA_Assets.DAI;
using DA_Assets.Extensions;
using DA_Assets.FCU.Extensions;
using DA_Assets.FCU.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace DA_Assets.FCU
{
    [Serializable]
    public class TextStyleBuilder : MonoBehaviourLinkerRuntime<FigmaConverterUnity>
    {
        public string CreateGlobalTextStyle(FObject fobject)
        {
            StringBuilder styleBuilder = new StringBuilder();

            int px0 = 0;

            styleBuilder.AddStyle("margin-left", $"{px0}px");
            styleBuilder.AddStyle("margin-right", $"{px0}px");
            styleBuilder.AddStyle("margin-top", $"{px0}px");
            styleBuilder.AddStyle("margin-bottom", $"{px0}px");

            styleBuilder.AddStyle("padding-left", $"{px0}px");
            styleBuilder.AddStyle("padding-right", $"{px0}px");
            styleBuilder.AddStyle("padding-top", $"{px0}px");
            styleBuilder.AddStyle("padding-bottom", $"{px0}px");

            FGraphic graphic = fobject.Data.Graphic;

            string rgba = null;

            if (graphic.HasFill)
            {
                rgba = graphic.Fill.SingleColor.ToCssColor(graphic.Fill.SingleColor.a);
            }
            else if (graphic.HasStroke)
            {
                rgba = graphic.Stroke.SingleColor.ToCssColor(graphic.Stroke.SingleColor.a);
            }

            if (!rgba.IsEmpty())
            {
                styleBuilder.AddStyle("color", rgba);
            }

            //////////////////////////////////////////////////////

            styleBuilder.AddStyle("white-space", "nowrap");//normal

            string acnhor = fobject.GetTextAnchor().ToUITKAnchor();
            styleBuilder.AddStyle("-unity-text-align", acnhor);

            KeyValuePair<string, string> kvp = GetFontDefinition(fobject);
            if (kvp.Key != null)
            {
                styleBuilder.AddStyle(kvp.Key, kvp.Value);
            }
            else
            {
                //no font or font == NotInter
            }

            int fontSize = System.Convert.ToInt32(fobject.Style.FontSize);
            styleBuilder.AddStyle("font-size", $"{fontSize}px");

            //////////////////////////////////////////////////////

            return styleBuilder.ToString();
        }

        public KeyValuePair<string, string> GetFontDefinition(FObject fobject)
        {
#if FCU_UITK_EXT_EXISTS
            string fontPath = fobject.Data.Names.UITK_FontPath;

            if (!fontPath.IsEmpty() && !fontPath.StartsWith("project://database/Library"))
            {
                return new KeyValuePair<string, string>("-unity-font-definition", $"url({fobject.Data.Names.UITK_FontPath})");
            }
            else
#endif
            {
                return new KeyValuePair<string, string>(null, null);
            }
        }
    }
}
#endif
