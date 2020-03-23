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


            using (var form = new FormOpenFile())
            {

                form.ShowDialog();

                if (form.DialogResult == System.Windows.Forms.DialogResult.Cancel)
                {
                    return Result.Cancelled;
                }

                Categories allCategories = doc.Settings.Categories;

                string inputFile = form.filePath;

                using (Transaction t = new Transaction(doc, "Update Data from Excel"))
                {

                    t.Start();


                    using (var reader = new StreamReader(inputFile))
                    {

                        List<string> parameters = reader.ReadLine().Split(',').ToList();

                        int mappingColumn = parameters.IndexOf("Package Specific Description");

                        string report = "";

                        while (!reader.EndOfStream)
                        {
                            var line = reader.ReadLine();

                            var values = line.Split(',').ToList();

                            IList<Element> elementsByCategory = new FilteredElementCollector(doc).OfCategoryId(allCategories.get_Item(values[mappingColumn]).Id).WhereElementIsNotElementType().ToElements();

                            int elementsUpdated = 0;
                            string tempParams = "";
                            string currentCategory = values[mappingColumn];

                            foreach (Element item in elementsByCategory)
                            {
                                
                                string tempTempParams = "";
                                for (int i = 0; i < parameters.Count; i++)
                                {
                                    if (parameters[i] != "Package Specific Description")
                                    {
                                        try
                                        {
                                            Parameter p = item.LookupParameter(parameters[i].Trim());
                                            p.Set(values[i]);
                                        }
                                        catch
                                        {
                                            tempTempParams += $"Parameter {parameters[i]} not found\n";
                                        }
                                    }
                                }
                                tempParams = tempTempParams;
                                elementsUpdated++;
                            }

                            
                            report += $"{elementsUpdated} elements in {currentCategory}\n Errors: {tempParams}";

                        }

                        TaskDialog.Show("Report", $"{report}");

                    }

                    t.Commit();
                }

                return Result.Succeeded;

            }
        }
    }
}
