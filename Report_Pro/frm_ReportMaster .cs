﻿using DevExpress.XtraEditors;
using Report_Pro.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Report_Pro
{
    public partial class frm_ReportMaster : XtraForm
    {
       bool IsNew;
        public static string ErrorText
        {
            get
            {
                return "هذا الحقل مطلوب";
            }
        }

        public frm_ReportMaster()
        {
            InitializeComponent();
        }


        private void frm_Master_Load(object sender, EventArgs e)
        {
        
        }

       
        public virtual void Report()
        {

        }

       

        public virtual void CloseForm(Form frm)
        {
            frm.Close();
        }
        public virtual void MaxForm(Form frm)
        {
            if (frm.WindowState == FormWindowState.Normal)
            {
                frm.WindowState = FormWindowState.Maximized;
            }
            else
            {
                frm.WindowState = FormWindowState.Normal;
            }
        }
        public virtual void MinForm(Form frm)
        {
            frm.WindowState=FormWindowState.Minimized;
        }



        public virtual void Option()
        {

        }

        public virtual void RefreshData()
        {

        }
        public virtual bool IsDataVailde()
        {
            return true;
        }



 

        private void btn_Print_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //if (CheckActionAuthorization(this.Name, Master.Actions.Print))
            //    Print();
        }

        


        public static bool CheckActionAuthorization(string formName, Master.Actions actions, DAL.wh_USER user = null)
        {
            if (user == null)
                user = Session.User;

            if (user.CAN_GIVE_PERM == (byte)Master.UserType.Admin)
                return true;
            else
            {
                var screen = Session.ScreensAccesses.SingleOrDefault(x => x.ScreenName == formName);
                bool flag = true;
                if (screen != null)
                {
                    switch (actions)
                    {
                        case Master.Actions.Add:
                            flag = screen.CanAdd;
                            break;
                        case Master.Actions.Edit:
                            flag = screen.CanEdit;

                            break;
                        case Master.Actions.Delete:
                            flag = screen.CanDelete;

                            break;
                        case Master.Actions.Print:
                            flag = screen.CanPrint;

                            break;
                        default:
                            break;
                    }
                }
                if (flag == false)
                {
                    XtraMessageBox.Show(
         text: "غير مصرح لك ",
         caption: "",
         icon: MessageBoxIcon.Error,
         buttons: MessageBoxButtons.OK
         );
                }
                return flag;
            }


        }





        private void frm_Master_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F9)
            {
                Report();
            }
            if (e.KeyCode == Keys.F1)
            {
                Option();
            }
         
        }


       

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frm_ReportMaster));
            this.barManager1 = new DevExpress.XtraBars.BarManager(this.components);
            this.bar2 = new DevExpress.XtraBars.Bar();
            this.btn_Min = new DevExpress.XtraBars.BarButtonItem();
            this.btn_Max = new DevExpress.XtraBars.BarButtonItem();
            this.btn_close = new DevExpress.XtraBars.BarButtonItem();
            this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
            this.btn_Save = new DevExpress.XtraBars.BarButtonItem();
            this.btn_New = new DevExpress.XtraBars.BarButtonItem();
            this.btn_Delete = new DevExpress.XtraBars.BarButtonItem();
            this.btn_Print = new DevExpress.XtraBars.BarButtonItem();
            this.btn_search = new DevExpress.XtraBars.BarButtonItem();
            this.skinDropDownButtonItem1 = new DevExpress.XtraBars.SkinDropDownButtonItem();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btn_Report = new DevExpress.XtraEditors.SimpleButton();
            this.btn_Option = new DevExpress.XtraEditors.SimpleButton();
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // barManager1
            // 
            this.barManager1.Bars.AddRange(new DevExpress.XtraBars.Bar[] {
            this.bar2});
            this.barManager1.DockControls.Add(this.barDockControlTop);
            this.barManager1.DockControls.Add(this.barDockControlBottom);
            this.barManager1.DockControls.Add(this.barDockControlLeft);
            this.barManager1.DockControls.Add(this.barDockControlRight);
            this.barManager1.Form = this;
            this.barManager1.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
            this.btn_Save,
            this.btn_New,
            this.btn_Delete,
            this.btn_Print,
            this.btn_close,
            this.btn_search,
            this.btn_Max,
            this.btn_Min,
            this.skinDropDownButtonItem1});
            this.barManager1.MainMenu = this.bar2;
            this.barManager1.MaxItemId = 9;
            // 
            // bar2
            // 
            this.bar2.BarName = "Main menu";
            this.bar2.CanDockStyle = DevExpress.XtraBars.BarCanDockStyle.Top;
            this.bar2.DockCol = 0;
            this.bar2.DockRow = 0;
            this.bar2.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
            this.bar2.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.btn_Min),
            new DevExpress.XtraBars.LinkPersistInfo(this.btn_Max),
            new DevExpress.XtraBars.LinkPersistInfo(this.btn_close)});
            this.bar2.OptionsBar.AllowQuickCustomization = false;
            this.bar2.OptionsBar.AutoPopupMode = DevExpress.XtraBars.BarAutoPopupMode.None;
            this.bar2.OptionsBar.DisableClose = true;
            this.bar2.OptionsBar.DisableCustomization = true;
            this.bar2.OptionsBar.MultiLine = true;
            this.bar2.OptionsBar.UseWholeRow = true;
            resources.ApplyResources(this.bar2, "bar2");
            // 
            // btn_Min
            // 
            this.btn_Min.Alignment = DevExpress.XtraBars.BarItemLinkAlignment.Right;
            this.btn_Min.Id = 7;
            this.btn_Min.ImageOptions.Image = global::Report_Pro.Properties.Resources.minimize;
            this.btn_Min.ImageOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("btn_Min.ImageOptions.LargeImage")));
            this.btn_Min.Name = "btn_Min";
            this.btn_Min.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph;
            this.btn_Min.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btn_Min_ItemClick);
            // 
            // btn_Max
            // 
            this.btn_Max.Alignment = DevExpress.XtraBars.BarItemLinkAlignment.Right;
            this.btn_Max.Id = 6;
            this.btn_Max.ImageOptions.Image = global::Report_Pro.Properties.Resources.squares;
            this.btn_Max.ImageOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("btn_Max.ImageOptions.LargeImage")));
            this.btn_Max.Name = "btn_Max";
            this.btn_Max.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btn_Max_ItemClick);
            // 
            // btn_close
            // 
            this.btn_close.Alignment = DevExpress.XtraBars.BarItemLinkAlignment.Right;
            resources.ApplyResources(this.btn_close, "btn_close");
            this.btn_close.Id = 4;
            this.btn_close.ItemAppearance.Hovered.BackColor = System.Drawing.Color.Red;
            this.btn_close.ItemAppearance.Hovered.Font = ((System.Drawing.Font)(resources.GetObject("btn_close.ItemAppearance.Hovered.Font")));
            this.btn_close.ItemAppearance.Hovered.ForeColor = System.Drawing.Color.White;
            this.btn_close.ItemAppearance.Hovered.Options.UseBackColor = true;
            this.btn_close.ItemAppearance.Hovered.Options.UseFont = true;
            this.btn_close.ItemAppearance.Hovered.Options.UseForeColor = true;
            this.btn_close.ItemAppearance.Normal.Font = ((System.Drawing.Font)(resources.GetObject("btn_close.ItemAppearance.Normal.Font")));
            this.btn_close.ItemAppearance.Normal.Options.UseFont = true;
            this.btn_close.Name = "btn_close";
            this.btn_close.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.Caption;
            this.btn_close.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btn_close_ItemClick);
            // 
            // barDockControlTop
            // 
            this.barDockControlTop.CausesValidation = false;
            resources.ApplyResources(this.barDockControlTop, "barDockControlTop");
            this.barDockControlTop.Manager = this.barManager1;
            // 
            // barDockControlBottom
            // 
            this.barDockControlBottom.CausesValidation = false;
            resources.ApplyResources(this.barDockControlBottom, "barDockControlBottom");
            this.barDockControlBottom.Manager = this.barManager1;
            // 
            // barDockControlLeft
            // 
            this.barDockControlLeft.CausesValidation = false;
            resources.ApplyResources(this.barDockControlLeft, "barDockControlLeft");
            this.barDockControlLeft.Manager = this.barManager1;
            // 
            // barDockControlRight
            // 
            this.barDockControlRight.CausesValidation = false;
            resources.ApplyResources(this.barDockControlRight, "barDockControlRight");
            this.barDockControlRight.Manager = this.barManager1;
            // 
            // btn_Save
            // 
            resources.ApplyResources(this.btn_Save, "btn_Save");
            this.btn_Save.Id = 0;
            this.btn_Save.ImageOptions.SvgImage = ((DevExpress.Utils.Svg.SvgImage)(resources.GetObject("btn_Save.ImageOptions.SvgImage")));
            this.btn_Save.Name = "btn_Save";
            this.btn_Save.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph;
            this.btn_Save.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btn_Save_ItemClick);
            // 
            // btn_New
            // 
            resources.ApplyResources(this.btn_New, "btn_New");
            this.btn_New.Id = 1;
            this.btn_New.ImageOptions.SvgImage = ((DevExpress.Utils.Svg.SvgImage)(resources.GetObject("btn_New.ImageOptions.SvgImage")));
            this.btn_New.Name = "btn_New";
            this.btn_New.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph;
            // 
            // btn_Delete
            // 
            resources.ApplyResources(this.btn_Delete, "btn_Delete");
            this.btn_Delete.Id = 2;
            this.btn_Delete.ImageOptions.SvgImage = ((DevExpress.Utils.Svg.SvgImage)(resources.GetObject("btn_Delete.ImageOptions.SvgImage")));
            this.btn_Delete.Name = "btn_Delete";
            this.btn_Delete.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph;
            // 
            // btn_Print
            // 
            resources.ApplyResources(this.btn_Print, "btn_Print");
            this.btn_Print.Id = 3;
            this.btn_Print.ImageOptions.SvgImage = ((DevExpress.Utils.Svg.SvgImage)(resources.GetObject("btn_Print.ImageOptions.SvgImage")));
            this.btn_Print.Name = "btn_Print";
            this.btn_Print.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph;
            this.btn_Print.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            this.btn_Print.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btn_Print_ItemClick);
            // 
            // btn_search
            // 
            resources.ApplyResources(this.btn_search, "btn_search");
            this.btn_search.Id = 5;
            this.btn_search.ImageOptions.SvgImage = ((DevExpress.Utils.Svg.SvgImage)(resources.GetObject("btn_search.ImageOptions.SvgImage")));
            this.btn_search.Name = "btn_search";
            this.btn_search.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph;
            // 
            // skinDropDownButtonItem1
            // 
            this.skinDropDownButtonItem1.Id = 8;
            this.skinDropDownButtonItem1.Name = "skinDropDownButtonItem1";
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.btn_Report);
            this.panel1.Controls.Add(this.btn_Option);
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            // 
            // btn_Report
            // 
            this.btn_Report.ImageOptions.Image = global::Report_Pro.Properties.Resources.business_report_32;
            resources.ApplyResources(this.btn_Report, "btn_Report");
            this.btn_Report.Name = "btn_Report";
            this.btn_Report.Click += new System.EventHandler(this.btn_Report_Click_1);
            // 
            // btn_Option
            // 
            this.btn_Option.AppearanceHovered.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            this.btn_Option.AppearanceHovered.BackColor2 = ((System.Drawing.Color)(resources.GetObject("btn_Option.AppearanceHovered.BackColor2")));
            this.btn_Option.AppearanceHovered.Options.UseBackColor = true;
            this.btn_Option.ImageOptions.Image = global::Report_Pro.Properties.Resources.multiple;
            resources.ApplyResources(this.btn_Option, "btn_Option");
            this.btn_Option.Name = "btn_Option";
            this.btn_Option.Click += new System.EventHandler(this.btn_Option_Click_1);
            // 
            // frm_ReportMaster
            // 
            this.Appearance.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.Appearance.Options.UseBackColor = true;
            resources.ApplyResources(this, "$this");
            this.ControlBox = false;
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.barDockControlLeft);
            this.Controls.Add(this.barDockControlRight);
            this.Controls.Add(this.barDockControlBottom);
            this.Controls.Add(this.barDockControlTop);
            this.Name = "frm_ReportMaster";
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).EndInit();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void barButtonItem2_ItemClick_1(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

        }

        private void barButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

        }

        private void btn_close_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CloseForm(this);
        }

       

        private void btn_Max_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            MaxForm(this);
        }

        private void btn_Min_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            MinForm(this);
        }

        private void btn_Save_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
                    }

        private void btn_Option_Click(object sender, EventArgs e)
        {
            Option();
        }

        private void btn_Report_Click(object sender, EventArgs e)
        {
            if (CheckActionAuthorization(this.Name, Master.Actions.Print))
                Report();
        }

        private void btn_Option_Click_1(object sender, EventArgs e)
        {
            Option();
        }

        private void btn_Report_Click_1(object sender, EventArgs e)
        {
            if (CheckActionAuthorization(this.Name, Master.Actions.Print))
                Report();

        }
    }
    }

