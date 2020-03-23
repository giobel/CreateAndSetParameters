#region Namespaces
using System;
using System.Collections.Generic;
using Autodesk.Revit.UI;
using System.Reflection;
using System.Windows.Media.Imaging;
using System.Windows.Forms;
#endregion

namespace WPA
{
    class App : IExternalApplication
    {
        public Result OnStartup(UIControlledApplication a)
        {

            try
            {
                a.CreateRibbonTab("WPA");

                RibbonPanel ifcParamPanel = GetSetRibbonPanel(a, "WPA", "IFC Parameters");
                
                if (AddPushButton(ifcParamPanel, "btnCreateParams", "Add Shared\nParameters", "", "pack://application:,,,/WPA;component/Resources/addParameters.png", "WPA.CreateSharedParameters", "Add the required shared parameters to the selected categories.") == false)
                {
                    MessageBox.Show("Failed to add button Create Parameters", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                
                if (AddPushButton(ifcParamPanel, "btnSetParams", "Set Parameters\nFrom Excel", "", "pack://application:,,,/WPA;component/Resources/addParameters.png", "WPA.SetParameters", "Set the selected shared parameters for the selected categories.") == false)
                {
                    MessageBox.Show("Failed to add button Create Parameters", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                return Result.Succeeded;
            }

            catch
            {
                return Result.Failed;
            }

            
        }

        public Result OnShutdown(UIControlledApplication a)
        {
            return Result.Succeeded;
        }

          private RibbonPanel GetSetRibbonPanel(UIControlledApplication application, string tabName, string panelName)
        {
            List<RibbonPanel> tabList = new List<RibbonPanel>();

            tabList = application.GetRibbonPanels(tabName);

            RibbonPanel tab = null;

            foreach (RibbonPanel r in tabList)
            {
                if (r.Name.ToUpper() == panelName.ToUpper())
                {
                    tab = r;
                }
            }

            if (tab is null)
                tab = application.CreateRibbonPanel(tabName, panelName);

            return tab;
        }


        ///<summary>
        ///Add a PushButton to the Ribbon Panel
        ///</summary>
        private Boolean AddPushButton(RibbonPanel Panel, string ButtonName, string ButtonText, string ImagePath16, string ImagePath32, string dllClass, string Tooltip)
        {

            string thisAssemblyPath = Assembly.GetExecutingAssembly().Location;

            try
            {
                PushButtonData m_pbData = new PushButtonData(ButtonName, ButtonText, thisAssemblyPath, dllClass);

                if (ImagePath16 != "")
                {
                    try
                    {
                        m_pbData.Image = new BitmapImage(new Uri(ImagePath16));
                    }
                    catch
                    {
                        //Could not find the image
                    }
                }
                if (ImagePath32 != "")
                {
                    try
                    {
                        m_pbData.LargeImage = new BitmapImage(new Uri(ImagePath32));
                    }
                    catch
                    {
                        //Could not find the image
                    }
                }

                m_pbData.ToolTip = Tooltip;


                PushButton m_pb = Panel.AddItem(m_pbData) as PushButton;

                return true;
            }
            catch
            {
                return false;
            }
        }

    }
}
