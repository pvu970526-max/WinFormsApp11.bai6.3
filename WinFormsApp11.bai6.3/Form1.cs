using WinFormsApp11.bai6._3.BUS;
using WinFormsApp11.bai6._3.DTO;

namespace WinFormsApp11.bai6._3;

public partial class Form1 : Form
{
    private readonly ProjectBus projectBus = new();
    private readonly TaskBus taskBus = new();
    private readonly MemberBus memberBus = new();
    private readonly ReportBus reportBus = new();

    private readonly DataGridView projectGrid = new();
    private readonly DataGridView taskGrid = new();
    private readonly DataGridView overdueGrid = new();
    private readonly DataGridView reportGrid = new();
    private readonly ComboBox projectSelector = new();
    private readonly ComboBox kanbanSelector = new();
    private readonly ComboBox taskStatusFilter = new();
    private readonly ComboBox statusSelector = new();
    private readonly ComboBox prioritySelector = new();
    private readonly ComboBox memberSelector = new();
    private readonly TextBox projectName = new();
    private readonly TextBox taskName = new();
    private readonly TextBox taskDescription = new();
    private readonly DateTimePicker projectStart = NewDatePicker();
    private readonly DateTimePicker projectEnd = NewDatePicker();
    private readonly DateTimePicker taskDue = NewDatePicker();
    private readonly Label message = new();
    private readonly ListBox todoList = new();
    private readonly ListBox progressList = new();
    private readonly ListBox reviewList = new();
    private readonly ListBox doneList = new();
    private int selectedProjectId;

    public Form1()
    {
        InitializeComponent();
        Text = "Quản lý công việc & tiến độ dự án";
        Width = 1250;
        Height = 780;
        StartPosition = FormStartPosition.CenterScreen;
        BuildInterface();
        Load += (_, _) => LoadData();
    }

    private void BuildInterface()
    {
        var tabs = new TabControl { Dock = DockStyle.Fill };
        tabs.TabPages.Add(BuildManagementTab());
        tabs.TabPages.Add(BuildKanbanTab());
        tabs.TabPages.Add(BuildOverdueTab());
        tabs.TabPages.Add(BuildReportTab());
        Controls.Add(tabs);
        message.Dock = DockStyle.Bottom;
        message.Height = 28;
        message.Padding = new Padding(8, 5, 0, 0);
        Controls.Add(message);
    }

    private TabPage BuildManagementTab()
    {
        var page = new TabPage("Dự án & Task");
        var split = new SplitContainer { Dock = DockStyle.Fill, Orientation = Orientation.Horizontal, SplitterDistance = 270 };
        var projectPanel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(8) };
        projectPanel.Controls.Add(projectGrid);
        projectGrid.Dock = DockStyle.Fill;
        ConfigureGrid(projectGrid);
        projectGrid.SelectionChanged += (_, _) => SelectProject();

