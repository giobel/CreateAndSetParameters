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

            //categories selected by default
           List<string> defaultSelectedCategories = new List<string>() { "Air Terminals", "Cable Trays", "Casework", "Ceilings",
            "Duct Accessories", "Duct Fittings", "Duct Systems", "Wires"};

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

                StringBuilder sb = new StringBuilder();

                List<Category> selectedCategories = new List<Category>();
                Dictionary<string, string> parametersNameAndValues = new Dictionary<string, string>();


                Categories allCategories = doc.Settings.Categories;
                

                foreach (string item in form.SelectedCategories)
                {
                    sb.AppendLine(item);

                    selectedCategories.Add( allCategories.get_Item(item) );
                
                }

                for (int i = 0; i < form.ParametersNames.Count; i++)
                {
                    sb.AppendLine($"{form.ParametersNames[i]} : {form.ParametersValues[i]}");
                    if (form.ParametersNames[i].Length > 3)
                    {
                        parametersNameAndValues.Add(form.ParametersNames[i], form.ParametersValues[i]);
                    }
                }


                //if a parameter with the same is already in the project, throw an error
                
                using (Transaction t = new Transaction(doc, "Add parameters"))
                {
                    t.Start();

                    CreateProjectParameters(app, doc, selectedCategories, parametersNameAndValues, "Data");

                    t.Commit();
                }


                //TaskDialog.Show("r", sb.ToString());
            }
            


            return Result.Succeeded;
        }

        private static void CreateProjectParameters(Application app, Document doc, List<Category> RevitCategories, Dictionary<string, string> Parameters, string GroupName)
        {
            CategorySet cats = app.Create.NewCategorySet();

            foreach (Category item in RevitCategories)
            {
                IList<Element> fec = new FilteredElementCollector(doc).OfCategoryId(item.Id).WhereElementIsNotElementType().ToElements();

                //only checks the first parameter..not very strong
                if (fec.Count > 0 && fec.First().LookupParameter(Parameters.First().Key) == null)
                {
                    cats.Insert(item);
                }
            }

            //get the parameter file
            string sharedParameterFile = doc.Application.SharedParametersFilename;
            string tempSharedParameterFile = @"C:\Temp\tempSharedParam.txt";

            using (File.Create(tempSharedParameterFile)) { }
            app.SharedParametersFilename = tempSharedParameterFile;

            DefinitionGroup groupName = app.OpenSharedParameterFile().Groups.Create(GroupName); //store under Data

            try
            {
                foreach (string name in Parameters.Keys)
                {
                        ExternalDefinition externalDefinition = groupName.Definitions.Create(new ExternalDefinitionCreationOptions(name, ParameterType.Text)) as ExternalDefinition;

                        InstanceBinding newInstanceBinding = app.Create.NewInstanceBinding(cats);

                        doc.ParameterBindings.Insert(externalDefinition, newInstanceBinding, BuiltInParameterGroup.PG_DATA);              
                }

            }
            catch(Exception ex)
            {
                TaskDialog.Show("Error", ex.Message);
            }
            finally
            {
                app.SharedParametersFilename = sharedParameterFile;
            }

            File.Delete(tempSharedParameterFile);
        }
    }
}
