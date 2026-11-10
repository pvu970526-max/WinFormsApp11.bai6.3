using WinFormsApp11.bai6._3.DAL;
using WinFormsApp11.bai6._3.DTO;

namespace WinFormsApp11.bai6._3.BUS;

public sealed class ReportBus
{
    private readonly ReportDal dal = new();
    public List<MemberProgressDto> GetMemberProgress(int projectId) => dal.GetMemberProgress(projectId);
}
