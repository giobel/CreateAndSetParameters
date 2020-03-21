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
        public List<string> parametersNames { get; set; }
        public List<string> parametersValues { get; set; }

        public CreateParamsForm()
        {
            InitializeComponent();

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
            #region Get Parameter Names
            parametersNames = new List<string>();
            parametersNames.Add(textBoxModelName.Text);
            parametersNames.Add(textBoxDesignPackageNumber.Text);
            parametersNames.Add(textBoxElementAuthor.Text);
            parametersNames.Add(textBoxElementDiscipline.Text);
            parametersNames.Add(textBoxElementStatus.Text);
            parametersNames.Add(textBoxElementSuitability.Text);
            parametersNames.Add(textBoxElementLOD.Text);
            parametersNames.Add(textBoxIsConstructed.Text);
            parametersNames.Add(textBoxSBSID.Text);
            parametersNames.Add(textBoxHandover.Text);

            //additional parameters
            if (textBoxNew1.Text.Length > 3)
            {
                parametersNames.Add(textBoxNew1.Text);
            }
            if (textBoxNew2.Text.Length > 3)
            {
                parametersNames.Add(textBoxNew2.Text);
            }
            if (textBoxNew3.Text.Length > 3)
            {
                parametersNames.Add(textBoxNew3.Text);
            }
            #endregion

            #region Get Parameter Values
            parametersValues = new List<string>();
            parametersValues.Add(textBoxModelNameValue.Text);
            parametersValues.Add(comboBoxDesignPackageNumber.Text);
            parametersValues.Add(comboBoxElementAuthor.Text);
            parametersValues.Add(comboBoxElementDiscipline.Text);
            parametersValues.Add(comboBoxElementStatus.Text);
            parametersValues.Add(comboBoxElementSuitability.Text);
            parametersValues.Add(comboBoxElementLOD.Text);
            parametersValues.Add(comboBoxIsConstructed.Text);
            parametersValues.Add(textBoxSBSIDValue.Text);
            parametersValues.Add(textBoxHandoverValue.Text);

            //additional parameters
            if (textBoxValueNew1.Text.Length > 3)
            {
                parametersValues.Add(textBoxValueNew1.Text);
            }
            if (textBoxValueNew2.Text.Length > 3)
            {
                parametersValues.Add(textBoxValueNew2.Text);
            }
            if (textBoxValueNew3.Text.Length > 3)
            {
                parametersValues.Add(textBoxValueNew3.Text);
            }
            #endregion
        }
    }
}
