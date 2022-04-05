using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WebservicesSage.Cotnroller;
using WebservicesSage.Singleton;


namespace WebservicesSage
{
    public partial class MappingExpeditionMode : Form
    {
        public MappingExpeditionMode()
        {
            InitializeComponent();
            SingletonUI.Instance.Transporteur = new Dictionary<string, string>();
            SingletonUI.Instance.DefaultExpedition = DefaultExpedition;
            SingletonUI.Instance.ExpeditionOrderDataGrid = ExpeditionOrderDataGrid;
            SingletonUI.Instance.ExpeditionSage1 = ExpeditionSage1;
            SingletonUI.Instance.ExpeditionSage2 = ExpeditionSage2;
            SingletonUI.Instance.ExpeditionSage3 = ExpeditionSage3;
            ControllerConfiguration.loadDefaultOrderMappingConfiguration();
            ControllerConfiguration.LoadAllOrderMappingConfiguration();
            
        }
        private void MappingExpeditionModeCloseButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }


        private void SaveExpidationMode_Click(object sender, EventArgs e)
        {
            Utils.UtilsConfig.UpdateNodeInCustomSection("OrderCarrierMapping", "default", DefaultExpedition.selectedValue.ToString());
            Utils.UtilsConfig.UpdateNodeInCustomSection("OrderCarrierMapping", "1", ExpeditionSage1.selectedValue.ToString());
            Utils.UtilsConfig.UpdateNodeInCustomSection("OrderCarrierMapping", "2", ExpeditionSage2.selectedValue.ToString());
            Utils.UtilsConfig.UpdateNodeInCustomSection("OrderCarrierMapping", "3", ExpeditionSage3.selectedValue.ToString());
        }

        private void CancelExpéditionMode_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
