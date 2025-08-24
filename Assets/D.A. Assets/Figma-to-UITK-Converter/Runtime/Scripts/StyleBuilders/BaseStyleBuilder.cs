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
    public class BaseStyleBuilder : MonoBehaviourLinkerRuntime<FigmaConverterUnity>
    {
        /// <summary>
        /// hash, style name
        /// </summary>
        private static Dictionary<int, string> _styles = new Dictionary<int, string>();

        public static void ClearStyles()
        {
            _styles.Clear();
            _styles = null;
            _styles = new Dictionary<int, string>();
        }

        public void SetStyle(FObject fobject, StringBuilder styleBuilder)
        {
            fobject.Data.XmlElement.SetAttribute("name", fobject.Data.Names.ObjectName);

#if UITK_LINKER_EXISTS
            if (monoBeh.Settings.UITK_Settings.UitkLinkingMode == UEL.UitkLinkingMode.Guid)
                fobject.Data.XmlElement.SetAttribute("guid", fobject.Data.Names.UitkGuid);
#endif

            SetLocalStyle(fobject);
            SetGlobalStyle(fobject, styleBuilder);

            if (fobject.Type == NodeType.TEXT)
            {
                fobject.Data.XmlElement.SetAttribute("text", fobject.GetText());
            }
        }

        private void SetGlobalStyle(FObject fobject, StringBuilder styleBuilder1)
        {
            StringBuilder tempSb = new StringBuilder();

            tempSb.AppendLine();

            if (fobject.Type == NodeType.TEXT)
            {
                string textStyle = this.TextStyleBuilder.CreateGlobalTextStyle(fobject);
                tempSb.Append(textStyle);
            }
            else
            {
                string imageStyle = this.ImageStyleBuilder.CreateImageGlobalStyle(fobject);
                tempSb.Append(imageStyle);
            }

            if (fobject.IsDrawableType())
            {
                string cornerStyle = CreateCornerGlobalStyle(fobject);
                tempSb.Append(cornerStyle);

                string strokeStyle = CreateStrokeStyle(fobject);
                tempSb.Append(strokeStyle);
            }

            string rawStyle = tempSb
                .ToString()
                .Replace(" ", "")
                .Replace("\n", "")
                .Replace("\r", "");

            int styleHash = rawStyle.GetDeterministicHashCode();

            if (_styles.TryGetValue(styleHash, out string styleName))
            {

            }
            else
            {
                styleName = fobject.Data.Names.UssClassName;

                tempSb.Insert(0, $".{styleName} {{");
                tempSb.AppendLine($"}}");
                tempSb.AppendLine();

                styleBuilder1.Append(tempSb);
                _styles.Add(styleHash, styleName);
            }


            fobject.Data.XmlElement.SetAttribute("class", styleName);
        }

        private string CreateCornerGlobalStyle(FObject fobject)
        {
            StringBuilder styleBuilder = new StringBuilder();

            FRect rect = monoBeh.TransformSetter.GetGlobalRect(fobject);

            Vector4 radii;

            if (fobject.Type == NodeType.ELLIPSE)
            {
                int ev = 10000;
                radii = new Vector4(ev, ev, ev, ev);
            }
            else
            {
                radii = monoBeh.GraphicHelpers.GetCornerRadius(fobject).Round();
                radii[0] = radii[0].NormalizeAngleToSize(rect.size.x, rect.size.y);
                radii[1] = radii[1].NormalizeAngleToSize(rect.size.x, rect.size.y);
                radii[2] = radii[2].NormalizeAngleToSize(rect.size.x, rect.size.y);
                radii[3] = radii[3].NormalizeAngleToSize(rect.size.x, rect.size.y);
            }

            if (radii != new Vector4(0, 0, 0, 0))
            {
                styleBuilder.AddStyle("border-top-left-radius", $"{radii[0]}px");
                styleBuilder.AddStyle("border-top-right-radius", $"{radii[3]}px");
                styleBuilder.AddStyle("border-bottom-right-radius", $"{radii[2]}px");
                styleBuilder.AddStyle("border-bottom-left-radius", $"{radii[1]}px");
            }

            return styleBuilder.ToString();
        }

        private string CreateStrokeStyle(FObject fobject)
        {
            StringBuilder styleBuilder = new StringBuilder();

            FGraphic graphic = fobject.Data.Graphic;

            if (graphic.HasStroke)
            {
                int w = (int)fobject.StrokeWeight.Round(0);

                styleBuilder.AddStyle("border-left-width", $"{w}px");
                styleBuilder.AddStyle("border-right-width", $"{w}px");
                styleBuilder.AddStyle("border-top-width", $"{w}px");
                styleBuilder.AddStyle("border-bottom-width", $"{w}px");

                string rgba = graphic.Stroke.SingleColor.ToCssColor(graphic.Stroke.SingleColor.a);

                styleBuilder.AddStyle("border-left-color", $"{rgba}");
                styleBuilder.AddStyle("border-right-color", $"{rgba}");
                styleBuilder.AddStyle("border-top-color", $"{rgba}");
                styleBuilder.AddStyle("border-bottom-color", $"{rgba}");
            }

            return styleBuilder.ToString();
        }

        private void SetLocalStyle(FObject fobject)
        {
            StringBuilder styleBuilder = new StringBuilder();
            string baseStyle = CreateBaseLocalStyle(fobject);
            styleBuilder.Append(baseStyle);

            string imageStyle = this.ImageStyleBuilder.CreateImageLocalStyle(fobject);
            styleBuilder.Append(imageStyle);

            fobject.Data.XmlElement.SetAttribute("style", styleBuilder.ToString());
        }

        private void SetAutolayoutPropsForCurrent(FObject fobject, StringBuilder styleBuilder)
        {
            if (fobject.ContainsTag(FcuTag.AutoLayoutGroup))
            {
                var padding = fobject.Data.FRect.padding;
                styleBuilder.AddStyle("padding-left", $"{padding.left}px");
                styleBuilder.AddStyle("padding-right", $"{padding.right}px");
                styleBuilder.AddStyle("padding-top", $"{padding.top}px");
                styleBuilder.AddStyle("padding-bottom", $"{padding.bottom}px");

                if (fobject.LayoutMode == LayoutMode.VERTICAL)
                {
                    float spacing = fobject.GetVertSpacing();
                    fobject.Data.XmlElement.SetAttribute(GapContainer.gapY_name, $"{spacing.Round(0)}");
                    styleBuilder.AddLocalStyle("flex-direction", "column");

                    var childAlignment = fobject.GetVertLayoutAnchor().ToUITK(isHorizontal: false);

                    styleBuilder.AddLocalStyle("align-items", $"{childAlignment.alignItems}");
                    styleBuilder.AddLocalStyle("justify-content", $"{childAlignment.justifyContent}");

                }
                else if (fobject.LayoutMode == LayoutMode.HORIZONTAL)
                {
                    float spacing = fobject.GetHorSpacing();
                    fobject.Data.XmlElement.SetAttribute(GapContainer.gapX_name, $"{spacing.Round(0)}");
                    styleBuilder.AddLocalStyle("flex-direction", "row");

                    var childAlignment = fobject.GetHorLayoutAnchor().ToUITK(isHorizontal: true);

                    styleBuilder.AddLocalStyle("align-items", $"{childAlignment.alignItems}");
                    styleBuilder.AddLocalStyle("justify-content", $"{childAlignment.justifyContent}");
                }
            }
        }

        private void SetAutolayoutPropsForChild(FObject fobject, StringBuilder styleBuilder)
        {
            FObject parent = fobject.Data.Parent;

            FRect rect = fobject.Data.FRect;
            FRect pRect = parent.Data.FRect;

            if (parent.ContainsTag(FcuTag.AutoLayoutGroup))
            {
                styleBuilder.AddLocalStyle("position", "relative");

                if (parent.LayoutMode == LayoutMode.VERTICAL)
                {
                    if (fobject.LayoutGrow == 1)
                    {
                        styleBuilder.AddLocalStyle("height", $"auto");
                        styleBuilder.AddLocalStyle("flex-grow", $"1");
                    }
                    else
                    {
                        styleBuilder.AddLocalStyle("height", $"{rect.size.y.Round(0)}px");
                    }

                    if (fobject.LayoutAlign == LayoutAlign.STRETCH)
                    {
                        styleBuilder.AddLocalStyle("width", $"auto");
                        styleBuilder.AddLocalStyle("align-self", $"stretch");
                    }
                    else
                    {
                        styleBuilder.AddLocalStyle("width", $"{rect.size.x.Round(0)}px");
                    }
                }
                else if (parent.LayoutMode == LayoutMode.HORIZONTAL)
                {
                    if (fobject.LayoutGrow == 1)
                    {
                        styleBuilder.AddLocalStyle("width", $"auto");
                        styleBuilder.AddLocalStyle("flex-grow", $"1");
                    }
                    else
                    {
                        styleBuilder.AddLocalStyle("width", $"{rect.size.x.Round(0)}px");
                    }

                    if (fobject.LayoutAlign == LayoutAlign.STRETCH)
                    {
                        styleBuilder.AddLocalStyle("height", $"auto");
                        styleBuilder.AddLocalStyle("align-self", $"stretch");
                    }
                    else
                    {
                        styleBuilder.AddLocalStyle("height", $"{rect.size.y.Round(0)}px");
                    }
                }
            }
            else
            {
                styleBuilder.AddLocalStyle("position", "absolute");

                float left = rect.position.x - pRect.position.x;
                float top = rect.position.y - pRect.position.y;

                Vector2 strokeOffset = GetParentStrokeOffset(fobject);
                left -= strokeOffset.x;
                top -= strokeOffset.y;

                styleBuilder.AddLocalStyle("left", $"{left.Round(0)}px");
                styleBuilder.AddLocalStyle("top", $"{top.Round(0)}px");
                styleBuilder.AddLocalStyle("right", "auto");
                styleBuilder.AddLocalStyle("bottom", "auto");

                styleBuilder.AddLocalStyle("width", $"{rect.size.x.Round(0)}px");
                styleBuilder.AddLocalStyle("height", $"{rect.size.y.Round(0)}px");
            }
        }

        private static Vector2 GetParentStrokeOffset(FObject fobject) =>
            (fobject.Data.Parent.Data.Graphic?.HasStroke).ToBoolNullFalse() ? 
            new Vector2(fobject.Data.Parent.StrokeWeight, fobject.Data.Parent.StrokeWeight) : 
            Vector2.zero;

        private string CreateBaseLocalStyle(FObject fobject)
        {
            StringBuilder styleBuilder = new StringBuilder();

            FRect rect = monoBeh.TransformSetter.GetGlobalRect(fobject);
            fobject.Data.FRect = rect;

            SetAutolayoutPropsForCurrent(fobject, styleBuilder);
            SetAutolayoutPropsForChild(fobject, styleBuilder);

            if (fobject.IsFrameMask() || fobject.IsClipMask())
            {
                styleBuilder.AddStyle("overflow", "hidden");
            }
            else
            {
                bool find = false;

                foreach (int childIndex in fobject.Data.ChildIndexes)
                {
                    if (monoBeh.CurrentProject.TryGetByIndex(childIndex, out FObject childFO))
                    {
                        if (!childFO.IsMask.ToBoolNullFalse())
                            continue;

                        styleBuilder.AddStyle("overflow", "hidden");

                        if (fobject.IsSprite())
                        {
                            if (!fobject.Data.SpritePath.IsEmpty())
                            {
#if FCU_UITK_EXT_EXISTS
                                var kvp = monoBeh.UITK_Converter.BaseStyleBuilder.ImageStyleBuilder.GetSpriteLocalStyle(fobject);
                                styleBuilder.AddLocalStyle(kvp.Key, kvp.Value);

                                find = true;
                                break;
#endif
                            }
                        }
                    }
                }

                if (!find)
                {
                    styleBuilder.AddStyle("overflow", "visible");
                }
            }

            if (fobject.IsVisible() == false)
            {
                styleBuilder.AddLocalStyle("display", "none");
            }

            return styleBuilder.ToString();
        }

        [SerializeField] ImageStyleBuilder imageStyleBuilder;
        [SerializeProperty(nameof(imageStyleBuilder))]
        public ImageStyleBuilder ImageStyleBuilder => monoBeh.Link(ref imageStyleBuilder);

        [SerializeField] TextStyleBuilder textStyleBuilder;
        [SerializeProperty(nameof(textStyleBuilder))]
        public TextStyleBuilder TextStyleBuilder => monoBeh.Link(ref textStyleBuilder);
    }
}
#endif