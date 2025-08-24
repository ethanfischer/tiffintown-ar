#if FCU_EXISTS
using DA_Assets.DAI;
using DA_Assets.Extensions;
using DA_Assets.FCU.Extensions;
using DA_Assets.FCU.Model;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace DA_Assets.FCU
{
    [Serializable]
    public class ImageStyleBuilder : MonoBehaviourLinkerRuntime<FigmaConverterUnity>
    {
        public KeyValuePair<string, string> GetSpriteLocalStyle(FObject fobject)
        {
#if FCU_UITK_EXT_EXISTS
            string assetPath = fobject.Data.Names.UITK_SpritePath;
            return new KeyValuePair<string, string>("background-image", $"url({assetPath})");
#else
            return default;
#endif
        }

        public string CreateImageLocalStyle(FObject fobject)
        {
            StringBuilder styleBuilder = new StringBuilder();

            if (fobject.IsSprite())
            {
                if (!fobject.Data.SpritePath.IsEmpty())
                {
                    var kvp = GetSpriteLocalStyle(fobject);
                    styleBuilder.AddLocalStyle(kvp.Key, kvp.Value);
                }
            }

            return styleBuilder.ToString();
        }

        public string CreateImageGlobalStyle(FObject fobject)
        {
            StringBuilder styleBuilder = new StringBuilder();

            if (fobject.IsSprite() && !fobject.Data.Graphic.SpriteSingleColor.IsDefault())
            {
                Color c = fobject.Data.Graphic.SpriteSingleColor;
                string _rgba = c.ToCssColor(c.a);
                styleBuilder.AddStyle("-unity-background-image-tint-color", _rgba);
            }
            else
            {
                FGraphic graphic = fobject.Data.Graphic;

                if (graphic.HasFill)
                {
                    Color color = graphic.Fill.SingleColor;

                    if (fobject.Data.FcuImageType == FcuImageType.Downloadable)
                    {
                        color.a = 0;
                    }
                    else if (fobject.IsMask.ToBoolNullFalse())
                    {
                        color.a = 0;
                    }

                    string rgba = color.ToCssColor(color.a);

                    styleBuilder.AddStyle("background-color", rgba);
                }
            }

            return styleBuilder.ToString();
        }
    }
}
#endif