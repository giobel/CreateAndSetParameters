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
    public class CreateSharedParameters : IExternalCommand
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
                FilteredElementCollector fec = new FilteredElementCollector(doc).OfCategoryId(item.Id);

                if (!item.Name.Contains("Tags") && !item.Name.Contains("Analytical") && fec.Count()>0)
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

                string count = "";

                using (Transaction t = new Transaction(doc, "Add parameters"))
                {
                    t.Start();

                    count = CreateProjectParameters(app, doc, selectedCategories, parametersNamesAndValues.Keys.ToList(), "WPA-Data");

                    t.Commit();
                }

                TaskDialog.Show("Result", count);

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
                                //check that the parameter exists first!
                                try
                                {
                                    element.LookupParameter(param).Set(parametersNamesAndValues[param]);
                                }
                                catch
                                {

                                }

                            }
                        }
                    }

                    t.Commit();
                }



                //TaskDialog.Show("r", sb.ToString());
            }



            return Result.Succeeded;
        }

        private static string CreateProjectParameters(Application app, Document doc, List<Category> RevitCategories, List<string> Parameters, string GroupName)
        {
            int newParameters = 0;
            int newCategories = 0;
            
            CategorySet tempCategorySet = app.Create.NewCategorySet();

            foreach (Category item in RevitCategories)
            {
                IList<Element> fec = new FilteredElementCollector(doc).OfCategoryId(item.Id).WhereElementIsNotElementType().ToElements();

                if (fec.Count > 0)
                {
                        tempCategorySet.Insert(item);
                    

                }
            }

            //get the parameter file
            string sharedParameterFile = doc.Application.SharedParametersFilename;

            DefinitionGroups existingGroups = app.OpenSharedParameterFile().Groups;

            DefinitionGroup groupName = existingGroups.get_Item(GroupName);

            if (existingGroups.get_Item(GroupName) == null)
            {
                groupName = existingGroups.Create(GroupName); //store under Data
            }

            try
            {
                foreach (string name in Parameters)
                {

                    if (!tempCategorySet.IsEmpty)
                    {
                        
                        //shared parameter does not exist
                        if (groupName.Definitions.get_Item(name) == null)
                        {
                            InstanceBinding newInstanceBinding = app.Create.NewInstanceBinding(tempCategorySet); //cats
                            
                            ExternalDefinition externalDefinition = groupName.Definitions.Create(new ExternalDefinitionCreationOptions(name, ParameterType.Text)) as ExternalDefinition;

                            doc.ParameterBindings.Insert(externalDefinition, newInstanceBinding, BuiltInParameterGroup.PG_DATA);

                            newParameters++;

                        }
                        //shared parameter exists
                        else
                        {
                            ExternalDefinition existingDefinition = groupName.Definitions.get_Item(name) as ExternalDefinition;

                            ElementBinding binding = doc.ParameterBindings.get_Item(existingDefinition) as ElementBinding;

                            //additional categories added to an existing shared parameter (re-insert)
                            if (binding != null)
                            {
                                foreach (Category item in binding.Categories)
                                {
                                    tempCategorySet.Insert(item);
                                }

                                InstanceBinding newInstanceBinding = app.Create.NewInstanceBinding(tempCategorySet); //cats

                                doc.ParameterBindings.ReInsert(existingDefinition, newInstanceBinding, BuiltInParameterGroup.PG_DATA);

                                newCategories++;
                            }
                            //shared parameter exists but has never been inserted (if the user undo the operation)
                            else
                            {
                                InstanceBinding newInstanceBinding = app.Create.NewInstanceBinding(tempCategorySet); //cats

                                doc.ParameterBindings.Insert(existingDefinition, newInstanceBinding, BuiltInParameterGroup.PG_DATA);

                                newCategories++;
                            }

                        }

                    }
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

            return $"Parameters created: {newParameters}\nParameters categories updated: {newCategories}";
        }
    }
}
