﻿using DevExpress.XtraEditors.Controls;
using DevExpress.XtraReports.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using static Report_Pro.Classes.Master;

namespace Report_Pro.CTR
{
    public partial class frm_Cash_Receipt : frm_Master
    {

        DAL.dbDataContext sdb = new DAL.dbDataContext();
        DAL.daily_transaction DailyTransaction;
        DAL.Sands_Detail SandDetail;
        DAL.serial_no serialNo;
        List<CurrencyInfo> currencies = new List<CurrencyInfo>();
        int currencyNo = 2;
        DAL.DataAccesslayer1 dal = new DAL.DataAccesslayer1();
       //DAL.dbDataContext sdb = new DAL.dbDataContext();


        public frm_Cash_Receipt()
        {
            InitializeComponent();

            


            paied_amount.DicemalDigits = Properties.Settings.Default.digitNo_;

            cmb_Pay.DataSource = dal.getDataTabl_1("SELECT * FROM pay_method");
            if (Properties.Settings.Default.lungh == "0")
            {
                cmb_Pay.DisplayMember = "Pay_name";
            }
            else
            {
                cmb_Pay.DisplayMember = "Pay_name_E";
            }
            cmb_Pay.ValueMember = "Pay_ID";
            cmb_Pay.SelectedIndex = -1;

            // cmb_Bank.DataSource = dal.getDataTabl_1("SELECT * FROM SHEEK_BANKS_TYPE");

            //if (Properties.Settings.Default.lungh == "0")
            //{
            //    cmb_Bank.DisplayMember = "BANK_NAME";
            //}
            //else
            //{
            //    cmb_Bank.DisplayMember = "BANK_NAME_E";
            //}
            //cmb_Bank.ValueMember = "BANK_NO";
            //cmb_Bank.SelectedIndex = -1;
            New();
            RefreshData();
        }

        public override void RefreshData()
        {
            using (var db = new DAL.dbDataContext())
            {

                if (_Languh == "0")
                {
                    glkp_bank.IntializeGlkpData(db.SHEEK_BANKS_TYPEs.Select(p => new { p.BANK_NO, p.BANK_NAME }), "BANK_NAME", "BANK_NO");
                }
                else
                {
                    glkp_bank.IntializeGlkpData(db.SHEEK_BANKS_TYPEs.Select(p => new { p.BANK_NO, BANK_NAME = p.BANK_NAME_E == null ? p.BANK_NAME : p.BANK_NAME_E }), "BANK_NAME", "BANK_NO");
                }

            }

                base.RefreshData();
        }


        public override void Save()
        {

            sanadValidity();

            int JorSer;
            if (AccSer_No.TextS.Contains('M'))
            {
                var Jor_ser = AccSer_No.TextS.Split('M');
                JorSer = Convert.ToInt32(Jor_ser[1]);
            }

            else
            {
                JorSer = Convert.ToInt32(AccSer_No.TextS);
            }



            for (int i = 0; i <= dgv2.Rows.Count - 1; i++)
            {
                DataGridViewRow DgRow = dgv2.Rows[i];
                if (DgRow.Cells[0].Value != null && DgRow.Cells[8].Value != null)
                {
                    var SandDetail = new DAL.Sands_Detail
                    {
                        ACC_YEAR = acc_year.Text,
                        BRANCH_code = txtStore_ID.Text,
                        ser_no = txt_sandNo.TextS,
                        SOURCE_CODE = txt_source_code.Text,
                        g_date = txt_sandDate.Value.Date,
                        Inv_No = Convert.ToInt32(DgRow.Cells[0].Value),
                        Inv_Date = Convert.ToDateTime(DgRow.Cells[2].Value),
                        Po_no = DgRow.Cells[3].Value.ToString(),
                        Inv_Amount = DgRow.Cells[4].Value.ToString().ToDecimal(),
                        Returened = DgRow.Cells[5].Value.ToString().ToDecimal(),
                        OldPaid = DgRow.Cells[6].Value.ToString().ToDecimal(),
                        OldBalance = DgRow.Cells[7].Value.ToString().ToDecimal(),
                        CurrentPaid = DgRow.Cells[8].Value.ToString().ToDecimal(),
                        NewBalance = DgRow.Cells[9].Value.ToString().ToDecimal(),
                        main_counter = DgRow.Index,
                        Inv_Transaction_Code = DgRow.Cells[11].Value.ToString(),
                        cyear = DgRow.Cells[1].Value.ToString(),
                        totalPaid = DgRow.Cells[10].Value.ToString().ToDecimal(),
                    };
                    if (SandDetail.ACC_YEAR == null && SandDetail.BRANCH_code == null && SandDetail.ser_no == null && SandDetail.SOURCE_CODE == null)
                    {
                        sdb.Sands_Details.InsertOnSubmit(SandDetail);

                        sdb.serial_nos.Where(p => p.ACC_YEAR == acc_year.Text && p.BRANCH_CODE == txtStore_ID.Text)
                        .ToList().ForEach(x => {
                            x.daily_sn_ser = Convert.ToInt32(txt_sandNo.TextS);
                            x.main_daily_ser = JorSer;
                            x.BOX_ED_SER = Convert.ToInt32(txt_sandNo.TextS);
                            });

                    }
                    else
                    {
                        sdb.Sands_Details.DeleteAllOnSubmit(sdb.Sands_Details.Where(x => x.ser_no == txt_sandNo.TextS && x.ACC_YEAR == acc_year.Text && x.BRANCH_code == txtStore_ID.Text && x.SOURCE_CODE == txt_source_code.Text));
                        sdb.Sands_Details.InsertOnSubmit(SandDetail);
                    }


                }

            }



           
            sdb.daily_transactions.DeleteAllOnSubmit(sdb.daily_transactions.Where(x => x.ser_no == AccSer_No.TextS && x.BRANCH_code == txtStore_ID.Text));
            sdb.daily_transactions.InsertOnSubmit(new DAL.daily_transaction() // Part
            {
                ACC_YEAR = acc_year.Text,
                ACC_NO = txtCashAcc.ID.Text,
                BRANCH_code = txtStore_ID.Text,
                ser_no = AccSer_No.TextS,
                COST_CENTER = Cost.ID.Text,
                meno = paied_amount.Value,
                loh = 0,
                balance = paied_amount.Value,
                g_date = txt_sandDate.Value.Date,
                sanad_no = txt_sandNo.TextS,
                user_name = Program.userID,
                desc2 = txtDescr.Text + " -سند رقم "+ txt_sandNo.TextS,
                POASTING = false,
                CAT_CODE = "1",
                MAIN_SER_NO = JorSer,
                Wh_Branch_Code = txtStore_ID.Text,
                SANAD_TYPE = Payment_Type.Text,
                SANAD_TYPE2 = txt_sanad_type2.Text,
                desc_E = txtDescr_E.Text + " -Sanad No " + txt_sandNo.TextS,
                SOURCE_CODE = txt_source_code.Text,
                sheek_or_cash = cheuqeOrCash.Text,
                Sheek = Convert.ToString(cmb_Pay.SelectedValue),
                sp_ser_no = txtSpecialNo.Text,
                sheek_no = txt_Check.Text,
                sheek_bank = Convert.ToString(glkp_bank.EditValue),
                sheek_date = txt_source_code.Text == "S" ? Check_Date.Value.Date : (DateTime)System.Data.SqlTypes.SqlDateTime.Null,
                notes = txtCust.Text,
                
            });
            sdb.daily_transactions.InsertOnSubmit(new DAL.daily_transaction() // Part
            {
                ACC_YEAR = acc_year.Text,
                ACC_NO = Acc_Cr.ID.Text,
                BRANCH_code = txtStore_ID.Text,
                ser_no = AccSer_No.TextS,
                COST_CENTER = Cost.ID.Text,
                meno = 0,
                loh = paied_amount.Value,
                balance = -paied_amount.Value,
                g_date = txt_sandDate.Value.Date,
                sanad_no = txt_sandNo.TextS,
                user_name = Program.userID,
                desc2 = txtDescr.Text + " -سند رقم " + txt_sandNo.TextS,
                POASTING = false,
                CAT_CODE = "1",
                MAIN_SER_NO = JorSer,
                Wh_Branch_Code = txtStore_ID.Text,
                SANAD_TYPE = Payment_Type.Text,
                SANAD_TYPE2 = txt_sanad_type2.Text,
                desc_E = txtDescr_E.Text + " -Sanad No " + txt_sandNo.TextS,
                SOURCE_CODE = txt_source_code.Text,
                sheek_or_cash = cheuqeOrCash.Text,
                Sheek = Convert.ToString(cmb_Pay.SelectedValue),
                sp_ser_no = txtSpecialNo.Text,
                sheek_no = txt_Check.Text,
                sheek_bank = Convert.ToString(glkp_bank.EditValue),

                sheek_date = txt_source_code.Text == "S" ? Check_Date.Value.Date : (DateTime)System.Data.SqlTypes.SqlDateTime.Null,
                notes = txtCust.Text,
            });

           
                    



                sdb.SubmitChanges();
            base.Save();
        }

        public static DataTable ToDataTable<T>(IList<T> data)
        {
            PropertyDescriptorCollection properties =
                TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();
            foreach (PropertyDescriptor prop in properties)
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            foreach (T item in data)
            {
                DataRow row = table.NewRow();
                foreach (PropertyDescriptor prop in properties)
                    row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                table.Rows.Add(row);
            }
            return table;
        }