        var projectActions = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 100, WrapContents = false, AutoScroll = true };
        projectActions.Controls.Add(new Label { Text = "Tên dự án", AutoSize = true, Padding = new Padding(0, 8, 4, 0) });
        projectName.Width = 180;
        projectActions.Controls.Add(projectName);
        projectActions.Controls.Add(new Label { Text = "Bắt đầu", AutoSize = true, Padding = new Padding(8, 8, 4, 0) });
        projectActions.Controls.Add(projectStart);
        projectActions.Controls.Add(new Label { Text = "Kết thúc", AutoSize = true, Padding = new Padding(8, 8, 4, 0) });
        projectActions.Controls.Add(projectEnd);
        projectActions.Controls.Add(Button("Lưu dự án", SaveProject));
        projectActions.Controls.Add(Button("Xóa dự án", DeleteProject));
        projectPanel.Controls.Add(projectActions);
        split.Panel1.Controls.Add(projectPanel);

        var taskPanel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(8) };
        taskPanel.Controls.Add(taskGrid);
        taskGrid.Dock = DockStyle.Fill;
        ConfigureGrid(taskGrid);
        taskGrid.SelectionChanged += (_, _) => UpdateTaskActions();
        var taskActions = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 135, WrapContents = true, AutoScroll = true };
        taskActions.Controls.Add(new Label { Text = "Dự án", AutoSize = true, Padding = new Padding(0, 8, 4, 0) });
        projectSelector.Width = 190;
        projectSelector.DropDownStyle = ComboBoxStyle.DropDownList;
        projectSelector.SelectedIndexChanged += (_, _) => RefreshTasks();
        taskActions.Controls.Add(projectSelector);
        taskActions.Controls.Add(new Label { Text = "Tên task", AutoSize = true, Padding = new Padding(8, 8, 4, 0) });
        taskName.Width = 190;
        taskActions.Controls.Add(taskName);
        taskActions.Controls.Add(new Label { Text = "Hạn nộp", AutoSize = true, Padding = new Padding(8, 8, 4, 0) });
        taskActions.Controls.Add(taskDue);
        prioritySelector.Items.AddRange(TaskBus.Priorities);
        prioritySelector.SelectedIndex = 1;
        taskActions.Controls.Add(prioritySelector);
        statusSelector.Items.AddRange(TaskBus.Statuses);
        statusSelector.SelectedIndex = 0;
        taskActions.Controls.Add(statusSelector);
        taskActions.Controls.Add(Button("Tạo task", CreateTask));
        taskActions.Controls.Add(new Label { Text = "Lọc trạng thái", AutoSize = true, Padding = new Padding(8, 8, 4, 0) });
        taskStatusFilter.Items.Add("Tất cả");
        taskStatusFilter.Items.AddRange(TaskBus.Statuses);
        taskStatusFilter.SelectedIndex = 0;
        taskStatusFilter.SelectedIndexChanged += (_, _) => RefreshTasks();
        taskActions.Controls.Add(taskStatusFilter);
        taskActions.Controls.Add(new Label { Text = "Mô tả", AutoSize = true, Padding = new Padding(8, 8, 4, 0) });
        taskDescription.Width = 320;
        taskActions.Controls.Add(taskDescription);
        taskActions.Controls.Add(Button("Cập nhật trạng thái", ChangeStatus));
        memberSelector.Width = 190;
        memberSelector.DropDownStyle = ComboBoxStyle.DropDownList;
        taskActions.Controls.Add(memberSelector);
        taskActions.Controls.Add(Button("Giao việc", AssignTask));
        split.Panel2.Controls.Add(taskPanel);
        page.Controls.Add(split);
        return page;
    }

    private TabPage BuildKanbanTab()
    {
        var page = new TabPage("Kanban");
        var top = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 42, WrapContents = false };
        top.Controls.Add(new Label { Text = "Dự án", AutoSize = true, Padding = new Padding(8, 10, 4, 0) });
        projectSelector.Tag = "shared";
        var kanbanProject = new ComboBox { Width = 220, DropDownStyle = ComboBoxStyle.DropDownList };
        kanbanSelector.SelectedIndexChanged += (_, _) => LoadKanban(kanbanSelector.SelectedItem as ProjectDto);
        top.Controls.Add(kanbanSelector);
        page.Controls.Add(top);
        var columns = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 4, RowCount = 1, Padding = new Padding(8) };
        var entries = new[] { ("To Do", todoList), ("In Progress", progressList), ("Review", reviewList), ("Done", doneList) };
        for (var i = 0; i < entries.Length; i++)
        {
            columns.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
            entries[i].Item2.Dock = DockStyle.Fill;
            columns.Controls.Add(Group(entries[i].Item1, entries[i].Item2), i, 0);
        }
        page.Controls.Add(columns);
        return page;
    }

    private TabPage BuildOverdueTab()
    {
        var page = new TabPage("Cảnh báo trễ hạn");
        page.Controls.Add(overdueGrid);
        overdueGrid.Dock = DockStyle.Fill;
        ConfigureGrid(overdueGrid);
        page.Enter += (_, _) => RefreshOverdue();
        return page;
    }

    private TabPage BuildReportTab()
    {
        var page = new TabPage("Báo cáo thành viên");
        page.Controls.Add(reportGrid);
        reportGrid.Dock = DockStyle.Fill;
        ConfigureGrid(reportGrid);
        page.Enter += (_, _) => RefreshReport();
        return page;
    }

    private void LoadData()
    {
        try
        {
            var projects = projectBus.GetAll();
            projectGrid.DataSource = projects;
            projectSelector.DataSource = projects.ToList();
            projectSelector.DisplayMember = nameof(ProjectDto.TenDuAn);
            projectSelector.ValueMember = nameof(ProjectDto.MaDuAn);
            kanbanSelector.DataSource = projects.ToList();
            kanbanSelector.DisplayMember = nameof(ProjectDto.TenDuAn);
            kanbanSelector.ValueMember = nameof(ProjectDto.MaDuAn);
            var members = memberBus.GetAll();
            memberSelector.DataSource = members;
            memberSelector.DisplayMember = nameof(MemberDto.HoTen);
            memberSelector.ValueMember = nameof(MemberDto.MaThanhVien);
            if (projects.Count > 0) SelectProject();
            SetMessage($"Đã tải {projects.Count} dự án và {members.Count} thành viên.");
        }
        catch (Exception ex) { SetMessage($"Không thể kết nối CSDL: {ex.Message}"); }
    }

    private void SelectProject()
    {
        if (projectGrid.CurrentRow?.DataBoundItem is not ProjectDto project) return;
        selectedProjectId = project.MaDuAn;
        projectName.Text = project.TenDuAn;
        projectStart.Value = project.NgayBatDau ?? DateTime.Today;
        projectEnd.Value = project.NgayKetThuc ?? DateTime.Today.AddMonths(1);
        projectSelector.SelectedValue = selectedProjectId;
        RefreshTasks();
    }

    private void RefreshTasks()
    {
        if (projectSelector.SelectedValue is not int projectId || projectId <= 0) return;
        var status = taskStatusFilter.SelectedItem?.ToString();
        taskGrid.DataSource = taskBus.GetByProject(projectId, status == "Tất cả" ? null : status);
    }

    private void SaveProject(object? sender, EventArgs e)
    {
        try
        {
            var project = projectGrid.CurrentRow?.DataBoundItem as ProjectDto ?? new ProjectDto();
            project.TenDuAn = projectName.Text.Trim();
            project.NgayBatDau = projectStart.Value.Date;
            project.NgayKetThuc = projectEnd.Value.Date;
            project.TrangThai = "Đang chạy";
            projectBus.Save(project);
            LoadData();
            SetMessage("Đã lưu dự án.");
        }
        catch (Exception ex) { ShowError(ex); }
    }

    private void DeleteProject(object? sender, EventArgs e)
    {
        if (projectGrid.CurrentRow?.DataBoundItem is not ProjectDto project) return;
        try { projectBus.Delete(project.MaDuAn); LoadData(); SetMessage("Đã xóa dự án và các task liên quan."); }
        catch (Exception ex) { ShowError(ex); }
    }

    private void CreateTask(object? sender, EventArgs e)
    {
        try
        {
            var projectId = projectSelector.SelectedValue is int id ? id : 0;
            taskBus.Create(new TaskDto { MaDuAn = projectId, TenTask = taskName.Text.Trim(), MoTa = taskDescription.Text.Trim(), HanNop = taskDue.Value, DoUuTien = prioritySelector.Text, TrangThai = statusSelector.Text });
            RefreshTasks();
            SetMessage("Đã tạo task.");
        }
        catch (Exception ex) { ShowError(ex); }
    }

    private void ChangeStatus(object? sender, EventArgs e)
    {
        if (taskGrid.CurrentRow?.DataBoundItem is not TaskDto task) return;
        try { taskBus.ChangeStatus(task.MaTask, statusSelector.Text); RefreshTasks(); SetMessage("Đã cập nhật trạng thái bằng parameterized UPDATE."); }
        catch (Exception ex) { ShowError(ex); }
    }

    private void AssignTask(object? sender, EventArgs e)
    {
        if (taskGrid.CurrentRow?.DataBoundItem is not TaskDto task || memberSelector.SelectedValue is not int memberId) return;
        try { taskBus.Assign(task.MaTask, memberId); RefreshTasks(); SetMessage("Đã phân công thành viên."); }
        catch (Exception ex) { ShowError(ex); }
    }

    private void UpdateTaskActions() { if (taskGrid.CurrentRow?.DataBoundItem is TaskDto task) statusSelector.Text = task.TrangThai; }

    private void LoadKanban(ProjectDto? project)
    {
        if (project is null) return;
        try
        {
            var tasks = taskBus.GetByProject(project.MaDuAn);
            FillList(todoList, tasks.Where(t => t.TrangThai == "To Do"));
            FillList(progressList, tasks.Where(t => t.TrangThai == "In Progress"));
            FillList(reviewList, tasks.Where(t => t.TrangThai == "Review"));
            FillList(doneList, tasks.Where(t => t.TrangThai == "Done"));
        }
        catch (Exception ex) { SetMessage(ex.Message); }
    }

    private void RefreshOverdue() { try { overdueGrid.DataSource = taskBus.GetOverdue(); } catch (Exception ex) { SetMessage(ex.Message); } }
    private void RefreshReport() { try { reportGrid.DataSource = selectedProjectId > 0 ? reportBus.GetMemberProgress(selectedProjectId) : null; } catch (Exception ex) { SetMessage(ex.Message); } }
    private static void FillList(ListBox list, IEnumerable<TaskDto> tasks) { list.Items.Clear(); foreach (var task in tasks) list.Items.Add($"#{task.MaTask} {task.TenTask} | {task.DoUuTien}"); }
    private static GroupBox Group(string title, Control content) { var group = new GroupBox { Text = title, Dock = DockStyle.Fill, Padding = new Padding(8) }; group.Controls.Add(content); return group; }
    private static DateTimePicker NewDatePicker() => new() { Width = 105, Format = DateTimePickerFormat.Short };
    private static Button Button(string text, EventHandler handler) { var button = new Button { Text = text, AutoSize = true, Height = 28 }; button.Click += handler; return button; }
    private static void ConfigureGrid(DataGridView grid) { grid.ReadOnly = true; grid.AllowUserToAddRows = false; grid.AutoGenerateColumns = true; grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill; grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect; }
    private void SetMessage(string text) => message.Text = text;
    private void ShowError(Exception ex) => SetMessage($"Lỗi: {ex.Message}");
}
