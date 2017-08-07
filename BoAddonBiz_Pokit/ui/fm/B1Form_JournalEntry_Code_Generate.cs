/**
 *   AVATech Addon [Pokit]'s Function
 *   **************************************************************************************************
 *   
 *   File:      B1Form_JournalEntry_Code_Generate.cs
 *   
 *   Copyright (c) AVATech
 *   
 *   Create Date:2017-08-07
 *   Create Programmer:Harold Duan
 *   Create Reason:[日记账凭单编码生成]窗体页面控件功能源码
 *   
 *   **************************************************************************************************
**/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SAPBoAddon;

namespace BoAddonBiz_Pokit.ui.fm
{
    internal class B1Form_JournalEntry_Code_Generate : SAPBoAddon.B1Form
    {
        private const string GRID_SCHEME = "grid_schm", GRID_LIST = "grid_list",
            BTN_OK = "btn_ok", BTN_SEARCH = "btn_search", BTN_SAVE = "btn_save",
            BTN_GENERATE = "btn_gen", BTN_UNFOLD = "btn_unfold",
            BTN_SURE = "1", BTN_CANCEL = "2",
            EDT_DATE_FROM = "edt_dateF", EDT_DATE_TO = "edt_dateT", CBX_SCHEMECODE = "cbx_Code", EDT_FORMAT = "edt_Format",
            LAB_DATE = "lab_date", LAB_SAVE_SQL = "lab_savSql",
            RECT_1 = "rect_1", CMBX_CYCLE = "cbx_cycle",
            UDS_UNFOLD = "uds_unfold", UDS_SCHEMECODE = "uds_schmcd", UDS_FORMAT = "uds_format", UDS_CYCLE = "uds_cycle",
            UDS_DATE_FROM = "uds_dateF", UDS_DATE_TO = "uds_dateT";

        internal B1Form_JournalEntry_Code_Generate()
        {
            this.frm_ItemEvent += B1Form_JournalEntry_Code_Generate_frm_ItemEvent;
            GetDatetimeList();
        }

        private IList<KeyValuePair<int, KeyValuePair<DateTime, DateTime>>> date_time_list =
            default(IList<KeyValuePair<int, KeyValuePair<DateTime, DateTime>>>);
        private IList<Scheme> Scheme = default(IList<Scheme>);


        #region Events
        /// <summary>
        /// Item Events
        /// </summary>
        /// <param name="pVal"></param>
        /// <param name="BubbleEvent"></param>
        private void B1Form_JournalEntry_Code_Generate_frm_ItemEvent(SAPBoAddon.B1AddonBase.B1ItemEvent pVal, ref bool BubbleEvent)
        {
            if (pVal.BeforeAction) { }
            else
            {
                switch (pVal.EventType)
                {
                    case SAPbouiCOM.BoEventTypes.et_COMBO_SELECT:
                        if (pVal.ItemUID.Equals(CMBX_CYCLE))
                            SelectCycle();
                        else if (pVal.ItemUID.Equals(CBX_SCHEMECODE))
                        {
                            SAPbouiCOM.ComboBox cbxSchemes = this.CurrentForm.Items.Item(CBX_SCHEMECODE).Specific;
                            LoadScheme(cbxSchemes.Selected.Value);
                        }
                        break;
                    case SAPbouiCOM.BoEventTypes.et_LOST_FOCUS:
                        if (pVal.ItemUID.Equals(GRID_SCHEME) & pVal.Row >= 0)
                            GetCodeFormatterString(pVal.Row);
                        break;
                    case SAPbouiCOM.BoEventTypes.et_ITEM_PRESSED:
                        switch (pVal.ItemUID)
                        {
                            case BTN_SEARCH:
                                Search();
                                break;
                            case BTN_UNFOLD:
                                UnFold(
                                   this.CurrentForm.DataSources.UserDataSources.Item(UDS_UNFOLD).Value == "Y" ? false : true);
                                break;
                            case BTN_GENERATE:
                                Generate();
                                break;
                            case BTN_SAVE:
                                Save();
                                break;
                            case BTN_OK:
                                this.CurrentForm.Close();
                                break;
                        }
                        break;
                }
            }
        }
        #endregion