        void LoadSanad(string SanadNo, string SanadYear, string SanadBranch, string SanadSource)
        {
            dgv2.Rows.Clear();
            //using (var db = new DAL.dbDataContext())
            //{
            var _sandDetail = (from d in sdb.Sands_Details.Where(x => x.ser_no == SanadNo && x.ACC_YEAR == SanadYear && x.BRANCH_code == SanadBranch && x.SOURCE_CODE == SanadSource)
                               select new
                               {
                                   d.Inv_No,
                                   d.ACC_YEAR,
                                   d.Inv_Date,
                                   d.Po_no,
                                   d.Inv_Amount,
                                   d.Returened,
                                   d.OldPaid,
                                   d.OldBalance,
                                   d.CurrentPaid,
                                   d.NewBalance,
                                   d.Inv_Transaction_Code,
                                   d.cyear,
                                   d.totalPaid,
                               }).ToList();

            //from d in db.InvoiceDetails.Where(x => x.InoiceID == inv.ID)

            DataTable dt = ToDataTable(_sandDetail);



            if (dt.Rows.Count == 0)
                return;

            foreach (DataRow row in dt.Rows)
            {
                DataGridViewRow dgvr = dgv2.Rows[dgv2.Rows.Add()];
                dgvr.Cells[col_invNo.Index].Value  = row["Inv_No"];
                dgvr.Cells[col_year.Index].Value = row["cyear"];
                dgvr.Cells[col_invDate.Index].Value         = row["Inv_Date"];
                dgvr.Cells[col_poNo.Index].Value            = row["Po_no"];
                dgvr.Cells[col_invAmount.Index].Value       = row["Inv_Amount"];
                dgvr.Cells[col_retruned.Index].Value        = row["Returened"];
                dgvr.Cells[col_oldPaid.Index].Value         = row["OldPaid"];
                dgvr.Cells[col_oldBalance.Index].Value      = row["OldBalance"];
                dgvr.Cells[col_currentPaid.Index].Value     = row["CurrentPaid"];
                dgvr.Cells[col_newBalance.Index].Value      = row["NewBalance"];
                dgvr.Cells[col_transactionCode.Index].Value = row["Inv_Transaction_Code"];
                dgvr.Cells[col_sanadBalance.Index].Value    = row["totalPaid"];


            }


            GetData();
        }
        



        public override void SetData()
        {


            //DailyTransaction.ACC_YEAR = acc_year.Text;
            //DailyTransaction.ACC_NO = txtCashAcc.ID.Text;
            //DailyTransaction.BRANCH_code = txtStore_ID.Text;
            //DailyTransaction.ser_no = AccSer_No.TextS;
            //DailyTransaction.COST_CENTER = Cost.ID.Text;
            //DailyTransaction.meno = paied_amount.Value;
            //DailyTransaction.loh = 0;
            //DailyTransaction.balance = paied_amount.Value;
            //DailyTransaction.g_date = txt_sandDate.Value.Date;
            //DailyTransaction.sanad_no = txt_sandNo.TextS;
            //DailyTransaction.user_name = Program.userID;
            //DailyTransaction.desc2 = txtDescr.Text;
            //DailyTransaction.POASTING = false;
            //DailyTransaction.CAT_CODE = "1";
            //DailyTransaction.MAIN_SER_NO = JorSer;
            //DailyTransaction.Wh_Branch_Code = txtStore_ID.Text;
            //DailyTransaction.SANAD_TYPE2 = txt_sanad_type2.Text;
            //DailyTransaction.desc_E = txtDescr_E.Text;
            //DailyTransaction.SOURCE_CODE = txt_source_code.Text;
            //DailyTransaction.sheek_or_cash = cheuqeOrCash.Text;
            //DailyTransaction.Sheek = Convert.ToString(cmb_Pay.SelectedValue);
            //DailyTransaction.sp_ser_no = txt_source_code.Text;
            //DailyTransaction.sheek_no = txt_Check.Text;
            //DailyTransaction.sheek_bank = Convert.ToString(cmb_Bank.SelectedValue);
            //DailyTransaction.sheek_date = Check_Date.Value.Date;
            //DailyTransaction.notes = txtCust.Text;



















            base.SetData();
        }

        public override void GetData()
        {
                       base.GetData();
        }


        public override void New()
        {

            ClearTextBoxes();

            txt_sandDate.Value = DateTime.Today;
            acc_year.Text = "cy";
            Payment_Type.Text = "2";
            user_id.Text = Program.userID;
            txtStore_ID.Text = Properties.Settings.Default.BranchId;
            txt_source_code.Text = "CR";
            txtCashAcc.ID.Text = dal.getDataTabl_1(@"select Cash_acc_no from Wh_branches where branch_code='" + Properties.Settings.Default.BranchId + "' ").Rows[0][0].ToString();
            dgv1.Rows.Clear();
            dgv2.Rows.Clear();
            getJorSer();

            base.New();
        }

        private void frm_recet_Load(object sender, EventArgs e)
        {
           // New();

            btn_Print.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
           
            currencies.Add(new CurrencyInfo(CurrencyInfo.Currencies.Syria));
            currencies.Add(new CurrencyInfo(CurrencyInfo.Currencies.UAE));
            currencies.Add(new CurrencyInfo(CurrencyInfo.Currencies.s));
            currencies.Add(new CurrencyInfo(CurrencyInfo.Currencies.Tunisia));
            currencies.Add(new CurrencyInfo(CurrencyInfo.Currencies.Gold));
            currencies.Add(new CurrencyInfo(CurrencyInfo.Currencies.Bahrain));
            currencies.Add(new CurrencyInfo(CurrencyInfo.Currencies.Oman));

            switch (Properties.Settings.Default.Currency)
            {
                case "s":
                    currencyNo = 2;
                    break;
                case "BH":
                    currencyNo = 5;
                    break;
                case "OM":
                    currencyNo = 6;
                    break;
                case "DR":
                    currencyNo = 1;
                    break;
            }

            Acc_Cr.txtFinal.Text = "1";
            Acc_Cr.txtMainAcc.Text = dal.GetCell_1(@"select Costmers_acc_no from wh_BRANCHES where Branch_code= '"+Properties.Settings.Default.BranchId+"' ").ToString();
            txtCashAcc.ID.Text = dal.GetCell_1(@"select Cash_acc_no from wh_BRANCHES where Branch_code= '" + Properties.Settings.Default.BranchId + "' ").ToString();

        }

        private void BSave_Click(object sender, EventArgs e)
        {
            sanadValidity();
                   Add_Jor();
            

        }

