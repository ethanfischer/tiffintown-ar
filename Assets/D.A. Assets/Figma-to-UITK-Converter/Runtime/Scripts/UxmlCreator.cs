#if FCU_EXISTS
using DA_Assets.FCU.Model;

using DA_Assets.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using UnityEngine;
using UnityEngine.UIElements;
using DA_Assets.DAI;
using DA_Assets.Logging;


#if UITK_LINKER_EXISTS
using DA_Assets.UEL;
#endif

namespace DA_Assets.FCU
{
    [Serializable]
    public class UxmlCreator : MonoBehaviourLinkerRuntime<FigmaConverterUnity>
    {
        private static string _projectName;
        private static string _ussName;
        private const string _uxmlExtension = "uxml";
        private const string _ussExtension = "uss";
        private const string _csExtension = "cs";

        public void Draw(FObject virtualPage)
        {
            _projectName = monoBeh.NameSetter.GetFcuName(virtualPage, FcuNameType.File);
            _ussName = monoBeh.NameSetter.GetFcuName(virtualPage, FcuNameType.Class);

            DALogger.Log(FcuLocKey.log_instantiate_game_objects.Localize());

            DrawProject(virtualPage);
        }

        private void DrawProject(FObject virtualPage)
        {
            int projectNumber = GetMaxProjectNumber(virtualPage);

            string outputFolder = Path.Combine(monoBeh.Settings.UITK_Settings.UitkOutputPath, $"{_projectName}-{projectNumber}");
            outputFolder.CreateFolderIfNotExists();

            string styleName = $"{_ussName}_Style_{projectNumber}.{_ussExtension}";
            string stylePath = Path.Combine(outputFolder, styleName);

            StringBuilder styleBuilder = new StringBuilder();

            foreach (FObject frame in virtualPage.Children)
            {
                FObject vPage = virtualPage;
                vPage.Children = new List<FObject>
                {
                    frame
                };

                string uxmlPath = CreateFrameUXML(vPage, styleBuilder, projectNumber, outputFolder, styleName);
                frame.Data.Names.UxmlPath = uxmlPath;
            }

            File.WriteAllText(stylePath, styleBuilder.ToString());
        }

        private string CreateFrameUXML(FObject virtualPage, StringBuilder styleBuilder, int projectNumber, string outputFolder, string styleName)
        {
            string frameName = virtualPage.Children.First().Data.Names.MethodName;
            string frameUxmlPath = Path.Combine(outputFolder, $"{frameName}-{projectNumber}.{_uxmlExtension}");
            string className = $"{frameName}_{projectNumber}";
            string scriptName = $"{className}.{_csExtension}";
            string scriptPath = Path.Combine(outputFolder, scriptName);

            XmlDocument doc = new XmlDocument();

            XmlElement root = CreateRootXmlElement(doc);
            virtualPage.Data.XmlElement = root;

            XmlElement styleElement = doc.CreateElement("Style");
            styleElement.SetAttribute("src", styleName);
            root.AppendChild(styleElement);

            DrawFObject(virtualPage, doc, styleBuilder, new List<FObject>());

            doc.AppendChild(root);
            doc.Save(frameUxmlPath);

            return frameUxmlPath;
        }

        private XmlElement CreateRootXmlElement(XmlDocument doc)
        {
            XmlElement root = doc.CreateElement("ui", "UXML", "UnityEngine.UIElements");
            root.SetAttribute("xmlns:uie", "UnityEditor.UIElements");

#if UITK_LINKER_EXISTS
            if (monoBeh.Settings.UITK_Settings.UitkLinkingMode == UitkLinkingMode.Guid || monoBeh.Settings.UITK_Settings.UitkLinkingMode == UitkLinkingMode.Guids)
            {
                Type type = typeof(UitkLinkerBase);
                root.SetAttribute("xmlns:uida", type.Namespace);
            }
#endif

            root.SetAttribute("xsi", "http://www.w3.org/2001/XMLSchema-instance");
            root.SetAttribute("engine", "UnityEngine.UIElements");
            root.SetAttribute("fcu", typeof(UxmlCreator).Namespace);//DA_Assets.FCU

            return root;
        }

        private void DrawFObject(FObject parent, XmlDocument doc, StringBuilder styleBuilder, List<FObject> drawn)
        {
            foreach (FObject fobject in parent.Children)
            {
                if (fobject.Data.IsEmpty)
                    continue;

                if (fobject.IsMask.ToBoolNullFalse())
                    continue;

                fobject.Data.UitkType = GetUitkType(fobject);
                fobject.Data.XmlElement = CreateXmlElement(fobject, doc);

                this.BaseStyleBuilder.SetStyle(fobject, styleBuilder);

                if (fobject.Data.Parent.Data.XmlElement != null)
                {
                    fobject.Data.Parent.Data.XmlElement.AppendChild(fobject.Data.XmlElement);
                }

                drawn.Add(fobject);

                if (fobject.Children.IsEmpty())
                    continue;

                DrawFObject(fobject, doc, styleBuilder, drawn);
            }
        }

        private XmlElement CreateXmlElement(FObject fobject, XmlDocument doc)
        {
#if UITK_LINKER_EXISTS
            Type type = typeof(UitkLinkerBase);

            if (monoBeh.Settings.UITK_Settings.UitkLinkingMode == UEL.UitkLinkingMode.Guid || monoBeh.Settings.UITK_Settings.UitkLinkingMode == UEL.UitkLinkingMode.Guids)
            {
                return doc.CreateElement("uida", fobject.Data.UitkType, type.Namespace);
            }
            else 
#endif 
            if (fobject.LayoutMode != LayoutMode.NONE)
            {
                return doc.CreateElement("fcu", fobject.Data.UitkType, "DA_Assets.FCU");
            }
            else
            {
                return doc.CreateElement("ui", fobject.Data.UitkType, "UnityEngine.UIElements");
            }
        }

        private int GetMaxProjectNumber(FObject virtualPage)
        {
            int maxNumber = 0;

            foreach (FObject frame in virtualPage.Children)
            {
                string frameName = frame.Data.Names.MethodName;
                int mn = AssetTools.GetMaxFileNumber(monoBeh.Settings.UITK_Settings.UitkOutputPath, frameName, _uxmlExtension);

                if (mn > maxNumber)
                {
                    maxNumber = mn;
                }
            }

            int newNumber = maxNumber + 1;
            return newNumber;
        }

        public string GetUitkType(FObject fobject)
        {
#if UITK_LINKER_EXISTS
            if (monoBeh.Settings.UITK_Settings.UitkLinkingMode == UEL.UitkLinkingMode.Guid)
            {
                if (fobject.Type == NodeType.TEXT)
                {
                    return nameof(LabelG);
                }
                else
                {
                    return nameof(VisualElementG);
                }
            }
            else
#endif
            {
                if (fobject.Type == NodeType.TEXT)
                {
                    return nameof(Label);
                }
                else if (fobject.LayoutMode != LayoutMode.NONE)
                {
                    return nameof(GapContainer);
                }
                else
                {
                    return nameof(VisualElement);
                }
            }
        }

        [SerializeField] BaseStyleBuilder _baseStyleBuilder;
        [SerializeProperty(nameof(_baseStyleBuilder))]
        public BaseStyleBuilder BaseStyleBuilder => monoBeh.Link(ref _baseStyleBuilder);
    }
}
#endif