        #region B1Form 函数重载
        protected override bool BeforeSaveCheckFormData()
        {
            return true;
        }

        protected override void FormEditModeChange(frmItemEditMode frmEditMode)
        {

        }

        protected override void FormInfoSetting(B1FormSetting FormSetting)
        {
            FormSetting.FormType = B1FormTypes.ado_FM_JournalEntry_Code_Generate;
            FormSetting.FormFileName = FormSetting.FormType + "Form.srf";
        }

        protected override void InitializeForm(B1FormInitializePar InitPar)
        {
            Initialize(InitPar);
        }
        #endregion

        #region 界面逻辑处理
        private void GetDatetimeList()
        {
            try
            {
                var dt = DateTime.Now;  //当前时间
                var week_start = dt.AddDays(Convert.ToInt32(dt.DayOfWeek.ToString("d")) - 6);  //本周周一
                var week_end = week_start.AddDays(6);  //本周周日
                var last_week_start = week_start.AddDays(-7);  //上周周一
                var last_week_end = last_week_start.AddDays(6);  //上周周日
                var month_start = dt.AddDays(1 - dt.Day);  //本月月初
                var month_end = month_start.AddMonths(1).AddDays(-1);  //本月月末
                var last_month_start = month_start.AddMonths(-1);  //上月月初
                var last_month_end = month_start.AddDays(-1);  //上月月末
                if (date_time_list == null)
                {
                    date_time_list = new List<KeyValuePair<int, KeyValuePair<DateTime, DateTime>>>();
                    date_time_list.Add(new KeyValuePair<int, KeyValuePair<DateTime, DateTime>>
                        (-2, new KeyValuePair<DateTime, DateTime>(last_month_start, last_month_end)));
                    date_time_list.Add(new KeyValuePair<int, KeyValuePair<DateTime, DateTime>>
                        (-1, new KeyValuePair<DateTime, DateTime>(last_week_start, last_week_end)));
                    date_time_list.Add(new KeyValuePair<int, KeyValuePair<DateTime, DateTime>>
                        (1, new KeyValuePair<DateTime, DateTime>(week_start, week_end)));
                    date_time_list.Add(new KeyValuePair<int, KeyValuePair<DateTime, DateTime>>
                        (2, new KeyValuePair<DateTime, DateTime>(month_start, month_end)));
                }
            }
            catch (Exception ex) { throw ex; }
        }

        private void Initialize(B1FormInitializePar InitPar)
        {
            try
            {
                this.CurrentForm.Freeze(true);
                IList<Scheme> list = default(IList<Scheme>);
                Scheme = list;
                LoadSchemeCodes();
                this.CurrentForm.DataSources.UserDataSources.Item(UDS_CYCLE).Value = "1";
                this.CurrentForm.DataSources.UserDataSources.Item(UDS_DATE_FROM).Value
                    = date_time_list.Where(c => c.Key.Equals(1)).FirstOrDefault().Value.Key.ToString("yyyyMMdd");
                this.CurrentForm.DataSources.UserDataSources.Item(UDS_DATE_TO).Value
                    = date_time_list.Where(c => c.Key.Equals(1)).FirstOrDefault().Value.Value.ToString("yyyyMMdd");
                this.CurrentForm.Items.Item(EDT_DATE_FROM).Enabled = false;
                this.CurrentForm.Items.Item(EDT_DATE_TO).Enabled = false;
                UnFold();
            }
            catch (Exception ex) { throw ex; }
            finally { this.CurrentForm.Freeze(false); }
        }

