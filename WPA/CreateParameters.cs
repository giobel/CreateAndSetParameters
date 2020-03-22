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

                StringBuilder sb = new StringBuilder();

                List<Category> selectedCategories = new List<Category>();

                Dictionary<string, string> parametersNamesAndValues = new Dictionary<string, string>();


                Categories allCategories = doc.Settings.Categories;
                

                foreach (string item in form.SelectedCategories)
                {
                    sb.AppendLine(item);

                    selectedCategories.Add( allCategories.get_Item(item) );
                
                }

                for (int i = 0; i < form.ParametersNames.Count; i++)
                {
                    //sb.AppendLine($"{form.ParametersNames[i]} : {form.ParametersValues[i]}");
                    sb.AppendLine($"{form.ParametersNames[i]}");
                    if (form.ParametersNames[i].Length > 3)
                    {
                        parametersNamesAndValues.Add(form.ParametersNames[i], form.ParametersValues[i]);
                    }
                }


                //if a parameter with the same is already in the project, throw an error

                int count = 0;

                using (Transaction t = new Transaction(doc, "Add parameters"))
                {
                    t.Start();

                    count = CreateProjectParameters(app, doc, selectedCategories, parametersNamesAndValues.Keys.ToList(), "WPA-Data");

                    t.Commit();
                }

                TaskDialog.Show("Result", $"{count} parameters added");

                using (Transaction t = new Transaction(doc, "Set parameters"))
                {
                    t.Start();

                    foreach (string categoryName in form.SelectedCategories)
                    {
                        Category cat = doc.Settings.Categories.get_Item(categoryName);

                        IList<Element> allElements = new FilteredElementCollector(doc).OfCategoryId(cat.Id).WhereElementIsNotElementType().ToElements();

                        foreach (Element element in allElements)
                        {
                            foreach (string param in parametersNamesAndValues.Keys)
                            {
                                element.LookupParameter(param).Set(parametersNamesAndValues[param]);
                            }
                        }
                    }

                    t.Commit();
                }



                //TaskDialog.Show("r", sb.ToString());
            }



            return Result.Succeeded;
        }

        private static int CreateProjectParameters(Application app, Document doc, List<Category> RevitCategories, List<string> Parameters, string GroupName)
        {
            int count = 0;
            CategorySet cats = app.Create.NewCategorySet();

            foreach (Category item in RevitCategories)
            {
                IList<Element> fec = new FilteredElementCollector(doc).OfCategoryId(item.Id).WhereElementIsNotElementType().ToElements();

                //only checks the first parameter..not very strong..NOT the best way to do it
                //if (fec.Count > 0 && fec.First().LookupParameter(Parameters.First()) == null)
                if (fec.Count > 0)
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

            FilteredElementCollector fecParameterElements = new FilteredElementCollector(doc).OfClass(typeof(ParameterElement));

            List<string> existingParametersName = new List<string>();

            foreach (ParameterElement item in fecParameterElements)
            {
                existingParametersName.Add(item.Name);
            }

            try
            {
                foreach (string name in Parameters)
                {
                    //for each category in cats, if the category has already a parameter, do not create a new one!
                    //make a copy of the list of category and remove the non required ones

                    CategorySet tempCategorySet = app.Create.NewCategorySet();

                    foreach (Category item in RevitCategories)
                    {
                        IList<Element> fec = new FilteredElementCollector(doc).OfCategoryId(item.Id).WhereElementIsNotElementType().ToElements();

                        if (fec.Count > 0 && fec.First().LookupParameter(name) == null)
                        {
                            tempCategorySet.Insert(item);
                        }
                    }

                    ExternalDefinition externalDefinition = groupName.Definitions.Create(new ExternalDefinitionCreationOptions(name, ParameterType.Text)) as ExternalDefinition;

                        InstanceBinding newInstanceBinding = app.Create.NewInstanceBinding(tempCategorySet); //cats

                        doc.ParameterBindings.Insert(externalDefinition, newInstanceBinding, BuiltInParameterGroup.PG_DATA);

                        count++;                        
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

            //File.Delete(tempSharedParameterFile);

            return count;
        }
    }
}
