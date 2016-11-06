namespace AsyncRircGisService
{
    partial class AsyncRircGisService
    {
        /// <summary> 
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором компонентов

        /// <summary> 
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.SrvcEventLog = new System.Diagnostics.EventLog();
            ((System.ComponentModel.ISupportInitialize)(this.SrvcEventLog)).BeginInit();
            // 
            // AsyncRircGisService
            // 
            this.ServiceName = "AsyncRircGisService";
            ((System.ComponentModel.ISupportInitialize)(this.SrvcEventLog)).EndInit();

        }

        #endregion

        private System.Diagnostics.EventLog SrvcEventLog;
    }
}