        private void LoadSchemeCodes()
        {
            SAPbouiCOM.Form oForm = default(SAPbouiCOM.Form);
            try
            {
                oForm = this.CurrentForm;
                oForm.Freeze(true);
                SAPbobsCOM.Recordset oRs = SBOCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                string sql_GetSchemeCodes = @"select * from ""@AVA_FM_OCRS"" where ""U_Active"" = N'Y' ";
                oRs.DoQuery(sql_GetSchemeCodes);
                SAPbouiCOM.ComboBox cbxSchemes = oForm.Items.Item(CBX_SCHEMECODE).Specific;
                while (cbxSchemes.ValidValues.Count > 0)
                {
                    cbxSchemes.ValidValues.Remove(0, SAPbouiCOM.BoSearchKey.psk_Index);
                }
                for (int iRSRowCounter = 0; iRSRowCounter < oRs.RecordCount; iRSRowCounter++)
                {
                    cbxSchemes.ValidValues.Add(oRs.Fields.Item("Code").Value, oRs.Fields.Item("Name").Value);
                    oRs.MoveNext();
                }
                if (cbxSchemes.ValidValues.Count > 0)
                {
                    cbxSchemes.Select(0, SAPbouiCOM.BoSearchKey.psk_Index);
                    LoadScheme(cbxSchemes.Selected.Value);
                }
            }
            catch (Exception ex) { throw ex; }
            finally { oForm.Freeze(false); }

            #endregion
        }

        private void LoadScheme(string SchemeCode)
        {
            SAPbouiCOM.Form oForm = default(SAPbouiCOM.Form);
            try
            {
                oForm = this.CurrentForm;
                oForm.Freeze(true);
                SAPbouiCOM.Grid oGrid = oForm.Items.Item(GRID_SCHEME).Specific;
                oGrid.DataTable.Clear();
                SAPbobsCOM.Recordset oRs = SBOCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                string sql_GetScheme = @"select * from ""@AVA_FM_CRS1"" where ""Code"" = N'{0}'";
                sql_GetScheme = string.Format(sql_GetScheme, SchemeCode);
                oRs.DoQuery(sql_GetScheme);
                if (oRs.RecordCount > 0)
                {
                    IList<Scheme> schemes = new List<Scheme>();
                    Scheme temp = default(Scheme);
                    for (int iRSRowCounter = 0; iRSRowCounter < oRs.RecordCount; iRSRowCounter++)
                    {
                        temp = new Scheme(oRs.Fields.Item("U_RuleSchmsID").Value,
                            oRs.Fields.Item("U_RuleSchmsDesc").Value,
                            oRs.Fields.Item("U_RuleSchmsType").Value,
                            oRs.Fields.Item("U_RuleSchmsLen").Value,
                            oRs.Fields.Item("U_RuleSchmsDefault").Value,
                            oRs.Fields.Item("U_Padding").Value == "Y" ? true : false,
                            oRs.Fields.Item("U_PadString").Value);
                        oGrid.DataTable.Columns.Add(oRs.Fields.Item("U_RuleSchmsID").Value,
                            oRs.Fields.Item("U_RuleSchmsType").Value == "I" ? SAPbouiCOM.BoFieldsType.ft_Integer : SAPbouiCOM.BoFieldsType.ft_AlphaNumeric,
                            oRs.Fields.Item("U_RuleSchmsLen").Value);
                        oGrid.Columns.Item(oRs.Fields.Item("U_RuleSchmsID").Value).TitleObject.Caption = oRs.Fields.Item("U_RuleSchmsDesc").Value;
                        schemes.Add(temp);
                        oRs.MoveNext();
                    }
                    Scheme = schemes;
                    oGrid.DataTable.Rows.Add();
                    foreach (var item in schemes)
                    {
                        if (string.IsNullOrEmpty(item.RuleSchmsDefault)) continue;
                        else if (item.RuleSchmsDefault.ToUpper().Equals("NULL")) continue;
                        oGrid.DataTable.SetValue(item.RuleSchmsID, oGrid.DataTable.Rows.Offset, item.RuleSchmsDefault);
                    }
                    oGrid.RowHeaders.Width = 20;
                }
            }
            catch (Exception ex) { throw ex; }
            finally { oForm.Freeze(false); }
        }

