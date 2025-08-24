#if FCU_EXISTS
using DA_Assets.FCU.Model;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading.Tasks;
using DA_Assets.DAI;
using DA_Assets.FCU.Extensions;

#if UITK_LINKER_EXISTS
using DA_Assets.UEL;
#endif

#pragma warning disable IDE0003

namespace DA_Assets.FCU
{
    [Serializable]
    public class UITK_Converter : MonoBehaviourLinkerRuntime<FigmaConverterUnity>
    {
        public async Task Convert(FObject virtualPage, List<FObject> currPage)
        {
            BaseStyleBuilder.ClearStyles();

            await Task.Run(() =>
            {
                this.UxmlCreator.Draw(virtualPage);

            }, monoBeh.GetToken(TokenType.Import));

            monoBeh.CurrentProject.SetRootFrames(currPage);

#if UNITY_EDITOR
            UnityEditor.AssetDatabase.Refresh();
#endif

#if UITK_LINKER_EXISTS
            if (monoBeh.Settings.UITK_Settings.UitkLinkingMode != UitkLinkingMode.None)
            {
                monoBeh.CanvasDrawer.LocalizationDrawer.LocalizationDictionary.Clear();
                this.ComponentDrawer.Draw(virtualPage, currPage);
                monoBeh.CanvasDrawer.LocalizationDrawer.SaveAndConnectTable();
            }
#endif
        }

        [SerializeField] UxmlCreator uxmlCreator;
        [SerializeProperty(nameof(uxmlCreator))]
        public UxmlCreator UxmlCreator => monoBeh.Link(ref uxmlCreator);

        [SerializeField] BaseStyleBuilder baseStyleBuilder;
        [SerializeProperty(nameof(baseStyleBuilder))]
        public BaseStyleBuilder BaseStyleBuilder => monoBeh.Link(ref baseStyleBuilder);

#if UITK_LINKER_EXISTS
        [SerializeField] ComponentDrawer componentDrawer;
        [SerializeProperty(nameof(componentDrawer))]
        public ComponentDrawer ComponentDrawer => monoBeh.Link(ref componentDrawer);
#endif
    }
}
#endif
