using WinFormsApp11.bai6._3.DAL;
using WinFormsApp11.bai6._3.DTO;

namespace WinFormsApp11.bai6._3.BUS;

public sealed class ProjectBus
{
    private readonly ProjectDal dal = new();
    public List<ProjectDto> GetAll() => dal.GetAll();

    public int Save(ProjectDto project)
    {
        if (string.IsNullOrWhiteSpace(project.TenDuAn)) throw new ArgumentException("Tên dự án không được để trống.");
        if (project.NgayBatDau.HasValue && project.NgayKetThuc.HasValue && project.NgayKetThuc < project.NgayBatDau)
            throw new ArgumentException("Ngày kết thúc phải lớn hơn hoặc bằng ngày bắt đầu.");
        if (project.MaDuAn == 0) return dal.Insert(project);
        dal.Update(project);
        return project.MaDuAn;
    }

    public void Delete(int projectId)
    {
        if (projectId <= 0) throw new ArgumentException("Dự án không hợp lệ.");
        dal.Delete(projectId);
    }
}