        private void GetCodeFormatterString(int Row)
        {
            SAPbouiCOM.Form oForm = default(SAPbouiCOM.Form);
            try
            {
                oForm = this.CurrentForm;
                oForm.Freeze(true);
                SAPbouiCOM.Grid oGridScheme = oForm.Items.Item(GRID_SCHEME).Specific;
                IList<Scheme> schemes = Scheme;
                string formatterString = default(string), temp = default(string);
                if (schemes != null)
                {
                    oGridScheme.DataTable.Rows.Offset = Row;
                    foreach (var item in schemes)
                    {
                        temp = Convert.ToString(oGridScheme.DataTable.GetValue(item.RuleSchmsID, oGridScheme.DataTable.Rows.Offset));
                        temp = item.Padding ? temp.PadLeft(item.RuleSchmsLen, Convert.ToChar(item.PadString)) : temp;
                        formatterString += temp;
                    }
                }
                oForm.DataSources.UserDataSources.Item(UDS_FORMAT).Value = formatterString;
            }
            catch (Exception ex) { throw ex; }
            finally { oForm.Freeze(false); }
        }

        private void SelectCycle()
        {
            try
            {
                this.CurrentForm.Freeze(true);
                var cycle = int.Parse(this.CurrentForm.DataSources.UserDataSources.Item(UDS_CYCLE).Value);
                this.CurrentForm.DataSources.UserDataSources.Item(UDS_DATE_FROM).Value =
                    cycle == 0 ? string.Empty :
                    date_time_list.Where(c => c.Key.Equals(cycle)).FirstOrDefault().Value.Key.ToString("yyyyMMdd");
                this.CurrentForm.DataSources.UserDataSources.Item(UDS_DATE_TO).Value =
                    cycle == 0 ? string.Empty :
                    date_time_list.Where(c => c.Key.Equals(cycle)).FirstOrDefault().Value.Value.ToString("yyyyMMdd");
                this.CurrentForm.Items.Item(EDT_DATE_FROM).Enabled = cycle == 0 ? true : false;
                this.CurrentForm.Items.Item(EDT_DATE_TO).Enabled = cycle == 0 ? true : false;
            }
            catch (Exception ex) { throw ex; }
            finally { this.CurrentForm.Freeze(false); }
        }

        private void UnFold(bool unfold = false)
        {
            try
            {
                this.CurrentForm.Freeze(true);
                this.CurrentForm.DataSources.UserDataSources.Item(UDS_UNFOLD).Value = unfold ? "Y" : "N";
                SAPbouiCOM.Button btn_unfold = this.CurrentForm.Items.Item("btn_unfold").Specific;
                btn_unfold.Image = string.Format("{0}{1}.png", SAPBoAddon.B1Addon.B1Addon.FolderForm,
                    unfold ? "BUTTON_UNFOLD" : "BUTTON_FOLD");
                this.CurrentForm.Height = unfold ? 545 : 545 - 94;
                this.CurrentForm.Items.Item(CMBX_CYCLE).Top = unfold ? 128 : 128 - 94;
                this.CurrentForm.Items.Item(EDT_DATE_FROM).Top = unfold ? 143 : 143 - 94;
                this.CurrentForm.Items.Item(EDT_DATE_TO).Top = unfold ? 143 : 143 - 94;
                this.CurrentForm.Items.Item(LAB_DATE).Top = unfold ? 143 : 143 - 94;
                this.CurrentForm.Items.Item(BTN_SEARCH).Top = unfold ? 128 : 128 - 94;
                this.CurrentForm.Items.Item(BTN_GENERATE).Top = unfold ? 128 : 128 - 94;
                this.CurrentForm.Items.Item(GRID_LIST).Top = unfold ? 160 : 160 - 94;
                this.CurrentForm.Items.Item(LAB_SAVE_SQL).Top = unfold ? 481 : 481 - 94;
                this.CurrentForm.Items.Item(BTN_SAVE).Top = unfold ? 481 : 481 - 94;
                this.CurrentForm.Items.Item(BTN_OK).Top = unfold ? 486 : 486 - 94;
                this.CurrentForm.Items.Item(BTN_CANCEL).Top = unfold ? 486 : 486 - 94;
                this.CurrentForm.Items.Item(BTN_SURE).Top = unfold ? 486 : 486 - 94;
                this.CurrentForm.PaneLevel = unfold ? 2 : 1;
            }
            catch (Exception ex) { throw ex; }
            finally { this.CurrentForm.Freeze(false); }
        }

