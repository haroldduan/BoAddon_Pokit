using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BoAddonBiz_Pokit
{
    public class B1Menu_Pokit : SAPBoAddon.B1Menu
    {
        public B1Menu_Pokit()
        {
            this.myMenuEvent += new myMenuEventEventHandler(B1Menu_B1InSide_Delivery_myMenuEvent);
        }

        void B1Menu_B1InSide_Delivery_myMenuEvent(SAPBoAddon.B1AddonBase.B1MenuEvent pVal, ref bool BubbleEvent)
        {
            if (pVal.BeforeAction)
            {

            }
            else
            {
                if (pVal.MenuUID == B1MenuTypes.ado_FM_JournalEntry_Code_Generate)
                {
                    this.ShowForm(B1FormTypes.ado_FM_JournalEntry_Code_Generate, SBOApp.Forms.ActiveForm.UniqueID);
                }
            }
        }

        protected override void AddMenuItems()
        {
            #region Financials Management
            this.Menus.AddMenu(B1MenuTypes.ado_FM_JournalEntry_Code_Generate, "日记账凭单编码生成", SAPbouiCOM.BoMenuType.mt_STRING,
                B1MenuTypes.sys_Financials, 3);
            #endregion
        }
    }
}
