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

            string messasge = "";

            //categories selected by default
            List<string> defaultSelectedCategories = new List<string>() { "Air Terminals", "Cable Trays", "Casework", "Ceilings",
            "Duct Accessories", "Duct Fittings", "Duct Systems", "Floors", "Wires"};

            //categories in the current model excluding tags and analytical
            List<string> documentCategories = new List<string>();

            foreach (Category item in doc.Settings.Categories)
            {
                if (!item.Name.Contains("Tags") && !item.Name.Contains("Analytical"))
                {
                    documentCategories.Add(item.Name);
                }

            }

            using (var form = new CreateParamsForm(documentCategories, defaultSelectedCategories))
            {

                form.ShowDialog();

                if (form.DialogResult == System.Windows.Forms.DialogResult.Cancel)
                {
                    return Result.Cancelled;
                }

                Categories allCategories = doc.Settings.Categories;

                List<Category> selectedCategories = new List<Category>();

                List<ElementId> builtInCats = new List<ElementId>();

                foreach (string item in form.SelectedCategories)
                {

                    builtInCats.Add(allCategories.get_Item(item).Id);

                }

                ElementMulticategoryFilter categoryFilter = new ElementMulticategoryFilter(builtInCats);

                IList<Element> fec = new FilteredElementCollector(doc).WherePasses(categoryFilter).WhereElementIsNotElementType().ToElements();


                using (Transaction t = new Transaction(doc, "Set parameters"))
                {
                    t.Start();

                    foreach (Element item in fec)
                    {
                        try
                        {
                            item.LookupParameter("Model Name").Set("Site");
                        }
                        catch (Exception ex)
                        {
                            message = ex.Message;
                        }

                    }
                    t.Commit();
                }

            }

            TaskDialog.Show("r", messasge);
                return Result.Succeeded;

        }
    }
}