        private void sanadValidity()
        {

            if (paied_amount.Value <= 0)
            {
                MessageBox.Show("فضلا.. تاكد من مبلغ السند", "تنبية !!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (cmb_Pay.SelectedIndex < 0)
            {
                MessageBox.Show("فضلا.. تاكد من طريقة السداد", "تنبية !!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }


            if (Acc_Cr.ID.Text == "")
            {
                MessageBox.Show("فضلا.. تاكد من الحساب ", "تنبية !!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (txtCashAcc.ID.Text == "")
            {
                MessageBox.Show("فضلا.. تاكد من حساب النقدية / البنك ", "تنبية !!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (Cost.ID.Text == "")
            {
                MessageBox.Show("فضلا.. تاكد من مركز التكلفة ", "تنبية !!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (txtCust.Text == "")
            {
                MessageBox.Show("فضلا.. تاكد من اسم العميل ", "تنبية !!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (txtDescr.Text == "")
            {
                MessageBox.Show("فضلا.. تاكد من البيان ", "تنبية !!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }


        }



        private void Add_sands()
        {
            //getJorSer();
            //dal.Execute_1(@"Insert into Sands_tbl values( '" + acc_year.Text + "', '" + Acc_Cr.ID.Text + "','"
            // + txtStore_ID.Text + "', '" + txt_sandNo.TextS + "','" + Cost.ID.Text + "',0, '" + paied_amount.Text + "','" + txt_sandDate.Value.Date.ToString("yyyy/MM/dd")
            //+ "' , '" + AccSer_No.TextS + "','" + Payment_Type.Text + "','" + user_id.Text + "','"+ txtDescr.Text + "', '" + txt_Check.Text + "' ,'" +
            //Convert.ToString(cmb_Bank.SelectedValue) + "','" + (Check_Date.Value > Check_Date.MinDate ? Check_Date.Value.Date.ToString("yyyy/MM/dd") : Check_Date.MinDate.Date.ToString("yyyy/MM/dd"))
            //+ "','" + Convert.ToString(cmb_Pay.SelectedValue) + "','','','"+ txt_source_code.Text+"','" + txtStore_ID.Text 
            //+ "','','" + txtCashAcc.ID.Text + "','','" + paied_amount.Text + "','"+ txtCust.Text +"','','','','','','','','','','','','','')");


            //dal.Execute_1(@"UPDATE serial_no SET BOX_ED_SER='" + txt_sandNo.TextS + "' WHERE BRANCH_CODE=  '" + txtStore_ID.Text + "' and ACC_YEAR='" + acc_year.Text + "' ");


    }

        private void Add_Jor()
        {
            ////string cyear = txt_InvDate.Value.Year.ToString();
            //string H_Date;
            //// DataTable dtCurrntdate_ =  dal.getDataTabl("convertdate_G", txt_sandDate.Value);
            //H_Date = dal.convertToHijri(txt_sandDate.Value.ToString());
            ////this.AccSer_No.TextS = dal.getDataTabl("get_ser", Properties.Settings.Default.BranchId, txt_InvDate.Value.Year.ToString(), "ENT").Rows[0][0].ToString().PadLeft(4, '0');

            ////==================////=============================////==============


            //int JorSer;
            //if (AccSer_No.TextS.Contains('M'))
            //{
            //    var Jor_ser = AccSer_No.TextS.Split('M');
            //    JorSer = Convert.ToInt32(Jor_ser[1]);
            //}

            //else
            //{
            //    JorSer = Convert.ToInt32(AccSer_No.TextS);
            //}


            //if (dal.sqlconn_1.State == ConnectionState.Closed)
            //{
            //    dal.sqlconn_1.Open();
            //}
            //SqlCommand cmd = dal.sqlconn_1.CreateCommand();
            //SqlTransaction trans;
            //trans = dal.sqlconn_1.BeginTransaction();
            //cmd.Connection = dal.sqlconn_1;
            //cmd.Transaction = trans;


            //try
            //{

            //    cmd.CommandText = @"INSERT INTO daily_transaction(ACC_YEAR, ACC_NO, BRANCH_code, ser_no, COST_CENTER, meno, loh
            //        , balance, g_date, sanad_no, SANAD_TYPE, user_name, desc2, POASTING, CAT_CODE, MAIN_SER_NO,Wh_Branch_Code
            //        ,SANAD_TYPE2,desc_E,SOURCE_CODE,sheek_or_cash,Sheek,sp_ser_no,sheek_no,sheek_bank,sheek_date,notes)
            //        VALUES('" + acc_year.Text + "','" + txtCashAcc.ID.Text + "','" + txtStore_ID.Text + "','" +
            //        AccSer_No.TextS + "','"+Cost.ID.Text+"','" + paied_amount.Value + "','0','" + paied_amount.Value + "','" + txt_sandDate.Value.ToString("yyyy/MM/dd HH:mm:ss") +
            //        "','" + txt_sandNo.TextS + "','2','" + Program.userID + "','" + txtDescr.Text + "','0','1','" + JorSer + "','"+txtStore_ID.Text+"','"+ 
            //        txt_sanad_type2.Text+ "','"+txtDescr_E.Text+ "','" + txt_source_code.Text + "','"+cheuqeOrCash.Text+"','"+ 
            //        Convert.ToString(cmb_Pay.SelectedValue)+"','"+ txt_source_code.Text + "','"+txt_Check.Text+"','"+ 
            //        Convert.ToString(cmb_Bank.SelectedValue)+ "',   '" + (Check_Date.Value.ToString("yyyy/MM/dd") == null ? string.Empty : Check_Date.Value.ToString("yyyy/MM/dd")) + "','" + txtCust.Text + "')";
            //    cmd.ExecuteNonQuery();



            //    cmd.CommandText = @"INSERT INTO daily_transaction(ACC_YEAR, ACC_NO, BRANCH_code, ser_no, COST_CENTER, meno, loh
            //        , balance, g_date, sanad_no, SANAD_TYPE, user_name, desc2, POASTING, CAT_CODE, MAIN_SER_NO,Wh_Branch_Code
            //        ,SANAD_TYPE2,desc_E,SOURCE_CODE,sheek_or_cash,Sheek,sp_ser_no,sheek_no,sheek_bank,sheek_date,notes)
            //        VALUES('" + acc_year.Text + "','" + Acc_Cr.ID.Text + "','" + txtStore_ID.Text + "','" +
            //        AccSer_No.TextS + "','"+Cost.ID.Text+"','0','" + paied_amount.Value + "','" + -paied_amount.Value + "','" + txt_sandDate.Value.ToString("yyyy/MM/dd HH:mm:ss") +
            //        "','" + txt_sandNo.TextS + "','2','" + Program.userID + "','" + txtDescr.Text + "','0','1','" + JorSer + "','" + txtStore_ID.Text + "','" + 
            //        txt_sanad_type2.Text + "','" + txtDescr_E.Text + "','"+ txt_source_code.Text+ "','" + cheuqeOrCash.Text + "','" + 
            //        Convert.ToString(cmb_Pay.SelectedValue) + "','" + txt_source_code.Text + "','" + txt_Check.Text + "','" + 
            //        Convert.ToString(cmb_Bank.SelectedValue) + "',  '" + (Check_Date.Value.ToString("yyyy/MM/dd") == null? string.Empty : Check_Date.Value.ToString("yyyy/MM/dd")) + "','" + txtCust.Text+"')";
            //    cmd.ExecuteNonQuery();
                

            //    for (int i = 0; i <= dgv2.Rows.Count - 1; i++)
            //    {
            //        DataGridViewRow DgRow = dgv2.Rows[i];
            //        if (DgRow.Cells[0].Value != null && DgRow.Cells[8].Value != null)
            //        {
            //            cmd.CommandText = @"INSERT INTO Sands_Details(ACC_YEAR,BRANCH_code,ser_no,SOURCE_CODE,g_date,Inv_No
            //            ,Inv_Date,Po_no,Inv_Amount,Returened,OldPaid,OldBalance,CurrentPaid,NewBalance,main_counter,Inv_Transaction_Code)
            //            Values ('" + acc_year.Text + "','" + txtStore_ID.Text + "','" + txt_sandNo.TextS + "', '" +
            //            txt_source_code.Text + "','" + txt_sandDate.Value.ToString("yyyy/MM/dd HH: mm:ss") + "','" + DgRow.Cells[0].Value + "','" +
            //            (Convert.ToDateTime(DgRow.Cells[2].Value)).ToString("yyyy/MM/dd HH: mm:ss") + "','" + DgRow.Cells[3].Value + "','" + DgRow.Cells[4].Value + "','" + DgRow.Cells[5].Value + "','" +
            //            DgRow.Cells[6].Value + "','" + DgRow.Cells[7].Value + "','" + DgRow.Cells[8].Value + "','" + DgRow.Cells[9].Value + "','" + 
            //            DgRow.Index + "','" + DgRow.Cells[11].Value + "')";

            //            cmd.ExecuteNonQuery();

            //            cmd.CommandText = @"update wh_inv_data 
            //                set PanyedAmount = '" + DgRow.Cells[8].Value +
            //                "' where  Ser_no = '"+DgRow.Cells[0].Value + 
            //                "' and Branch_code = '"+txtStore_ID.Text + 
            //                "' and Transaction_code = '"+DgRow.Cells[11].Value + 
            //                "' and Cyear='" + DgRow.Cells[1].Value+"' ";
            //            cmd.ExecuteNonQuery();

            //        }
            //    }


            //    cmd.CommandText = @"UPDATE serial_no SET daily_sn_ser='" + txt_sandNo.TextS + "' , main_daily_ser = '" + JorSer + "',BOX_ED_SER='" + txt_sandNo.TextS + "' WHERE BRANCH_CODE=  '" + txtStore_ID.Text + "' and ACC_YEAR='" + acc_year.Text + "' ";
            //    cmd.ExecuteNonQuery();


            //    trans.Commit();
            //    BSave.Enabled = false;
            //    MessageBox.Show("تم الحفظ بنجاح", "حفظ ", MessageBoxButtons.OK, MessageBoxIcon.Information);



            //}
            //catch (Exception ex)
            //{

                    
            //    trans.Rollback();
            //    MessageBox.Show(ex.Message);
            //}
            //finally
            //{
            //    dal.sqlconn_1.Close();
            //}



        }

        //        //---


        //        // UpdateJor();









        //    }














        //    //=====================///====================///===========

        //    //     }




        //    dal.Execute("Add_daily_transaction",
        //    acc_year.Text,
        //    txtCashAcc.ID.Text,
        //    txtStore_ID.Text,
        //    AccSer_No.TextS,
        //    "",
        //    "",
        //    "",
        //    paied_amount.Text,
        //    0,
        //    paied_amount.Text,
        //    H_Date,
        //    txt_sandDate.Value,
        //    txt_sandNo.TextS,
        //    txt_source_code.Text,
        //    txt_source_code.Text + txt_sandNo.TextS,
        //    user_id.Text,
        //    txtDescr.Text,
        //    DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value,
        //    DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value,
        //    DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value,
        //    String.Empty, DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value,
        //    DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value,
        //   txt_source_code.Text, txtStore_ID.Text, DBNull.Value, DBNull.Value, DBNull.Value,
        //    DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value,
        //    DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value,
        //    DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value,
        //    DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value,
        //    DBNull.Value, DBNull.Value, '0', DBNull.Value, DBNull.Value,
        //    DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value,
        //    txtCust.Text, DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value,
        //    DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value,
        //    DBNull.Value, DBNull.Value, DBNull.Value, AccSer_No.TextS);

        //    dal.Execute("Add_daily_transaction",
        // acc_year.Text,
        // Acc_Cr.ID.Text,
        // txtStore_ID.Text,
        // AccSer_No.TextS,
        // "",
        // "",
        // "",
        // 0,
        // paied_amount.Value,
        // -paied_amount.Value,
        // H_Date,
        // txt_sandDate.Value,
        // txt_sandNo.TextS,
        // txt_source_code.Text,
        // txt_source_code.Text + txt_sandNo.TextS,
        // user_id.Text,
        // txtDescr.Text,
        // DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value,
        // DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value,
        // DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value,
        // String.Empty, DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value,
        // DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value,
        //txt_source_code.Text, txtStore_ID.Text, DBNull.Value, DBNull.Value, DBNull.Value,
        // DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value,
        // DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value,
        // DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value,
        // DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value,
        // DBNull.Value, DBNull.Value, '0', DBNull.Value, DBNull.Value,
        // DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value,
        // txtCust.Text, DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value,
        // DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value,
        // DBNull.Value, DBNull.Value, DBNull.Value, AccSer_No.TextS);

        //   dal.Execute_1(@"UPDATE serial_no SET daily_sn_ser='" + txt_sandNo.TextS + "' , main_daily_ser = '" + AccSer_No + "' WHERE BRANCH_CODE=  '" + txtStore_ID.Text + "' and ACC_YEAR='" + acc_year.Text + "' ");
          
        //}


        private void BExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void cmb_Pay_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Convert.ToString(cmb_Pay.SelectedValue) == "04")
            {
                txt_Check.Enabled = true;
                glkp_bank.Enabled = true;
                Check_Date.Enabled = true;
                cheuqeOrCash.Text = "S";
                txt_sanad_type2.Text = "BR";
            }
           else if (Convert.ToString(cmb_Pay.SelectedValue) == "05")
            {
                txt_Check.Enabled = false;
                glkp_bank.Enabled = false;
                Check_Date.Enabled = false;
                cheuqeOrCash.Text = "S";
                txt_sanad_type2.Text = "BR";
                txt_Check.Clear();
                glkp_bank.EditValue = "";
                Check_Date.Text = "";
            }
            else
            {
                cheuqeOrCash.Text = "";
                txt_sanad_type2.Text = "CR";
                txt_Check.Enabled = false;
                glkp_bank.Enabled = false;
                Check_Date.Enabled = false;
                txt_Check.Clear();
                glkp_bank.EditValue = "";
                Check_Date.Text = "";
                

            }
        }

        private void txtAcc_Load(object sender, EventArgs e)
        {
            txtCust.Text = Acc_Cr.Desc.Text;
        }

      

   

      

     
      

          

     

        private void print_sand_Click(object sender, EventArgs e)
        {
            //try
            //{
            if (Properties.Settings.Default.lungh == "0")
            {

                Form1 frmSand = new Form1();
                CrystalReport5 rpt = new CrystalReport5();
                DataTable dt1 = new DataTable();

                dt1 = dal.getDataTabl_1(@"select A.*,B.*,P.PAYER_NAME,C.branch_name from (SELECT ACC_YEAR,ACC_NO,BRANCH_code,ser_no,COST_CENTER,meno,g_date,sanad_no,SANAD_TYPE,sp_ser_no
                ,user_name,desc2,sheek_no,sheek_bank,sheek_date,sheek_or_cash,notes,SOURCE_CODE,Wh_Branch_Code,Sheek
                ,Dafter_no,Dafter_ser,SANAD_TYPE2  FROM daily_transaction where SANAD_TYPE2='" + txt_sanad_type2.Text + "' and BRANCH_code='" + txtStore_ID.Text + "' and  ser_no='" + AccSer_No.TextS + "' and  meno>0) as A " +
                "inner join wh_BRANCHES as C on C.Branch_code=A.Branch_code " +
                ",(select acc_no as acc_cr,desc2 as desc_cr  from daily_transaction where SANAD_TYPE2='" + txt_sanad_type2.Text + "' and BRANCH_code='" + txtStore_ID.Text + "' and  ser_no='" + AccSer_No.TextS + "' and loh>0)  as B " +
                "inner join payer2 as P on P.ACC_NO=B.acc_cr");


                decimal balance_ = Convert.ToDecimal(dt1.Rows[0]["meno"].ToString());
                ToWord toWord = new ToWord(Math.Abs(Math.Round(balance_, dal.digits_)), currencies[currencyNo]);
                rpt.DataDefinition.FormulaFields["Tafqeet"].Text = "'" + toWord.ConvertToArabic().ToString() + "'";


                DataSet ds = new DataSet("sanads");
                ds.Tables.Add(dt1);
                rpt.SetDataSource(ds);
                frmSand.crystalReportViewer1.ReportSource = rpt;
                frmSand.ShowDialog();

                //rpt.PrintOptions.PrinterName = Properties.Settings.Default.Report_P;
                //rpt.PrintToPrinter(2, true, 0, 15);


                //ds.WriteXmlSchema("schema3.xml");
            }
            else
            {
                Form1 frmSand = new Form1();
                CTR.print_CashReceipt rpt = new CTR.print_CashReceipt();
                DataTable dt1 = new DataTable();

                dt1 = dal.getDataTabl_1(@"select A.*,B.*,P.PAYER_NAME,p.PAYER_l_NAME,C.branch_name from (SELECT ACC_YEAR,ACC_NO,BRANCH_code,ser_no,COST_CENTER,meno,g_date,sanad_no,SANAD_TYPE,sp_ser_no
                ,user_name,desc2,sheek_no,sheek_bank,sheek_date,sheek_or_cash,notes,SOURCE_CODE,Wh_Branch_Code,Sheek
                ,Dafter_no,Dafter_ser,SANAD_TYPE2  FROM daily_transaction where SANAD_TYPE2='" + txt_sanad_type2.Text + "' and BRANCH_code='" + txtStore_ID.Text + "' and  ser_no='" + AccSer_No.TextS + "' and  meno>0) as A " +
                "inner join wh_BRANCHES as C on C.Branch_code=A.Branch_code " +
                ",(select acc_no as acc_cr,desc2 as desc_cr  from daily_transaction where SANAD_TYPE2='" + txt_sanad_type2.Text + "' and BRANCH_code='" + txtStore_ID.Text + "' and  ser_no='" + AccSer_No.TextS + "' and loh>0)  as B " +
                "inner join payer2 as P on P.ACC_NO=B.acc_cr");


                decimal balance_ = Convert.ToDecimal(dt1.Rows[0]["meno"].ToString());
                ToWord toWord = new ToWord(Math.Abs(Math.Round(balance_, dal.digits_)), currencies[currencyNo]);
                rpt.DataDefinition.FormulaFields["Tafqeet"].Text = "'" + toWord.ConvertToEnglish().ToString() + "'";


                DataSet ds = new DataSet("sanads");
                ds.Tables.Add(dt1);
                rpt.SetDataSource(ds);
                frmSand.crystalReportViewer1.ReportSource = rpt;
                frmSand.ShowDialog();




                //ds.WriteXmlSchema("schema3.xml");
                //}
                //catch { }
            }
            //}
            //catch { }
        
        }

        private void BNew_Click(object sender, EventArgs e)
        {
            ClearTextBoxes();
            BSave.Enabled = true;
            Payment_Type.Text = "2";
            txtCashAcc.ID.Text = dal.getDataTabl_1(@"select Cash_acc_no from Wh_branches where branch_code='" + Properties.Settings.Default.BranchId + "' ").Rows[0][0].ToString();
            //txtCashAcc.ID.Text = dal.getDataTabl("Get_Branche_data", Properties.Settings.Default.BranchId).Rows[0][14].ToString();
            txt_sandNo.TextS = dal.getDataTabl_1(@"select isnull((BOX_ED_SER)+1,1) from serial_no where Branch_code='" + txtStore_ID.Text
                 + "' and ACC_YEAR= '" + acc_year.Text + "' ").Rows[0][0].ToString();
        }


         private void ClearTextBoxes()
        {
            Action<Control.ControlCollection> func = null;

            func = (controls) =>
            {
                foreach (Control control in controls)
                    if (control is TextBox)
                        (control as TextBox).Clear();
                    else if (control is ComboBox)
                        (control as ComboBox).SelectedIndex = -1;
                    else if (control is DateTimePicker)
                        (control as DateTimePicker).Value = DateTime.Now;
                    else if (control is DevComponents.Editors.DateTimeAdv.DateTimeInput)
                        (control as DevComponents.Editors.DateTimeAdv.DateTimeInput).Value = DateTime.Now;
                    else if (control is DevComponents.Editors.DoubleInput)
                        (control as DevComponents.Editors.DoubleInput).Value = 0;

                    else
                        func(control.Controls);
            };
            func(Controls);
            // txtCoId.Text = Properties.Settings.Default.CoId;
            //BranchId.Text = Properties.Settings.Default.BranchId;
        }


        private void BSearch_Click(object sender, EventArgs e)
        {
            //try
            //{
            //    frm_search_recect frm_recet = new frm_search_recect();
            //    frm_recet.txt_source_code.Text = "CR";
            //    frm_recet.ShowDialog();
               

            //    int ii = frm_recet.DGV1.CurrentRow.Index;

                DataTable dt_ = dal.getDataTabl_1(@"select * from (SELECT ACC_YEAR,ACC_NO,BRANCH_code,ser_no,COST_CENTER,meno,g_date,sanad_no,SANAD_TYPE,sp_ser_no
                ,user_name,desc2,sheek_no,sheek_bank,sheek_date,sheek_or_cash,notes,SOURCE_CODE,Wh_Branch_Code,Sheek
                ,Dafter_no,Dafter_ser,SANAD_TYPE2  FROM daily_transaction where SANAD_TYPE2='cr' and BRANCH_code='a21106' and  ser_no='74654'and  meno>0) as A
                , (select acc_no as acc_cr  from daily_transaction where SANAD_TYPE2='cr' and BRANCH_code='a21106' and  ser_no='74654' and loh>0)  as cr_acc ");

                if (dt_.Rows.Count > 0)
                {

                    acc_year.Text = dt_.Rows[0]["ACC_YEAR"].ToString();
                    Acc_Cr.ID.Text = dt_.Rows[0]["acc_cr"].ToString();
                    AccSer_No.TextS = dt_.Rows[0]["ser_no"].ToString();
                    Cost.ID.Text = dt_.Rows[0]["COST_CENTER"].ToString();
                    paied_amount.Text = dt_.Rows[0]["meno"].ToString();
                    txt_sandDate.Text = dt_.Rows[0]["g_date"].ToString();
                    txt_sandNo.TextS = dt_.Rows[0]["sanad_no"].ToString();
                    Payment_Type.Text = "2";
                    user_id.Text = dt_.Rows[0]["user_name"].ToString();
                    txtDescr.Text = dt_.Rows[0]["desc2"].ToString();
                    txt_Check.Text = dt_.Rows[0]["sheek_no"].ToString();
                    glkp_bank.EditValue = dt_.Rows[0]["sheek_bank"].ToString();
                    Check_Date.Text = dt_.Rows[0]["sheek_date"].ToString();
                    cmb_Pay.Text = dt_.Rows[0]["Sheek"].ToString();
                    txt_source_code.Text = dt_.Rows[0]["SOURCE_CODE"].ToString();
                    txtStore_ID.Text = dt_.Rows[0]["Wh_Branch_Code"].ToString();
                    txtCashAcc.ID.Text = dt_.Rows[0]["ACC_NO"].ToString();
                    txt_sanad_type2.Text = dt_.Rows[0]["SANAD_TYPE2"].ToString();
                    txtCust.Text = dt_.Rows[0]["notes"].ToString();
                }
            //}
            //catch { }
        }

     
        private void search_1_Click(object sender, EventArgs e)
        {
            PL.frmSerch frm = new PL.frmSerch();
            frm.ShowDialog();

            DataTable dt_ = dal.getDataTabl_1(@" select * from (SELECT ACC_YEAR,ACC_NO,BRANCH_code,ser_no,COST_CENTER,meno,g_date,sanad_no,SANAD_TYPE,sp_ser_no
                ,user_name,desc2,sheek_no,sheek_bank,sheek_date,sheek_or_cash,notes,SOURCE_CODE,Wh_Branch_Code,Sheek
                ,Dafter_no,Dafter_ser,SANAD_TYPE2,desc_E  FROM daily_transaction where SANAD_TYPE2 in('CR','BR') and BRANCH_code='" + txtStore_ID.Text + "' and  sanad_no ='" + frm.txtSearch.t.Text + "' and  meno>0) as A , (select acc_no as acc_cr,desc2 as desc_cr   from daily_transaction where SANAD_TYPE2 in('CR','BR') and BRANCH_code='" + txtStore_ID.Text + "' and  sanad_no ='" + frm.txtSearch.t.Text + "' and loh>0)  as cr_acc ");

            ClearTextBoxes();
            //G_Search.Visible = false;
            if (dt_.Rows.Count > 0)
            {
                acc_year.Text = dt_.Rows[0]["ACC_YEAR"].ToString();
                Acc_Cr.ID.Text = dt_.Rows[0]["acc_cr"].ToString();
                AccSer_No.TextS = dt_.Rows[0]["ser_no"].ToString();
                Cost.ID.Text = dt_.Rows[0]["COST_CENTER"].ToString();
                paied_amount.Text = dt_.Rows[0]["meno"].ToString();
                txt_sandDate.Text = dt_.Rows[0]["g_date"].ToString();
                txt_sandNo.TextS = dt_.Rows[0]["sanad_no"].ToString();
                Payment_Type.Text = dt_.Rows[0]["SANAD_TYPE"].ToString();
                user_id.Text = dt_.Rows[0]["user_name"].ToString();
                txtDescr.Text = dt_.Rows[0]["desc_cr"].ToString();
                txt_Check.Text = dt_.Rows[0]["sheek_no"].ToString();
                glkp_bank.EditValue = dt_.Rows[0]["sheek_bank"].ToString();
                Check_Date.Text = dt_.Rows[0]["sheek_date"].ToString();
                cmb_Pay.SelectedValue = dt_.Rows[0]["Sheek"].ToString();
                txt_source_code.Text = dt_.Rows[0]["SOURCE_CODE"].ToString();
                txtStore_ID.Text = dt_.Rows[0]["Wh_Branch_Code"].ToString();
                txtCashAcc.ID.Text = dt_.Rows[0]["ACC_NO"].ToString();
                txt_sanad_type2.Text = dt_.Rows[0]["SANAD_TYPE2"].ToString();
                txtCust.Text = dt_.Rows[0]["notes"].ToString();
                txtDescr_E.Text = dt_.Rows[0]["desc_E"].ToString();
                cheuqeOrCash.Text = dt_.Rows[0]["sheek_or_cash"].ToString();
                txtSpecialNo.Text = dt_.Rows[0]["sp_ser_no"].ToString();

                LoadSanad(txt_sandNo.TextS, acc_year.Text, txtStore_ID.Text, txt_source_code.Text);
            }
            Get_Total();
            IsNew = false;
        }

        private void search_2_Click(object sender, EventArgs e)
        {
      
            PL.frmSerch frm = new PL.frmSerch();
            frm.ShowDialog();


            DataTable dt_ = dal.getDataTabl_1(@" select * from (SELECT ACC_YEAR,ACC_NO,BRANCH_code,ser_no,COST_CENTER,meno,g_date,sanad_no,SANAD_TYPE,sp_ser_no
            ,user_name,desc2,sheek_no,sheek_bank,sheek_date,sheek_or_cash,notes,SOURCE_CODE,Wh_Branch_Code,Sheek
            ,Dafter_no,Dafter_ser,SANAD_TYPE2,desc_E  FROM daily_transaction where SANAD_TYPE2 in('CR','BR') and BRANCH_code='" + txtStore_ID.Text + "' and  ser_no ='" + frm.txtSearch.t.Text + "' and  meno>0) as A , (select acc_no as acc_cr,desc2 as desc_cr   from daily_transaction where SANAD_TYPE2 in('CR','BR') and BRANCH_code='" + txtStore_ID.Text + "' and  ser_no ='" + frm.txtSearch.t.Text + "' and loh>0)  as cr_acc ");

            ClearTextBoxes();
            //G_Search.Visible = false;
            if (dt_.Rows.Count > 0)
            {
                acc_year.Text = dt_.Rows[0]["ACC_YEAR"].ToString();
                Acc_Cr.ID.Text = dt_.Rows[0]["acc_cr"].ToString();
                AccSer_No.TextS = dt_.Rows[0]["ser_no"].ToString();
                Cost.ID.Text = dt_.Rows[0]["COST_CENTER"].ToString();
                paied_amount.Text = dt_.Rows[0]["meno"].ToString();
                txt_sandDate.Text = dt_.Rows[0]["g_date"].ToString();
                txt_sandNo.TextS = dt_.Rows[0]["sanad_no"].ToString();
                Payment_Type.Text = "2";
                user_id.Text = dt_.Rows[0]["user_name"].ToString();
                txtDescr.Text = dt_.Rows[0]["desc_cr"].ToString();
                txt_Check.Text = dt_.Rows[0]["sheek_no"].ToString();
                glkp_bank.EditValue = dt_.Rows[0]["sheek_bank"].ToString();
                Check_Date.Text = dt_.Rows[0]["sheek_date"].ToString();
                cmb_Pay.SelectedValue = dt_.Rows[0]["Sheek"].ToString();
                txt_source_code.Text = dt_.Rows[0]["SOURCE_CODE"].ToString();
                txtStore_ID.Text = dt_.Rows[0]["Wh_Branch_Code"].ToString();
                txtCashAcc.ID.Text = dt_.Rows[0]["ACC_NO"].ToString();
                txt_sanad_type2.Text = dt_.Rows[0]["SANAD_TYPE2"].ToString();
                txtCust.Text = dt_.Rows[0]["notes"].ToString();
                txtDescr_E.Text = dt_.Rows[0]["desc_E"].ToString();
                cheuqeOrCash.Text = dt_.Rows[0]["sheek_or_cash"].ToString();
            }


            IsNew = false;
        }


       
        private void btnCancelSearch_Click(object sender, EventArgs e)
        {
            //txtsearch.Clear();
            //G_Search.Visible = false;
        }





        private void getJorSer()
        {

           AccSer_No.TextS = "M" + dal.getDataTabl_1(@"select isnull(main_daily_ser+1,1) from serial_no where BRANCH_CODE='" + Properties.Settings.Default.BranchId
                  + "' and ACC_YEAR= '" + acc_year.Text + "'").Rows[0][0].ToString().PadLeft(4, '0');

           txt_sandNo.TextS = dal.getDataTabl_1(@"select isnull((BOX_ED_SER)+1,1) from serial_no where Branch_code='" + txtStore_ID.Text
                         + "' and ACC_YEAR= '" + acc_year.Text + "' ").Rows[0][0].ToString();



        }

        
        private void Acc_Cr_Click(object sender, EventArgs e)
        {
                Acc_Cr.x =  Cursor.Position.X;
                Acc_Cr.y =  Cursor.Position.Y;
        }
            

        public override void Print()
        {

          // CTR.CrystalReport5 rpt = new CTR.CrystalReport5();
            CTR.rpt_Receipt rpt = new CTR.rpt_Receipt();
            DataTable dt1 = new DataTable();
            DataSet ds = new DataSet();

            dt1 = dal.getDataTabl_1(@"select A.*,B.*,P.PAYER_NAME,P.PAYER_l_NAME,C.branch_name from (SELECT LEFT(BRANCH_code,1) as com_code,ACC_YEAR,ACC_NO,BRANCH_code,ser_no,COST_CENTER,meno,g_date,sanad_no,SANAD_TYPE,sp_ser_no
                    ,user_name,desc2,sheek_no,sheek_bank,sheek_date,sheek_or_cash,notes,SOURCE_CODE,Wh_Branch_Code,Sheek
                    ,Dafter_no,Dafter_ser,SANAD_TYPE2  FROM daily_transaction where SANAD_TYPE2='" + txt_sanad_type2.Text + "' and BRANCH_code='" + txtStore_ID.Text + "' and  ser_no='" + AccSer_No.TextS + "' and  meno>0) as A " +
                   "inner join wh_BRANCHES as C on C.Branch_code=A.Branch_code " +
                   ",(select BRANCH_code,acc_no as acc_cr,desc2 as desc_cr  from daily_transaction where SANAD_TYPE2='" + txt_sanad_type2.Text + "' and BRANCH_code='" + txtStore_ID.Text + "' and  ser_no='" + AccSer_No.TextS + "' and loh>0)  as B " +
                   "inner join payer2 as P on P.ACC_NO=B.acc_cr and P.BRANCH_code=B.BRANCH_code");
            if (dt1.Rows.Count > 0)
            {
                DataTable dt_Co = dal.getDataTabl_1(@"select * from Wh_Oiner_Comp where Company_No='" + dt1.Rows[0]["com_code"] + "'  ");

                DataTable dtSanadInv = dal.getDataTabl_1(@" select * from  Sands_Details where ser_no = '" + dt1.Rows[0]["sanad_no"] + "' and BRANCH_code ='" + dt1.Rows[0]["Branch_code"] + "' and SOURCE_CODE = '" + dt1.Rows[0]["SOURCE_CODE"] + "' ");
                ds.Tables.Add(dtSanadInv);
                rpt.DataSource = ds; 
                ////ds.WriteXmlSchema("schema3.xml");

                decimal balance_ = Convert.ToDecimal(dt1.Rows[0]["meno"].ToString());
                ToWord toWord = new ToWord(Math.Abs(Math.Round(balance_, dal.digits_)), currencies[currencyNo]);
                if (Properties.Settings.Default.lungh == "0")
                {
                    rpt.txtAmountinLetter.Text = "'" + toWord.ConvertToArabic().ToString() + "'";
                    rpt.txtAccount.Text = dt1.Rows[0]["PAYER_NAME"].ToString();

                }
                else
                {
                    rpt.txtAmountinLetter.Text = "'" + toWord.ConvertToEnglish().ToString() + "'";
                    rpt.txtAccount.Text = dt1.Rows[0]["PAYER_l_NAME"].ToString();

                }
                if (dt1.Rows[0]["Sheek"].ToString() == "04")
                {
                    rpt.chCheque.Checked = true;
                    rpt.txtChequeNo.Text = dt1.Rows[0]["sheek_no"].ToString();
                    rpt.txtChequeBank.Text = dt1.Rows[0]["sheek_bank"].ToString();
                    rpt.txtChequeDate.Text = Convert.ToDateTime(dt1.Rows[0]["sheek_date"]).ToString("dd/MM/yyyy");
                }
                else
                {
                    rpt.chCash.Checked = true;
                    rpt.chCheque.Checked = false;
                    rpt.txtChequeNo.Text = "";
                    rpt.txtChequeBank.Text = "";
                    rpt.txtChequeDate.Text = "";
                }

                rpt.txtDesc1.Text = dt1.Rows[0]["desc2"].ToString();

                rpt.txtSer.Text = dt1.Rows[0]["Sanad_No"].ToString();
                rpt.txtDate.Text = Convert.ToDateTime(dt1.Rows[0]["g_date"]).ToString("dd/MM/yyyy");
                rpt.txtAmount.Text = Math.Floor(dt1.Rows[0]["meno"].ToString().ToDecimal()).ToString("N0");
                rpt.txtSubAmount.Text = ((dt1.Rows[0]["meno"].ToString().ToDecimal() - Math.Floor(dt1.Rows[0]["meno"].ToString().ToDecimal())) * dal.dicimalRate().ToString().ToDecimal()).ToString("N0");
                rpt.txtCruncy.Text = Properties.Settings.Default.E_Main_currancy;
                rpt.txtSubCruncy.Text = Properties.Settings.Default.e_sub_currancy;
                if (dt_Co.Rows[0]["stamp_pic"].ToString() != string.Empty)
                {
                    using (MemoryStream mStream = new MemoryStream((Byte[])dt_Co.Rows[0]["stamp_pic"]))
                    {
                        rpt.Pic2.Image = Image.FromStream(mStream);
                    }
                }
                if (dt_Co.Rows[0]["h_pic"].ToString() != string.Empty)
                {
                    using (MemoryStream ms_Hpic = new MemoryStream((Byte[])dt_Co.Rows[0]["h_pic"]))
                    {
                        rpt.PicH.Image = Image.FromStream(ms_Hpic);
                    }
                }
                if (dt_Co.Rows[0]["f_pic"].ToString() != string.Empty)
                {
                    using (MemoryStream ms_Fpic = new MemoryStream((Byte[])dt_Co.Rows[0]["f_pic"]))
                    {
                        rpt.picF.Image = Image.FromStream(ms_Fpic);
                    }
                }
                //rpt.CoName_E.Value = Properties.Settings.Default.head_txt_EN;
                //rpt.balance_.Value = dt1.Rows[0]["Ending_balance"].ToString().ToDecimal().ToString("N" + dal.digits_);

                rpt.ShowPreview();



                ////try


                ////{
                //if (Properties.Settings.Default.lungh == "0")
                //    {

                //        Form1 frmSand = new Form1();
                //        CrystalReport5 rpt = new CrystalReport5();
                //        DataTable dt1 = new DataTable();

                //        dt1 = dal.getDataTabl_1(@"select A.*,B.*,P.PAYER_NAME,C.branch_name from (SELECT ACC_YEAR,ACC_NO,BRANCH_code,ser_no,COST_CENTER,meno,g_date,sanad_no,SANAD_TYPE,sp_ser_no
                //        ,user_name,desc2,sheek_no,sheek_bank,sheek_date,sheek_or_cash,notes,SOURCE_CODE,Wh_Branch_Code,Sheek
                //        ,Dafter_no,Dafter_ser,SANAD_TYPE2  FROM daily_transaction where SANAD_TYPE2='" + txt_sanad_type2.Text + "' and BRANCH_code='" + txtStore_ID.Text + "' and  ser_no='" + AccSer_No.TextS + "' and  meno>0) as A " +
                //        "inner join wh_BRANCHES as C on C.Branch_code=A.Branch_code " +
                //        ",(select BRANCH_code,acc_no as acc_cr,desc2 as desc_cr  from daily_transaction where SANAD_TYPE2='" + txt_sanad_type2.Text + "' and BRANCH_code='" + txtStore_ID.Text + "' and  ser_no='" + AccSer_No.TextS + "' and loh>0)  as B " +
                //        "inner join payer2 as P on P.ACC_NO=B.acc_cr and P.BRANCH_code=B.BRANCH_code");


                //        decimal balance_ = Convert.ToDecimal(dt1.Rows[0]["meno"].ToString());
                //        ToWord toWord = new ToWord(Math.Abs(Math.Round(balance_, dal.digits_)), currencies[currencyNo]);
                //        rpt.DataDefinition.FormulaFields["Tafqeet"].Text = "'" + toWord.ConvertToArabic().ToString() + "'";


                //        DataSet ds = new DataSet("sanads");
                //        ds.Tables.Add(dt1);
                //        rpt.SetDataSource(ds);
                //        frmSand.crystalReportViewer1.ReportSource = rpt;
                //        frmSand.ShowDialog();

                //        //rpt.PrintOptions.PrinterName = Properties.Settings.Default.Report_P;
                //        //rpt.PrintToPrinter(2, true, 0, 15);


                //        //ds.WriteXmlSchema("schema3.xml");
                //    }
                //    else
                //    {
                //    Form1 frmSand = new Form1();
                //    CrystalReport5 rpt = new CrystalReport5();
                //    DataTable dt1 = new DataTable();

                //    dt1 = dal.getDataTabl_1(@"select A.*,B.*,P.PAYER_NAME,p.PAYER_l_NAME,C.branch_name from (SELECT ACC_YEAR,ACC_NO,BRANCH_code,ser_no,COST_CENTER,meno,g_date,sanad_no,SANAD_TYPE,sp_ser_no
                //    ,user_name,desc2,sheek_no,sheek_bank,sheek_date,sheek_or_cash,notes,SOURCE_CODE,Wh_Branch_Code,Sheek
                //    ,Dafter_no,Dafter_ser,SANAD_TYPE2  FROM daily_transaction where SANAD_TYPE2='" + txt_sanad_type2.Text + "' and BRANCH_code='" + txtStore_ID.Text + "' and  ser_no='" + AccSer_No.TextS + "' and  meno>0) as A " +
                //        "inner join wh_BRANCHES as C on C.Branch_code=A.Branch_code " +
                //        ",(select BRANCH_code,acc_no as acc_cr,desc2 as desc_cr  from daily_transaction where SANAD_TYPE2='" + txt_sanad_type2.Text + "' and BRANCH_code='" + txtStore_ID.Text + "' and  ser_no='" + AccSer_No.TextS + "' and loh>0)  as B " +
                //        "inner join payer2 as P on P.ACC_NO=B.acc_cr and P.BRANCH_code=B.BRANCH_code");


                //        decimal balance_ = Convert.ToDecimal(dt1.Rows[0]["meno"].ToString());
                //        ToWord toWord = new ToWord(Math.Abs(Math.Round(balance_, dal.digits_)), currencies[currencyNo]);
                //        rpt.DataDefinition.FormulaFields["Tafqeet"].Text = "'" + toWord.ConvertToEnglish().ToString() + "'";


                //        DataSet ds = new DataSet("sanads");
                //        ds.Tables.Add(dt1);
                //        rpt.SetDataSource(ds);
                //        frmSand.crystalReportViewer1.ReportSource = rpt;
                //        frmSand.ShowDialog();




                //        //ds.WriteXmlSchema("schema3.xml");
                //        //}
                //        //catch { }
                //    }
                //    //}
                //    //catch { }



            }
            base.Print();
        }


        private void panel1_DragDrop(object sender, DragEventArgs e)
        {
            MessageBox.Show("not allawed");
            return;
        }


        private void Btn_NonPayInvoice_Click(object sender, EventArgs e)
        {
            if(Acc_Cr.ID.Text.Trim()!= string.Empty)
            {
                DataTable dt_inv = dal.getDataTabl_1(@"select x.Ser_no,x.Cyear,x.Branch_code,x.Transaction_code,x.G_date,x.Acc_no,x.Acc_Branch_code,x.Payment_Type
,x.Inv_no,x.Inv_date,x.Po_no,x.Inv_Notes,Return_reson,Reten_Notes,x.NetAmount,x.K_M_Credit_TAX,B.paidAmount
,x.InvoiceAmount,isnull(y.returnAmount,0) as returnAmount,(x.InvoiceAmount+isnull(y.returnAmount,0) - isnull(B.paidAmount,0)) as Balance

 from( SELECT Ser_no,Cyear,Branch_code,Transaction_code,G_date,Acc_no,Acc_Branch_code,Payment_Type
,Inv_no,Inv_date,Po_no,NetAmount,K_M_Credit_TAX,PanyedAmount,(NetAmount+K_M_Credit_TAX) as InvoiceAmount,Inv_Notes
 FROM  wh_inv_data  where Transaction_code ='xsd') as x

 left join (SELECT cyear,BRANCH_code,Inv_No,Inv_Transaction_Code, sum(isnull(CurrentPaid,0)) as paidAmount  FROM Sands_Details where SOURCE_CODE ='cr' group by cyear,BRANCH_code,Inv_Transaction_Code,Inv_No ) as B
on  B.cyear = x.Cyear and B.BRANCH_code = x.BRANCH_code and B.Inv_No=x.Ser_no and B.Inv_Transaction_Code =x.Transaction_code 


 left join ( SELECT Ser_no,cyear,Branch_code,Transaction_code,G_date,Acc_no,Acc_Branch_code,Payment_Type
,Inv_no,Inv_date,Inv_Notes,Return_reson,Reten_Notes,sum(NetAmount) as NetAmount ,sum(K_M_Debit_TAX) as K_M_Debit_TAX,sum((isnull(NetAmount,0)- isnull(K_M_Debit_TAX,0))) as returnAmount
FROM  wh_inv_data where Transaction_code in('xsr','xst') group by  Ser_no,cyear,Branch_code,Transaction_code,G_date,Acc_no,Acc_Branch_code,Payment_Type
,Inv_no,Inv_date,Inv_Notes,Return_reson,Reten_Notes) as Y
 on x.Acc_no = y.Acc_no and x.Branch_code = y.Branch_code  and x.Ser_no =y.Inv_no and cast(x.G_date as date ) = cast(y.Inv_date as date ) and x.Payment_Type = y.Payment_Type
where x.acc_no = '" + Acc_Cr.ID.Text+ "' and x.InvoiceAmount+isnull(y.returnAmount,0)- isnull(B.paidAmount,0)<>0");
                if (dt_inv.Rows.Count > 0)
                {
                    int B_rowscount = dt_inv.Rows.Count;

                   dgv1.Rows.Clear();
                    dgv1.Rows.Add(B_rowscount);
                    for (int i = 0; i <= (B_rowscount - 1); i++)
                    {


                        dgv1.Rows[i].Cells[0].Value = dt_inv.Rows[i]["Ser_no"];
                        dgv1.Rows[i].Cells[1].Value = dt_inv.Rows[i]["Cyear"];
                        dgv1.Rows[i].Cells[2].Value = dt_inv.Rows[i]["G_date"];
                        dgv1.Rows[i].Cells[3].Value = dt_inv.Rows[i]["Po_no"];
                        dgv1.Rows[i].Cells[4].Value = dt_inv.Rows[i]["InvoiceAmount"];
                        dgv1.Rows[i].Cells[5].Value = dt_inv.Rows[i]["returnAmount"];
                        dgv1.Rows[i].Cells[6].Value = dt_inv.Rows[i]["paidAmount"];
                        dgv1.Rows[i].Cells[7].Value = dt_inv.Rows[i]["Balance"];
                        dgv1.Rows[i].Cells[8].Value = dt_inv.Rows[i]["Transaction_code"];


                    }
                }
                Get_Total();
            }
        }

        private void dgv1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {

            for (int i = 0; i <= dgv2.Rows.Count - 1; i++)
            {
                if (dgv2.Rows[i].Cells[0].Value.ToString() == dgv1.CurrentRow.Cells[0].Value.ToString() && dgv2.Rows[i].Cells[1].Value.ToString() == dgv1.CurrentRow.Cells[1].Value.ToString())
                {
                    dgv2.Rows.RemoveAt(i);
                    Get_Total();
                }
            }


            int n1 = dgv2.Rows.Add();
            if (paied_amount.Value > 0 && paied_amount.Value > (txtTotalPaid.Value + dgv1.CurrentRow.Cells[7].Value.ToString().ToDecimal()))
                {
                    
                    dgv2.Rows[n1].Cells[0].Value = dgv1.CurrentRow.Cells[0].Value.ToString();
                    dgv2.Rows[n1].Cells[1].Value = dgv1.CurrentRow.Cells[1].Value.ToString();
                    dgv2.Rows[n1].Cells[2].Value = dgv1.CurrentRow.Cells[2].Value.ToString();
                    dgv2.Rows[n1].Cells[3].Value = dgv1.CurrentRow.Cells[3].Value.ToString();
                    dgv2.Rows[n1].Cells[4].Value = dgv1.CurrentRow.Cells[4].Value.ToString();
                    dgv2.Rows[n1].Cells[5].Value = dgv1.CurrentRow.Cells[5].Value.ToString();
                    dgv2.Rows[n1].Cells[6].Value = dgv1.CurrentRow.Cells[6].Value.ToString();
                    dgv2.Rows[n1].Cells[7].Value = dgv1.CurrentRow.Cells[7].Value.ToString();
                    dgv2.Rows[n1].Cells[8].Value = dgv1.CurrentRow.Cells[7].Value.ToString();
                    dgv2.Rows[n1].Cells[9].Value = (dgv2.Rows[n1].Cells[7].Value.ToString().ToDecimal() - dgv2.Rows[n1].Cells[8].Value.ToString().ToDecimal()).ToString();
                    dgv2.Rows[n1].Cells[11].Value = dgv1.CurrentRow.Cells[8].Value.ToString();


                dgv1.Rows.RemoveAt(this.dgv1.CurrentRow.Index);
                txtDescr.Text = arabicDesc();
                txtDescr_E.Text = englishDesc();
            }


                else if (paied_amount.Value > 0 && paied_amount.Value > txtTotalPaid.Value && paied_amount.Value < (txtTotalPaid.Value + dgv1.CurrentRow.Cells[7].Value.ToString().ToDecimal()))
                {
                    
                    dgv2.Rows[n1].Cells[0].Value = dgv1.CurrentRow.Cells[0].Value.ToString();
                    dgv2.Rows[n1].Cells[1].Value = dgv1.CurrentRow.Cells[1].Value.ToString();
                    dgv2.Rows[n1].Cells[2].Value = dgv1.CurrentRow.Cells[2].Value.ToString();
                    dgv2.Rows[n1].Cells[3].Value = dgv1.CurrentRow.Cells[3].Value.ToString();
                    dgv2.Rows[n1].Cells[4].Value = dgv1.CurrentRow.Cells[4].Value.ToString();
                    dgv2.Rows[n1].Cells[5].Value = dgv1.CurrentRow.Cells[5].Value.ToString();
                    dgv2.Rows[n1].Cells[6].Value = dgv1.CurrentRow.Cells[6].Value.ToString();
                    dgv2.Rows[n1].Cells[7].Value = dgv1.CurrentRow.Cells[7].Value.ToString();
                    dgv2.Rows[n1].Cells[8].Value = paied_amount.Value - txtTotalPaid.Value;
                    dgv2.Rows[n1].Cells[9].Value = (dgv2.Rows[n1].Cells[7].Value.ToString().ToDecimal() - dgv2.Rows[n1].Cells[8].Value.ToString().ToDecimal()).ToString();
                   dgv2.Rows[n1].Cells[11].Value = dgv1.CurrentRow.Cells[8].Value.ToString();

                // dgv1.CurrentRow.Cells[6].Value = dgv2.Rows[n1].Cells[8].Value.ToString();
                //dgv1.CurrentRow.Cells[7].Value = dgv1.CurrentRow.Cells[7].Value.ToString().ToDecimal() - dgv1.CurrentRow.Cells[6].Value.ToString().ToDecimal();

                // dgv1.Rows.RemoveAt(this.dgv1.CurrentRow.Index);

            }
            else if (paied_amount.Value > 0 && paied_amount.Value <= txtTotalPaid.Value)
                {
                    return;
                }
            
            else
            {
              
                dgv2.Rows[n1].Cells[0].Value = dgv1.CurrentRow.Cells[0].Value.ToString();
                dgv2.Rows[n1].Cells[1].Value = dgv1.CurrentRow.Cells[1].Value.ToString();
                dgv2.Rows[n1].Cells[2].Value = dgv1.CurrentRow.Cells[2].Value.ToString();
                dgv2.Rows[n1].Cells[3].Value = dgv1.CurrentRow.Cells[3].Value.ToString();
                dgv2.Rows[n1].Cells[4].Value = dgv1.CurrentRow.Cells[4].Value.ToString();
                dgv2.Rows[n1].Cells[5].Value = dgv1.CurrentRow.Cells[5].Value.ToString();
                dgv2.Rows[n1].Cells[6].Value = dgv1.CurrentRow.Cells[6].Value.ToString();
                dgv2.Rows[n1].Cells[7].Value = dgv1.CurrentRow.Cells[7].Value.ToString();
                dgv2.Rows[n1].Cells[8].Value = dgv1.CurrentRow.Cells[7].Value.ToString();
                dgv2.Rows[n1].Cells[9].Value = (dgv2.Rows[n1].Cells[7].Value.ToString().ToDecimal() - dgv2.Rows[n1].Cells[8].Value.ToString().ToDecimal()).ToString();
                dgv2.Rows[n1].Cells[11].Value = dgv1.CurrentRow.Cells[8].Value.ToString();
                dgv1.Rows.RemoveAt(this.dgv1.CurrentRow.Index);
            }

            if (n1 > 0)
            {
                dgv2.Rows[n1].Cells[10].Value = (dgv2.Rows[n1].Cells[8].Value.ToString().ToDecimal() + dgv2.Rows[n1 - 1].Cells[10].Value.ToString().ToDecimal()).ToString();
            }
            else
            {
                dgv2.Rows[n1].Cells[10].Value = dgv2.Rows[n1].Cells[8].Value;
            }

            Get_Total();

        }






        private void Get_Total()
        {
            txtTotalInvoices.Text =
                         (from DataGridViewRow row in dgv1.Rows
                          where row.Cells[0].FormattedValue.ToString() != string.Empty
                          select (row.Cells[7].FormattedValue).ToString().ToDecimal()).Sum().ToString("n" + dal.digits_);

            txtTotalChoseInvoices.Text =
             (from DataGridViewRow row in dgv2.Rows
              where row.Cells[col_invNo.Index].FormattedValue.ToString() != string.Empty
              select (row.Cells[col_oldBalance.Index].FormattedValue).ToString().ToDecimal()).Sum().ToString("n" + dal.digits_);

            txtTotalPaid.Text =
           (from DataGridViewRow row in dgv2.Rows
            where row.Cells[col_invNo.Index].FormattedValue.ToString() != string.Empty
            select (row.Cells[col_currentPaid.Index].FormattedValue).ToString().ToDecimal()).Sum().ToString("n" + dal.digits_);

            txtBalance.Text =
          (from DataGridViewRow row in dgv2.Rows
           where row.Cells[col_invNo.Index].FormattedValue.ToString() != string.Empty
           select (row.Cells[col_newBalance.Index].FormattedValue).ToString().ToDecimal()).Sum().ToString("n"+dal.digits_);

            txtNoOfInvoices.Text =
               (from DataGridViewRow row in dgv2.Rows
                where row.Cells[col_invNo.Index].FormattedValue.ToString() != string.Empty
                select (row.Cells[col_invNo.Index].FormattedValue).ToString()).Count().ToString();

           
        }

      
        private void paied_amount_Leave(object sender, EventArgs e)
        {
            for (int i = 0; i <= dgv2.Rows.Count - 1; i++)
            {
                if (dgv2.Rows[i].Cells[col_sanadBalance.Index].Value.ToString().ToDecimal()>paied_amount.Value)
                {
                    dgv2.Rows.RemoveAt(i);
                    Get_Total();
                }
            }
        }

        private void dgv2_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            string _inv = dgv2.CurrentRow.Cells[0].Value.ToString();
            string _year = dgv2.CurrentRow.Cells[1].Value.ToString();


            for (int i = 0; i <= dgv1.Rows.Count - 1; i++)
            {
                if (dgv1.Rows[i].Cells[0].Value.ToString() == _inv && dgv1.Rows[i].Cells[1].Value.ToString() == _year)
                {
                    dgv1.Rows[i].Cells[6].Value = dgv1.Rows[i].Cells[6].Value.ToString().ToDecimal() - dgv2.CurrentRow.Cells[8].Value.ToString().ToDecimal();
                    dgv1.Rows[i].Cells[7].Value = dgv1.Rows[i].Cells[7].Value.ToString().ToDecimal() + dgv2.CurrentRow.Cells[8].Value.ToString().ToDecimal();

                    dgv2.Rows.RemoveAt(this.dgv2.CurrentRow.Index);
                    Get_Total();
                    return;
                }
            }
            int n1 = dgv1.Rows.Add();
            dgv1.Rows[n1].Cells[0].Value = dgv2.CurrentRow.Cells[0].Value.ToString();
            dgv1.Rows[n1].Cells[1].Value = dgv2.CurrentRow.Cells[1].Value.ToString();
            dgv1.Rows[n1].Cells[2].Value = dgv2.CurrentRow.Cells[2].Value.ToString();
            dgv1.Rows[n1].Cells[3].Value = dgv2.CurrentRow.Cells[3].Value.ToString();
            dgv1.Rows[n1].Cells[4].Value = dgv2.CurrentRow.Cells[4].Value.ToString();
            dgv1.Rows[n1].Cells[5].Value = dgv2.CurrentRow.Cells[5].Value.ToString();
            dgv1.Rows[n1].Cells[6].Value = dgv2.CurrentRow.Cells[6].Value.ToString();
            dgv1.Rows[n1].Cells[7].Value = dgv2.CurrentRow.Cells[7].Value.ToString();
            dgv1.Rows[n1].Cells[8].Value = dgv2.CurrentRow.Cells[11].Value.ToString();
            dgv2.Rows.RemoveAt(this.dgv2.CurrentRow.Index);

            Get_Total();
            txtDescr.Text = arabicDesc();
            txtDescr_E.Text = englishDesc();
        }


        private string arabicDesc()
        {
            string _invoices = "";
            if (dgv2.Rows.Count > 0)
            {
                _invoices = "سداد فواتير ( ";
                for (int i = 0; i <= dgv2.Rows.Count - 2; i++)
                {
                    _invoices += dgv2.Rows[i].Cells[0].Value.ToString() +" & ";

                }
                for (int i = dgv2.Rows.Count - 1; i <= dgv2.Rows.Count - 1; i++)
                {
                    _invoices += dgv2.Rows[i].Cells[0].Value.ToString() + " )";

                }

            }
            return _invoices;
        }

        private string englishDesc()
        {
            string _invoices = "";
            if (dgv2.Rows.Count > 0)
            {
                _invoices = "payment of Invices ( ";
                for (int i = 0; i <= dgv2.Rows.Count - 2; i++)
                {
                    _invoices += dgv2.Rows[i].Cells[0].Value.ToString() + " & ";

                }
                for (int i = dgv2.Rows.Count - 1; i <= dgv2.Rows.Count - 1; i++)
                {
                    _invoices += dgv2.Rows[i].Cells[0].Value.ToString() + " )";

                }

            }
            return _invoices;
        }

        private void txt_sandNo_Paint(object sender, PaintEventArgs e)
        {

        }

        private void dgv2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void ribbonBar1_ItemClick(object sender, EventArgs e)
        {

        }

        private void BEdit_Click(object sender, EventArgs e)
        {

        }

        private void glkp_bank_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            if (e.Button.Kind == ButtonPredefines.Plus)
                Forms.frm_Main.OpenFormByName(nameof(CTR.frm_ChequeBanks));
        }
    }
}