        private void Search()
        {
            try
            {
                this.CurrentForm.Freeze(true);
                SBOApp.StatusBar.SetText("正在查找，请稍后...", SAPbouiCOM.BoMessageTime.bmt_Medium, SAPbouiCOM.BoStatusBarMessageType.smt_Warning);
                var date_from = this.CurrentForm.DataSources.UserDataSources.Item(UDS_DATE_FROM).Value;
                var date_to = this.CurrentForm.DataSources.UserDataSources.Item(UDS_DATE_TO).Value;
                SAPbouiCOM.Grid grid_list = this.CurrentForm.Items.Item(GRID_LIST).Specific;
                var sql_get_journal_entry = @"select row_number() over(order by ""TransId"" asc) ""RowIndex"",* from ""AVA_FM_FN_GET_JOURNAL_ENTRY_DATAS""() where 1 = 1 ";
                if (!string.IsNullOrEmpty(date_from))
                    sql_get_journal_entry += string.Format(@" and ""TaxDate"" >= N'{0}' ", date_from);
                if (!string.IsNullOrEmpty(date_to))
                    sql_get_journal_entry += string.Format(@" and ""TaxDate"" <= N'{0}' ", date_to);
                grid_list.DataTable.ExecuteQuery(sql_get_journal_entry);
                if (grid_list.DataTable.Rows.Count == 1)
                {
                    var docentry = Convert.ToString(
                        grid_list.DataTable.GetValue("TransId", grid_list.DataTable.Rows.Offset));
                    if (string.IsNullOrWhiteSpace(docentry) |
                        int.Parse(docentry) <= 0)
                        grid_list.DataTable.Rows.Remove(grid_list.DataTable.Rows.Offset);
                }
                #region Grid Layout
                grid_list.Columns.Item("RowIndex").TitleObject.Caption = "#";
                SAPbouiCOM.EditTextColumn col_docentry = (SAPbouiCOM.EditTextColumn)grid_list.Columns.Item("TransId");
                col_docentry.Type = SAPbouiCOM.BoGridColumnType.gct_EditText;
                col_docentry.LinkedObjectType = "30";
                grid_list.Columns.Item("TransId").TitleObject.Caption = "交易号";
                grid_list.Columns.Item("TransType").TitleObject.Caption = "原始单据类型";
                grid_list.Columns.Item("BaseRef").TitleObject.Caption = "原始编号";
                grid_list.Columns.Item("TaxDate").TitleObject.Caption = "单据日期";
                grid_list.Columns.Item("BPLId").TitleObject.Caption = "分支";
                grid_list.Columns.Item("BPLName").TitleObject.Caption = "分支名称";
                grid_list.Columns.Item("Ref1").TitleObject.Caption = "参考 1";
                grid_list.Columns.Item("Ref2").TitleObject.Caption = "参考 2";
                grid_list.Columns.Item("Memo").TitleObject.Caption = "备注";
                grid_list.Columns.Item("Project").TitleObject.Caption = "项目";
                grid_list.Columns.Item("LocTotal").TitleObject.Caption = "交易总计";
                grid_list.Columns.Item("VoucherCode").TitleObject.Caption = "凭单编码";
                foreach (SAPbouiCOM.GridColumn item in grid_list.Columns)
                {
                    if (item.UniqueID.Equals("VoucherCode"))
                    {
                        item.Editable = true;
                        continue;
                    }
                    item.Editable = false;
                }
                #endregion
                SBOApp.StatusBar.SetText("查找完成！", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Success);
            }
            catch (Exception ex) { throw ex; }
            finally { this.CurrentForm.Freeze(false); }
        }
        private void Generate()
        {
            try
            {
                this.CurrentForm.Freeze(true);
                SBOApp.StatusBar.SetText("正在处理，请稍后...", SAPbouiCOM.BoMessageTime.bmt_Medium, SAPbouiCOM.BoStatusBarMessageType.smt_Warning);
                SAPbouiCOM.Grid grid_list = this.CurrentForm.Items.Item(GRID_LIST).Specific;
                if (grid_list.Rows.Count > 0)
                {
                    SAPbouiCOM.EditTextColumn col_voucher_code = (SAPbouiCOM.EditTextColumn)grid_list.Columns.Item("VoucherCode");
                    if (col_voucher_code.PickerType != SAPbouiCOM.BoPickerType.pt_Search)
                        throw new Exception("请在[凭单编码]列上绑定运算逻辑格式化搜索！");
                    for (int i = 0; i < grid_list.Rows.Count; i++)
                    {
                        grid_list.SetCellFocus(i, 2);
                        col_voucher_code.ClickPicker(i + 1);
                        SBOApp.StatusBar.SetText(string.Format("处理完成{0}/{1}！", i, grid_list.Rows.Count),
                            SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Success);
                    }
                }
                SBOApp.StatusBar.SetText("处理完成！", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Success);
            }
            catch (Exception ex) { throw ex; }
            finally { this.CurrentForm.Freeze(false); }
        }

