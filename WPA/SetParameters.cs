#region Namespaces
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System.Text;
using System.IO;
using System.Linq;
#endregion

namespace WPA
{
    [Transaction(TransactionMode.Manual)]
    public class SetParameters : IExternalCommand
    {
        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Application app = uiapp.Application;
            Document doc = uidoc.Document;

            Debug.Print("Start");


            //categories in the current model excluding tags and analytical
            List<string> documentCategories = new List<string>();

            foreach (Category item in doc.Settings.Categories)
            {
                if (!item.Name.Contains("Tags") && !item.Name.Contains("Analytical"))
                {
                    documentCategories.Add(item.Name);
                }

            }
                StringBuilder sb = new StringBuilder();



                return Result.Succeeded;

        }
    }
}
