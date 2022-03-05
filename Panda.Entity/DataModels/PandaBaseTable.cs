using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Panda.Entity.DataModels;

public class PandaBaseTable : BaseTable
{

}

public class KeyGuidTable : PandaBaseTable
{
    [Key]
    public new Guid Id { get; set; }
}

public class BaseTable
{
    [Key] public virtual int Id { get; set; }


    /// <summary>
    /// 添加时间
    /// </summary>
    public DateTimeOffset AddTime { get; set; }

    /// <summary>
    /// 修改时间
    /// </summary>
    public DateTimeOffset UpdateTime { get; set; }
}