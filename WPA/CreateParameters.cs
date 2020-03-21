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
#endregion

namespace WPA
{
    [Transaction(TransactionMode.Manual)]
    public class CreateParameters : IExternalCommand
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

            using (var form = new CreateParamsForm())
            {
                form.ShowDialog();

                if (form.DialogResult == System.Windows.Forms.DialogResult.Cancel)
                {
                    return Result.Cancelled;
                }

                StringBuilder sb = new StringBuilder();

                for (int i = 0; i < form.parametersNames.Count; i++)
                {
                    sb.AppendLine($"{form.parametersNames[i]} : {form.parametersValues[i]}");
                }


                

                TaskDialog.Show("r", sb.ToString());
            }
            


            return Result.Succeeded;
        }
    }
}
