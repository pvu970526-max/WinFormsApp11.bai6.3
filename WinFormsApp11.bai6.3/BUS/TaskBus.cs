using WinFormsApp11.bai6._3.DAL;
using WinFormsApp11.bai6._3.DTO;

namespace WinFormsApp11.bai6._3.BUS;

public sealed class TaskBus
{
    private readonly TaskDal dal = new();
    public static readonly string[] Statuses = ["To Do", "In Progress", "Review", "Done"];
    public static readonly string[] Priorities = ["Cao", "Trung bình", "Thấp"];
    public List<TaskDto> GetByProject(int projectId, string? status = null) => dal.GetByProject(projectId, status);
    public List<TaskDto> GetOverdue() => dal.GetOverdue();

    public int Create(TaskDto task)
    {
        if (task.MaDuAn <= 0) throw new ArgumentException("Vui lòng chọn dự án.");
        if (string.IsNullOrWhiteSpace(task.TenTask)) throw new ArgumentException("Tên công việc không được để trống.");
        if (!Statuses.Contains(task.TrangThai)) throw new ArgumentException("Trạng thái không hợp lệ.");
        if (!Priorities.Contains(task.DoUuTien)) throw new ArgumentException("Độ ưu tiên không hợp lệ.");
        return dal.Insert(task);
    }

    public void ChangeStatus(int taskId, string status)
    {
        if (taskId <= 0 || !Statuses.Contains(status)) throw new ArgumentException("Task hoặc trạng thái không hợp lệ.");
        dal.UpdateStatus(taskId, status);
    }

    public void Assign(int taskId, int memberId)
    {
        if (taskId <= 0 || memberId <= 0) throw new ArgumentException("Task hoặc thành viên không hợp lệ.");
        dal.Assign(taskId, memberId);
    }
}