        private void Save()
        {
            SAPbobsCOM.Recordset oRs = default(SAPbobsCOM.Recordset);
            try
            {
                this.CurrentForm.Freeze(true);
                SBOApp.StatusBar.SetText("正在保存，请稍后...", SAPbouiCOM.BoMessageTime.bmt_Medium, SAPbouiCOM.BoStatusBarMessageType.smt_Warning);
                SAPbouiCOM.Grid grid_list = this.CurrentForm.Items.Item(GRID_LIST).Specific;
                if (grid_list.DataTable.Rows.Count > 0)
                {
                    oRs = SBOCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                    var offset = default(int);
                    var trans_id = default(int);
                    var voucher_code = default(string);
                    var sql_save = default(string);
                    var count = grid_list.Rows.Count;
                    while (offset < grid_list.DataTable.Rows.Count)
                    {
                        grid_list.DataTable.Rows.Offset = offset;
                        trans_id = grid_list.DataTable.GetValue("TransId", grid_list.DataTable.Rows.Offset);
                        voucher_code = grid_list.DataTable.GetValue("VoucherCode", grid_list.DataTable.Rows.Offset);
                        sql_save = @"exec ""AVA_FM_SP_SAVE_JOURNAL_ENTRY_DATA"" {0},'{1}' ";
                        sql_save = string.Format(sql_save, trans_id, voucher_code);
                        oRs.DoQuery(sql_save);
                        if (oRs.Fields.Item(0).Value != 0) throw new Exception(oRs.Fields.Item(1).Value);
                        else
                        {
                            grid_list.DataTable.Rows.Remove(grid_list.DataTable.Rows.Offset);
                            SBOApp.StatusBar.SetText(string.Format("保存完成{0}/{1}！", count - grid_list.Rows.Count, count),
                                SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Success);
                        }
                    }
                }
                SBOApp.StatusBar.SetText("保存完成！", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Success);
            }
            catch (Exception ex) { SBOApp.StatusBar.SetText(ex.Message, SAPbouiCOM.BoMessageTime.bmt_Short); }
            finally
            {
                if (oRs != null) System.Runtime.InteropServices.Marshal.FinalReleaseComObject(oRs);
                this.CurrentForm.Freeze(false);
            }
        }
    }
    internal class Scheme
    {
        internal Scheme(string RuleSchmsID, string RuleSchmsDesc, string RuleSchmsType, int RuleSchmsLen, string RuleSchmsDefault = default(string),
            bool Padding = default(bool), string PadString = default(string))
        {
            this.RuleSchmsID = RuleSchmsID;
            this.RuleSchmsDesc = RuleSchmsDesc;
            this.RuleSchmsType = RuleSchmsType;
            this.RuleSchmsLen = RuleSchmsLen;
            this.RuleSchmsDefault = RuleSchmsDefault;
            this.Padding = Padding;
            this.PadString = PadString;
        }
        internal string RuleSchmsID { get; }
        internal string RuleSchmsDesc { get; }
        internal string RuleSchmsType { get; }
        internal int RuleSchmsLen { get; }
        internal string RuleSchmsDefault { get; }
        internal bool Padding { get; }
        internal string PadString { get; }
    }
}


