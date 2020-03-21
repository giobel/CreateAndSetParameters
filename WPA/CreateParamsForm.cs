using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WPA
{
    public partial class CreateParamsForm : Form
    {
        public List<string> ParametersNames { get; set; }
        public List<string> ParametersValues { get; set; }

        public List<string> SelectedCategories= new List<string>();

        public CreateParamsForm(List<string> categories)
        {
            InitializeComponent();

            for (int i = 0; i < checkedListBoxCategories.Items.Count; i++)
            {
                if (categories.Contains(checkedListBoxCategories.Items[i]))
                {
                    checkedListBoxCategories.SetItemChecked(i, true);
                }
            }

            comboBoxElementLOD.SelectedIndex = 0;
            comboBoxIsConstructed.SelectedIndex = 1;
            comboBoxElementSuitability.SelectedIndex = 0;
            comboBoxElementStatus.SelectedIndex = 0;
            comboBoxElementDiscipline.SelectedIndex = 0;
            comboBoxElementAuthor.SelectedIndex = 0;
            comboBoxDesignPackageNumber.SelectedIndex = 0;
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            #region Get Selected Categories

            foreach (string item in checkedListBoxCategories.CheckedItems)
            {
                SelectedCategories.Add(item);
            }

            #endregion

            #region Get Parameter Names
            ParametersNames = new List<string>();
            ParametersNames.Add(textBoxModelName.Text);
            ParametersNames.Add(textBoxDesignPackageNumber.Text);
            ParametersNames.Add(textBoxElementAuthor.Text);
            ParametersNames.Add(textBoxElementDiscipline.Text);
            ParametersNames.Add(textBoxElementStatus.Text);
            ParametersNames.Add(textBoxElementSuitability.Text);
            ParametersNames.Add(textBoxElementLOD.Text);
            ParametersNames.Add(textBoxIsConstructed.Text);
            ParametersNames.Add(textBoxSBSID.Text);
            ParametersNames.Add(textBoxHandover.Text);

            //additional parameters
            if (textBoxNew1.Text.Length > 3)
            {
                ParametersNames.Add(textBoxNew1.Text);
            }
            if (textBoxNew2.Text.Length > 3)
            {
                ParametersNames.Add(textBoxNew2.Text);
            }
            if (textBoxNew3.Text.Length > 3)
            {
                ParametersNames.Add(textBoxNew3.Text);
            }
            #endregion

            #region Get Parameter Values
            ParametersValues = new List<string>();
            ParametersValues.Add(textBoxModelNameValue.Text);
            ParametersValues.Add(comboBoxDesignPackageNumber.Text);
            ParametersValues.Add(comboBoxElementAuthor.Text);
            ParametersValues.Add(comboBoxElementDiscipline.Text);
            ParametersValues.Add(comboBoxElementStatus.Text);
            ParametersValues.Add(comboBoxElementSuitability.Text);
            ParametersValues.Add(comboBoxElementLOD.Text);
            ParametersValues.Add(comboBoxIsConstructed.Text);
            ParametersValues.Add(textBoxSBSIDValue.Text);
            ParametersValues.Add(textBoxHandoverValue.Text);

            //additional parameters
            if (textBoxValueNew1.Text.Length > 3)
            {
                ParametersValues.Add(textBoxValueNew1.Text);
            }
            if (textBoxValueNew2.Text.Length > 3)
            {
                ParametersValues.Add(textBoxValueNew2.Text);
            }
            if (textBoxValueNew3.Text.Length > 3)
            {
                ParametersValues.Add(textBoxValueNew3.Text);
            }
            #endregion


        }
    }
}
