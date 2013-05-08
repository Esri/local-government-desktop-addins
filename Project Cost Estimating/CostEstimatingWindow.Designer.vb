<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class CostEstimatingWindow
    Inherits System.Windows.Forms.UserControl

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing AndAlso components IsNot Nothing Then
            components.Dispose()
        End If
        MyBase.Dispose(disposing)
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(CostEstimatingWindow))
        Me.gpBxCIPCostingLayers = New System.Windows.Forms.GroupBox()
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.rdoBtnShowLayers = New System.Windows.Forms.RadioButton()
        Me.rdoShowInv = New System.Windows.Forms.RadioButton()
        Me.gpBxSwitch = New System.Windows.Forms.GroupBox()
        Me.rdoBtnShowDetails = New System.Windows.Forms.RadioButton()
        Me.rdoBtnShowCan = New System.Windows.Forms.RadioButton()
        Me.gpBxCIPInven = New System.Windows.Forms.GroupBox()
        Me.lstInventory = New System.Windows.Forms.ListBox()
        Me.gpBxInv = New System.Windows.Forms.Panel()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.numCIPInvCount = New System.Windows.Forms.NumericUpDown()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.cboCIPInvTypes = New System.Windows.Forms.ComboBox()
        Me.btnRemoveInv = New System.Windows.Forms.Button()
        Me.btnAddInv = New System.Windows.Forms.Button()
        Me.cboAction = New System.Windows.Forms.ComboBox()
        Me.cboStrategy = New System.Windows.Forms.ComboBox()
        Me.lblCost = New System.Windows.Forms.Label()
        Me.ctxMenuTotals = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.ShowLength = New System.Windows.Forms.ToolStripMenuItem()
        Me.ShowArea = New System.Windows.Forms.ToolStripMenuItem()
        Me.ShowPoint = New System.Windows.Forms.ToolStripMenuItem()
        Me.TotalDisplay = New System.Windows.Forms.FlowLayoutPanel()
        Me.lblLength = New System.Windows.Forms.Label()
        Me.lblTotLength = New System.Windows.Forms.Label()
        Me.lblArea = New System.Windows.Forms.Label()
        Me.lblTotArea = New System.Windows.Forms.Label()
        Me.lblPoint = New System.Windows.Forms.Label()
        Me.lblTotPnt = New System.Windows.Forms.Label()
        Me.gpBxCIPCan = New System.Windows.Forms.GroupBox()
        Me.gpBxControls = New System.Windows.Forms.GroupBox()
        Me.btnSelectPrj = New System.Windows.Forms.RadioButton()
        Me.lblTotalCost = New System.Windows.Forms.Label()
        Me.btnSavePrj = New System.Windows.Forms.Button()
        Me.btnClear = New System.Windows.Forms.Button()
        Me.btnSelect = New System.Windows.Forms.RadioButton()
        Me.btnSketch = New System.Windows.Forms.RadioButton()
        Me.btnSelectAssets = New System.Windows.Forms.RadioButton()
        Me.btnSave = New System.Windows.Forms.Button()
        Me.btnStopEditing = New System.Windows.Forms.Button()
        Me.btnStartEditing = New System.Windows.Forms.Button()
        Me.cboDefLayers = New System.Windows.Forms.ComboBox()
        Me.tblDisabled = New System.Windows.Forms.TableLayoutPanel()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.tlStZoomTo = New System.Windows.Forms.ToolStripMenuItem()
        Me.tlStFlash = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator1 = New System.Windows.Forms.ToolStripSeparator()
        Me.tlStDelete = New System.Windows.Forms.ToolStripMenuItem()
        Me.ctxMenu = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.gpBxCIPPrj = New System.Windows.Forms.GroupBox()
        Me.tbCntCIPDetails = New System.Windows.Forms.TabControl()
        Me.tlTip = New System.Windows.Forms.ToolTip(Me.components)
        Me.gpBxSwitch.SuspendLayout()
        Me.gpBxCIPInven.SuspendLayout()
        Me.gpBxInv.SuspendLayout()
        CType(Me.numCIPInvCount, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.ctxMenuTotals.SuspendLayout()
        Me.TotalDisplay.SuspendLayout()
        Me.gpBxControls.SuspendLayout()
        Me.tblDisabled.SuspendLayout()
        Me.ctxMenu.SuspendLayout()
        Me.gpBxCIPPrj.SuspendLayout()
        Me.SuspendLayout()
        '
        'gpBxCIPCostingLayers
        '
        Me.gpBxCIPCostingLayers.Dock = System.Windows.Forms.DockStyle.Left
        Me.gpBxCIPCostingLayers.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!)
        Me.gpBxCIPCostingLayers.Location = New System.Drawing.Point(396, 47)
        Me.gpBxCIPCostingLayers.Name = "gpBxCIPCostingLayers"
        Me.gpBxCIPCostingLayers.Size = New System.Drawing.Size(107, 286)
        Me.gpBxCIPCostingLayers.TabIndex = 25
        Me.gpBxCIPCostingLayers.TabStop = False
        Me.gpBxCIPCostingLayers.Text = "Costing Layers"
        Me.gpBxCIPCostingLayers.Visible = False
        '
        'Panel1
        '
        Me.Panel1.BackColor = System.Drawing.SystemColors.ControlDark
        Me.Panel1.Location = New System.Drawing.Point(576, 16)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(5, 22)
        Me.Panel1.TabIndex = 33
        '
        'rdoBtnShowLayers
        '
        Me.rdoBtnShowLayers.Appearance = System.Windows.Forms.Appearance.Button
        Me.rdoBtnShowLayers.CausesValidation = False
        Me.rdoBtnShowLayers.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.rdoBtnShowLayers.Location = New System.Drawing.Point(6, 103)
        Me.rdoBtnShowLayers.Name = "rdoBtnShowLayers"
        Me.rdoBtnShowLayers.Size = New System.Drawing.Size(75, 40)
        Me.rdoBtnShowLayers.TabIndex = 3
        Me.rdoBtnShowLayers.Text = "Active"
        Me.rdoBtnShowLayers.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        Me.rdoBtnShowLayers.UseVisualStyleBackColor = True
        '
        'rdoShowInv
        '
        Me.rdoShowInv.Appearance = System.Windows.Forms.Appearance.Button
        Me.rdoShowInv.CausesValidation = False
        Me.rdoShowInv.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.rdoShowInv.Location = New System.Drawing.Point(6, 116)
        Me.rdoShowInv.Name = "rdoShowInv"
        Me.rdoShowInv.Size = New System.Drawing.Size(75, 27)
        Me.rdoShowInv.TabIndex = 2
        Me.rdoShowInv.Text = "Inventory"
        Me.rdoShowInv.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        Me.rdoShowInv.UseVisualStyleBackColor = True
        Me.rdoShowInv.Visible = False
        '
        'gpBxSwitch
        '
        Me.gpBxSwitch.Controls.Add(Me.rdoBtnShowLayers)
        Me.gpBxSwitch.Controls.Add(Me.rdoShowInv)
        Me.gpBxSwitch.Controls.Add(Me.rdoBtnShowDetails)
        Me.gpBxSwitch.Controls.Add(Me.rdoBtnShowCan)
        Me.gpBxSwitch.Dock = System.Windows.Forms.DockStyle.Left
        Me.gpBxSwitch.Location = New System.Drawing.Point(0, 47)
        Me.gpBxSwitch.Name = "gpBxSwitch"
        Me.gpBxSwitch.Size = New System.Drawing.Size(84, 286)
        Me.gpBxSwitch.TabIndex = 24
        Me.gpBxSwitch.TabStop = False
        Me.gpBxSwitch.Visible = False
        '
        'rdoBtnShowDetails
        '
        Me.rdoBtnShowDetails.Appearance = System.Windows.Forms.Appearance.Button
        Me.rdoBtnShowDetails.CausesValidation = False
        Me.rdoBtnShowDetails.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.rdoBtnShowDetails.Location = New System.Drawing.Point(6, 59)
        Me.rdoBtnShowDetails.Name = "rdoBtnShowDetails"
        Me.rdoBtnShowDetails.Size = New System.Drawing.Size(75, 42)
        Me.rdoBtnShowDetails.TabIndex = 1
        Me.rdoBtnShowDetails.Text = "Details"
        Me.rdoBtnShowDetails.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        Me.rdoBtnShowDetails.UseVisualStyleBackColor = True
        '
        'rdoBtnShowCan
        '
        Me.rdoBtnShowCan.Appearance = System.Windows.Forms.Appearance.Button
        Me.rdoBtnShowCan.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center
        Me.rdoBtnShowCan.CausesValidation = False
        Me.rdoBtnShowCan.Checked = True
        Me.rdoBtnShowCan.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.rdoBtnShowCan.Location = New System.Drawing.Point(6, 15)
        Me.rdoBtnShowCan.Name = "rdoBtnShowCan"
        Me.rdoBtnShowCan.Size = New System.Drawing.Size(75, 42)
        Me.rdoBtnShowCan.TabIndex = 0
        Me.rdoBtnShowCan.TabStop = True
        Me.rdoBtnShowCan.Text = "Assets"
        Me.rdoBtnShowCan.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        Me.rdoBtnShowCan.UseVisualStyleBackColor = True
        '
        'gpBxCIPInven
        '
        Me.gpBxCIPInven.Controls.Add(Me.lstInventory)
        Me.gpBxCIPInven.Controls.Add(Me.gpBxInv)
        Me.gpBxCIPInven.Dock = System.Windows.Forms.DockStyle.Left
        Me.gpBxCIPInven.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!)
        Me.gpBxCIPInven.Location = New System.Drawing.Point(158, 47)
        Me.gpBxCIPInven.Name = "gpBxCIPInven"
        Me.gpBxCIPInven.Size = New System.Drawing.Size(177, 286)
        Me.gpBxCIPInven.TabIndex = 26
        Me.gpBxCIPInven.TabStop = False
        Me.gpBxCIPInven.Text = "Project Inventory"
        Me.gpBxCIPInven.Visible = False
        '
        'lstInventory
        '
        Me.lstInventory.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lstInventory.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lstInventory.FormattingEnabled = True
        Me.lstInventory.ItemHeight = 20
        Me.lstInventory.Location = New System.Drawing.Point(317, 18)
        Me.lstInventory.Name = "lstInventory"
        Me.lstInventory.Size = New System.Drawing.Size(0, 265)
        Me.lstInventory.TabIndex = 1
        '
        'gpBxInv
        '
        Me.gpBxInv.Controls.Add(Me.Label7)
        Me.gpBxInv.Controls.Add(Me.numCIPInvCount)
        Me.gpBxInv.Controls.Add(Me.Label6)
        Me.gpBxInv.Controls.Add(Me.cboCIPInvTypes)
        Me.gpBxInv.Controls.Add(Me.btnRemoveInv)
        Me.gpBxInv.Controls.Add(Me.btnAddInv)
        Me.gpBxInv.Dock = System.Windows.Forms.DockStyle.Left
        Me.gpBxInv.Location = New System.Drawing.Point(3, 18)
        Me.gpBxInv.Name = "gpBxInv"
        Me.gpBxInv.Size = New System.Drawing.Size(314, 265)
        Me.gpBxInv.TabIndex = 0
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.Font = New System.Drawing.Font("Microsoft Sans Serif", 11.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label7.Location = New System.Drawing.Point(19, 71)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(120, 18)
        Me.Label7.TabIndex = 5
        Me.Label7.Text = "Number to Add"
        '
        'numCIPInvCount
        '
        Me.numCIPInvCount.Font = New System.Drawing.Font("Microsoft Sans Serif", 11.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.numCIPInvCount.Location = New System.Drawing.Point(27, 95)
        Me.numCIPInvCount.Name = "numCIPInvCount"
        Me.numCIPInvCount.Size = New System.Drawing.Size(186, 24)
        Me.numCIPInvCount.TabIndex = 4
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Font = New System.Drawing.Font("Microsoft Sans Serif", 11.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label6.Location = New System.Drawing.Point(19, 12)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(161, 18)
        Me.Label6.TabIndex = 3
        Me.Label6.Text = "Add to Inventory List"
        '
        'cboCIPInvTypes
        '
        Me.cboCIPInvTypes.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboCIPInvTypes.DropDownWidth = 300
        Me.cboCIPInvTypes.Font = New System.Drawing.Font("Microsoft Sans Serif", 11.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cboCIPInvTypes.FormattingEnabled = True
        Me.cboCIPInvTypes.Location = New System.Drawing.Point(27, 34)
        Me.cboCIPInvTypes.Name = "cboCIPInvTypes"
        Me.cboCIPInvTypes.Size = New System.Drawing.Size(186, 26)
        Me.cboCIPInvTypes.TabIndex = 2
        '
        'btnRemoveInv
        '
        Me.btnRemoveInv.Font = New System.Drawing.Font("Microsoft Sans Serif", 11.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnRemoveInv.Location = New System.Drawing.Point(226, 91)
        Me.btnRemoveInv.Name = "btnRemoveInv"
        Me.btnRemoveInv.Size = New System.Drawing.Size(81, 33)
        Me.btnRemoveInv.TabIndex = 1
        Me.btnRemoveInv.Text = "Remove"
        Me.btnRemoveInv.UseVisualStyleBackColor = True
        '
        'btnAddInv
        '
        Me.btnAddInv.Font = New System.Drawing.Font("Microsoft Sans Serif", 11.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnAddInv.Location = New System.Drawing.Point(226, 31)
        Me.btnAddInv.Name = "btnAddInv"
        Me.btnAddInv.Size = New System.Drawing.Size(81, 33)
        Me.btnAddInv.TabIndex = 0
        Me.btnAddInv.Text = "Add"
        Me.btnAddInv.UseVisualStyleBackColor = True
        '
        'cboAction
        '
        Me.cboAction.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboAction.DropDownWidth = 300
        Me.cboAction.Enabled = False
        Me.cboAction.Font = New System.Drawing.Font("Microsoft Sans Serif", 11.25!, System.Drawing.FontStyle.Bold)
        Me.cboAction.FormattingEnabled = True
        Me.cboAction.Location = New System.Drawing.Point(381, 14)
        Me.cboAction.Name = "cboAction"
        Me.cboAction.Size = New System.Drawing.Size(134, 26)
        Me.cboAction.TabIndex = 17
        '
        'cboStrategy
        '
        Me.cboStrategy.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboStrategy.DropDownWidth = 300
        Me.cboStrategy.Font = New System.Drawing.Font("Microsoft Sans Serif", 11.25!, System.Drawing.FontStyle.Bold)
        Me.cboStrategy.FormattingEnabled = True
        Me.cboStrategy.Location = New System.Drawing.Point(241, 14)
        Me.cboStrategy.Name = "cboStrategy"
        Me.cboStrategy.Size = New System.Drawing.Size(134, 26)
        Me.cboStrategy.TabIndex = 15
        '
        'lblCost
        '
        Me.lblCost.AutoSize = True
        Me.lblCost.ContextMenuStrip = Me.ctxMenuTotals
        Me.lblCost.Font = New System.Drawing.Font("Microsoft Sans Serif", 11.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblCost.Location = New System.Drawing.Point(676, 17)
        Me.lblCost.Name = "lblCost"
        Me.lblCost.Size = New System.Drawing.Size(49, 18)
        Me.lblCost.TabIndex = 32
        Me.lblCost.Text = "Cost:"
        '
        'ctxMenuTotals
        '
        Me.ctxMenuTotals.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ShowLength, Me.ShowArea, Me.ShowPoint})
        Me.ctxMenuTotals.Name = "ContextMenuStrip1"
        Me.ctxMenuTotals.Size = New System.Drawing.Size(179, 70)
        Me.ctxMenuTotals.Text = "Totals"
        '
        'ShowLength
        '
        Me.ShowLength.Checked = True
        Me.ShowLength.CheckOnClick = True
        Me.ShowLength.CheckState = System.Windows.Forms.CheckState.Checked
        Me.ShowLength.Name = "ShowLength"
        Me.ShowLength.Size = New System.Drawing.Size(178, 22)
        Me.ShowLength.Text = "Toggle Length"
        '
        'ShowArea
        '
        Me.ShowArea.CheckOnClick = True
        Me.ShowArea.Name = "ShowArea"
        Me.ShowArea.Size = New System.Drawing.Size(178, 22)
        Me.ShowArea.Text = "Toggle Area"
        '
        'ShowPoint
        '
        Me.ShowPoint.CheckOnClick = True
        Me.ShowPoint.Name = "ShowPoint"
        Me.ShowPoint.Size = New System.Drawing.Size(178, 22)
        Me.ShowPoint.Text = "Toggle Point Count"
        '
        'TotalDisplay
        '
        Me.TotalDisplay.AutoSize = True
        Me.TotalDisplay.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.TotalDisplay.Controls.Add(Me.lblLength)
        Me.TotalDisplay.Controls.Add(Me.lblTotLength)
        Me.TotalDisplay.Controls.Add(Me.lblArea)
        Me.TotalDisplay.Controls.Add(Me.lblTotArea)
        Me.TotalDisplay.Controls.Add(Me.lblPoint)
        Me.TotalDisplay.Controls.Add(Me.lblTotPnt)
        Me.TotalDisplay.Location = New System.Drawing.Point(798, 17)
        Me.TotalDisplay.Name = "TotalDisplay"
        Me.TotalDisplay.Size = New System.Drawing.Size(286, 18)
        Me.TotalDisplay.TabIndex = 19
        Me.TotalDisplay.WrapContents = False
        '
        'lblLength
        '
        Me.lblLength.AutoSize = True
        Me.lblLength.ContextMenuStrip = Me.ctxMenuTotals
        Me.lblLength.Font = New System.Drawing.Font("Microsoft Sans Serif", 11.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblLength.Location = New System.Drawing.Point(3, 0)
        Me.lblLength.Name = "lblLength"
        Me.lblLength.Size = New System.Drawing.Size(63, 18)
        Me.lblLength.TabIndex = 26
        Me.lblLength.Text = "Length:"
        '
        'lblTotLength
        '
        Me.lblTotLength.AutoSize = True
        Me.lblTotLength.ContextMenuStrip = Me.ctxMenuTotals
        Me.lblTotLength.Font = New System.Drawing.Font("Microsoft Sans Serif", 11.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblTotLength.ForeColor = System.Drawing.SystemColors.Highlight
        Me.lblTotLength.Location = New System.Drawing.Point(72, 0)
        Me.lblTotLength.Name = "lblTotLength"
        Me.lblTotLength.Size = New System.Drawing.Size(31, 18)
        Me.lblTotLength.TabIndex = 35
        Me.lblTotLength.Text = "0.0"
        '
        'lblArea
        '
        Me.lblArea.AutoSize = True
        Me.lblArea.ContextMenuStrip = Me.ctxMenuTotals
        Me.lblArea.Font = New System.Drawing.Font("Microsoft Sans Serif", 11.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblArea.Location = New System.Drawing.Point(109, 0)
        Me.lblArea.Name = "lblArea"
        Me.lblArea.Size = New System.Drawing.Size(47, 18)
        Me.lblArea.TabIndex = 36
        Me.lblArea.Text = "Area:"
        Me.lblArea.Visible = False
        '
        'lblTotArea
        '
        Me.lblTotArea.AutoSize = True
        Me.lblTotArea.ContextMenuStrip = Me.ctxMenuTotals
        Me.lblTotArea.Font = New System.Drawing.Font("Microsoft Sans Serif", 11.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblTotArea.ForeColor = System.Drawing.SystemColors.Highlight
        Me.lblTotArea.Location = New System.Drawing.Point(162, 0)
        Me.lblTotArea.Name = "lblTotArea"
        Me.lblTotArea.Size = New System.Drawing.Size(31, 18)
        Me.lblTotArea.TabIndex = 37
        Me.lblTotArea.Text = "0.0"
        Me.lblTotArea.Visible = False
        '
        'lblPoint
        '
        Me.lblPoint.AutoSize = True
        Me.lblPoint.ContextMenuStrip = Me.ctxMenuTotals
        Me.lblPoint.Font = New System.Drawing.Font("Microsoft Sans Serif", 11.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblPoint.Location = New System.Drawing.Point(199, 0)
        Me.lblPoint.Name = "lblPoint"
        Me.lblPoint.Size = New System.Drawing.Size(61, 18)
        Me.lblPoint.TabIndex = 38
        Me.lblPoint.Text = "Points:"
        Me.lblPoint.Visible = False
        '
        'lblTotPnt
        '
        Me.lblTotPnt.AutoSize = True
        Me.lblTotPnt.ContextMenuStrip = Me.ctxMenuTotals
        Me.lblTotPnt.Font = New System.Drawing.Font("Microsoft Sans Serif", 11.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblTotPnt.ForeColor = System.Drawing.SystemColors.Highlight
        Me.lblTotPnt.Location = New System.Drawing.Point(266, 0)
        Me.lblTotPnt.Name = "lblTotPnt"
        Me.lblTotPnt.Size = New System.Drawing.Size(17, 18)
        Me.lblTotPnt.TabIndex = 39
        Me.lblTotPnt.Text = "0"
        Me.lblTotPnt.Visible = False
        '
        'gpBxCIPCan
        '
        Me.gpBxCIPCan.Dock = System.Windows.Forms.DockStyle.Left
        Me.gpBxCIPCan.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.gpBxCIPCan.Location = New System.Drawing.Point(84, 47)
        Me.gpBxCIPCan.Name = "gpBxCIPCan"
        Me.gpBxCIPCan.Size = New System.Drawing.Size(74, 286)
        Me.gpBxCIPCan.TabIndex = 22
        Me.gpBxCIPCan.TabStop = False
        Me.gpBxCIPCan.Text = "CIP Candidates"
        Me.gpBxCIPCan.Visible = False
        '
        'gpBxControls
        '
        Me.gpBxControls.Controls.Add(Me.Panel1)
        Me.gpBxControls.Controls.Add(Me.lblCost)
        Me.gpBxControls.Controls.Add(Me.TotalDisplay)
        Me.gpBxControls.Controls.Add(Me.cboAction)
        Me.gpBxControls.Controls.Add(Me.btnSelectPrj)
        Me.gpBxControls.Controls.Add(Me.cboStrategy)
        Me.gpBxControls.Controls.Add(Me.lblTotalCost)
        Me.gpBxControls.Controls.Add(Me.btnSavePrj)
        Me.gpBxControls.Controls.Add(Me.btnClear)
        Me.gpBxControls.Controls.Add(Me.btnSelect)
        Me.gpBxControls.Controls.Add(Me.btnSketch)
        Me.gpBxControls.Controls.Add(Me.btnSelectAssets)
        Me.gpBxControls.Controls.Add(Me.btnSave)
        Me.gpBxControls.Controls.Add(Me.btnStopEditing)
        Me.gpBxControls.Controls.Add(Me.btnStartEditing)
        Me.gpBxControls.Controls.Add(Me.cboDefLayers)
        Me.gpBxControls.Dock = System.Windows.Forms.DockStyle.Top
        Me.gpBxControls.Font = New System.Drawing.Font("Microsoft Sans Serif", 11.25!, System.Drawing.FontStyle.Bold)
        Me.gpBxControls.Location = New System.Drawing.Point(0, 0)
        Me.gpBxControls.Name = "gpBxControls"
        Me.gpBxControls.Size = New System.Drawing.Size(902, 47)
        Me.gpBxControls.TabIndex = 21
        Me.gpBxControls.TabStop = False
        Me.gpBxControls.Visible = False
        '
        'btnSelectPrj
        '
        Me.btnSelectPrj.Appearance = System.Windows.Forms.Appearance.Button
        Me.btnSelectPrj.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom
        Me.btnSelectPrj.Enabled = False
        Me.btnSelectPrj.Image = CType(resources.GetObject("btnSelectPrj.Image"), System.Drawing.Image)
        Me.btnSelectPrj.Location = New System.Drawing.Point(149, 14)
        Me.btnSelectPrj.Name = "btnSelectPrj"
        Me.btnSelectPrj.Size = New System.Drawing.Size(26, 26)
        Me.btnSelectPrj.TabIndex = 16
        Me.tlTip.SetToolTip(Me.btnSelectPrj, "Select and Load an existing project")
        Me.btnSelectPrj.UseVisualStyleBackColor = True
        '
        'lblTotalCost
        '
        Me.lblTotalCost.AutoSize = True
        Me.lblTotalCost.ContextMenuStrip = Me.ctxMenuTotals
        Me.lblTotalCost.Font = New System.Drawing.Font("Microsoft Sans Serif", 11.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblTotalCost.ForeColor = System.Drawing.SystemColors.Highlight
        Me.lblTotalCost.Location = New System.Drawing.Point(721, 17)
        Me.lblTotalCost.Name = "lblTotalCost"
        Me.lblTotalCost.Size = New System.Drawing.Size(40, 18)
        Me.lblTotalCost.TabIndex = 25
        Me.lblTotalCost.Text = "0.00"
        '
        'btnSavePrj
        '
        Me.btnSavePrj.Enabled = False
        Me.btnSavePrj.Image = CType(resources.GetObject("btnSavePrj.Image"), System.Drawing.Image)
        Me.btnSavePrj.Location = New System.Drawing.Point(548, 14)
        Me.btnSavePrj.Name = "btnSavePrj"
        Me.btnSavePrj.Size = New System.Drawing.Size(26, 26)
        Me.btnSavePrj.TabIndex = 14
        Me.tlTip.SetToolTip(Me.btnSavePrj, "Save Selected Assets to a Project")
        Me.btnSavePrj.UseVisualStyleBackColor = True
        '
        'btnClear
        '
        Me.btnClear.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.btnClear.Image = CType(resources.GetObject("btnClear.Image"), System.Drawing.Image)
        Me.btnClear.Location = New System.Drawing.Point(518, 14)
        Me.btnClear.Name = "btnClear"
        Me.btnClear.Size = New System.Drawing.Size(26, 26)
        Me.btnClear.TabIndex = 11
        Me.tlTip.SetToolTip(Me.btnClear, "Clear Selected Assets from the costing window")
        Me.btnClear.UseVisualStyleBackColor = True
        '
        'btnSelect
        '
        Me.btnSelect.Appearance = System.Windows.Forms.Appearance.Button
        Me.btnSelect.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.btnSelect.Enabled = False
        Me.btnSelect.Image = Global.ArcGIS4LocalGovernment.My.Resources.Resources.CIPSelectAsset2Small
        Me.btnSelect.Location = New System.Drawing.Point(179, 14)
        Me.btnSelect.Name = "btnSelect"
        Me.btnSelect.Size = New System.Drawing.Size(26, 26)
        Me.btnSelect.TabIndex = 10
        Me.tlTip.SetToolTip(Me.btnSelect, "Highlight a costed asset")
        Me.btnSelect.UseVisualStyleBackColor = True
        '
        'btnSketch
        '
        Me.btnSketch.Appearance = System.Windows.Forms.Appearance.Button
        Me.btnSketch.Enabled = False
        Me.btnSketch.Image = CType(resources.GetObject("btnSketch.Image"), System.Drawing.Image)
        Me.btnSketch.Location = New System.Drawing.Point(119, 14)
        Me.btnSketch.Name = "btnSketch"
        Me.btnSketch.Size = New System.Drawing.Size(26, 26)
        Me.btnSketch.TabIndex = 9
        Me.tlTip.SetToolTip(Me.btnSketch, "Sketch in a new asset")
        Me.btnSketch.UseVisualStyleBackColor = True
        '
        'btnSelectAssets
        '
        Me.btnSelectAssets.Appearance = System.Windows.Forms.Appearance.Button
        Me.btnSelectAssets.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.btnSelectAssets.Enabled = False
        Me.btnSelectAssets.Image = Global.ArcGIS4LocalGovernment.My.Resources.Resources.CIPSelectAssetSmall
        Me.btnSelectAssets.Location = New System.Drawing.Point(209, 14)
        Me.btnSelectAssets.Name = "btnSelectAssets"
        Me.btnSelectAssets.Size = New System.Drawing.Size(26, 26)
        Me.btnSelectAssets.TabIndex = 8
        Me.tlTip.SetToolTip(Me.btnSelectAssets, "Select an Asset for Costing")
        Me.btnSelectAssets.UseVisualStyleBackColor = True
        '
        'btnSave
        '
        Me.btnSave.Enabled = False
        Me.btnSave.Image = Global.ArcGIS4LocalGovernment.My.Resources.Resources.SaveEdits
        Me.btnSave.Location = New System.Drawing.Point(644, 14)
        Me.btnSave.Name = "btnSave"
        Me.btnSave.Size = New System.Drawing.Size(26, 26)
        Me.btnSave.TabIndex = 6
        Me.tlTip.SetToolTip(Me.btnSave, "Save Edits")
        Me.btnSave.UseVisualStyleBackColor = True
        '
        'btnStopEditing
        '
        Me.btnStopEditing.Enabled = False
        Me.btnStopEditing.Image = Global.ArcGIS4LocalGovernment.My.Resources.Resources.StopEditing
        Me.btnStopEditing.Location = New System.Drawing.Point(614, 14)
        Me.btnStopEditing.Name = "btnStopEditing"
        Me.btnStopEditing.Size = New System.Drawing.Size(26, 26)
        Me.btnStopEditing.TabIndex = 5
        Me.tlTip.SetToolTip(Me.btnStopEditing, "Stop Editing the CIP Layers Workspace")
        Me.btnStopEditing.UseVisualStyleBackColor = True
        '
        'btnStartEditing
        '
        Me.btnStartEditing.ForeColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.btnStartEditing.Image = Global.ArcGIS4LocalGovernment.My.Resources.Resources.StartEditing
        Me.btnStartEditing.Location = New System.Drawing.Point(584, 14)
        Me.btnStartEditing.Name = "btnStartEditing"
        Me.btnStartEditing.Size = New System.Drawing.Size(26, 26)
        Me.btnStartEditing.TabIndex = 4
        Me.btnStartEditing.TextAlign = System.Drawing.ContentAlignment.BottomCenter
        Me.tlTip.SetToolTip(Me.btnStartEditing, "Start Editing the CIP Layers Workspace")
        Me.btnStartEditing.UseVisualStyleBackColor = True
        '
        'cboDefLayers
        '
        Me.cboDefLayers.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboDefLayers.DropDownWidth = 550
        Me.cboDefLayers.Enabled = False
        Me.cboDefLayers.Font = New System.Drawing.Font("Microsoft Sans Serif", 11.25!, System.Drawing.FontStyle.Bold)
        Me.cboDefLayers.FormattingEnabled = True
        Me.cboDefLayers.Location = New System.Drawing.Point(6, 14)
        Me.cboDefLayers.Name = "cboDefLayers"
        Me.cboDefLayers.Size = New System.Drawing.Size(109, 26)
        Me.cboDefLayers.TabIndex = 0
        '
        'tblDisabled
        '
        Me.tblDisabled.ColumnCount = 3
        Me.tblDisabled.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 27.51159!))
        Me.tblDisabled.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 52.24111!))
        Me.tblDisabled.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20.09274!))
        Me.tblDisabled.Controls.Add(Me.Label1, 1, 1)
        Me.tblDisabled.Dock = System.Windows.Forms.DockStyle.Left
        Me.tblDisabled.Location = New System.Drawing.Point(335, 47)
        Me.tblDisabled.Name = "tblDisabled"
        Me.tblDisabled.RowCount = 3
        Me.tblDisabled.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 36.96498!))
        Me.tblDisabled.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 32.29572!))
        Me.tblDisabled.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 30.35019!))
        Me.tblDisabled.Size = New System.Drawing.Size(61, 286)
        Me.tblDisabled.TabIndex = 27
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(19, 106)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(25, 92)
        Me.Label1.TabIndex = 0
        Me.Label1.Text = "Project Cost Estimating Extension is disable, please enable it"
        '
        'tlStZoomTo
        '
        Me.tlStZoomTo.Image = CType(resources.GetObject("tlStZoomTo.Image"), System.Drawing.Image)
        Me.tlStZoomTo.ImageTransparentColor = System.Drawing.Color.Fuchsia
        Me.tlStZoomTo.Name = "tlStZoomTo"
        Me.tlStZoomTo.Size = New System.Drawing.Size(123, 22)
        Me.tlStZoomTo.Text = "Zoom To"
        Me.tlStZoomTo.Visible = False
        '
        'tlStFlash
        '
        Me.tlStFlash.Image = CType(resources.GetObject("tlStFlash.Image"), System.Drawing.Image)
        Me.tlStFlash.ImageTransparentColor = System.Drawing.Color.White
        Me.tlStFlash.Name = "tlStFlash"
        Me.tlStFlash.Size = New System.Drawing.Size(123, 22)
        Me.tlStFlash.Text = "Flash"
        Me.tlStFlash.Visible = False
        '
        'ToolStripSeparator1
        '
        Me.ToolStripSeparator1.Name = "ToolStripSeparator1"
        Me.ToolStripSeparator1.Size = New System.Drawing.Size(120, 6)
        Me.ToolStripSeparator1.Visible = False
        '
        'tlStDelete
        '
        Me.tlStDelete.Image = CType(resources.GetObject("tlStDelete.Image"), System.Drawing.Image)
        Me.tlStDelete.ImageTransparentColor = System.Drawing.Color.Fuchsia
        Me.tlStDelete.Name = "tlStDelete"
        Me.tlStDelete.Size = New System.Drawing.Size(123, 22)
        Me.tlStDelete.Text = "Delete"
        '
        'ctxMenu
        '
        Me.ctxMenu.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.tlStZoomTo, Me.tlStFlash, Me.ToolStripSeparator1, Me.tlStDelete})
        Me.ctxMenu.Name = "ctxMenu"
        Me.ctxMenu.Size = New System.Drawing.Size(124, 76)
        '
        'gpBxCIPPrj
        '
        Me.gpBxCIPPrj.Controls.Add(Me.tbCntCIPDetails)
        Me.gpBxCIPPrj.Dock = System.Windows.Forms.DockStyle.Fill
        Me.gpBxCIPPrj.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.gpBxCIPPrj.Location = New System.Drawing.Point(396, 47)
        Me.gpBxCIPPrj.Name = "gpBxCIPPrj"
        Me.gpBxCIPPrj.Size = New System.Drawing.Size(506, 286)
        Me.gpBxCIPPrj.TabIndex = 28
        Me.gpBxCIPPrj.TabStop = False
        Me.gpBxCIPPrj.Text = "CIP Project Details"
        Me.gpBxCIPPrj.Visible = False
        '
        'tbCntCIPDetails
        '
        Me.tbCntCIPDetails.Alignment = System.Windows.Forms.TabAlignment.Bottom
        Me.tbCntCIPDetails.Dock = System.Windows.Forms.DockStyle.Fill
        Me.tbCntCIPDetails.Location = New System.Drawing.Point(3, 18)
        Me.tbCntCIPDetails.Multiline = True
        Me.tbCntCIPDetails.Name = "tbCntCIPDetails"
        Me.tbCntCIPDetails.SelectedIndex = 0
        Me.tbCntCIPDetails.Size = New System.Drawing.Size(500, 265)
        Me.tbCntCIPDetails.TabIndex = 0
        '
        'CostEstimatingWindow
        '
        Me.Controls.Add(Me.gpBxCIPCostingLayers)
        Me.Controls.Add(Me.gpBxCIPPrj)
        Me.Controls.Add(Me.tblDisabled)
        Me.Controls.Add(Me.gpBxCIPInven)
        Me.Controls.Add(Me.gpBxCIPCan)
        Me.Controls.Add(Me.gpBxSwitch)
        Me.Controls.Add(Me.gpBxControls)
        Me.Name = "CostEstimatingWindow"
        Me.Size = New System.Drawing.Size(902, 333)
        Me.gpBxSwitch.ResumeLayout(False)
        Me.gpBxCIPInven.ResumeLayout(False)
        Me.gpBxInv.ResumeLayout(False)
        Me.gpBxInv.PerformLayout()
        CType(Me.numCIPInvCount, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ctxMenuTotals.ResumeLayout(False)
        Me.TotalDisplay.ResumeLayout(False)
        Me.TotalDisplay.PerformLayout()
        Me.gpBxControls.ResumeLayout(False)
        Me.gpBxControls.PerformLayout()
        Me.tblDisabled.ResumeLayout(False)
        Me.tblDisabled.PerformLayout()
        Me.ctxMenu.ResumeLayout(False)
        Me.gpBxCIPPrj.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents gpBxCIPCostingLayers As System.Windows.Forms.GroupBox
    Friend WithEvents Panel1 As System.Windows.Forms.Panel
    Friend WithEvents rdoBtnShowLayers As System.Windows.Forms.RadioButton
    Friend WithEvents rdoShowInv As System.Windows.Forms.RadioButton
    Friend WithEvents gpBxSwitch As System.Windows.Forms.GroupBox
    Friend WithEvents rdoBtnShowDetails As System.Windows.Forms.RadioButton
    Friend WithEvents rdoBtnShowCan As System.Windows.Forms.RadioButton
    Friend WithEvents gpBxCIPInven As System.Windows.Forms.GroupBox
    Friend WithEvents lstInventory As System.Windows.Forms.ListBox
    Friend WithEvents gpBxInv As System.Windows.Forms.Panel
    Friend WithEvents Label7 As System.Windows.Forms.Label
    Friend WithEvents numCIPInvCount As System.Windows.Forms.NumericUpDown
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents cboCIPInvTypes As System.Windows.Forms.ComboBox
    Friend WithEvents btnRemoveInv As System.Windows.Forms.Button
    Friend WithEvents btnAddInv As System.Windows.Forms.Button
    Friend WithEvents btnSelectPrj As System.Windows.Forms.RadioButton
    Friend WithEvents cboAction As System.Windows.Forms.ComboBox
    Friend WithEvents cboStrategy As System.Windows.Forms.ComboBox
    Friend WithEvents lblCost As System.Windows.Forms.Label
    Friend WithEvents ctxMenuTotals As System.Windows.Forms.ContextMenuStrip
    Friend WithEvents ShowLength As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ShowArea As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ShowPoint As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents TotalDisplay As System.Windows.Forms.FlowLayoutPanel
    Friend WithEvents lblLength As System.Windows.Forms.Label
    Friend WithEvents lblTotLength As System.Windows.Forms.Label
    Friend WithEvents lblArea As System.Windows.Forms.Label
    Friend WithEvents lblTotArea As System.Windows.Forms.Label
    Friend WithEvents lblPoint As System.Windows.Forms.Label
    Friend WithEvents lblTotPnt As System.Windows.Forms.Label
    Friend WithEvents gpBxCIPCan As System.Windows.Forms.GroupBox
    Friend WithEvents gpBxControls As System.Windows.Forms.GroupBox
    Friend WithEvents lblTotalCost As System.Windows.Forms.Label
    Friend WithEvents btnSavePrj As System.Windows.Forms.Button
    Friend WithEvents btnClear As System.Windows.Forms.Button
    Friend WithEvents btnSelect As System.Windows.Forms.RadioButton
    Friend WithEvents btnSketch As System.Windows.Forms.RadioButton
    Friend WithEvents btnSelectAssets As System.Windows.Forms.RadioButton
    Friend WithEvents btnSave As System.Windows.Forms.Button
    Friend WithEvents btnStopEditing As System.Windows.Forms.Button
    Friend WithEvents btnStartEditing As System.Windows.Forms.Button
    Friend WithEvents cboDefLayers As System.Windows.Forms.ComboBox
    Friend WithEvents tblDisabled As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents tlStZoomTo As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents tlStFlash As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripSeparator1 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents tlStDelete As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ctxMenu As System.Windows.Forms.ContextMenuStrip
    Friend WithEvents gpBxCIPPrj As System.Windows.Forms.GroupBox
    Friend WithEvents tbCntCIPDetails As System.Windows.Forms.TabControl
    Friend WithEvents tlTip As System.Windows.Forms.ToolTip

End Class
