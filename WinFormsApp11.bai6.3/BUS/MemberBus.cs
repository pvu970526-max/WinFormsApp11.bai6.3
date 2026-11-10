using WinFormsApp11.bai6._3.DAL;
using WinFormsApp11.bai6._3.DTO;

namespace WinFormsApp11.bai6._3.BUS;

public sealed class MemberBus
{
    private readonly MemberDal dal = new();
    public List<MemberDto> GetAll() => dal.GetAll();
}
